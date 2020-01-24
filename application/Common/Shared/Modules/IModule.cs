using System;
using System.ComponentModel.Composition;

namespace MORR.Shared.Modules
{
    /// <summary>
    ///     A module.
    /// </summary>
    [InheritedExport]
    public interface IModule
    {
        /// <summary>
        ///     Indicates whether the module is currently active. <see langword="true" /> if the modules is active,
        ///     <see langword="false" /> otherwise.
        /// </summary>
        bool IsActive { get; set; }

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