using System;

namespace MORR.Shared.Events.Queue.Strategy
{
    /// <summary>
    /// A simple Exception that encapsulates errors occuring in ChannelStrategies.
    /// </summary>
    public class ChannelConsumingException : Exception
    {
        public ChannelConsumingException(string message) : base(message) { }
    }
}
