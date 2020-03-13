using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Session;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Configuration;
using MORRTest.TestHelper.Decoder;
using MORRTest.TestHelper.Encoder;
using System;

namespace MORRTest.Session
{
    [TestClass]
    public class SessionConfigurationTest : ConfigurationTest<SessionConfiguration>
    {
        private readonly Type[] encoderTypes = new Type[1] { typeof(TestEncoder) };
        private readonly Type[] decoderTypes = new Type[1] { typeof(TestDecoder) };
        
        // This is correct, however the actual json string contains 2 '\'.
        // It seems that the System.Text.JsonDecoder struggles with dealing C# language specified strings
        // which in my opinion seems quite ridiculous. Even tough we supply them with a raw string they
        // will try to check for any escaping characters, so 'C:\\' results to 'C:\'.
        private readonly DirectoryPath path = new DirectoryPath(@"C:\", true);

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
            const string config = @"{
                ""Encoders"": [
                    ""MORRTest.TestHelper.Encoder.TestEncoder""
                ],
                ""Decoders"": [
                    ""MORRTest.TestHelper.Decoder.TestDecoder""
                ],
                ""RecordingDirectory"": ""C:\\""
            }";

            return new RawConfiguration(config);
        }
    }
}
