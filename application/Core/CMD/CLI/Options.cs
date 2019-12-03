using CommandLine;

namespace Morr.Core.CMD.CLI
{
    internal class Options
    {
        [Option('i', "input", Required = true, HelpText = "The name of the file which contains metadata of a recording session.")] 
        internal string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "The filename of the output file.")]
        internal string Output { get; set; }

        [Option('p', "processing", Required = false, Default = true, HelpText = "If set to true the tool will semantically process the metadata.")]
        internal bool ProcessingEnabled { get; set; }

        [Option('v', "verbose", Required = false, Default = false, HelpText = "If set to true the tool will print verbose messages.")]
        internal bool Verbose { get; set; }
    }
}
