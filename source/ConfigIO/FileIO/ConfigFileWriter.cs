using System;
using System.IO;
using System.Text;

namespace Configuration.FileIO
{
    public interface IConfigFileWriter
    {
        ConfigFileWriterCallbacks Callbacks { get; set; }
        string IndentationString { get; set; }
        SyntaxMarkers Markers { get; set; }
        string NewLine { get; set; }
        void Write(TextWriter writer, ConfigFile cfg);
    }

    public class ConfigFileWriter : Configuration.FileIO.IConfigFileWriter
    {
        public SyntaxMarkers Markers { get; set; }

        public ConfigFileWriterCallbacks Callbacks { get; set; }

        public string IndentationString { get; set; }

        public string NewLine { get; set; }

        public ConfigFileWriter() : base()
        {
            Callbacks = new ConfigFileWriterCallbacks()
            {
                SectionNameProcessor = section => section,
                OptionValueProcessor = value => value,
                OptionNameProcessor = name => name,
                FileNameProcessor = fileName => fileName,
            };
            IndentationString = new string(' ', 4); // 4 spaces
            NewLine = "\n";
        }

        public void Write(TextWriter writer, ConfigFile cfg)
        {
            var previousNewLine = writer.NewLine;

            writer.NewLine = NewLine;
            WriteSectionBody(writer, cfg, indentationLevel: 0);

            writer.NewLine = previousNewLine;
        }

        private void WriteSection(TextWriter writer, ConfigSection section, int indentationLevel)
        {
            if (string.IsNullOrWhiteSpace(section.Name))
            {
                throw new InvalidObjectStateException(
                    "The given section does not contain a valid name.");
            }

            var cfg = section as ConfigFile;
            if (cfg != null)
            {
                if (string.IsNullOrWhiteSpace(cfg.FileName))
                {
                    throw new InvalidObjectStateException(
                        "The given section is a config file but does not contain a valid file name.");
                }

                writer.Write(string.Format("{0} {1} {2} {3}", // "[include] SectionName = Path/To/File.cfg"
                                           Markers.IncludeBeginMarker,
                                           Callbacks.SectionNameProcessor(section.Name),
                                           Markers.KeyValueDelimiter,
                                           Callbacks.FileNameProcessor(cfg.FileName)));
                writer.WriteLine();
                return;
            }

            writer.WriteLine("{0}{1}", Callbacks.SectionNameProcessor(section.Name), Markers.SectionBodyBeginMarker); // "SectionName:\n"
            WriteSectionBody(writer, section, indentationLevel);
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
                writer.WriteLine();
                Indent(writer, indentationLevel);
                WriteSection(writer, subSection, indentationLevel + 1);
            }
        }

        private void WriteOption(TextWriter writer, ConfigOption option)
        {
            writer.Write(Callbacks.OptionNameProcessor(option.Name));

            if (option.Value != string.Empty)
            {
                writer.Write(" {0} ", Markers.KeyValueDelimiter);
                var value = Callbacks.OptionValueProcessor(option.Value);
                var isLongValue = value.Contains(writer.NewLine);

                if (isLongValue)
                {
                    writer.Write(Markers.LongValueBeginMarker);
                    writer.Write(value);
                    writer.Write(Markers.LongValueEndMarker);
                }
                else
                {
                    writer.Write(value);
                }
            }

            writer.WriteLine();
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
