using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration.FileIO;

namespace Configuration
{
    public delegate ConfigOption ConfigOptionExtractor(string name);

    /// <summary>
    /// Use it like this:
    /// <code>var option = rootSection["SubSection"]["SubSubSection"].GetOption("Name");</code>
    /// </summary>
    public class ConfigSection
    {
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
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].Name == option.Name)
                {
                    Options[i] = option;
                    return;
                }
            }
            Options.Add(option);
        }

        public void RemoveOption(ConfigOption option)
        {
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i] == option)
                {
                    Options.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveOption(string name)
        {
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].Name == name)
                {
                    Options.RemoveAt(i);
                    return;
                }
            }
        }

        public ConfigSection GetSection(string name)
        {
            return Sections.Single(s => s.Name == name);
        }

        /// <summary>
        /// Gets a section that is below this section somewhere.
        /// </summary>
        /// <remarks>The order of the names in the <paramref name="names"/> array is important!</remarks>
        /// <param name="names">The names of the sections to find, in order.</param>
        /// <returns></returns>
        public ConfigSection GetSection(params string[] names)
        {
            var current = this;

            foreach (var name in names)
            {
                current = current.GetSection(name);
            }

            return current;
        }

        public void AddSection(ConfigSection section)
        {
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

        public void RemoveSection(ConfigSection section)
        {
            for (int i = 0; i < Sections.Count; i++)
            {
                if (Sections[i] == section)
                {
                    Sections.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveSection(string name)
        {
            for (int i = 0; i < Sections.Count; i++)
            {
                if (Sections[i].Name == name)
                {
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
