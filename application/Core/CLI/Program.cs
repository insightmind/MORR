using CommandLine;
using Morr.Core.CLI.Commands.Record;
using Morr.Core.CLI.Commands.ValidateConfig;

namespace MORR.Core.CLI
{
    /// <summary>
    /// Command line tool entry point
    /// </summary>
    public class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ValidateConfigOptions, RecordOptions>(args)
                       .MapResult(
                           (ValidateConfigOptions opts) => new ValidateConfigCommand().Execute(opts),
                           (RecordOptions opts) => new RecordCommand().Execute(opts),
                           errs => 1);
        }
    }
}
