using CommandLine;

namespace Morr.Core.CMD.CLI
{
    /// <summary>
    /// Defines the argument options/input of the command line interface.
    /// </summary>
    internal class Options
    {
        /// <summary>
        /// (Required) The name of the file which contains metadata of a recording session.
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "The name of the file which contains metadata of a recording session.")] 
        internal string Input { get; set; }

        /// <summary>
        /// (Required) The filename of the output file.
        /// </summary>
        [Option('o', "output", Required = true, HelpText = "The filename of the output file.")]
        internal string Output { get; set; }

        /// <summary>
        /// (Default: true) If set to true the tool will semantically process the metadata using the configured pipeline.
        /// </summary>
        [Option('p', "processing", Required = false, Default = true, HelpText = "If set to true the tool will semantically process the metadata.")]
        internal bool ProcessingEnabled { get; set; }

        /// <summary>
        /// (Default: false) If set to true the tool will print verbose messages.
        /// </summary>
        [Option('v', "verbose", Required = false, Default = false, HelpText = "If set to true the tool will print verbose messages.")]
        internal bool Verbose { get; set; }
    }
}
