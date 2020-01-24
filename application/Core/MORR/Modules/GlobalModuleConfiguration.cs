using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;
using MORR.Shared.Modules;

namespace MORR.Core.Modules
{
    [Export(typeof(GlobalModuleConfiguration)), PartCreationPolicy(CreationPolicy.Shared)]
    public class GlobalModuleConfiguration : IConfiguration
    {
        /// <summary>
        ///     The types of all <see cref="IModule" /> instances that should be enabled.
        /// </summary>
        public IEnumerable<Type> EnabledModules { get; private set; }

        public void Parse(RawConfiguration configuration)
        {
            var element = JsonDocument.Parse(configuration.RawValue).RootElement;

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