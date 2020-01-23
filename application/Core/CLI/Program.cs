using CommandLine;
using MORR.Core.CLI.Commands.Process;
using MORR.Core.CLI.Commands.Record;
using MORR.Core.CLI.Commands.Validate;

namespace MORR.Core.CLI
{
    /// <summary>
    /// Command line tool entry point
    /// </summary>
    public class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ValidateOptions, RecordOptions, ProcessOptions>(args)
                       .MapResult(
                           (ValidateOptions opts) => new ValidateCommand().Execute(opts),
                           (RecordOptions opts) => new RecordCommand().Execute(opts),
                           (ProcessOptions opts) => new ProcessCommand().Execute(opts),
                           errs => 1);
        }
    }
}
