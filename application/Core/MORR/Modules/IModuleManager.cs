namespace MORR.Core.Modules
{
    /// <summary>
    ///     Initializes and manages all modules.
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        ///     Initializes all modules.
        /// </summary>
        void InitializeModules();
    }
}