using CommandLine;

namespace Morr.Core.CLI.Commands.Record
{
    [Verb("record", HelpText = "Starts a new recording with the give configuration")]
    internal class RecordOptions : ICommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        public string ConfigPath { get; set; }
    }
}
