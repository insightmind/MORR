using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Configuration;

namespace SharedTest.Configuration
{
    [TestClass]
    public class RawConfigurationTest
    {
        [TestMethod]
        public void TestRawConfiguration_CorrectParamPropagation()
        {
            /* GIVEN */
            const string testString = "TestString";

            /* WHEN */
            var rawConfiguration = new RawConfiguration(testString);

            /* THEN */
            Assert.IsNotNull(rawConfiguration.RawValue);
            Assert.AreEqual(testString, rawConfiguration.RawValue);
        }
    }
}
