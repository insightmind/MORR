using CommandLine;

namespace Morr.Core.CLI.Commands.ValidateConfig
{
    [Verb("validate", HelpText = "Validates if a given config.")]
    public class ValidateConfigOptions : CommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        public string ConfigPath { get; set; }
    }
}
