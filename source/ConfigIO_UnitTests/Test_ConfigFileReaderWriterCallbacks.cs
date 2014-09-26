using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigFileReaderWriterCallbacks : TestBase
    {
        static readonly string cfgContent = "Option0 = Value0\nOption1 = Value1\nSection0:\n    Inner0 = Value2\n    InnerSection0:\n        InnerSub0 = Value3\n";

        [TestMethod]
        public void TestReaderCallbacks()
        {
            ConfigFile.Defaults.Parser.Callbacks.OptionNameProcessor =
                str =>
                {
                    var trimmed = str.Trim();

                    if (trimmed == "a")
                    {
                        return "First option name";
                    }

                    return trimmed;
                };

            ConfigFile.Defaults.Parser.Callbacks.OptionValueProcessor =
                str =>
                {
                    var trimmed = str.Trim();

                    if (trimmed.StartsWith("1"))
                    {
                        return string.Format("Option that starts with 1: {0}", trimmed);
                    }

                    return trimmed;
                };

            ConfigFile.Defaults.Parser.Callbacks.SectionNameProcessor =
                str => str.Trim().ToUpper();

            var cfg = ConfigFile.FromFile("data/Complete.cfg");

            // Global
            Assert.AreEqual(4, cfg.Options.Count);
            Assert.AreEqual(3.1415f, cfg.GetOption("pi"));
            Assert.AreEqual(string.Empty, cfg.GetOption("optionWithoutValue"));
            Assert.AreEqual(42, cfg.GetOption("fortyTwo"));
            Assert.AreEqual(666, cfg.GetOption("lastOption"));
            Assert.AreEqual(3, cfg.Sections.Count);

            // SECTION0
            Assert.AreEqual(3, cfg["SECTION0"].Options.Count);
            Assert.AreEqual(0, cfg["SECTION0"].GetOption("First option name"));
            Assert.AreEqual(string.Format("Option that starts with 1: {0}", 1), cfg["SECTION0"].GetOption("b"));
            Assert.AreEqual(2, cfg["SECTION0"].GetOption("c"));
            Assert.AreEqual(1, cfg["SECTION0"].Sections.Count);

            // SECTION0/SubSECTION0
            Assert.AreEqual(3, cfg["SECTION0"]["SUBSECTION0"].Options.Count);
            Assert.AreEqual(3, cfg["SECTION0"]["SUBSECTION0"].GetOption("d"));
            Assert.AreEqual(4, cfg["SECTION0"]["SUBSECTION0"].GetOption("e"));
            Assert.AreEqual(5, cfg["SECTION0"]["SUBSECTION0"].GetOption("f"));
            Assert.AreEqual(0, cfg["SECTION0"]["SUBSECTION0"].Sections.Count);

            // SECTION1
            Assert.AreEqual(3, cfg["SECTION1"].Options.Count);
            Assert.AreEqual(string.Format("Option that starts with 1: {0}", 10), cfg["SECTION1"].GetOption("First option name"));
            Assert.AreEqual(string.Format("Option that starts with 1: {0}", 11), cfg["SECTION1"].GetOption("b"));
            Assert.AreEqual(string.Format("Option that starts with 1: {0}", 12), cfg["SECTION1"].GetOption("c"));
            Assert.AreEqual(1, cfg["SECTION1"].Sections.Count);

            // SECTION1/SUBSECTION0
            Assert.AreEqual(3, cfg["SECTION1"]["SUBSECTION0"].Options.Count);
            Assert.AreEqual(string.Format("Option that starts with 1: {0}", 13), cfg["SECTION1"]["SUBSECTION0"].GetOption("d"));
            Assert.AreEqual(string.Format("Option that starts with 1: {0}", 14), cfg["SECTION1"]["SUBSECTION0"].GetOption("e"));
            Assert.AreEqual(string.Format("Option that starts with 1: {0}", 15), cfg["SECTION1"]["SUBSECTION0"].GetOption("f"));
            Assert.AreEqual(0, cfg["SECTION1"]["SUBSECTION0"].Sections.Count);

            // INCLUDEDSECTION
            Assert.AreEqual(1, cfg["INCLUDEDSECTION"].Options.Count);
            Assert.AreEqual("value1", cfg["INCLUDEDSECTION"].GetOption("Global0"));
            Assert.AreEqual(1, cfg["SECTION0"].Sections.Count);
        }

        [TestMethod]
        public void TestWriterCallbacks()
        {
            ConfigFile.Defaults.Writer.Callbacks.OptionNameProcessor =
                optionName => optionName.Trim().ToUpper();

            ConfigFile.Defaults.Writer.Callbacks.OptionValueProcessor =
                optionName => optionName.Trim().ToLower();

            var cfg = ConfigFile.FromString(cfgContent);

            var savedCfgContentBuilder = new StringBuilder();
            using (var savedCfgStream = new StringWriter(savedCfgContentBuilder))
            {
                cfg.SaveTo(savedCfgStream);
            }

            string newContent = "OPTION0 = value0\nOPTION1 = value1\n\nSection0:\n    INNER0 = value2\n\n    InnerSection0:\n        INNERSUB0 = value3\n";

            var savedContent = savedCfgContentBuilder.ToString();
            Assert.AreEqual(newContent, savedContent);
        }
    }
}
