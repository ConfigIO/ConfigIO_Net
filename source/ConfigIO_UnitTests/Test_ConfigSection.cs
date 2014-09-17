using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigSection
    {
        [TestMethod]
        public void Create()
        {
            var section = new ConfigSection();

            Assert.AreEqual(0, section.Options.Count);

            section["TestOption1"] = new ConfigOption("hello");
            Assert.AreEqual(1, section.Options.Count);
            Assert.AreEqual<string>("hello", section["TestOption1"]);

            section["TestOption1"] = "world";
            Assert.AreEqual(1, section.Options.Count);
            Assert.AreEqual<string>("world", section["TestOption1"]);
        }
    }
}
