using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;

namespace MORRTest.TestHelper.Configuration
{
    public abstract class ConfigurationTest<T> where T: IConfiguration, new()
    {
        public T Config = new T();

        /// <summary>
        /// Generates the DefaultConfig for tests. Make sure it matches with the RawConfiguration of GenerateDefault RawConfiguration!
        /// This should not be an empty configuration.
        /// </summary>
        /// <returns>The default configuration</returns>
        protected abstract T GenerateDefaultExpectedParsedConfig();

        /// <summary>
        /// Generates the default raw config associated to the default config above.
        /// </summary>
        /// <returns>The RawConfiguration which presents the DefaultConfig of GenerateDefaultConfig</returns>
        protected abstract RawConfiguration GenerateDefaultExpectedRawConfig();

        [TestInitialize]
        public virtual void BeforeTest() { }

        [TestMethod]
        public void TestConfiguration_NullConfiguration()
        {
            /* PRECONDITION */
            Debug.Assert(Config != null);

            /* WHEN */
            Assert.ThrowsException<ArgumentNullException>(() => Config.Parse(null));
        }

        [TestMethod]
        public void TestConfiguration_InvalidConfiguration()
        {
            /* PRECONDITION */
            Debug.Assert(Config != null);

            /* GIVEN */
            var rawConfig = new RawConfiguration("{ }");

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => Config.Parse(rawConfig));
        }

        [TestMethod]
        public void TestConfiguration_ValidConfiguration()
        {
            /* PRECONDITION */
            Debug.Assert(Config != null);

            /* GIVEN */
            var expectedConfig = GenerateDefaultExpectedParsedConfig();
            var rawConfig = GenerateDefaultExpectedRawConfig();

            /* WHEN */
            Config.Parse(rawConfig);

            /* THEN */
            Assert.AreEqual(expectedConfig, Config);
        }

        [TestMethod]
        public void TestConfiguration_EqualsSameObject()
        {
            var expectedConfig = GenerateDefaultExpectedParsedConfig();

            Assert.AreEqual(expectedConfig, expectedConfig);
        }

        [TestMethod]
        public void TestConfiguration_EqualsSameValues()
        {
            Assert.AreEqual(GenerateDefaultExpectedParsedConfig(), GenerateDefaultExpectedParsedConfig());
        }

        [TestMethod]
        public void TestConfiguration_NotEqualsDifferentObject()
        {
            Debug.Assert(Config != null);

            var notExpectedConfig = GenerateDefaultExpectedParsedConfig();
            Assert.AreNotEqual(notExpectedConfig, Config);
        }

        [TestMethod]
        public void TestConfiguration_NotEqualsNullObject()
        {
            var expectedConfig = GenerateDefaultExpectedParsedConfig();
            Assert.AreNotEqual(expectedConfig, null);
        }
    }
}
