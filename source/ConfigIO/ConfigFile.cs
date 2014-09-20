using Configuration.FileIO;
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

        public static ConfigFileReader Parser { get; set; }

        public static ConfigFileWriter Writer { get; set; }

        static ConfigFile()
        {
            CurrentCulture = CultureInfo.InvariantCulture;
            Parser = new ConfigFileReader();
            Writer = new ConfigFileWriter();
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
            return Parser.Parse(content);
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
        /// <param name="writer"></param>
        public void SaveToFile(TextWriter writer)
        {
            Writer.Write(writer, this);
        }
    }
}
