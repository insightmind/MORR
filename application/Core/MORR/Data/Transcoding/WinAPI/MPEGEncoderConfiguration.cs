using System;
using System.Composition;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.WinAPI
{
    [Export(typeof(MPEGEncoderConfiguration))]
    [Export(typeof(IConfiguration))]
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
        ///     The bit rate in bits per second of the resulting video stream.
        /// </summary>
        public uint BitsPerSecond { get; set; }

        /// <summary>
        ///     The frame rate in frames per second of the resulting video stream.
        /// </summary>
        public uint FramesPerSecond { get; set; }

        /// <summary>
        ///     The directory to store the recordings in.
        /// </summary>
        public FilePath RecordingsDirectory { get; set; }

        public string Identifier { get; } = "MPEGEncoder";

        public void Parse(string configuration)
        {
            // TODO Implement this once a format has been decided.
            throw new NotImplementedException();
        }
    }
}