using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigFile : TestBase
    {
        [TestMethod]
        public void TestSavingAndLoading()
        {
            var cfg = new ConfigFile() { FilePath = "data/CompleteCompact.cfg" };

            cfg.Load();
            cfg.FilePath = "temp/CompleteCompact.cfg";
            cfg.Save();

            // Check contents.
            var originalContent = string.Empty;
            using (var reader = new FileInfo("data/CompleteCompact.cfg").OpenText())
            {
                originalContent = reader.ReadToEnd();
            }

            var savedContent = string.Empty;
            using (var reader = new FileInfo("temp/CompleteCompact.cfg").OpenText())
            {
                savedContent = reader.ReadToEnd();
            }

            Assert.AreEqual(originalContent, savedContent);
        }
    }
}
