using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using MORR.Core.Data.Transcoding.Mpeg;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Configuration;

namespace MORRTest.Data.Transcoding.MPEG
{
    [TestClass]
    public class MpegEncoderConfigurationTest : ConfigurationTest<MpegEncoderConfiguration>
    {
        protected override MpegEncoderConfiguration GenerateDefaultExpectedParsedConfig()
        {
            return new MpegEncoderConfiguration
            {
                Width = 1920,
                Height = 1080,
                FramesPerSecond = 60,
                KiloBitsPerSecond = 4000,
                RelativeFilePath = new FilePath("desktop_capture.mp4", true)
            };
        }

        protected override RawConfiguration GenerateDefaultExpectedRawConfig()
        {
            var expectedConfig = GenerateDefaultExpectedParsedConfig();

            Debug.Assert(expectedConfig != null);

            return new RawConfiguration(@"{
                ""Width"": " + expectedConfig.Width + @",
                ""Height"": " + expectedConfig.Height + @",
                ""KiloBitsPerSecond"": " + expectedConfig.KiloBitsPerSecond + @",
                ""FramesPerSecond"": " + expectedConfig.FramesPerSecond + @",
                ""RelativeFilePath"": """ + expectedConfig.RelativeFilePath + @"""
            }");
        }
    }
}
