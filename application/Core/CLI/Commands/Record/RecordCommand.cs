using MORR.Core.CLI.Output;
using MORR.Core.Session;
using MORR.Shared.Utility;
using System;
using System.Diagnostics;
using System.IO;
using MORR.Core.CLI.Interactive;

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
        private readonly IOutputFormatter outputFormatter;
        private readonly IInteractiveCommandLine commandLine;

        #endregion

        #region LifeCycle

        public RecordCommand(
            ISessionManager sessionManager, 
            IOutputFormatter outputFormatter,
            IInteractiveCommandLine commandLine)
        {
            this.sessionManager = sessionManager;
            this.outputFormatter = outputFormatter;
            this.commandLine = commandLine;
        }

        #endregion

        #region Execution
        public int Execute(RecordOptions options)
        {
            Debug.Assert(outputFormatter != null, nameof(outputFormatter) + " != null");
            Debug.Assert(sessionManager != null, nameof(sessionManager) + " != null");
            Debug.Assert(commandLine != null, nameof(commandLine) + " != null");

            if (options == null) return -1;

            try
            {
                outputFormatter.IsVerbose = options.IsVerbose;

                // Load Configuration File
                outputFormatter.PrintDebug(loadedFileMessage);
                var configPath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // Load Session Manager
                outputFormatter.PrintDebug(sessionManagerMessage);

                // Start Recording
                outputFormatter.PrintDebug(startRecordingMessage);
                sessionManager.StartRecording();

                // If the user cancels via the command line we need to stop the message loop.
                commandLine.Launch(NativeMethods.StopMessageLoop);

                // Run message loop required for Windows hooks
                NativeMethods.DoWin32MessageLoop();

                // To prevent the generated video file from becoming corrupted, recording needs to be stopped manually
                sessionManager.StopRecording();
                Console.WriteLine(stopRecordingMessage);

                return 0;
            }
            catch (Exception exception) // I know this is not a recommend way to deal with exception, however this method receives a arbitrary amount of exception types.
            {
                outputFormatter.PrintError(exception);
                return -1;
            }
        }

        #endregion
    }
}
