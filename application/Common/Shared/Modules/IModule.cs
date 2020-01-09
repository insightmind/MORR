using System;

namespace MORR.Shared.Modules
{
    /// <summary>
    ///     A module.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        ///     Indicates whether the module is currently enabled. <see langword="true" /> if the module is enabled,
        ///     <see langword="false" /> otherwise.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        ///     The identifier of the module.
        /// </summary>
        Guid Identifier { get; }

        /// <summary>
        ///     Initializes the module.
        /// </summary>
        void Initialize();
    }
}