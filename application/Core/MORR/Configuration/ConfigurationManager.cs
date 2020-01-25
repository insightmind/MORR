using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.Json;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core.Configuration
{
    /// <summary>
    ///     Manages the application's configuration
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        /// <summary>
        ///     All configuration wrappers
        /// </summary>
        [ImportMany]
        private IEnumerable<IConfiguration>? Configurations { get; set; }

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

            CommitConfigurations(document);
        }

        private static JsonDocument LoadJsonDocument(FilePath path)
        {

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), "Invalid file path to configuration file.");
            }

            var jsonString = File.ReadAllText(path.ToString());
            var options = new JsonDocumentOptions()
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };

            return JsonDocument.Parse(jsonString, options);
        }

        private void CommitConfigurations(JsonDocument document)
        {
            if (Configurations == null)
            {
                return; // We simply return as we do not need to commit any configs.
            }

            if (document == null)
            {
                throw new InvalidConfigurationException("Invalid configuration file path!");
            }

            foreach (var configurationObject in document.RootElement.EnumerateObject())
            {
                var configurationType = Type.GetType(configurationObject.Name);
                var resolvedConfiguration = Configurations.SingleOrDefault(x => x.GetType() == configurationType);

                if (resolvedConfiguration == null)
                {
                    throw new InvalidConfigurationException($"Could not find specified configuration type {configurationObject}");
                }

                resolvedConfiguration.Parse(new RawConfiguration(configurationObject.Value.GetRawText()));
            }
        }
    }
}