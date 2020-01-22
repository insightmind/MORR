using System;
using MORR.Shared.Events;

namespace MORR.Core.Data.Sample.Metadata
{
    /// <summary>
    ///     A single metadata capture sample.
    /// </summary>
    public class MetadataSample : Event
    {
        /// <summary>
        ///     The type of the event that is serialized.
        /// </summary>
        public Type EventType { get; set; }

        /// <summary>
        ///     The data that is serialized.
        /// </summary>
        public byte[] SerializedData { get; set; }
    }
}