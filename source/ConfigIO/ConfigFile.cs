using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace Configuration
{
    public class ConfigFile : ConfigSection
    {
        #region Static

        public static CultureInfo CurrentCulture { get; set; }

        public static ConfigFileParser Parser { get; set; }

        static ConfigFile()
        {
            CurrentCulture = CultureInfo.InvariantCulture;
            Parser = new ConfigFileParser();
        }

        public static ConfigSection FromFile(string fileName)
        {
            var content = string.Empty;
            using (var fileStream = File.OpenText(fileName))
            {
                content = fileStream.ReadToEnd();
            }
            return FromString(content);
        }

        public static ConfigSection FromString(string content)
        {
            return Parser.Parse(content);
        }

        #endregion Static

        public string FileName { get; set; }

        internal ConfigFile() : base()
        {
        }
    }
}
