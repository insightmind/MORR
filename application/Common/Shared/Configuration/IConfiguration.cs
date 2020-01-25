using System.ComponentModel.Composition;
using System.Text.Json;

namespace MORR.Shared.Configuration
{
    /// <summary>
    ///     A self-contained unit of configuration
    /// </summary>
    [InheritedExport]
    public interface IConfiguration
    {
        /// <summary>
        ///     Parses the configuration from the provided value
        /// </summary>
        /// <param name="configuration">The configuration <see cref="JsonElement" /> to parse from</param>
        void Parse(RawConfiguration configuration);
    }
}