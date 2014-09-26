using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Tests
{
    public class TestBase
    {
        [TestInitialize]
        public void TestInitialization()
        {
            ConfigFile.Defaults = new ConfigFileDefaults();
            Directory.CreateDirectory("temp");
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            if (Directory.Exists("temp"))
            {
                Directory.Delete("temp", true);
            }
        }
    }
}
