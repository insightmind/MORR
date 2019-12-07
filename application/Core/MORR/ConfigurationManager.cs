using System;
using System.Collections.Generic;
using System.Composition;
using MORR.Shared.Configuration;

namespace MORR.Core
{
    /// <summary>
    ///     Manages the application's configuration
    /// </summary>
    public class ConfigurationManager
    {
        /// <summary>
        ///     All configuration wrappers
        /// </summary>
        [ImportMany]
        public IEnumerable<IConfiguration> Configurations { get; private set; }

        /// <summary>
        ///     Loads the configuration from the specified path
        /// </summary>
        /// <param name="path">The path to load the configuration from</param>
        public void LoadConfiguration(string path)
        {
            // TODO Implement
            throw new NotImplementedException();
        }
    }
}