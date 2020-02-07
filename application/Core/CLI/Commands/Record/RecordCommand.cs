using MORR.Core.CLI.Output;
using MORR.Core.Session;
using MORR.Shared.Utility;
using System;
using System.IO;
using MORR.Core.CLI.Interactive;

namespace MORR.Core.CLI.Commands.Record
{
    internal class RecordCommand : ICommand<RecordOptions>
    {
        private const string loadedFileMessage = "Load configuration file.";
        private const string sessionManagerMessage = "Load session manager with configuration file.";
        private const string startRecordingMessage = "Start recording session:";
        private const string stopRecordingMessage = "Recording session stopped!";

        public int Execute(RecordOptions options)
        {
            if (options == null)
            {
                return -1;
            }

            try
            {
                OutputFormatter.IsVerbose = options.IsVerbose;

                // Load Configuration File
                OutputFormatter.PrintDebug(loadedFileMessage);
                var configPath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // Load Session Manager
                OutputFormatter.PrintDebug(sessionManagerMessage);
                ISessionManager sessionManager = new SessionManager(configPath);

                // Start Recording
                OutputFormatter.PrintDebug(startRecordingMessage);
                sessionManager.StartRecording();

                // We start our interactive commandline so the user
                // can still interact with the application.
                var commandLine = new InteractiveCommandLine();

                // If the user cancels via the command line we need to stop the message loop.
                commandLine.Launch(NativeMethods.StopMessageLoop);

                // Run message loop required for Windows hooks
                NativeMethods.DoWin32MessageLoop();

                // To prevent the generated video file from becoming corrupted, recording needs to be stopped manually
                sessionManager.StopRecording();
                Console.WriteLine(stopRecordingMessage);
                GlobalHook.FreeLibrary();
                return 0;
            }
            catch (Exception exception) // I know this is not a recommend way to deal with exception, however this method receives a arbitrary amount of exception types.
            {
                OutputFormatter.PrintError(exception);
                return -1;
            }
        }
    }
}
