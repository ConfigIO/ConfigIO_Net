using System;
using System.IO;
using System.Text;

namespace Configuration.FileIO
{
    public class ConfigFileWriter : ConfigFileReaderWriterBase
    {
        public string IndentationString { get; set; }

        /// <summary>
        /// Wether to write statement delimiters or not (usually ";")
        /// </summary>
        /// <example>
        /// With statement delimiters:
        /// <code>
        /// [SectionName:
        ///     Option0 = Value0;
        ///     Option1 = Value1;
        /// ]
        /// </code>
        /// Without statement delimiters:
        /// <code>
        /// [SectionName:
        ///     Option0 = Value0
        ///     Option1 = Value1
        /// ]
        /// </code>
        /// </example>
        public bool WriteStatementDelimiters { get; set; }

        public ConfigFileWriter() : base()
        {
            IndentationString = new string(' ', 4); // 4 spaces
            WriteStatementDelimiters = false;
        }

        public void Write(TextWriter writer, ConfigFile cfg)
        {
            writer.NewLine = Syntax.NewlineDelimiter;
            WriteSectionBody(writer, cfg, indentationLevel: 0);
        }

        private void WriteSection(TextWriter writer, ConfigSection section, int indentationLevel)
        {
            if (string.IsNullOrWhiteSpace(section.Name))
            {
                throw new InvalidObjectStateException(
                    "The given section does not contain a valid name.");
            }

            writer.Write(string.Format("{0}{1}", // "[SectionName"
                                       Syntax.SectionPrefix,
                                       section.Name));

            var cfg = section as ConfigFile;
            if (cfg != null)
            {
                if (string.IsNullOrWhiteSpace(cfg.FileName))
                {
                    throw new InvalidObjectStateException(
                        "The given section is a config file but does not contain a valid file name.");
                }

                writer.Write(string.Format(" {0} {1}{2}", // " = Path/To/File.cfg]"
                                           Syntax.KeyValueDelimiter,
                                           cfg.FileName,
                                           Syntax.SectionSuffix));
            }

            writer.WriteLine(Syntax.SectionNameDelimiter); // ":\n"
            WriteSectionBody(writer, section, indentationLevel);
            Indent(writer, indentationLevel - 1);
            writer.WriteLine(Syntax.SectionSuffix);
        }

        private void WriteSectionBody(TextWriter writer, ConfigSection section, int indentationLevel)
        {
            foreach (var option in section.Options)
            {
                Indent(writer, indentationLevel);
                WriteOption(writer, option);
            }

            foreach (var subSection in section.Sections)
            {
                Indent(writer, indentationLevel);
                WriteSection(writer, subSection, indentationLevel + 1);
            }
        }

        private void WriteOption(TextWriter writer, ConfigOption option)
        {
            writer.WriteLine(string.Format("{0} {1} {2}",
                                           option.Name,
                                           Syntax.KeyValueDelimiter,
                                           option.Value));
        }

        private void Indent(TextWriter writer, int indentationLevel)
        {
            for (int i = 0; i < indentationLevel; i++)
            {
                writer.Write(IndentationString);
            }
        }
    }
}
