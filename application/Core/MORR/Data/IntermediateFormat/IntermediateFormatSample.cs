using System;
using MORR.Shared.Events;

namespace MORR.Core.Data.IntermediateFormat
{
    /// <summary>
    ///     A sample in an intermediate format.
    /// </summary>
    public abstract class IntermediateFormatSample : Event
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