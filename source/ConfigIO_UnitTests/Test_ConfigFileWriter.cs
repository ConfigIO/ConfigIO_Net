using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigFileWriter
    {
        [TestMethod]
        public void TestSavingToExistingWriter()
        {
            var cfgContent = "Option0 = Value0\nOption1 = Value1\n[Section0:\n    Inner0 = Value2\n    [InnerSection0:\n        InnerSub0 = Value3\n    ]\n]\n";

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
            //Assert.Inconclusive("Not implemented.");

            var cfgContent = "Option0 = Value0\nOption1 = Value1\n[Section0:\n    Inner0 = Value2\n    [InnerSection0:\n        InnerSub0 = Value3\n    ]\n]\n";

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
    }
}
