
using System;

namespace MORR.Shared.Configuration
{
    /// <summary>
    /// IModuleConfiguration represents a single configuration unit for a module
    /// </summary>
    public interface IModuleConfiguration
    {
        /// <summary>
        /// The identifier of the associated module.
        /// </summary>
        Guid ModuleIdentifierGuid { get; set; }

        /// <summary>
        ///     Parses the configuration from the provided value
        /// </summary>
        /// <param name="configuration">The configuration <see cref="string" /> to parse from</param>
        void Parse(string configuration);
    }
}
