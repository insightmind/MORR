﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MORR.Shared.Configuration;
using MORR.Shared.Modules;

namespace MORR.Core.Modules
{
    /// <summary>
    ///     Initializes and manages all modules.
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        private IEnumerable<IModule> enabledModules;

        /// <summary>
        ///     All <see cref="IModule" /> instances available through MEF.
        /// </summary>
        [ImportMany]
        private IEnumerable<IModule> Modules { get; set; }

        /// <summary>
        ///     The <see cref="IConfiguration" /> instance specifying configuration options regarding all modules.
        /// </summary>
        [Import]
        private GlobalModuleConfiguration ModuleConfiguration { get; set; }

        public void InitializeModules()
        {
            enabledModules = Modules.Where(x => ModuleConfiguration.EnabledModules.Contains(x.GetType()));

            foreach (var module in enabledModules)
            {
                module.Initialize();
            }
        }

        public void NotifyModulesOnSessionStart()
        {
            foreach (var module in enabledModules)
            {
                module.IsActive = true;
            }
        }

        public void NotifyModulesOnSessionStop()
        {
            foreach (var module in enabledModules)
            {
                module.IsActive = false;
            }
        }
    }
}