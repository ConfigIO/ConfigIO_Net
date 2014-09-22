using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigSection : TestBase
    {
        [TestMethod]
        public void Create()
        {
            var section = new ConfigSection();

            Assert.AreEqual(0, section.Options.Count);

            section.AddOption(new ConfigOption("TestOption1", "hello"));
            Assert.AreEqual(1, section.Options.Count);
            Assert.AreEqual<string>("hello", section.GetOption("TestOption1"));

            section.AddOption(new ConfigOption("TestOption1", "world"));
            Assert.AreEqual(1, section.Options.Count);
            Assert.AreEqual<string>("world", section.GetOption("TestOption1"));
        }
    }
}
