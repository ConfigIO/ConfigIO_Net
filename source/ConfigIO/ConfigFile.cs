﻿using Configuration.FileIO;
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

        public IConfigFileReader Parser { get; set; }

        public ConfigFileDefaults()
        {
            Culture = CultureInfo.InvariantCulture;

            // Create the instances.
            Parser = new ConfigFileReader();
            Writer = new ConfigFileWriter();

            SyntaxMarkers = new SyntaxMarkers()
                            {
                                KeyValueDelimiter = "=",
                                SectionBodyBeginMarker = ":",
                                IncludeBeginMarker = "[include]",
                                LongValueBeginMarker = "\"",
                                LongValueEndMarker = "\"",
                                SingleLineCommentBeginMarker = "//",
                                MultiLineCommentBeginMarker = "/*",
                                MultiLineCommentEndMarker = "*/",
                            };

            // Assign the necessary properties.
            Parser.Markers = SyntaxMarkers;
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
            cfg.LoadFrom(content);
            return cfg;
        }

        public static ConfigFile FromFile(string fileName)
        {
            var cfg = new ConfigFile() { FileName = fileName };
            cfg.Load();
            return cfg;
        }

        static ConfigFile()
        {
            Defaults = new ConfigFileDefaults();
        }

        #endregion

        private CultureInfo _culture;
        public CultureInfo Culture 
        {
            get { return _culture ?? Defaults.Culture; }
            set { _culture = value; }
        }

        IConfigFileWriter _writer;
        public IConfigFileWriter Writer
        {
            get { return _writer ?? Defaults.Writer; }
            set { _writer = value; }
        }

        SyntaxMarkers _syntaxMarkers;
        public SyntaxMarkers SyntaxMarkers
        {
            get { return _syntaxMarkers ?? Defaults.SyntaxMarkers; }
            set { _syntaxMarkers = value; }
        }

        IConfigFileReader _parser;
        public IConfigFileReader Parser
        {
            get { return _parser ?? Defaults.Parser; }
            set { _parser = value; }
        }

        /// <summary>
        /// Convenient access to the underlying FileInfo property.
        /// </summary>
        public string FileName { get; set; }

        public void Clear()
        {
            Options.Clear();
            Sections.Clear();
        }

        public void LoadFrom(string content)
        {
            Clear();
            ConfigFile cfg;
            using (var reader = new StringReader(content))
            {
                cfg = Parser.Parse(reader);
            }

            Options = cfg.Options;
            Sections = cfg.Sections;
        }

        /// <summary>
        /// Use the property FileInfo of this class to specify the file name.
        /// </summary>
        public void Load()
        {
            Clear();
            ConfigFile cfg = null;

            using (var reader = new FileInfo(FileName).OpenText())
            {
                cfg = Parser.Parse(reader);
            }

            Options = cfg.Options;
            Sections = cfg.Sections;
        }

        public void Save()
        {
            using (var writer = new FileInfo(FileName).CreateText())
            {
                SaveTo(writer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This method ignores this instance's <code>FileInfo</code>.</remarks>
        /// <param identifier="writer"></param>
        public void SaveTo(TextWriter writer)
        {
            Writer.Write(writer, this);
        }
    }
}
