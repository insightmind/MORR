using MORR.Core.CLI.Output;
using MORR.Core.Session;
using MORR.Shared.Utility;
using System;
using System.Diagnostics;
using System.IO;

namespace MORR.Core.CLI.Commands.Processing
{
    public class ProcessCommand : ICommand<ProcessOptions>
    {
        #region Constants

        private const string loadedFileMessage = "Load configuration file.";
        private const string loadInputMessage = "Load input file.";
        private const string sessionManagerMessage = "Load session manager with configuration file.";
        private const string startProcessingMessage = "Start processing session:";
        private const string completeProcessingMessage = "Processing did complete!";

        #endregion

        #region Dependencies

        private readonly ISessionManager sessionManager;
        private readonly IConsoleFormatter consoleFormatter;

        #endregion

        #region LifeCycle

        public ProcessCommand(
            ISessionManager sessionManager,
            IConsoleFormatter consoleFormatter)
        {
            this.sessionManager = sessionManager;
            this.consoleFormatter = consoleFormatter;
        }

        #endregion

        #region Execution

        public int Execute(ProcessOptions options)
        {
            Debug.Assert(consoleFormatter != null, nameof(consoleFormatter) + " != null");
            Debug.Assert(sessionManager != null, nameof(sessionManager) + " != null");

            if (options == null) return -1;

            try
            {
                consoleFormatter.IsVerbose = options.IsVerbose;

                // Load configuration file
                consoleFormatter.PrintDebug(loadedFileMessage);
                var configPath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // Load input file
                outputFormatter.PrintDebug(loadInputMessage);
                var inputPath = new DirectoryPath(Path.GetFullPath(options.InputFile));

                // Start session manager
                consoleFormatter.PrintDebug(sessionManagerMessage);

                // Start processing
                consoleFormatter.PrintDebug(startProcessingMessage);
                sessionManager.Process(new[] { inputPath });
                consoleFormatter.PrintDebug(completeProcessingMessage);

                return 0;
            }
            catch (Exception exception)
            {
                consoleFormatter.PrintError(exception);

                return -1;
            }
        }

        #endregion
    }
}
