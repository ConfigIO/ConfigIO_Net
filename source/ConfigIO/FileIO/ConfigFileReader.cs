using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Configuration.FileIO
{
    public interface IConfigFileReader
    {
        ConfigFileReaderCallbacks Callbacks { get; set; }

        SyntaxMarkers Markers { get; set; }

        bool NormalizeLineEndings { get; set; }

        ConfigFile Parse(TextReader reader);
    }

    public enum ReadStep
    {
        ReadName,
        ReadOptionValue,
        ReadSectionBody,
        ReadComment,
    }

    /// <summary>
    /// Parses only the body of a section, not its identifier/header.
    /// </summary>
    /// <example>
    /// In the following sample, Option0, Option1, and Option2 are all parsed by a different instance of this class.
    /// <code>
    /// Option0 = Value0
    /// Section:
    ///     Option1 = Value1
    ///     InnerSection:
    ///         Option2 = Value2
    /// </code>
    /// </example>
    public class ConfigFileReader : IConfigFileReader
    {
        public struct SectionInfo
        {
            public ConfigSection Section { get; set; }
            public int Indentation { get; set; }
        }

        /// <summary>
        /// Set of markers to determine syntax.
        /// Should be set by the user creating an instace of this class.
        /// </summary>
        public SyntaxMarkers Markers { get; set; }

        public ConfigFileReaderCallbacks Callbacks { get; set; }

        public bool NormalizeLineEndings { get; set; }

        public ConfigFileReader()
        {
            Callbacks = new ConfigFileReaderCallbacks()
                        {
                            SectionNameProcessor = section => section.Trim(),
                            OptionValueProcessor = value => value.Trim(),
                            OptionNameProcessor =  name => name.Trim(),
                            FileNameProcessor =    fileName => fileName.Trim(),
                        };
            NormalizeLineEndings = true;
        }

        public ConfigFile Parse(TextReader reader)
        {
            var content = reader.ReadToEnd();
            return Parse(new StringStream(content));
        }

        public ConfigFile Parse(StringStream stream)
        {
            if (NormalizeLineEndings)
            {
                var normalizedContent = stream.CurrentContent.Replace("\r", string.Empty);
                stream = new StringStream(normalizedContent);
            }

            var cfg = new ConfigFile();
            ParseSection(stream, new SectionInfo()
                                 {
                                     Section = cfg,
                                     Indentation = -1,
                                 });

            return cfg;
        }

        private void ParseSection(StringStream stream, SectionInfo parent)
        {
            SkipWhiteSpaceAndComments(stream);

            if (stream.IsAtEndOfStream) { return; }

            // Determine the indentation for this section.
            var sectionIndentation = DetermineLineIndentation(stream);
            var currentIndentation = sectionIndentation;

            Debug.Assert(sectionIndentation != parent.Indentation);

            while (true)
            {
                if (stream.IsAtEndOfStream) { break; }

                if (currentIndentation <= parent.Indentation)
                {
                    // We have something like this:
                    //    | Section0:
                    //    |     opt = val
                    // -> | Section1:
                    //    |     ...
                    break;
                }
                else if (currentIndentation != sectionIndentation)
                {
                    //    | Section0:
                    //    |     opt0 = val0
                    //    |     opt1 = val1
                    // -> |       opt2 = val2
                    // -> |         Section1:
                    Error(stream, new InvalidIndentationException(currentIndentation, sectionIndentation));
                }

                var name = ParseIdentifier(stream);

                if (stream.IsAt(Markers.KeyValueDelimiter))
                {
                    stream.SkipWhile(_ => stream.IsAt(Markers.KeyValueDelimiter));
                    CheckStream(stream, ReadStep.ReadOptionValue);

                    // We are at the value of an option.
                    var value = ParseIdentifier(stream);

                    if (name.TrimStart().StartsWith(Markers.IncludeBeginMarker))
                    {
                        // We are at something like:
                        // [include] SectionName = Path/To/File.cfg
                        var fileName = Callbacks.FileNameProcessor(value);
                        var cfg = new ConfigFile() { FileName = fileName };
                        cfg.LoadFromFile();
                        // Remove "[include]" from the name
                        name = name.Replace(Markers.IncludeBeginMarker, string.Empty);
                        cfg.Name = Callbacks.SectionNameProcessor(name);
                        parent.Section.AddSection(cfg);
                    }
                    else
                    {
                        // We are at something like:
                        // Option = Value
                        var option = new ConfigOption()
                        {
                            Name = Callbacks.OptionNameProcessor(name),
                            Value = Callbacks.OptionValueProcessor(value),
                        };
                        parent.Section.AddOption(option);
                    }
                }
                else if (stream.IsAt(Markers.SectionBodyBeginMarker))
                {
                    stream.SkipWhile(_ => stream.IsAt(Markers.SectionBodyBeginMarker));

                    // We are at the beginning of a section body
                    var subSection = new ConfigSection();
                    ParseSection(stream, new SectionInfo()
                                         {
                                             Section = subSection,
                                             Indentation = sectionIndentation
                                         });
                    subSection.Name = Callbacks.SectionNameProcessor(name);
                    parent.Section.AddSection(subSection);
                }
                else if (stream.IsAt(Markers.SingleLineCommentBeginMarker)
                      || stream.IsAt(Markers.MultiLineCommentBeginMarker))
                {
                    ParseComment(stream);
                }

                SkipWhiteSpaceAndComments(stream);
                currentIndentation = DetermineLineIndentation(stream);
            }
        }

        private void ParseComment(StringStream stream)
        {
            CheckStream(stream, ReadStep.ReadComment);

            if (stream.IsAt(Markers.SingleLineCommentBeginMarker))
            {
                stream.SkipUntil(_ => stream.IsAtNewLine);
                stream.SkipWhile(_ => stream.IsAtNewLine);
            }
            else if (stream.IsAt(Markers.MultiLineCommentBeginMarker))
            {
                stream.SkipUntil(_ => stream.IsAt(Markers.MultiLineCommentEndMarker));
                stream.SkipWhile(_ => stream.IsAt(Markers.MultiLineCommentEndMarker));
            }
            else
            {
                Error(stream, new InvalidOperationException("Current stream is not at a comment!"));
            }
        }

        private string ParseIdentifier(StringStream stream)
        {
            var start = new StringStream(stream);
            var length = stream.SkipUntil(_ => stream.IsAtNewLine
                                            || StreamIsAtIdentifier(stream));

            var identifier = start.Content.Substring(start.Index, length);
            return identifier;
        }

        private void SkipWhiteSpaceAndComments(StringStream stream)
        {
            while (true)
            {
                stream.SkipWhile(c => char.IsWhiteSpace(c));

                if (stream.IsAt(Markers.SingleLineCommentBeginMarker)
                 || stream.IsAt(Markers.MultiLineCommentBeginMarker))
                {
                    ParseComment(stream);
                }
                else
                {
                    break;
                }
            }
        }

        private int DetermineLineIndentation(StringStream stream)
        {
            var copy = new StringStream(stream);

            // Skip back to the beginning of the line.
            copy.SkipReverseUntil(_ => copy.IsAtBeginning || copy.IsAtNewLine);

            // Skip ahead until we are no longer at the new line character
            copy.SkipWhile(_ => copy.IsAtNewLine);

            // Now skip all white space and return how much was skipped.
            return copy.SkipWhile(c => char.IsWhiteSpace(c));
        }

        private void CheckStream(StringStream stream, ReadStep step)
        {
            if (stream.IsValid) { return; }
            
            throw new NotImplementedException();
        }

        private bool StreamIsAtIdentifier(StringStream stream)
        {
            return stream.IsAt(Markers.KeyValueDelimiter)
                || stream.IsAt(Markers.SectionBodyBeginMarker)
                || stream.IsAt(Markers.SingleLineCommentBeginMarker)
                || stream.IsAt(Markers.MultiLineCommentBeginMarker);
        }

        private void Error(StringStream currentStream, Exception exception)
        {
            throw new Exception(string.Format("Line {0}:",
                                              currentStream.CurrentLineNumber),
                                exception);
        }
    }

}
