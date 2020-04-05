using CommandLine;

namespace MORR.Core.CLI.Commands
{
    /// <summary>
    /// A CommandOption includes the default options for any
    /// command in the CLI.
    /// </summary>
    public abstract class CommandOptions
    {
        /// <summary>
        /// Defines whether the output of the command should be verbose.
        /// </summary>
        [Option('v', "verbose", Required = false, Default = false, HelpText = "Defines whether the application should run in verbose mode.")]
        public bool IsVerbose { get; set; }
    }
}
