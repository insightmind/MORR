using System;

namespace MORR.Core.Data.Transcoding.Exceptions
{
    /// <summary>
    ///     A generic decoding exception
    /// </summary>
    public class DecodingException : Exception
    {
        public DecodingException() { }

        public DecodingException(string message) : base(message) { }

        public DecodingException(string message, Exception innerException) : base(message, innerException) { }
    }
}