using System;

namespace MORR.Core.Data.Capture
{
    /// <summary>
    ///     A generic capture exception.
    /// </summary>
    public class CaptureException : Exception
    {
        public CaptureException() { }

        public CaptureException(string message) : base(message) { }

        public CaptureException(string message, Exception innerException) : base(message, innerException) { }
    }
}