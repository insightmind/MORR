using CommandLine;

namespace MORR.Core.CLI.Commands
{
    internal abstract class CommandOptions
    {
        [Option('v', "verbose", Required = false, Default = false, HelpText = "Defines whether the application should run in verbose mode.")]
        public bool IsVerbose { get; set; }
    }
}
