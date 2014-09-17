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
            var option = ConfigOption.Create();
            Assert.AreEqual(string.Empty, option.Value);
        }

        [TestMethod]
        public void CreateWithArgument()
        {
            var option = ConfigOption.Create(1);
            Assert.AreEqual<int>(1, option);

            option = ConfigOption.Create(3.1415f);
            Assert.AreEqual<float>(3.1415f, option);

            option = ConfigOption.Create("hello world");
            Assert.AreEqual<string>("hello world", option);
        }

        [TestMethod]
        public void ImplicitConversion()
        {
            var option = ConfigOption.Create();

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
            var option = ConfigOption.Create("3.1415");

            Assert.AreEqual<float>(3.1415f, option);
            Assert.AreEqual<double>(3.1415, option);
        }
    }
}
