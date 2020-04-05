using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using MORR.Modules.Mouse;
using MORR.Shared.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseTest
{
    [TestClass]
    public class MouseModuleConfigurationTest
    {
        private MouseModuleConfiguration config;

        [TestInitialize]
        public void TestInit()
        {
            config = new MouseModuleConfiguration();
        }
        
        [TestMethod]
        public void ParseTest()
        {
            config.Parse(new RawConfiguration("{\r\n    \"SamplingRateInHz\": 10,\r\n    \"Threshold\": 50\r\n  }"));
            Assert.AreEqual((UInt32)10, config.SamplingRateInHz);
            Assert.AreEqual((int)50, config.Threshold);
        }
        
        [TestMethod]
        public void ParseInvalidTest()
        {
            Assert.ThrowsException<InvalidConfigurationException>(
                () => config.Parse(new RawConfiguration("{ }")));
        }
    }
}
