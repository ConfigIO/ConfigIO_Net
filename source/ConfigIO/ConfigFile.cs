using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using Configuration.FileIO;

namespace Configuration
{
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
            if (string.IsNullOrWhiteSpace(FileName))
            {
                throw new InvalidFileNameException(string.Format("FileName: \"{0}\"", FileName));
            }

            Writer.Write(this);
        }
    }
}
