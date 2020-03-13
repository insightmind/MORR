using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;

namespace MORRTest.TestHelper.Configuration
{
    /// <summary>
    /// The ConfigurationTest class implements certain shared tests for any class which implements the
    /// IConfiguration Interface.
    ///
    /// However this class should provide a custom Equals and GetHashCode method to
    /// allow proper validation of the correct properties of an parsed configuration.
    /// </summary>
    /// <typeparam name="T">The IConfiguration Type to be tested by this class</typeparam>
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

        /// <summary>
        /// Tests whether the Parse method correctly asserts a null configuration call.
        /// </summary>
        [TestMethod]
        public void TestConfiguration_NullConfiguration()
        {
            /* PRECONDITION */
            Debug.Assert(Config != null);

            /* WHEN */
            Assert.ThrowsException<ArgumentNullException>(() => Config.Parse(null));
        }

        /// <summary>
        /// Tests whether the Parse function correctly handles an empty json body.
        /// </summary>
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

        /// <summary>
        /// Tests whether the parsing of an expected valid configuration works fine.
        /// </summary>
        [TestMethod]
        public void TestConfiguration_ValidConfiguration()
        {
            /* GIVEN */
            var expectedConfig = GenerateDefaultExpectedParsedConfig();
            var rawConfig = GenerateDefaultExpectedRawConfig();

            /* WHEN */
            Debug.Assert(Config != null);
            Config.Parse(rawConfig);

            /* THEN */
            Console.WriteLine(rawConfig.RawValue);
            
            Assert.AreEqual(expectedConfig, Config);
        }

        /// <summary>
        /// Tests the Equals method to check if the same object is equal to itself.
        /// </summary>
        [TestMethod]
        public void TestConfiguration_EqualsSameObject()
        {
            var expectedConfig = GenerateDefaultExpectedParsedConfig();

            Debug.Assert(expectedConfig != null);

            Assert.AreEqual(expectedConfig, expectedConfig);
            Assert.AreEqual(expectedConfig.GetHashCode(), expectedConfig.GetHashCode());
        }

        /// <summary>
        /// Test the Equals method to check if two configurations with the same values are equal.
        /// </summary>
        [TestMethod]
        public void TestConfiguration_EqualsSameValues()
        {
            var expectedConfig = GenerateDefaultExpectedParsedConfig();
            var otherConfig = GenerateDefaultExpectedParsedConfig();

            Debug.Assert(expectedConfig != null);
            Debug.Assert(otherConfig != null);

            Assert.AreEqual(expectedConfig, otherConfig);
        }

        /// <summary>
        /// Test the Equals method to check if two configurations with the different values are not equal.
        /// </summary>
        [TestMethod]
        public void TestConfiguration_NotEqualsDifferentObject()
        {
            var notExpectedConfig = GenerateDefaultExpectedParsedConfig();

            Debug.Assert(Config != null);
            Debug.Assert(notExpectedConfig != null);

            Assert.AreNotEqual(notExpectedConfig, Config);
            Assert.AreNotEqual(notExpectedConfig.GetHashCode(), Config.GetHashCode());
        }

        /// <summary>
        /// Test the Equals method to check if configuration is correctly not equal to null.
        /// </summary>
        [TestMethod]
        public void TestConfiguration_NotEqualsNullObject()
        {
            var expectedConfig = GenerateDefaultExpectedParsedConfig();

            Assert.AreNotEqual(expectedConfig, null);
        }
    }
}
