using System.Collections.Generic;
using MORR.Shared.Modules;
using System.Composition;
using System.Reflection;
using MORR.Shared.Configuration;

namespace MORR.Core.Modules
{
    /// <summary>
    ///     Manages all modules
    /// </summary>
    public class ModuleManager
    {
        [ImportMany]

        /// <summary>
        /// All Collecting Modules which are offered as assemblies through MEF.
        /// </summary>
        [ImportMany] 
        public IEnumerable<ICollectingModule> CollectingModules { get; private set; }

        /// <summary>
        /// Loads the parsed configuration on the associated module.
        /// </summary>
        /// <param name="configuration">The parsed configuration loaded by the application</param>
        public void LoadConfiguration(IConfiguration configuration)
        {
            // TODO: Give parsed configuration to modules.
        }
    }
}