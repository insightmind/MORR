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

        /// <summary>
        ///     Notifies all modules when a session starts.
        /// </summary>
        void NotifyModulesOnSessionStart();

        /// <summary>
        ///     Notifies all modules when a session stops.
        /// </summary>
        void NotifyModulesOnSessionStop();
    }
}