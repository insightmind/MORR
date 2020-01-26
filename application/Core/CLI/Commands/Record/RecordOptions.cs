using CommandLine;

namespace MORR.Core.CLI.Commands.Record
{
    [Verb("record", HelpText = "Starts a new recording with the give configuration")]
    internal class RecordOptions : ICommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        public string ConfigPath { get; set; }

        [Option('v', "verbose", Required = false, Default = false, HelpText = "Defines whether the application should run in verbose mode.")]
        public bool IsVerbose { get; set; }
    }
}
