using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigFileWriter : TestBase
    {
        static readonly string cfgContent = "Option0 = Value0\nOption1 = Value1\n\nSection0:\n    Inner0 = Value2\n\n    InnerSection0:\n        InnerSub0 = Value3\n";

        [TestMethod]
        public void TestSavingToExistingWriter()
        {
            var cfg = ConfigFile.FromString(cfgContent);

            var savedCfgContentBuilder = new StringBuilder();
            using (var savedCfgStream = new StringWriter(savedCfgContentBuilder))
            {
                cfg.SaveTo(savedCfgStream);
            }

            var savedCfgContent = savedCfgContentBuilder.ToString();
            Assert.AreEqual(cfgContent, savedCfgContent);
        }

        [TestMethod]
        public void TestSavingToFile()
        {
            var cfg = ConfigFile.FromString(cfgContent);
            cfg.FileName = "temp/Test_ConfigFileWriter.TestSavingToFile.cfg";

            Directory.CreateDirectory("temp");

            WriteFileStream(cfg.FileName, FileMode.Create, FileAccess.Write,
                writer => cfg.SaveTo(writer));

            var savedContent = string.Empty;
            ReadFileStream(cfg.FileName, FileMode.Open, FileAccess.Read,
                reader => savedContent = reader.ReadToEnd());
            Assert.AreEqual(cfgContent, savedContent);
        }

        [TestMethod]
        public void TestAddSectionAndSavingToFile()
        {
            var cfg = ConfigFile.FromString(cfgContent);
            cfg.FileName = "temp/Test_ConfigFileWriter.TestAddSectionAndSavingToFile.cfg";

            cfg.AddSection(new ConfigSection() { Name = "Section1" });
            cfg["Section1"].AddOption(new ConfigOption() { Name = "Inner1", Value = "value with spaces" });

            WriteFileStream(cfg.FileName, FileMode.Create, FileAccess.Write,
                writer => cfg.SaveTo(writer));

            // Read the written content.
            var savedContent = string.Empty;
            ReadFileStream(cfg.FileName, FileMode.Open, FileAccess.Read,
                reader => savedContent = reader.ReadToEnd());
            var newContent = cfgContent + "\nSection1:\n    Inner1 = value with spaces\n";
            Assert.AreEqual(newContent, savedContent);
        }
    }
}
