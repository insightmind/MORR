using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using MORR.Core.Data.Capture.Video.Desktop;
using MORR.Shared.Configuration;
using MORRTest.TestHelper.Configuration;

namespace MORRTest.Data.Capture.Video.Desktop
{
    [TestClass]
    public class DesktopCaptureConfigurationTest: ConfigurationTest<DesktopCaptureConfiguration>
    {
        protected override DesktopCaptureConfiguration GenerateDefaultExpectedParsedConfig()
        {
            return new DesktopCaptureConfiguration
            {
                MonitorIndex = 20,
                PromptUserForMonitorSelection = true
            };
        }

        protected override RawConfiguration GenerateDefaultExpectedRawConfig()
        {
            const string config = @"{
                ""MonitorIndex"": 20,
                ""PromptUserForMonitorSelection"": true
            }";

            return new RawConfiguration(config);
        }

        [TestMethod]
        public void TestDesktopCaptureConfiguration_ParseFailsInvalidPromptUser()
        {
            /* PRECONDITION */
            Debug.Assert(Config != null);

            /* GIVEN */
            const string config = @"{
                ""MonitorIndex"": 20
            }";

            var rawConfig = new RawConfiguration(config);

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => Config.Parse(rawConfig));
        }

        [TestMethod]
        public void TestDesktopCaptureConfiguration_ParseFailsInvalidMonitorIndex()
        {
            /* PRECONDITION */
            Debug.Assert(Config != null);

            /* GIVEN */
            const string config = @"{
                ""PromptUserForMonitorSelection"": true
            }";

            var rawConfig = new RawConfiguration(config);

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => Config.Parse(rawConfig));
        }
    }
}
