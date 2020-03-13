using MORR.Shared.Configuration;
using SharedTest.TestHelpers.Result;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace MORRTest.TestHelper.Configuration
{
    /// <summary>
    /// TestConfiguration is a class which implements the IConfiguration.
    /// It can therefore be used to be discovered and injected for the ConfigurationManager.
    ///
    /// This class then uses the provided expectedConfiguration to check whether it was at least called
    /// and the configuration provided via the Parse method does equal the expected one.
    /// </summary>
    public class TestConfiguration : IConfiguration
    {
        /// <summary>
        /// This result contains the asynchronous state of the configuration parse validation.
        ///
        /// It will be completed if Parse() has been called at least once.
        /// It is successful if the configuration met the expected one, otherwise it fails with an ArgumentException.
        /// </summary>
        public TestResult TestResult = new TestResult();

        private readonly RawConfiguration expectedConfiguration;

        /// <summary>
        /// Creates a new TestConfiguration which validates the configuration meets the expectedConfiguration.
        /// </summary>
        /// <param name="expectedConfiguration">The expected configuration used for validating the Parse method.</param>
        public TestConfiguration(RawConfiguration expectedConfiguration)
        {
            this.expectedConfiguration = expectedConfiguration;
        }

        /// <summary>
        ///     Parses the configuration from the provided value
        /// </summary>
        /// <param name="configuration">The configuration <see cref="JsonElement" /> to parse from</param>
        public void Parse(RawConfiguration configuration)
        {
            Debug.Assert(expectedConfiguration != null);
            Debug.Assert(TestResult != null);

            if (configuration != null && expectedConfiguration.RawValue.Equals(configuration.RawValue))
            {
                TestResult.Complete();
            } 
            else
            {
                TestResult.Fail(new ArgumentException());
            }
        }
    }
}
