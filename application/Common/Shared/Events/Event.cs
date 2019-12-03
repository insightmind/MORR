using System;

namespace MORR.Shared // TODO Move to correct namespace
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

        /// <summary>
        ///     The actual type of the event
        /// </summary>
        public Type Type => GetType(); // TODO We can probably remove this and just use GetType()
    }
}