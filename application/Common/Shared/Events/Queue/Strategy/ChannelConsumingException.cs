using System;
using System.Collections.Generic;
using System.Text;

namespace MORR.Shared.Events.Queue.Strategy
{
    /// <summary>
    /// A simple Exception that encapsulates errors occuring in ChannelStrategies.
    /// </summary>
    public class ChannelConsumingException: Exception
    {
        public ChannelConsumingException()
        { }

        public ChannelConsumingException(string message) : base(message)
        { }

        public ChannelConsumingException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
