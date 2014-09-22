using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigFileReaderWriterCallbacks
    {
        [TestMethod]
        public void TestReaderCallbacks()
        {
            ConfigFile.Reader.Callbacks.OptionNameProcessor =
                str =>
                {
                    var trimmed = str.Trim();

                    if (trimmed == "a")
                    {
                        return "First option name";
                    }

                    return trimmed;
                };

            ConfigFile.Reader.Callbacks.OptionValueProcessor =
                str =>
                {
                    var trimmed = str.Trim();

                    if (trimmed.StartsWith("1"))
                    {
                        return string.Format("Option that starts with 1: {0}", trimmed);
                    }

                    return trimmed;
                };

            ConfigFile.Reader.Callbacks.SectionNameProcessor =
                str => str.Trim().ToUpper();

            var cfg = ConfigFile.FromFile("data/Complete.cfg");

            // Global
            Assert.AreEqual(3, cfg.Options.Count);
            Assert.AreEqual(3.1415f, cfg.GetOption("pi"));
            Assert.AreEqual(42, cfg.GetOption("fortyTwo"));
            Assert.AreEqual(666, cfg.GetOption("lastOption"));
            Assert.AreEqual(2, cfg.Sections.Count);

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
        }
    }
}
