using System;

namespace MORR.Core.Data.Transcoding.Exceptions
{
    /// <summary>
    ///     A generic encoding exception
    /// </summary>
    public class EncodingException : Exception
    {
        public EncodingException() { }

        public EncodingException(string message) : base(message) { }

        public EncodingException(string message, Exception innerException) : base(message, innerException) { }
    }
}