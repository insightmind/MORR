using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Data.Transcoding.Json;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Configuration;
using System.Diagnostics;

namespace MORRTest.Data.Transcoding.Json
{
    [TestClass]
    public class JsonDecoderConfigurationTest : ConfigurationTest<JsonDecoderConfiguration>
    {
        protected override JsonDecoderConfiguration GenerateDefaultExpectedParsedConfig()
        {
            return new JsonDecoderConfiguration
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
