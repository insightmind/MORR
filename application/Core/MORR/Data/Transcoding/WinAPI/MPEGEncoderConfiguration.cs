using System;
using System.ComponentModel.Composition;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.WinAPI
{
    [Export(typeof(MPEGEncoderConfiguration))]
    public class MPEGEncoderConfiguration : IConfiguration
    {
        /// <summary>
        ///     The width in pixels of the incoming video stream.
        /// </summary>
        public uint InputWidth { get; set; }

        /// <summary>
        ///     The height in pixels of the incoming video stream.
        /// </summary>
        public uint InputHeight { get; set; }

        /// <summary>
        ///     The width in pixels of the resulting video stream.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        ///     The height in pixels of the resulting video stream.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        ///     The bit rate in kilo bits per second of the resulting video stream.
        /// </summary>
        public uint KiloBitsPerSecond { get; set; }

        /// <summary>
        ///     The frame rate in frames per second of the resulting video stream.
        /// </summary>
        public uint FramesPerSecond { get; set; }

        /// <summary>
        ///     The directory to store the recordings in.
        /// </summary>
        public DirectoryPath RecordingsDirectory { get; set; }

        public void Parse(RawConfiguration configuration)
        {
            var element = JsonDocument.Parse(configuration.RawValue).RootElement;

            InputWidth = GetUintFromProperty(element, nameof(InputWidth));
            InputHeight = GetUintFromProperty(element, nameof(InputHeight));
            Width = GetUintFromProperty(element, nameof(Width));
            Height = GetUintFromProperty(element, nameof(Height));
            KiloBitsPerSecond = GetUintFromProperty(element, nameof(KiloBitsPerSecond));
            FramesPerSecond = GetUintFromProperty(element, nameof(FramesPerSecond));

            if (!element.TryGetProperty(nameof(RecordingsDirectory), out var directoryElement))
            {
                throw new InvalidConfigurationException("Failed to parse directory path.");
            }

            var directory = directoryElement.ToString();
            var resolvedDirectory = Environment.ExpandEnvironmentVariables(directory);
            RecordingsDirectory = new DirectoryPath(resolvedDirectory);
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