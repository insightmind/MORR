using MORR.Core.CLI.Output;
using MORR.Core.Session;
using MORR.Shared.Utility;
using System;
using System.Diagnostics;
using System.IO;
using MORR.Core.CLI.Interactive;
using MORR.Core.CLI.Utility;

namespace MORR.Core.CLI.Commands.Record
{
    public class RecordCommand : ICommand<RecordOptions>
    {
        #region Constants
        
        private const string loadedFileMessage = "Load configuration file.";
        private const string sessionManagerMessage = "Load session manager with configuration file.";
        private const string startRecordingMessage = "Start recording session:";
        private const string stopRecordingMessage = "Recording session stopped!";
        
        #endregion

        #region Dependencies

        private readonly ISessionManager sessionManager;
        private readonly IConsoleFormatter consoleFormatter;
        private readonly IInteractiveCommandLine commandLine;
        private readonly IMessageLoop messageLoop;

        #endregion

        #region LifeCycle

        public RecordCommand(
            ISessionManager sessionManager, 
            IConsoleFormatter consoleFormatter,
            IInteractiveCommandLine commandLine,
            IMessageLoop messageLoop)
        {
            this.sessionManager = sessionManager;
            this.consoleFormatter = consoleFormatter;
            this.commandLine = commandLine;
            this.messageLoop = messageLoop;
        }

        #endregion

        #region Execution
        public int Execute(RecordOptions options)
        {
            Debug.Assert(consoleFormatter != null, nameof(consoleFormatter) + " != null");
            Debug.Assert(sessionManager != null, nameof(sessionManager) + " != null");
            Debug.Assert(commandLine != null, nameof(commandLine) + " != null");
            Debug.Assert(messageLoop != null, nameof(messageLoop) + " != null");

            if (options == null) return -1;

            try
            {
                consoleFormatter.IsVerbose = options.IsVerbose;

                // Load Configuration File
                consoleFormatter.PrintDebug(loadedFileMessage);
                var configPath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // Load Session Manager
                consoleFormatter.PrintDebug(sessionManagerMessage);

                // Start Recording
                consoleFormatter.PrintDebug(startRecordingMessage);
                sessionManager.StartRecording();

                // If the user cancels via the command line we need to stop the message loop.
                commandLine.Launch(messageLoop.Stop);

                // Run message loop required for Windows hooks
                messageLoop.Start();

                // To prevent the generated video file from becoming corrupted, recording needs to be stopped manually
                sessionManager.StopRecording();
                Console.WriteLine(stopRecordingMessage);

                return 0;
            }
            catch (Exception exception) // I know this is not a recommend way to deal with exception, however this method receives a arbitrary amount of exception types.
            {
                consoleFormatter.PrintError(exception);
                return -1;
            }
        }

        #endregion
    }
}
