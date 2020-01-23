using CommandLine;

namespace Morr.Core.CLI.Commands.ValidateConfig
{
    [Verb("validate", HelpText = "Validates if a given config.")]
    internal class ValidateConfigOptions : ICommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        internal string ConfigPath { get; set; }
    }
}
