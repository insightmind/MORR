using System.Collections.Generic;
using System.Composition;
using System.Linq;
using MORR.Shared.Configuration;
using MORR.Shared.Modules;

namespace MORR.Core.Modules
{
    /// <summary>
    ///     Manages all modules
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        /// <summary>
        ///     All <see cref="IModule" /> instances available through MEF.
        /// </summary>
        [ImportMany]
        public IEnumerable<IModule> Modules { get; private set; }

        /// <summary>
        ///     All <see cref="ICollectingModule" /> instances available through MEF.
        /// </summary>
        [ImportMany]
        public IEnumerable<ICollectingModule> CollectingModules { get; private set; }

        /// <summary>
        ///     The <see cref="IConfiguration" /> instance specifying configuration options regarding all modules.
        /// </summary>
        [Import]
        public GlobalModuleConfiguration ModuleConfiguration { get; private set; }

        /// <summary>
        ///     Initializes all modules.
        /// </summary>
        public void InitializeModules()
        {
            foreach (var module in Modules.Where(x => ModuleConfiguration.EnabledModules.Contains(x.GetType())))
            {
                module.Initialize();
                module.IsEnabled = true;
            }
        }
    }
}