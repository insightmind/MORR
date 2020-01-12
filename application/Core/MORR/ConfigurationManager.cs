using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core
{
    /// <summary>
    ///     Manages the application's configuration
    /// </summary>
    public class ConfigurationManager: IConfigurationManager
    {
        /// <summary>
        ///     All configuration wrappers
        /// </summary>
        [ImportMany]
        public IEnumerable<IConfiguration> Configurations { get; private set; }

        internal ApplicationConfiguration AppConfig { get; private set; }

        private const string moduleIdentifierKey = "ModuleID";
        private const string moduleConfigKey = "ModuleConfiguration";


        /// <summary>
        ///     Loads the configuration from the specified path
        /// </summary>
        /// <param name="path">The path to load the configuration from</param>
        public void LoadConfiguration(FilePath path)
        {
            using var document = LoadJsonDocument(path);

            if (document == null)
            {
                throw new InvalidConfigurationException("Invalid configuration file path!");
            }

            AppConfig = new ApplicationConfiguration(document.RootElement);
            CommitConfigurations(document);
        }

        private static JsonDocument LoadJsonDocument(FilePath path)
        {
            var jsonString = File.ReadAllText(path.ToString());
            var options = new JsonDocumentOptions()
            {
                AllowTrailingCommas = true
            };

            return JsonDocument.Parse(jsonString, options);
        }

        private void CommitConfigurations(JsonDocument document)
        {
            var resolvedConfigs = ResolveModuleConfigurations(document);

            foreach (var config in Configurations)
            {
                if (!resolvedConfigs.ContainsKey(config.Identifier)) { continue; }

                config.Parse(resolvedConfigs[config.Identifier]);
            }
        }

        private static Dictionary<string, string> ResolveModuleConfigurations(JsonDocument document)
        {
            var resolvedConfigs = new Dictionary<string, string>();

            foreach (var moduleConfig in document.RootElement.GetProperty(moduleConfigKey).EnumerateArray())
            {
                var moduleIdentifier = moduleConfig.GetProperty(moduleIdentifierKey).GetString();

                if (resolvedConfigs.ContainsKey(moduleIdentifier))
                {
                    throw new InvalidConfigurationException($"Ambiguous moduleID: {moduleIdentifier}!");
                }

                resolvedConfigs[moduleIdentifier] = moduleConfig.ToString();
            }

            return resolvedConfigs;
        }
    }
}