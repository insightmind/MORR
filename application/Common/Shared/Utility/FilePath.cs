namespace MORR.Shared.Utility
{
    /// <summary>
    /// Encapsulates a local file path.
    /// </summary>
    public sealed class FilePath
    {
        /// <summary>
        /// Creates a new <see cref="FilePath"/> with the provided value.
        /// </summary>
        /// <param name="value">The value to create the <see cref="FilePath"/> with.</param>
        public FilePath(string value)
        {
            Value = value;
        }

        /// <summary>
        /// The value of the path.
        /// </summary>
        public string Value { get; set; }
    }
}