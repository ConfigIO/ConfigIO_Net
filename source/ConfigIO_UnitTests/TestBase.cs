using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            ConfigFile.ResetStaticState();
        }
    }
}
