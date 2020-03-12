using System;
using System.Collections.Generic;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.Json
{
    public class JsonDecoderConfiguration : IConfiguration
    {
        /// <summary>
        ///     The path to the file to store the data in relative to the recording directory.
        /// </summary>
        public FilePath RelativeFilePath { get; set; }

        public void Parse(RawConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException();
            }

            var element = JsonDocument.Parse(configuration.RawValue).RootElement;

            if (!element.TryGetProperty(nameof(RelativeFilePath), out var relativeFilePathElement))
            {
                throw new InvalidConfigurationException("Failed to parse relative file path.");
            }

            RelativeFilePath = new FilePath(relativeFilePathElement.GetString(), true);
        }

        public override bool Equals(object? obj)
        {
            return obj is JsonDecoderConfiguration configuration
                && RelativeFilePath.Equals(configuration.RelativeFilePath);
        }

        public override int GetHashCode() => HashCode.Combine(RelativeFilePath);
    }
}