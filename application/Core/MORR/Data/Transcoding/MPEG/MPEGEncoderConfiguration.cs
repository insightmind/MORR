using System;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.Mpeg
{
    public class MpegEncoderConfiguration : IConfiguration
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

            Width = GetUintFromProperty(element, nameof(Width));
            Height = GetUintFromProperty(element, nameof(Height));
            KiloBitsPerSecond = GetUintFromProperty(element, nameof(KiloBitsPerSecond));
            FramesPerSecond = GetUintFromProperty(element, nameof(FramesPerSecond));

            if (!element.TryGetProperty(nameof(RelativeFilePath), out var recordingElement))
            {
                throw new InvalidConfigurationException("Failed to parse relative file path.");
            }

            RelativeFilePath = new FilePath(recordingElement.GetString(), true);
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

        protected bool Equals(MpegEncoderConfiguration other)
        {
            return other != null 
                   && Width == other.Width 
                   && Height == other.Height 
                   && KiloBitsPerSecond == other.KiloBitsPerSecond 
                   && FramesPerSecond == other.FramesPerSecond 
                   && RelativeFilePath.ToString().Equals(other.RelativeFilePath.ToString());
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MpegEncoderConfiguration) obj);
        }
    }
}