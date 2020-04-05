using MORR.Shared.Utility;

namespace MORR.Core.Configuration
{
    /// <summary>
    ///     Loads and manages the application's configuration.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        ///     Loads the configuration from the specified path.
        /// </summary>
        /// <param name="path">The path to load the configuration from.</param>
        void LoadConfiguration(FilePath path); // TODO This depends on the changes made to the Shared project
    }
}