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
                       (RecordOptions opts) => // Loads and executes the Record Command
                       {
                           var configPath = new FilePath(Path.GetFullPath(opts.ConfigPath));
                           var command = new RecordCommand(new SessionManager(configPath));
                           return command.Execute(opts);
                       },
                       (ProcessOptions opts) => // Loads and executes the ProcessCommand
                       {
                           var configPath = new FilePath(Path.GetFullPath(opts.ConfigPath));
                           var command = new ProcessCommand(new SessionManager(configPath));
                           return command.Execute(opts);
                       },
                       errs => 1);
        }
    }
}
