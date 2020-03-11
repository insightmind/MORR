using MORR.Shared.Configuration;
using SharedTest.TestHelpers.Result;
using System;

namespace MORRTest.TestHelper.Configuration
{
    class TestConfiguration : IConfiguration
    {
        public TestResult testResult = new TestResult();
        private readonly RawConfiguration expectedConfiguration;

        public TestConfiguration(RawConfiguration expectedConfiguration)
        {
            this.expectedConfiguration = expectedConfiguration;
        }

        public void Parse(RawConfiguration configuration)
        {
            if (expectedConfiguration.RawValue.Equals(configuration.RawValue))
            {
                testResult.Complete();
            } 
            else
            {
                testResult.Fail(new ArgumentException());
            }
        }
    }
}
