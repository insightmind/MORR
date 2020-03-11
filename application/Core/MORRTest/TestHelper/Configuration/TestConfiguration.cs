using MORR.Shared.Configuration;
using SharedTest.TestHelpers.Result;
using System;
using System.Diagnostics;

namespace MORRTest.TestHelper.Configuration
{
    public class TestConfiguration : IConfiguration
    {
        public TestResult TestResult = new TestResult();
        private readonly RawConfiguration expectedConfiguration;

        public TestConfiguration(RawConfiguration expectedConfiguration)
        {
            this.expectedConfiguration = expectedConfiguration;
        }

        public void Parse(RawConfiguration configuration)
        {
            Debug.Assert(expectedConfiguration != null);
            Debug.Assert(TestResult != null);

            if (expectedConfiguration.RawValue.Equals(configuration.RawValue))
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
