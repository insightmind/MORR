using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;

namespace MORR.Core.Data.Transcoding.MPEG
{
    public class MPEGEncoderConfiguration : IConfiguration
    {
        /// <summary>
        ///     The width in pixels of the resulting video stream.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        ///     The height in pixels of the resulting video stream.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        ///     The bit rate in bits per second of the resulting video stream.
        /// </summary>
        public uint KiloBitsPerSecond { get; set; }

        /// <summary>
        ///     The frame rate in frames per second of the resulting video stream.
        /// </summary>
        public uint FramesPerSecond { get; set; }

        /// <summary>
        ///     The name of the recording file.
        /// </summary>
        public string RecordingName { get; set; }

        public void Parse(RawConfiguration configuration)
        {
            var element = JsonDocument.Parse(configuration.RawValue).RootElement;

            Width = GetUintFromProperty(element, nameof(Width));
            Height = GetUintFromProperty(element, nameof(Height));
            KiloBitsPerSecond = GetUintFromProperty(element, nameof(KiloBitsPerSecond));
            FramesPerSecond = GetUintFromProperty(element, nameof(FramesPerSecond));

            if (!element.TryGetProperty(nameof(RecordingName), out var recordingElement))
            {
                throw new InvalidConfigurationException("Failed to parse directory path.");
            }

            RecordingName = recordingElement.GetString();
        }

        private static uint GetUintFromProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var propertyElement)
                || !propertyElement.TryGetUInt32(out var parsedValue))
            {
                throw new InvalidConfigurationException("Failed to parse value.");
            }

            return parsedValue;
        }
    }
}