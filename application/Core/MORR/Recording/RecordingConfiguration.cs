using System;
using System.Composition;
using MORR.Shared.Configuration;

namespace MORR.Core.Recording
{
    [Export(typeof(RecordingConfiguration))]
    [Export(typeof(IConfiguration))]
    public class RecordingConfiguration : IConfiguration
    {
        public Type Encoder { get; private set; }

        public Type Decoder { get; private set; }

        public void Parse(string configuration)
        {
            throw new System.NotImplementedException();
        }
    }
}