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
    }

    [TestClass]
    public class Test_TestFiles
    {
        [TestMethod]
        public void TestComplete()
        {
            Assert.Inconclusive("Not implemented.");
        }

        [TestMethod]
        public void TestGlobalOptionAndSection()
        {
            Assert.Inconclusive("Not implemented.");
        }

        [TestMethod]
        public void TestGlobalOptions2()
        {
            Assert.Inconclusive("Not implemented.");
        }

        [TestMethod]
        public void TestInlineComment()
        {
            Assert.Inconclusive("Not implemented.");
        }

        [TestMethod]
        public void TestMultiLineComment()
        {
            Assert.Inconclusive("Not implemented.");
        }

        [TestMethod]
        public void TestSubSectionOptions()
        {
            Assert.Inconclusive("Not implemented.");
        }
    }
}
