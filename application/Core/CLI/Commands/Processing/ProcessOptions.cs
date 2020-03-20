using CommandLine;

namespace MORR.Core.CLI.Commands.Processing
{
    [Verb("process", HelpText = "Starts the processing from a container file to a new output file using the configuration.")]
    public class ProcessOptions : CommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        #pragma warning disable CS8618 // Will always be non nonull as it is required!
        public string ConfigPath { get; set; }
        #pragma warning restore CS8618

        [Option('i', "inputFile", Required = true, HelpText = "Path to input file, which should be processed")]
        #pragma warning disable CS8618 //  Will always be non nonull as it is required!
        public string InputFile { get; set; }
        #pragma warning restore CS8618
    }
}
