using System;

namespace MORR.Core.Data.Capture.Video.Exceptions
{
    /// <summary>
    ///     An exception thrown if video sample capturing fails.
    /// </summary>
    public class VideoCaptureException : CaptureException
    {
        public VideoCaptureException() { }

        public VideoCaptureException(string message) : base(message) { }

        public VideoCaptureException(string message, Exception innerException) : base(message, innerException) { }
    }
}