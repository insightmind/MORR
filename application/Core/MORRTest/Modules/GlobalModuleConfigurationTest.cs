using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Modules;
using MORR.Shared.Configuration;
using MORRTest.TestHelper.Configuration;
using MORRTest.TestHelper.Modules;
using System;

namespace MORRTest.Modules
{
    /// <summary>
    /// This TestClass purpose is to test the parsing functionality of the
    /// GlobalModuleConfiguration.
    ///
    /// We use the ConfigurationTest superclass to test basic functionality
    /// shared between all configuration classes.
    /// </summary>
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
