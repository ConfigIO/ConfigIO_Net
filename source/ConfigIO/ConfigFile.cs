using Configuration.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Configuration
{
    public class ConfigFileDefaults
    {
        public CultureInfo Culture { get; set; }

        public IConfigFileWriter Writer { get; set; }

        public SyntaxMarkers SyntaxMarkers { get; set; }

        public IConfigFileReader Reader { get; set; }

        public ConfigFileDefaults()
        {
            Culture = CultureInfo.InvariantCulture;

            // Create the instances.
            Reader = new ConfigFileReader();
            Writer = new ConfigFileWriter();

            SyntaxMarkers = new SyntaxMarkers()
            {
                KeyValueDelimiter = "=",
                SectionBodyBeginMarker = ":",
                IncludeBeginMarker = "[include]",
                SingleLineCommentBeginMarker = "//",
                MultiLineCommentBeginMarker = "/*",
                MultiLineCommentEndMarker = "*/",
            };

            // Assign the necessary properties.
            Reader.Markers = SyntaxMarkers;
            Writer.Markers = SyntaxMarkers;
        }
    }
    
    public class ConfigFile : ConfigSection
    {
        #region Static

        public static ConfigFileDefaults Defaults { get; set; }

        public static ConfigFile FromString(string content)
        {
            var cfg = new ConfigFile();
            cfg.LoadFromString(content);
            return cfg;
        }

        public static ConfigFile FromFile(string fileName)
        {
            var cfg = new ConfigFile() { FileName = fileName };
            cfg.LoadFromFile();
            return cfg;
        }

        static ConfigFile()
        {
            Defaults = new ConfigFileDefaults();
        }

        #endregion

        public CultureInfo Culture { get; set; }

        public IConfigFileWriter Writer { get; set; }

        public SyntaxMarkers SyntaxMarkers { get; set; }

        public IConfigFileReader Reader { get; set; }

        public string FileName { get; set; }

        public ConfigFile() : base()
        {
            Culture = Defaults.Culture;
            SyntaxMarkers = Defaults.SyntaxMarkers;
            Writer = Defaults.Writer;
            Reader = Defaults.Reader;
        }

        public void Clear()
        {
            Options.Clear();
            Sections.Clear();
        }

        public void LoadFromString(string content)
        {
            Clear();
            ConfigFile cfg;
            using (var reader = new StringReader(content))
            {
                cfg = Reader.Parse(reader);
            }

            Options = cfg.Options;
            Sections = cfg.Sections;
        }

        /// <summary>
        /// Use the property FileName of this class to specify the file name.
        /// </summary>
        public void LoadFromFile()
        {
            Clear();
            ConfigFile cfg;
            using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    cfg = Reader.Parse(reader);
                }
            }

            Options = cfg.Options;
            Sections = cfg.Sections;
        }

        public void SaveToFile()
        {
            using (var fileStream = new FileStream(FileName, FileMode.Create))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    Writer.Write(writer, this);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This method ignores this instance's <code>FileName</code>.</remarks>
        /// <param identifier="writer"></param>
        public void SaveToFile(TextWriter writer)
        {
            Writer.Write(writer, this);
        }
    }
}
