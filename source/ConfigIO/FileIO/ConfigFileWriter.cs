using System;
using System.IO;
using System.Text;

namespace Configuration.FileIO
{
    public class ConfigFileWriter
    {
        public SyntaxMarkers Markers { get; set; }

        public string IndentationString { get; set; }

        public ConfigFileWriter() : base()
        {
            IndentationString = new string(' ', 4); // 4 spaces
        }

        public void Write(TextWriter writer, ConfigFile cfg)
        {
            writer.NewLine = "\n";
            WriteSectionBody(writer, cfg, indentationLevel: 0);
        }

        private void WriteSection(TextWriter writer, ConfigSection section, int indentationLevel)
        {
            if (string.IsNullOrWhiteSpace(section.Name))
            {
                throw new InvalidObjectStateException(
                    "The given section does not contain a valid identifier.");
            }

            var cfg = section as ConfigFile;
            if (cfg != null)
            {
                if (string.IsNullOrWhiteSpace(cfg.FileName))
                {
                    throw new InvalidObjectStateException(
                        "The given section is a config file but does not contain a valid file identifier.");
                }

                writer.Write(string.Format("{0} {1} {2} {3}", // "[include] SectionName = Path/To/File.cfg"
                                           Markers.IncludeBeginMarker,
                                           section.Name,
                                           Markers.KeyValueDelimiter,
                                           cfg.FileName));
            }

            writer.WriteLine("{0}{1}", section.Name, Markers.SectionBodyBeginMarker); // "SectionName:\n"
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
                Indent(writer, indentationLevel);
                WriteSection(writer, subSection, indentationLevel + 1);
            }
        }

        private void WriteOption(TextWriter writer, ConfigOption option)
        {
            writer.WriteLine(string.Format("{0} {1} {2}",
                                           option.Name,
                                           Markers.KeyValueDelimiter,
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
