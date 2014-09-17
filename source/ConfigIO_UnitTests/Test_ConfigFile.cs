using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigFile
    {
        [TestMethod]
        public void DefaultCreate()
        {
            var cfg = new ConfigFile();

            Assert.AreEqual("", ConfigFile.GlobalSectionName, "If you changed the global section's name, you also have to update this unit test.");
            Assert.AreSame(cfg.GlobalSection, cfg[""]);
        }

        [TestMethod]
        public void CreateFromFile()
        {
            var cfg = ConfigFile.FromFile("data/sample.cfg");

            Assert.AreEqual<int>(1337, cfg[""]["globalOption"]);
            return;
        }
    }
}
