using System;
using System.Composition;
using System.Text.Json;
using MORR.Shared.Configuration;

namespace MORR.Core.Recording
{
    [Export(typeof(RecordingConfiguration))]
    [Export(typeof(IConfiguration))]
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
            var instance = JsonSerializer.Deserialize<RecordingConfiguration>(configuration);
            Encoder = instance.Encoder;
            Decoder = instance.Decoder;
        }
    }
}