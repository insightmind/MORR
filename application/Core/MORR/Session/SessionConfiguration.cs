﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core.Session
{
    public class SessionConfiguration : IConfiguration
    {
        /// <summary>
        ///     The type of the encoder to use.
        /// </summary>
        public Type Encoder { get; private set; }

        /// <summary>
        ///     The type of the decoder to use.
        /// </summary>
        public Type? Decoder { get; private set; }

        /// <summary>
        ///     The directory in which to store new recordings.
        /// </summary>
        public DirectoryPath RecordingDirectory { get; private set; }

        public void Parse(RawConfiguration configuration)
        {
            var element = JsonDocument.Parse(configuration.RawValue).RootElement;

            if (!element.TryGetProperty(nameof(Encoder), out var encoderElement) ||
                !TryGetType(encoderElement, out var encoder))
            {
                throw new InvalidConfigurationException("Failed to parse encoder type.");
            }

            Encoder = encoder;

            Type? decoder = null;

            // Specifying a decoder is optional; only thrown an error if a value was specified but could not be parsed
            if (element.TryGetProperty(nameof(Decoder), out var decoderElement) &&
                !TryGetType(decoderElement, out decoder))
            {
                throw new InvalidConfigurationException("Failed to parse decoder type.");
            }

            Decoder = decoder;

            if (!element.TryGetProperty(nameof(RecordingDirectory), out var directoryElement))
            {
                throw new InvalidConfigurationException("Failed to parse directory path.");
            }

            var directoryPath = directoryElement.GetString();
            directoryPath = Environment.ExpandEnvironmentVariables(directoryPath);
            RecordingDirectory = new DirectoryPath(directoryPath);
        }

        private static bool TryGetType(JsonElement element, [NotNullWhen(true)] out Type? value)
        {
            value = Utility.GetTypeFromAnyAssembly(element.ToString());
            return value != null;
        }
    }
}