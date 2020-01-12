using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        /// <summary>
        ///     Loads the configuration from the specified path
        /// </summary>
        /// <param name="path">The path to load the configuration from</param>
        public void LoadConfiguration(FilePath path)
        {
            using var document = LoadJsonDocument(path);

            if (document == null)
            {
                throw new InvalidConfigurationException();
            }
            
            var saveLocation = new FilePath(document.RootElement.GetProperty("SaveLocation").GetString());
            var saveName = document.RootElement.GetProperty("SaveName").GetString();

            AppConfig = new ApplicationConfiguration(saveLocation, saveName);
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

        private void CommitToConfigurations(JsonDocument document)
        {
            foreach (var configuration in Configurations)
            {
                configuration.
            }
        }
    }
}