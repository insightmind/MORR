using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Session;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Configuration;
using MORRTest.TestHelper.Decoder;
using MORRTest.TestHelper.Encoder;
using System;
using System.Text;

namespace MORRTest.Session
{
    [TestClass]
    public class SessionConfigurationTest : ConfigurationTest<SessionConfiguration>
    {
        private readonly Type[] encoderTypes = new Type[1] { typeof(TestEncoder) };
        private readonly Type[] decoderTypes = new Type[1] { typeof(TestDecoder) };
        private readonly DirectoryPath path = new DirectoryPath("%userprofile%", true);

        protected override SessionConfiguration GenerateDefaultExpectedParsedConfig()
        {
            return new SessionConfiguration
            {
                Encoders = encoderTypes,
                Decoders = decoderTypes,
                RecordingDirectory = path
            };
        }

        protected override RawConfiguration GenerateDefaultExpectedRawConfig()
        {
            var config = @"
            {
                ""Encoders"": [
                    ""MORRTest.TestHelper.Encoder.TestEncoder""
                ],
                ""Decoders"": [
                    ""MORRTest.TestHelper.Decoder.TestDecoder""
                ],
                ""RecordingDirectory"": ""%userprofile%\\Videos\\MORR""
            }";

            return new RawConfiguration(config);
        }
    }
}
