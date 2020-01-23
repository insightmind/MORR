using System;
using System.Collections.Generic;
using System.Composition;
using System.Text.Json;
using MORR.Shared.Configuration;
using MORR.Shared.Modules;

namespace MORR.Core.Modules
{
    [Export(typeof(GlobalModuleConfiguration)), Export(typeof(IConfiguration)), Shared]
    public class GlobalModuleConfiguration : IConfiguration
    {
        /// <summary>
        ///     The types of all <see cref="IModule" /> instances that should be enabled.
        /// </summary>
        public IEnumerable<Type> EnabledModules { get; private set; }

        public string Identifier { get; } = "Global";

        public void Parse(string configuration)
        {
            var instance = JsonSerializer.Deserialize<GlobalModuleConfiguration>(configuration);
            EnabledModules = instance.EnabledModules;
        }
    }
}