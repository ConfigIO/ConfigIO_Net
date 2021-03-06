﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigFileReader : TestBase
    {
        [TestMethod]
        public void TestComplete()
        {
            var cfg = ConfigFile.FromFile("data/Complete.cfg");

            // Global
            Assert.AreEqual(5, cfg.Options.Count);
            Assert.AreEqual(3.1415f, cfg.GetOption("pi"));
            Assert.AreEqual(42, cfg.GetOption("fortyTwo"));
            Assert.AreEqual("This is a long value\n             that spans multiple lines.", cfg.GetOption("longValue"));
            Assert.AreEqual(666, cfg.GetOption("lastOption"));
            Assert.AreEqual(string.Empty, cfg.GetOption("optionWithoutValue"));
            Assert.AreEqual(3, cfg.Sections.Count);

            // Section0
            Assert.AreEqual(3, cfg["Section0"].Options.Count);
            Assert.AreEqual(0, cfg["Section0"].GetOption("a"));
            Assert.AreEqual(1, cfg["Section0"].GetOption("b"));
            Assert.AreEqual(2, cfg["Section0"].GetOption("c"));
            Assert.AreEqual(1, cfg["Section0"].Sections.Count);

            // Section0/SubSection0
            Assert.AreEqual(3, cfg["Section0"]["SubSection0"].Options.Count);
            Assert.AreEqual(3, cfg["Section0"]["SubSection0"].GetOption("d"));
            Assert.AreEqual(4, cfg["Section0"]["SubSection0"].GetOption("e"));
            Assert.AreEqual(5, cfg["Section0"]["SubSection0"].GetOption("f"));
            Assert.AreEqual(0, cfg["Section0"]["SubSection0"].Sections.Count);

            // Section1
            Assert.AreEqual(3, cfg["Section1"].Options.Count);
            Assert.AreEqual(10, cfg["Section1"].GetOption("a"));
            Assert.AreEqual(11, cfg["Section1"].GetOption("b"));
            Assert.AreEqual(12, cfg["Section1"].GetOption("c"));
            Assert.AreEqual(1, cfg["Section1"].Sections.Count);

            // Section1/SubSection0
            Assert.AreEqual(3, cfg["Section1"]["SubSection0"].Options.Count);
            Assert.AreEqual(13, cfg["Section1"]["SubSection0"].GetOption("d"));
            Assert.AreEqual(14, cfg["Section1"]["SubSection0"].GetOption("e"));
            Assert.AreEqual(15, cfg["Section1"]["SubSection0"].GetOption("f"));
            Assert.AreEqual(0, cfg["Section1"]["SubSection0"].Sections.Count);

            // Section1/SubSection0
            Assert.AreEqual(1, cfg["IncludedSection"].Options.Count);
            Assert.AreEqual("value1", cfg["IncludedSection"].GetOption("Global0"));
            Assert.AreEqual(0, cfg["IncludedSection"].Sections.Count);
        }

        [TestMethod]
        public void TestGlobalOptionAndSection()
        {
            var cfg = ConfigFile.FromFile("data/GlobalOptionAndSection.cfg");
        }

        [TestMethod]
        public void TestGlobalOptions2()
        {
            var cfg = ConfigFile.FromFile("data/GlobalOptions2.cfg");
            Assert.AreEqual(2, cfg.Options.Count);
            Assert.AreEqual(0, cfg.Sections.Count);
            Assert.AreEqual(0, cfg.GetOption("Option0"));
            Assert.AreEqual(1, cfg.GetOption("Option1"));
        }

        [TestMethod]
        public void TestInlineComment()
        {
            var cfg = ConfigFile.FromFile("data/InlineComment.cfg");
            Assert.AreEqual(1, cfg.Options.Count);
            Assert.AreEqual(42, cfg.GetOption("Option0"));
        }

        [TestMethod]
        public void TestMultiLineComment()
        {
            var cfg = ConfigFile.FromFile("data/MultiLineComment.cfg");
            Assert.AreEqual(1, cfg.Options.Count);
            Assert.AreEqual(1337, cfg.GetOption("Option0"));
        }

        [TestMethod]
        public void TestSubSectionOptions()
        {
            var cfg = ConfigFile.FromFile("data/SubSectionOptions.cfg");
            Assert.AreEqual(1, cfg.Options.Count);
            Assert.AreEqual(42, cfg.GetOption("Option0"));
            Assert.AreEqual(1, cfg.Sections.Count);

            Assert.AreEqual(1, cfg["Section0"].Options.Count);
            Assert.AreEqual(43, cfg["Section0"].GetOption("Option0"));
            Assert.AreEqual(1, cfg["Section0"].Sections.Count);

            Assert.AreEqual(1, cfg["Section0"]["SubSection0"].Options.Count);
            Assert.AreEqual(44, cfg["Section0"]["SubSection0"].GetOption("Option0"));
            Assert.AreEqual(0, cfg["Section0"]["SubSection0"].Sections.Count);
        }

        [TestMethod]
        public void TestSection()
        {
            var cfg = ConfigFile.FromFile("data/Section.cfg");
            Assert.AreEqual(1, cfg.Sections.Count);
            Assert.AreEqual("SectionName", cfg["SectionName"].Name);
            Assert.AreEqual(0, cfg["SectionName"].Options.Count);
        }

        [TestMethod]
        public void TestIncludeOtherFile()
        {
            var cfg = ConfigFile.FromFile("data/IncludeOtherFile.cfg");
            Assert.AreEqual(1, cfg.Options.Count);
            Assert.AreEqual(1, cfg.Sections.Count);
            Assert.AreEqual("value0", cfg.GetOption("Global0"));
            Assert.AreEqual("OtherConfigFile", cfg["OtherConfigFile"].Name);
            Assert.IsTrue(cfg["OtherConfigFile"] is ConfigFile);

            Assert.AreEqual(1, cfg["OtherConfigFile"].Options.Count);
            Assert.AreEqual("value1", cfg["OtherConfigFile"].GetOption("Global0"));
        }
    }
}
