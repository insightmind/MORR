using System.IO;
using CommandLine;
using MORR.Core.CLI.Commands.Processing;
using MORR.Core.CLI.Commands.Record;
using MORR.Core.CLI.Commands.Validate;
using MORR.Core.CLI.Interactive;
using MORR.Core.CLI.Output;
using MORR.Core.CLI.Utility;
using MORR.Core.Configuration;
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
            var output = new ConsoleFormatter();

            return Parser
                   .Default
                   .ParseArguments<ValidateOptions, RecordOptions, ProcessOptions>(args)
                   .MapResult(
                       (ValidateOptions opts) => // Loads and executes the Validate Command
                       {
                           var configurationManager = new ConfigurationManager();
                           var bootstrapper = new Bootstrapper();

                           var command = new ValidateCommand(configurationManager, output, bootstrapper);
                           return command.Execute(opts);
                       },
                       (RecordOptions opts) => // Loads and executes the Record Command
                       {
                           var configPath = new FilePath(Path.GetFullPath(opts.ConfigPath));
                           var sessionManager = new SessionManager(configPath);
                           var commandLine = new InteractiveCommandLine(output);
                           var messageLoop = new MessageLoop();

                           var command = new RecordCommand(sessionManager, output, commandLine, messageLoop);
                           return command.Execute(opts);
                       },
                       (ProcessOptions opts) => // Loads and executes the ProcessCommand
                       {
                           var configPath = new FilePath(Path.GetFullPath(opts.ConfigPath));
                           var sessionManager = new SessionManager(configPath);

                           var command = new ProcessCommand(sessionManager, output);
                           return command.Execute(opts);
                       },
                       errs => 1);
        }
    }
}
