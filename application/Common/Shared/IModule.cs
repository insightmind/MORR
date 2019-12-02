using System;

namespace MORR.Shared
{
    /// <summary>
    ///     A module
    /// </summary>
    public interface IModule
    {
        /// <summary>
        ///     Whether the module is currently enabled
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        ///     The identifier of the module
        /// </summary>
        Guid Identifier { get; }

        /// <summary>
        ///     Initializes the module
        /// </summary>
        void Initialize();
    }
}