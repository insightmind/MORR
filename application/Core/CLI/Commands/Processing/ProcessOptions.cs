using CommandLine;

namespace MORR.Core.CLI.Commands.Processing
{
    [Verb("process", HelpText = "Starts the processing from a container file to a new output file using the configuration.")]
    public class ProcessOptions : CommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        public string ConfigPath { get; set; }

        [Option('i', "inputFile", Required = true, HelpText = "Path to input file, which should be processed")]
        public string InputFile { get; set; }
    }
}
