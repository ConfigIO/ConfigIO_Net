using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace Configuration
{
    public static class ConfigFile
    {
        static public string GlobalSectionName { get; set; }

        public static CultureInfo CurrentCulture { get; set; }

        public static ConfigFileParser Parser { get; set; }

        static ConfigFile()
        {
            GlobalSectionName = "";
            CurrentCulture = CultureInfo.InvariantCulture;
            Parser = new ConfigFileParser();
        }

        static public ConfigSection FromFile(string fileName)
        {
            var content = string.Empty;
            using (var fileStream = File.OpenText(fileName))
            {
                content = fileStream.ReadToEnd();
            }
            return FromString(content);
        }

        static public ConfigSection FromString(string content)
        {
            return Parser.Parse(content);
        }

    }
}
