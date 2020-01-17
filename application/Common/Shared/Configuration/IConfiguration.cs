namespace MORR.Shared.Configuration
{
    /// <summary>
    ///     A self-contained unit of configuration
    /// </summary>
    public interface IConfiguration
    {
        string Identifier { get; }

        /// <summary>
        ///     Parses the configuration from the provided value
        /// </summary>
        /// <param name="configuration">The configuration <see cref="string" /> to parse from</param>
        void Parse(string configuration);
    }
}