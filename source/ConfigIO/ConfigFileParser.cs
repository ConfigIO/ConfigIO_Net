using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class SyntaxCharacters
    {
        /// <summary>
        ///    # This is a comment
        /// ---^
        /// </summary>
        public char CommentPrefix { get; set; }

        /// <summary>
        ///   #[ Multiline comment. ]
        /// ---^
        /// </summary>
        public char MultiLineCommentPrefix { get; set; }

        /// <summary>
        ///   #[ Multiline comment. ]
        /// ------------------------^
        /// </summary>
        public char MultiLineCommentSuffix { get; set; }

        /// <summary>
        /// [Section: a = 1; b = 2;]
        /// --------^
        /// </summary>
        public char SectionPrefix { get; set; }

        /// <summary>
        /// [ThisIsMyName: a = 1; b = 2;]
        /// -------------^
        /// </summary>
        public char SectionNameDelimiter { get; set; }

        /// <summary>
        /// [Section: a = 1; b = 2;]
        /// -----------------------^
        /// </summary>
        public char SectionSuffix { get; set; }

        /// <summary>
        /// [Section: a = 1; b = 2;]
        /// ---------------^------^
        /// <remarks>Note that a line-break is also a statement delimiter.</remarks>
        /// </summary>
        public char StatementDelimiter { get; set; }

        /// <summary>
        /// [Section: a = 1; b = 2;]
        /// ------------^------^
        /// </summary>
        public char OptionNameValueDelimiter { get; set; }

        /// <summary>
        /// A string of characters defining a new-line (in its entirety).
        /// e.g. "\n" or "\r\n"
        /// <remarks>"\r\n" and "\n\r" are NOT treated as the same!</remarks>
        /// </summary>
        public string NewlineDelimiter { get; set; }
    }

    public delegate string PreprocessorCallback(string toProcess);

    public class ConfigFileParser
    {
        public SyntaxCharacters Syntax { get; set; }

        public PreprocessorCallback SectionNamePreprocessor { get; set; }

        public PreprocessorCallback OptionNamePreprocessor { get; set; }

        public PreprocessorCallback OptionValuePreprocessor { get; set; }

        public ConfigFileParser()
        {
            Syntax = new SyntaxCharacters();
            Syntax.CommentPrefix = '#';
            Syntax.MultiLineCommentPrefix = '[';
            Syntax.MultiLineCommentSuffix = ']';
            Syntax.SectionPrefix = '[';
            Syntax.SectionNameDelimiter = ':';
            Syntax.SectionSuffix = ']';
            Syntax.StatementDelimiter = ';';
            Syntax.OptionNameValueDelimiter = '=';
            Syntax.NewlineDelimiter = "\n";

            SectionNamePreprocessor = section => section.Trim();
            OptionNamePreprocessor = key => key.Trim();
            OptionValuePreprocessor = value => value.Trim();
        }

        public ConfigSection Parse(string serializedConfigFile)
        {
            var stream = new StringStream(serializedConfigFile);
            return Parse(stream);
        }

        public ConfigSection Parse(StringStream stream)
        {
            var result = new ConfigSection();

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
            var section = new ConfigSection();

            stream.SkipUntil(c => c == Syntax.SectionPrefix);
            stream.Read(); // Read the section prefix character.

            var nameStart = new StringStream(stream);
            var nameLength = stream.SkipUntil(c => c == Syntax.SectionNameDelimiter);
            stream.Read(); // Read the SectionNameDelimiter ':'

            if (stream.IsAtEndOfStream)
            {
                throw new InvalidSyntaxException(
                    string.Format("Missing section value delimiter: {0}",
                                  Syntax.SectionNameDelimiter));
            }

            var name = nameStart.Content.Substring(nameStart.Index, nameLength);
            section.Name = SectionNamePreprocessor(name);

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
                var nameLength = stream.SkipUntil(c => c == Syntax.OptionNameValueDelimiter);
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
