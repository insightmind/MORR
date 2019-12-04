using System.Collections.Generic;
using System.Composition;
using MORR.Shared.Modules;

namespace MORR.Core
{
    /// <summary>
    ///     Manages all modules
    /// </summary>
    public class ModuleManager
    {
        /// <summary>
        ///     All <see cref="IModule" /> instances available through MEF
        /// </summary>
        [ImportMany]
        public IEnumerable<IModule> Modules { get; private set; }

        /// <summary>
        ///     All <see cref="ICollectingModule" /> instances available through MEF.
        /// </summary>
        [ImportMany]
        public IEnumerable<ICollectingModule> CollectingModules { get; private set; }

        /// <summary>
        ///     Initializes all modules
        /// </summary>
        public void InitializeModules()
        {
            // TODO Probably check here which modules are disabled by configuration
            // Also need a way to enable modules
            if (Modules != null)
            {
                foreach (var module in Modules)
                {
                    module.Initialize();
                }
            }
        }
    }
}