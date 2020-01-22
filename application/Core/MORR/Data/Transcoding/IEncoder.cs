namespace MORR.Core.Data.Transcoding
{
    /// <summary>
    ///     Encodes provided samples to a file.
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        ///     Encodes the provided samples to a file.
        ///     <remarks>This method will not return before the encoding is finished.</remarks>
        /// </summary>
        void Encode();
    }
}