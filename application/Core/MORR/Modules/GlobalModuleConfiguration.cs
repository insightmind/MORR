using System;
using System.Collections.Generic;
using System.Composition;
using MORR.Shared.Configuration;
using MORR.Shared.Modules;

namespace MORR.Core.Modules
{
    [Export(typeof(GlobalModuleConfiguration))]
    [Export(typeof(IConfiguration))]
    public class GlobalModuleConfiguration : IConfiguration
    {
        /// <summary>
        ///     The types of all <see cref="IModule" /> instances that should be enabled.
        /// </summary>
        public IEnumerable<Type> EnabledModules { get; private set; }

        public void Parse(string configuration)
        {
            // TODO Implement mechanism to read this once a format has been decided
            throw new NotImplementedException();
        }
    }
}