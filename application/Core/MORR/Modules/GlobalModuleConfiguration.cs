using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;
using MORR.Shared.Modules;

namespace MORR.Core.Modules
{
    [Export(typeof(GlobalModuleConfiguration))]
    [Export(typeof(IConfiguration))]
    [Shared]
    public class GlobalModuleConfiguration : IConfiguration
    {
        public string Identifier => "Global";

        /// <summary>
        ///     The types of all <see cref="IModule" /> instances that should be enabled.
        /// </summary>
        public IEnumerable<Type> EnabledModules { get; private set; }

        public string Identifier { get; } = "Global";

        public void Parse(string configuration)
        {
            var element = JsonDocument.Parse(configuration).RootElement;

            if (!element.TryGetProperty(nameof(EnabledModules), out var enabledModulesElement))
            {
                throw new InvalidConfigurationException("Failed to parse enabled modules list.");
            }

            var enabledModules = new List<Type>();

            foreach (var value in enabledModulesElement.EnumerateArray().Select(x => x.ToString()))
            {
                var type = Type.GetType(value);

                if (type == null)
                {
                    throw new InvalidConfigurationException($"Failed to find module {value}.");
                }

                enabledModules.Add(type);
            }

            EnabledModules = enabledModules;
        }
    }
}