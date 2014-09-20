using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.FileIO
{
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
