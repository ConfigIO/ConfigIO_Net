using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Configuration;

namespace ConfigFile_UnitTests
{
    [TestClass]
    public class Test_ConfigFile
    {
        [TestMethod]
        public void CreateFromFile()
        {
            var cfg = ConfigFile.FromFile("data/testData/sample.cfg");
            return;
        }
    }
}
