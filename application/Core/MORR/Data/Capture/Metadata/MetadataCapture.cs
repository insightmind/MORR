using System;
using System.Composition;
using MORR.Shared.Modules;

namespace MORR.Core.Data.Capture.Metadata
{
    public class MetadataCapture : IMetadataCapture
    {
        internal static Guid Identifier => new Guid("2D61FFB2-9CC1-4AAD-B1B9-A362FCF022A0");

        [Import]
        private MetadataSampleProducer Producer { get; set; }

        public bool IsEnabled { get; set; }

        Guid IModule.Identifier => Identifier;

        public void Initialize()
        {
            Producer.Initialize();
        }
    }
}