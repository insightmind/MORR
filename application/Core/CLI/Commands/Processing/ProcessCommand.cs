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
        private readonly IOutputFormatter outputFormatter;

        #endregion

        #region LifeCycle

        public ProcessCommand(
            ISessionManager sessionManager,
            IOutputFormatter outputFormatter)
        {
            this.sessionManager = sessionManager;
            this.outputFormatter = outputFormatter;
        }

        #endregion

        #region Execution

        public int Execute(ProcessOptions options)
        {
            Debug.Assert(outputFormatter != null, nameof(outputFormatter) + " != null");
            Debug.Assert(sessionManager != null, nameof(sessionManager) + " != null");

            if (options == null) return -1;

            try
            {
                outputFormatter.IsVerbose = options.IsVerbose;

                // Load configuration file
                outputFormatter.PrintDebug(loadedFileMessage);
                var configPath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // Load input file
                outputFormatter.PrintDebug(loadInputMessage);
                var inputPath = new DirectoryPath(Path.GetFullPath(options.InputFile));

                // Start session manager
                outputFormatter.PrintDebug(sessionManagerMessage);

                // Start processing
                outputFormatter.PrintDebug(startProcessingMessage);
                sessionManager.Process(new[] { inputPath });
                outputFormatter.PrintDebug(completeProcessingMessage);

                return 0;
            }
            catch (Exception exception)
            {
                outputFormatter.PrintError(exception);

                return -1;
            }
        }

        #endregion
    }
}
