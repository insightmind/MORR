using System;

namespace MORR.Shared.Events
{
    /// <summary>
    ///     The base type of every user interaction event
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        ///     The timestamp at which the event occured
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        ///     The identifier of the module that issued the event
        /// </summary>
        public Guid IssuingModule { get; set; }
    }
}