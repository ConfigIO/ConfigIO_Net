
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace Configuration.FileIO
{
    public class SyntaxMarkers
    {
        /// <summary>
        /// Key = Value
        /// ----^
        /// </summary>
        public string KeyValueDelimiter { get; set; }

        /// <summary>
        /// ---------v
        /// Section 0:
        ///     opt0 = val0
        ///     opt1 = val1
        /// </summary>
        public string SectionBodyBeginMarker { get; set; }

        /// <summary>
        ///     [include] Section 0 = Path/To/My/File.cfg
        /// ----^^^^^^^^^
        /// or
        ///     $ Section 0 = Path/To/My/File.cfg
        /// ----^
        /// </summary>
        public string IncludeBeginMarker { get; set; }

        /// <summary>
        ///     // This is a comment.
        /// ----^^
        /// </summary>
        public string SingleLineCommentBeginMarker { get; set; }


        /// <summary>
        ///     /* This is a multi-line comment. */
        /// ----^^
        /// </summary>
        public string MultiLineCommentBeginMarker { get; set; }


        /// <summary>
        ///     /* This is a multi-line comment. */
        /// -------------------------------------^^
        /// </summary>
        public string MultiLineCommentEndMarker { get; set; }
    }

    public class ConfigReaderCallbacks
    {
        public PreprocessorCallback SectionNameProcessor { get; set; }

        public PreprocessorCallback OptionNameProcessor { get; set; }

        public PreprocessorCallback OptionValueProcessor { get; set; }

        public PreprocessorCallback FileNameProcessor { get; set; }
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
    public class ConfigSectionReader
    {
        /// <summary>
        /// Set of markers to determine syntax.
        /// Should be set by the user creating an instace of this class.
        /// </summary>
        public SyntaxMarkers Markers { get; set; }

        public ConfigReaderCallbacks Callbacks { get; set; }

        public ConfigSectionReader()
        {
            Callbacks = new ConfigReaderCallbacks()
                        {
                            SectionNameProcessor = section => section.Trim(),
                            OptionValueProcessor = value => value.Trim(),
                            OptionNameProcessor =  name => name.Trim(),
                            FileNameProcessor =    fileName => fileName.Trim(),
                        };
        }

        public ConfigSection ParseSection(StringStream stream, int referenceIndentation)
        {
            var section = new ConfigSection();

            SkipWhiteSpaceAndComments(stream);
            CheckStream(stream, ReadStep.ReadSectionBody);

            // Determine the indentation for this section.
            var sectionIndentation = DetermineLineIndentation(stream);
            var currentIndentation = sectionIndentation;

            Debug.Assert(referenceIndentation == 0 || sectionIndentation != referenceIndentation);

            if (currentIndentation <= referenceIndentation)
            {
                // This is an empty section.
                return section;
            }

            while (true)
            {
                if (stream.IsAtEndOfStream) { break; }

                if (currentIndentation <= referenceIndentation)
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
                    var option = new ConfigOption()
                    {
                        Name = Callbacks.OptionNameProcessor(name),
                        Value = Callbacks.OptionValueProcessor(value),
                    };
                    section.AddOption(option);
                }
                else if (stream.IsAt(Markers.SectionBodyBeginMarker))
                {
                    stream.SkipWhile(_ => stream.IsAt(Markers.SectionBodyBeginMarker));

                    // We are at the beginning of a section body
                    var subSection = ParseSection(stream, sectionIndentation);
                    subSection.Name = Callbacks.SectionNameProcessor(name);
                    section.AddSection(subSection);
                }
                else if (stream.IsAt(Markers.SingleLineCommentBeginMarker)
                      || stream.IsAt(Markers.MultiLineCommentBeginMarker))
                {
                    ParseComment(stream);
                }

                SkipWhiteSpaceAndComments(stream);
                currentIndentation = DetermineLineIndentation(stream);
            }

            return section;
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

        public void CheckStream(StringStream stream, ReadStep step)
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

    public class ConfigFileReader : ConfigFileReaderWriterBase
    {
        public ConfigFileReader() : base() {}

        public ConfigFile Parse(string serializedConfigFile)
        {
            var stream = new StringStream(serializedConfigFile);
            return Parse(stream);
        }

        public ConfigFile Parse(StringStream stream)
        {
            var result = new ConfigFile();

            while (true)
            {
                stream.SkipWhile(c => char.IsWhiteSpace(c));

                if (stream.IsAtEndOfStream)
                {
                    break;
                }

                var currentCharacter = stream.PeekUnchecked();

                if (currentCharacter == Syntax.CommentPrefix)
                {
                    ParseComment(stream);
                }
                else if (currentCharacter == Syntax.SectionPrefix)
                {
                    var section = ParseSection(stream);
                    result.Sections.Add(section);
                }
                else
                {
                    var option = ParseOption(stream);
                    result.Options.Add(option);
                }
            }

            return result;
        }

        public ConfigSection ParseSection(StringStream stream)
        {
            stream.SkipUntil(c => c == Syntax.SectionPrefix);
            stream.Read(); // Read the section prefix character.

            var nameStart = new StringStream(stream);
            var nameLength = stream.SkipUntil(
                c => c == Syntax.SectionNameDelimiter || c == Syntax.KeyValueDelimiter);
            var delimiter = stream.Read(); // Read the SectionNameDelimiter ':' or the KeyValueDelimiter '='

            if (stream.IsAtEndOfStream)
            {
                // This code is reached when the cfg looks something like this:
                //     [Hello Option = Value; AnotherOption = Value]
                // or this:
                //     [Hello =]
                throw new InvalidSyntaxException(
                    string.Format("Missing section value delimiter: {0}",
                                  Syntax.SectionNameDelimiter));
            }

            var name = nameStart.Content.Substring(nameStart.Index, nameLength);
            name = SectionNamePreprocessor(name);

            if (delimiter == Syntax.KeyValueDelimiter)
            {
                var fileNameStart = new StringStream(stream);
                var fileNameLength = stream.SkipUntil(c => c == Syntax.SectionSuffix); // until ']'
                stream.Read(); // Read the SectionSuffix

                if (stream.IsAtEndOfStream)
                {
                    // This code is reached when the cfg looks something like this:
                    //     [Hello =
                    if (stream.IsAtEndOfStream)
                    {
                        throw new InvalidSyntaxException(
                            string.Format("Missing section suffix: {0}",
                                            Syntax.SectionSuffix));
                    }
                }

                var fileName = fileNameStart.Content.Substring(fileNameStart.Index, fileNameLength);
                fileName = FileNamePreprocessor(fileName);
                var cfg = ConfigFile.FromFile(fileName);
                cfg.Name = name;
                return cfg;
            }

            var section = new ConfigSection();
            section.Name = name;

            while (true)
            {
                stream.SkipWhile(c => char.IsWhiteSpace(c));

                if (stream.IsAtEndOfStream)
                {
                    throw new InvalidSyntaxException(
                        string.Format("Missing section suffix: {0}",
                                        Syntax.SectionSuffix));
                }

                if (stream.PeekUnchecked() == Syntax.CommentPrefix)
                {
                    ParseComment(stream);
                    continue;
                }

                if (stream.PeekUnchecked() == Syntax.SectionPrefix)
                {
                    var subSection = ParseSection(stream);
                    section.AddSection(subSection);
                    continue;
                }

                if (stream.PeekUnchecked() == Syntax.SectionSuffix)
                {
                    stream.Read(); // Read the SectionSuffix ']'
                    // Note: [:] is a perfectly valid section.
                    break;
                }

                var option = ParseOption(stream);
                section.AddOption(option);
            }

            return section;
        }

        public void ParseComment(StringStream stream)
        {
            stream.SkipWhile(c => char.IsWhiteSpace(c));

            if (stream.IsAtEndOfStream)
            {
                return;
            }

            var currentCharacter = stream.PeekUnchecked();

            if (currentCharacter != Syntax.CommentPrefix)
            {
                return;
            }

            stream.Read(); // Read the comment prefix '#'

            if (stream.IsAtEndOfStream)
            {
                return;
            }

            currentCharacter = stream.PeekUnchecked();

            if (currentCharacter == Syntax.MultiLineCommentPrefix)
            {
                while (true)
                {
                    if (stream.IsAtEndOfStream
                     || stream.PeekUnchecked() == Syntax.MultiLineCommentSuffix)
                    {
                        stream.Read();
                        break;
                    }

                    stream.SkipUntil(c => c == Syntax.CommentPrefix);
                    stream.Read(); // Read the comment prefix '#'
                }
            }
            else
            {
                stream.SkipUntil(_ => stream.IsAt(Syntax.NewlineDelimiter));
                stream.Next(Syntax.NewlineDelimiter.Length);
            }
        }

        public ConfigOption ParseOption(StringStream stream)
        {
            var option = new ConfigOption();

            stream.SkipWhile(c => char.IsWhiteSpace(c));

            // Read the value of the option
            {
                var nameStart = new StringStream(stream);
                var nameLength = stream.SkipUntil(c => c == Syntax.KeyValueDelimiter);
                var name = nameStart.Content.Substring(nameStart.Index, nameLength);
                option.Name = OptionNamePreprocessor(name);
            }

            // Read the value-value delimiter '='
            stream.Read();

            // Read the value of the option
            {
                var valueStart = new StringStream(stream);
                var valueLength = 0;
                while (true)
                {
                    if (stream.IsAtEndOfStream
                        || stream.PeekUnchecked() == Syntax.StatementDelimiter
                        || stream.PeekUnchecked() == Syntax.SectionPrefix
                        || stream.PeekUnchecked() == Syntax.SectionSuffix
                        || stream.PeekUnchecked() == Syntax.CommentPrefix
                        || stream.IsAt(Syntax.NewlineDelimiter))
                    {
                        break;
                    }

                    stream.Next();
                    ++valueLength;
                }

                var value = valueStart.Content.Substring(valueStart.Index, valueLength);
                option.Value = OptionValuePreprocessor(value);
            }

            return option;
        }
    }
}
