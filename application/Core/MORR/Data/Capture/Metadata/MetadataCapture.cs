using System;
using System.Composition;
using MORR.Shared.Modules;

namespace MORR.Core.Data.Capture.Metadata
{
    [Export(typeof(IModule))]
    public class MetadataCapture : IMetadataCapture
    {
        public bool IsActive { get; set; }
        internal static Guid Identifier => new Guid("2D61FFB2-9CC1-4AAD-B1B9-A362FCF022A0");

        [Import]
        private MetadataSampleProducer Producer { get; set; }

        Guid IModule.Identifier => Identifier;

        public void Initialize()
        {
            Producer.Initialize();
        }
    }
}