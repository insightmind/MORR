using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Text.Json;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core.Configuration
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
        private IEnumerable<IConfiguration> Configurations { get; set; }

        internal ApplicationConfiguration AppConfig { get; private set; }

        private const string moduleIdentifierKey = "Identifier";
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

            if (path == null)
            {
                throw new InvalidConfigurationException("Internal error occured!");
            }

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

            if ((resolvedConfigs == null) || (Configurations == null))
            {
                throw new InvalidConfigurationException("Could not resolve configuration!");
            }

            foreach (var config in Configurations)
            {
                if (config.Identifier == null)
                {
                    throw new InvalidConfigurationException("Configuration did not offer valid identifier! Please check loaded modules.");
                }

                if (!resolvedConfigs.ContainsKey(config.Identifier)) { continue; }

                config.Parse(resolvedConfigs[config.Identifier]);
            }
        }

        private static Dictionary<string, string> ResolveModuleConfigurations(JsonDocument document)
        {
            if (document == null)
            {
                throw new InvalidConfigurationException("Internal error occured!");
            }

            var resolvedConfigs = new Dictionary<string, string>();

            foreach (var moduleConfig in document.RootElement.GetProperty(moduleConfigKey).EnumerateArray())
            {
                var moduleIdentifier = moduleConfig.GetProperty(moduleIdentifierKey).GetString();

                if ((moduleIdentifier == null) || (resolvedConfigs.ContainsKey(moduleIdentifier)))
                {
                    throw new InvalidConfigurationException($"Ambiguous moduleID: {moduleIdentifier}!");
                }

                resolvedConfigs[moduleIdentifier] = moduleConfig.ToString();
            }

            return resolvedConfigs;
        }
    }
}