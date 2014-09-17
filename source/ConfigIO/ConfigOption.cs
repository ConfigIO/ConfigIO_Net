using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class ConfigOption
    {
        public string Comment { get; set; }

        public string Value { get; set; }
        
        public ConfigOption()
        {
            Value = string.Empty;
        }

        public ConfigOption(string value)
        {
            Value = value;
        }
        public ConfigOption(bool value)
        {
            Value = value.ToString();
        }
        public ConfigOption(byte value)
        {
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(char value)
        {
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(short value)
        {
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(int value)
        {
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(long value)
        {
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(float value)
        {
            Value = value.ToString(ConfigFile.CurrentCulture);
        }
        public ConfigOption(double value)
        {
            Value = value.ToString(ConfigFile.CurrentCulture);
        }

        public override string ToString()
        {
            return Value;
        }

        #region Implicit conversion operators to a ConfigOption

        public static implicit operator ConfigOption(bool value)
        {
            return new ConfigOption(value);
        }

        public static implicit operator ConfigOption(byte value)
        {
            return new ConfigOption(value);
        }

        public static implicit operator ConfigOption(char value)
        {
            return new ConfigOption(value);
        }

        public static implicit operator ConfigOption(short value)
        {
            return new ConfigOption(value);
        }

        public static implicit operator ConfigOption(int value)
        {
            return new ConfigOption(value);
        }

        public static implicit operator ConfigOption(long value)
        {
            return new ConfigOption(value);
        }

        public static implicit operator ConfigOption(float value)
        {
            return new ConfigOption(value);
        }

        public static implicit operator ConfigOption(double value)
        {
            return new ConfigOption(value);
        }

        public static implicit operator ConfigOption(string value)
        {
            return new ConfigOption(value);
        }

        #endregion

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
