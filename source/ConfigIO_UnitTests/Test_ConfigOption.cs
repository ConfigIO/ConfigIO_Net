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
            var option = new ConfigOption(1);
            Assert.AreEqual<int>(1, option);

            option = new ConfigOption(3.1415f);
            Assert.AreEqual<float>(3.1415f, option);

            option = new ConfigOption("hello world");
            Assert.AreEqual<string>("hello world", option);
        }

        [TestMethod]
        public void ImplicitConversion()
        {
            var option = new ConfigOption();

            option = 1;
            Assert.AreEqual<int>(1, option);

            int i = option;
            Assert.AreEqual(1, i);

            option = 3.1415f;
            Assert.AreEqual<float>(3.1415f, option);

            float f = option;
            Assert.AreEqual(3.1415f, f);

            option = "hello";
            Assert.AreEqual<string>("hello", option);

            string s = option;
            Assert.AreEqual("hello", s);
        }

        [TestMethod]
        public void InvariantCulture()
        {
            var option = new ConfigOption("3.1415");

            Assert.AreEqual<float>(3.1415f, option);
            Assert.AreEqual<double>(3.1415, option);
        }
    }
}
