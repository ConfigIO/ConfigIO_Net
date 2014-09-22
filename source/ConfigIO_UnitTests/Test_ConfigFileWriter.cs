using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigFileWriter : TestBase
    {
        static readonly string cfgContent = "Option0 = Value0\nOption1 = Value1\nSection0:\n    Inner0 = Value2\n    InnerSection0:\n        InnerSub0 = Value3\n";

        [TestMethod]
        public void TestSavingToExistingWriter()
        {
            var cfg = ConfigFile.FromString(cfgContent);

            var savedCfgContentBuilder = new StringBuilder();
            using (var savedCfgStream = new StringWriter(savedCfgContentBuilder))
            {
                cfg.SaveToFile(savedCfgStream);
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

            try
            {
                using (var fileStream = new FileStream(cfg.FileName, FileMode.Create))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        cfg.SaveToFile(writer);
                    }
                }

                using (var fileStream = new FileStream(cfg.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        var savedContent = reader.ReadToEnd();
                        Assert.AreEqual(cfgContent, savedContent);
                    }
                }
            }
            finally
            {
                Directory.Delete("temp", true);
            }
        }

        [TestMethod]
        public void TestAddSectionAndSavingToFile()
        {
            var cfg = ConfigFile.FromString(cfgContent);
            cfg.FileName = "temp/Test_ConfigFileWriter.TestAddSectionAndSavingToFile.cfg";

            Directory.CreateDirectory("temp");

            try
            {
                cfg.AddSection(new ConfigSection() { Name = "Section1" });
                cfg["Section1"].AddOption(new ConfigOption() { Name = "Inner1", Value = "value with spaces" });

                using (var fileStream = new FileStream(cfg.FileName, FileMode.Create))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        cfg.SaveToFile(writer);
                    }
                }

                using (var fileStream = new FileStream(cfg.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        var savedContent = reader.ReadToEnd();
                        var newContent = cfgContent + "Section1:\n    Inner1 = value with spaces\n";
                        Assert.AreEqual(newContent, savedContent);
                    }
                }
            }
            finally
            {
                Directory.Delete("temp", true);
            }
        }
    }
}
