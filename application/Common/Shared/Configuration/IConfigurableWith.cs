using System;
using System.Collections.Generic;
using System.Text;

namespace MORR.Shared.Configuration
{
    /// <summary>
    /// This interface defines a configurable component and its configuration interface.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of configuration used to configure the component</typeparam>
    public interface IConfigurableWith<in TConfiguration> where TConfiguration : IModuleConfiguration

    {
        /// <summary>
        /// Gets the specified configuration type of the interface.
        /// </summary>
        /// <returns>A inherited TConfiguration type</returns>
        Type GetConfigurationType();

        /// <summary>
        /// Loads the configuration onto the component.
        /// </summary>
        /// <param name="configuration">The specific parsed configuration instance.</param>
        void LoadConfiguration(TConfiguration configuration);
    }
}
