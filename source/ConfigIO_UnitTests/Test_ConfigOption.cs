using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_ConfigOption
    {
        [TestMethod]
        public void Create()
        {
            var option = new ConfigOption();
            Assert.AreEqual(string.Empty, option.Value);
        }

        [TestMethod]
        public void CreateWithArgument()
        {
            var option = new ConfigOption("", 1);
            Assert.AreEqual<int>(1, option);

            option = new ConfigOption("", 3.1415f);
            Assert.AreEqual<float>(3.1415f, option);

            option = new ConfigOption("", "hello world");
            Assert.AreEqual<string>("hello world", option);
        }

        [TestMethod]
        public void ImplicitConversion()
        {
            Assert.AreEqual(1, new ConfigOption("", 1));

            Assert.AreEqual<float>(3.1415f, new ConfigOption("", 3.1415f));

            Assert.AreEqual<string>("hello", new ConfigOption("", "hello"));
        }

        [TestMethod]
        public void InvariantCulture()
        {
            var option = new ConfigOption("", "3.1415");

            Assert.AreEqual<float>(3.1415f, option);
            Assert.AreEqual<double>(3.1415, option);
        }
    }
}
