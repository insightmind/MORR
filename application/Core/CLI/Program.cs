using System.IO;
using CommandLine;
using MORR.Core.CLI.Commands.Processing;
using MORR.Core.CLI.Commands.Record;
using MORR.Core.CLI.Commands.Validate;
using MORR.Core.Session;
using MORR.Shared.Utility;

namespace MORR.Core.CLI
{
    /// <summary>
    /// Command line tool entry point
    /// </summary>
    public class Program
    {
        public static int Main(string[] args)
        {
            return Parser
                   .Default
                   .ParseArguments<ValidateOptions, RecordOptions, ProcessOptions>(args)
                   .MapResult(
                       (ValidateOptions opts) => new ValidateCommand().Execute(opts),
                       (RecordOptions opts) =>
                       {
                           var sessionManager = new SessionManager(new FilePath(Path.GetFullPath(opts.ConfigPath)));
                           return new RecordCommand(sessionManager).Execute(opts);
                       },
                       (ProcessOptions opts) =>
                       {
                           var sessionManager = new SessionManager(new FilePath(Path.GetFullPath(opts.ConfigPath)));
                           return new ProcessCommand(sessionManager).Execute(opts);
                       },
                       errs => 1);
        }
    }
}
