using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using MORR.Core.Data.Transcoding.Mpeg;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Configuration;
using System.Diagnostics;

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

        [TestMethod]
        public void TestDesktopCaptureConfiguration_ParseFailsInvalidHeight()
        {
            /* PRECONDITION */
            var expectedConfig = GenerateDefaultExpectedParsedConfig();
            Debug.Assert(expectedConfig != null);
            Debug.Assert(Config != null);

            /* GIVEN */
            var config = @"{
                ""Width"": " + expectedConfig.Width + @",
                ""KiloBitsPerSecond"": " + expectedConfig.KiloBitsPerSecond + @",
                ""FramesPerSecond"": " + expectedConfig.FramesPerSecond + @",
                ""RelativeFilePath"": """ + expectedConfig.RelativeFilePath + @"""
            }";

            var rawConfig = new RawConfiguration(config);

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => Config.Parse(rawConfig));
        }

        [TestMethod]
        public void TestDesktopCaptureConfiguration_ParseFailsInvalidKiloBits()
        {
            /* PRECONDITION */
            var expectedConfig = GenerateDefaultExpectedParsedConfig();
            Debug.Assert(expectedConfig != null);
            Debug.Assert(Config != null);

            /* GIVEN */
            var config = @"{
                ""Width"": " + expectedConfig.Width + @",
                ""Height"": " + expectedConfig.Height + @",
                ""FramesPerSecond"": " + expectedConfig.FramesPerSecond + @",
                ""RelativeFilePath"": """ + expectedConfig.RelativeFilePath + @"""
            }";

            var rawConfig = new RawConfiguration(config);

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => Config.Parse(rawConfig));
        }

        [TestMethod]
        public void TestDesktopCaptureConfiguration_ParseFailsInvalidFPS()
        {
            /* PRECONDITION */
            var expectedConfig = GenerateDefaultExpectedParsedConfig();
            Debug.Assert(expectedConfig != null);
            Debug.Assert(Config != null);

            /* GIVEN */
            var config = @"{
                ""Width"": " + expectedConfig.Width + @",
                ""Height"": " + expectedConfig.Height + @",
                ""KiloBitsPerSecond"": " + expectedConfig.KiloBitsPerSecond + @",
                ""RelativeFilePath"": """ + expectedConfig.RelativeFilePath + @"""
            }";

            var rawConfig = new RawConfiguration(config);

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => Config.Parse(rawConfig));
        }

        [TestMethod]
        public void TestDesktopCaptureConfiguration_ParseFailsInvalidFilePath()
        {
            /* PRECONDITION */
            var expectedConfig = GenerateDefaultExpectedParsedConfig();
            Debug.Assert(expectedConfig != null);
            Debug.Assert(Config != null);

            /* GIVEN */
            var config = @"{
                ""Width"": " + expectedConfig.Width + @",
                ""Height"": " + expectedConfig.Height + @",
                ""KiloBitsPerSecond"": " + expectedConfig.KiloBitsPerSecond + @",
                ""FramesPerSecond"": " + expectedConfig.FramesPerSecond + @"
            }";

            var rawConfig = new RawConfiguration(config);

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => Config.Parse(rawConfig));
        }

        [TestMethod]
        public void TestDesktopCaptureConfiguration_ParseFailsInvalidWidth()
        {
            /* PRECONDITION */
            var expectedConfig = GenerateDefaultExpectedParsedConfig();
            Debug.Assert(expectedConfig != null);
            Debug.Assert(Config != null);

            /* GIVEN */
            var config = @"{
                ""Height"": " + expectedConfig.Height + @",
                ""KiloBitsPerSecond"": " + expectedConfig.KiloBitsPerSecond + @",
                ""FramesPerSecond"": " + expectedConfig.FramesPerSecond + @",
                ""RelativeFilePath"": """ + expectedConfig.RelativeFilePath + @"""
            }";

            var rawConfig = new RawConfiguration(config);

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => Config.Parse(rawConfig));
        }
    }
}
