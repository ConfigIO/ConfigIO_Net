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
            var cfg = new ConfigFile() { FileName = "data/CompleteCompact.cfg" };

            cfg.Load();
            cfg.FileName = "temp/CompleteCompact.cfg";
            cfg.Save();

            // Check contents.
            var originalContent = string.Empty;
            ReadFileStream("data/CompleteCompact.cfg", FileMode.Open, FileAccess.Read,
                reader => originalContent = reader.ReadToEnd());

            var savedContent = string.Empty;
            ReadFileStream("temp/CompleteCompact.cfg", FileMode.Open, FileAccess.Read,
                reader => savedContent = reader.ReadToEnd());

            Assert.AreEqual(originalContent, savedContent);
        }
    }
}
