using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;

namespace MORR.Core.Recording
{
    [Export(typeof(RecordingConfiguration))]
    [Export(typeof(IConfiguration))]
    [Shared]
    public class RecordingConfiguration : IConfiguration
    {
        /// <summary>
        ///     The type of the encoder to use.
        /// </summary>
        public Type Encoder { get; private set; }

        /// <summary>
        ///     The type of the decoder to use.
        /// </summary>
        public Type Decoder { get; private set; }

        public string Identifier => "Recording";

        public void Parse(string configuration)
        {
            var element = JsonDocument.Parse(configuration).RootElement;

            if (!TryGetTypeFromProperty(element, nameof(Encoder), out var encoder))
            {
                throw new InvalidConfigurationException("Failed to parse encoder type.");
            }

            Encoder = encoder;

            if (!TryGetTypeFromProperty(element, nameof(Decoder), out var decoder))
            {
                throw new InvalidConfigurationException("Failed to parse decoder type.");
            }

            Decoder = decoder;
        }

        private static bool TryGetTypeFromProperty(JsonElement element,
                                                   string propertyName,
                                                   [NotNullWhen(true)] out Type? value)
        {
            if (!element.TryGetProperty(propertyName, out var propertyElement))
            {
                value = null;
                return false;
            }

            value = Type.GetType(propertyElement.GetString());
            return value != null;
        }
    }
}