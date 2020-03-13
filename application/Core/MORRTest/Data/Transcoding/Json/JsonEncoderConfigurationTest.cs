using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Data.Transcoding.Json;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Configuration;
using System.Diagnostics;

namespace MORRTest.Data.Transcoding.Json
{
    [TestClass]
    public class JsonEncoderConfigurationTest : ConfigurationTest<JsonEncoderConfiguration>
    {
        protected override JsonEncoderConfiguration GenerateDefaultExpectedParsedConfig()
        {
            return new JsonEncoderConfiguration
            {
                RelativeFilePath = new FilePath("event_data.json", true)
            };
        }

        protected override RawConfiguration GenerateDefaultExpectedRawConfig()
        {
            var config = GenerateDefaultExpectedParsedConfig();
            Debug.Assert(config != null);
            return new RawConfiguration("{\n\"RelativeFilePath\":\"" + config.RelativeFilePath + "\"\n}");
        }
    }
}
