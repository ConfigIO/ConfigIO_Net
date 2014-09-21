using Configuration.FileIO;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Configuration
{

    public enum ConfigFileSaveOptions
    {

    }
    
    public class ConfigFile : ConfigSection
    {
        #region Static

        public static CultureInfo CurrentCulture { get; set; }

        public static ConfigFileWriter Writer { get; set; }

        public static SyntaxMarkers SyntaxMarkers { get; set; }

        public static ConfigFileReader Reader { get; set; }

        static ConfigFile()
        {
            CurrentCulture = CultureInfo.InvariantCulture;

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

        public static ConfigFile FromFile(string fileName)
        {
            var content = string.Empty;
            using (var fileStream = File.OpenText(fileName))
            {
                content = fileStream.ReadToEnd();
            }
            var cfg = FromString(content);
            cfg.FileName = fileName;
            return cfg;
        }

        public static ConfigFile FromString(string content)
        {
            return Reader.Parse(content);
        }

        #endregion Static

        public string FileName { get; set; }

        internal ConfigFile() : base()
        {
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
