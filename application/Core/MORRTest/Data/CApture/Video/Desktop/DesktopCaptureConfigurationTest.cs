using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
