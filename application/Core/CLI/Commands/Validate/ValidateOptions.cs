using CommandLine;

namespace MORR.Core.CLI.Commands.Validate
{
    [Verb("validate", HelpText = "Validates if a given config.")]
    internal class ValidateOptions : ICommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        public string ConfigPath { get; set; }
    }
}
