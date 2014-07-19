using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Configuration;

namespace ConfigFile_UnitTests
{
    [TestClass]
    public class Test_ConfigSection
    {
        [TestMethod]
        public void Create()
        {
            var section = new ConfigSection();

            Assert.AreEqual(0, section.Options.Count);

            section["TestOption1"] = ConfigOption.Create("hello");
            Assert.AreEqual(1, section.Options.Count);
            Assert.AreEqual<string>("hello", section["TestOption1"]);

            section["TestOption1"] = ConfigOption.Create("world");
            Assert.AreEqual(1, section.Options.Count);
            Assert.AreEqual<string>("world", section["TestOption1"]);
        }
    }
}
