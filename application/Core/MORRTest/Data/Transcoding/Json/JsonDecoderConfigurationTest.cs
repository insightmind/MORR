using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using MORR.Core.Data.Transcoding.Json;
using MORR.Shared.Configuration;

namespace MORRTest.Data.Transcoding.Json
{
    [TestClass]
    public class JsonDecoderConfigurationTest
    {
        public JsonDecoderConfiguration config;

        [TestInitialize]
        public void BeforeTest()
        {
            config = new JsonDecoderConfiguration();
        }

        [TestMethod]
        public void TestJsonDecoderConfiguration_NullConfiguration()
        {
            /* PRECONDITION */
            Debug.Assert(config != null);

            /* WHEN */
            Assert.ThrowsException<ArgumentNullException>(() => config.Parse(null));
        }

        [TestMethod]
        public void TestJsonDecoderConfiguration_InvalidConfiguration()
        {
            /* PRECONDITION */
            Debug.Assert(config != null);

            /* GIVEN */
            var rawConfig = new RawConfiguration("{ }");

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => config.Parse(rawConfig));
        }

        [TestMethod]
        public void TestJsonDecoderConfiguration_ValidConfiguration()
        {
            /* PRECONDITION */
            Debug.Assert(config != null);

            /* GIVEN */
            const string relativeFilePath = "event_data.json";
            var rawConfig = new RawConfiguration("{\n\"RelativeFilePath\":\"" + relativeFilePath + "\"\n}");

            /* WHEN */
            config.Parse(rawConfig);

            /* THEN */
            Assert.IsNotNull(config.RelativeFilePath);
            Assert.AreEqual(relativeFilePath, config.RelativeFilePath.ToString());
        }
    }
}
