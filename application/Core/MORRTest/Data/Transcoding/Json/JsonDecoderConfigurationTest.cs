﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Data.Transcoding.Json;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Configuration;

namespace MORRTest.Data.Transcoding.Json
{
    [TestClass]
    public class JsonDecoderConfigurationTest: ConfigurationTest<JsonDecoderConfiguration>
    {
        protected override JsonDecoderConfiguration GenerateDefaultExpectedParsedConfig()
        {
            return new JsonDecoderConfiguration {
                RelativeFilePath = new FilePath("event_data.json", true)
            };
        }

        protected override RawConfiguration GenerateDefaultExpectedRawConfig()
        {
            return new RawConfiguration("{\n\"RelativeFilePath\":\"" + GenerateDefaultExpectedParsedConfig().RelativeFilePath + "\"\n}");
        }
    }
}
