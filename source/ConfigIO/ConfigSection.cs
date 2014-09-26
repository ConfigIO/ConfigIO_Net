using System.Collections.Generic;
using System.Linq;

namespace Configuration
{
    public delegate ConfigOption ConfigOptionExtractor(string name);

    /// <summary>
    /// Use it like this:
    /// <code>var option = rootSection["SubSection"]["SubSubSection"].GetOption("ReadName");</code>
    /// </summary>
    public class ConfigSection
    {
        public ConfigFile Owner { get; set; }

        public string Name { get; set; }

        public IList<ConfigSection> Sections { get; set; }

        public IList<ConfigOption> Options { get; set; }

        public ConfigSection()
        {
            Name = string.Empty;
            Options = new List<ConfigOption>();
            Sections = new List<ConfigSection>();
        }

        public ConfigOption GetOption(string name)
        {
            return Options.Single(o => o.Name == name);
        }

        public void AddOption(ConfigOption option)
        {
            option.Owner = Owner;
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].Name == option.Name)
                {
                    Options[i].Owner = null;
                    Options[i] = option;
                    return;
                }
            }
            Options.Add(option);
        }

        public void RemoveOption(string name)
        {
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].Name == name)
                {
                    Options[i].Owner = null;
                    Options.RemoveAt(i);
                    return;
                }
            }
        }

        public ConfigSection GetSection(string name)
        {
            return Sections.Single(s => s.Name == name);
        }

        public void AddSection(ConfigSection section)
        {
            section.Owner = Owner;
            for (int i = 0; i < Sections.Count; i++)
            {
                if (Sections[i].Name == section.Name)
                {
                    Sections[i] = section;
                    return;
                }
            }
            Sections.Add(section);
        }

        public void RemoveSection(string name)
        {
            for (int i = 0; i < Sections.Count; i++)
            {
                if (Sections[i].Name == name)
                {
                    Sections[i].Owner = null;
                    Sections.RemoveAt(i);
                    return;
                }
            }
        }

        public ConfigSection this[string name]
        {
            get { return GetSection(name); }
            set
            {
                RemoveSection(name);
                AddSection(value);
            }
        }
    }
}
