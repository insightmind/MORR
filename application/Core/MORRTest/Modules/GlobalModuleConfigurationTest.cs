using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Modules;
using MORR.Shared.Configuration;
using MORRTest.TestHelper.Configuration;
using MORRTest.TestHelper.Modules;
using System;

namespace MORRTest.Modules
{
    [TestClass]
    public class GlobalModuleConfigurationTest : ConfigurationTest<GlobalModuleConfiguration>
    {
        private readonly Type[] testTypes = new Type[1] { typeof(TestModule) };

        protected override GlobalModuleConfiguration GenerateDefaultExpectedParsedConfig()
        {
            return new GlobalModuleConfiguration
            {
                EnabledModules = testTypes
            };
        }

        protected override RawConfiguration GenerateDefaultExpectedRawConfig()
        {
            const string config = @"{
                ""EnabledModules"": [
                    ""MORRTest.TestHelper.Modules.TestModule""
                ]
            }";

            return new RawConfiguration(config);
        }
    }
}
