using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Modules;
using MORR.Shared.Configuration;
using MORRTest.TestHelper.Configuration;
using MORRTest.TestHelper.Modules;
using System;
using System.Text;

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
            var builder = new StringBuilder();
            builder.Append("{\n \"EnabledModules\": [\n");
            
            for (var index = 0; index < testTypes.Length; index++)
            {
                builder.Append("\"" + testTypes[index].FullName + "\"");
                
                if (index + 1 != testTypes.Length)
                {
                    builder.Append(",\n");
                }
            }

            builder.Append("\n]\n}");

            return new RawConfiguration(builder.ToString());
        }
    }
}
