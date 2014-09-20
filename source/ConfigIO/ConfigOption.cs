using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration.FileIO;

namespace Configuration
{
    public class ConfigOption
    {
        public string Name { get; set; }

        public string Value { get; set; }
        
        public ConfigOption()
        {
            Name = string.Empty;
            Value = string.Empty;
        }

        public ConfigOption(string name)
        {
            Name = name;
            Value = string.Empty;
        }

        public ConfigOption(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public ConfigOption(string name, bool value)
        {
            Name = name;
            Value = value.ToString();
        }
        public ConfigOption(string name, byte value)
        {
            Name = name;
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(string name, char value)
        {
            Name = name;
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(string name, short value)
        {
            Name = name;
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(string name, int value)
        {
            Name = name;
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(string name, long value)
        {
            Name = name;
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(string name, float value)
        {
            Name = name;
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(string name, double value)
        {
            Name = name;
            Value = value.ToString(ConfigFile.CurrentCulture);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}",
                Name, ConfigFile.Parser.Syntax.KeyValueDelimiter, Value);
        }

        #region Implicit conversion operators to other types

        public static implicit operator bool(ConfigOption option)
        {
            return bool.Parse(option.Value);
        }

        public static implicit operator byte(ConfigOption option)
        {
            return byte.Parse(option.Value, ConfigFile.CurrentCulture);
        }

        public static implicit operator char(ConfigOption option)
        {
            return char.Parse(option.Value);
        }

        public static implicit operator short(ConfigOption option)
        {
            return short.Parse(option.Value, ConfigFile.CurrentCulture);
        }

        public static implicit operator int(ConfigOption option)
        {
            return int.Parse(option.Value, ConfigFile.CurrentCulture);
        }

        public static implicit operator long(ConfigOption option)
        {
            return long.Parse(option.Value, ConfigFile.CurrentCulture);
        }

        public static implicit operator float(ConfigOption option)
        {
            return float.Parse(option.Value, ConfigFile.CurrentCulture);
        }

        public static implicit operator double(ConfigOption option)
        {
            return double.Parse(option.Value, ConfigFile.CurrentCulture);
        }

        public static implicit operator string(ConfigOption option)
        {
            return option.Value;
        }

        #endregion
    }
}
