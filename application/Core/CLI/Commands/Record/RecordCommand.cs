using MORR.Core.CLI.Output;
using MORR.Core.Session;
using MORR.Shared.Utility;
using System;
using System.IO;

namespace MORR.Core.CLI.Commands.Record
{
    internal class RecordCommand : ICLICommand<RecordOptions>
    {
        private const string loadedFileMessage = "Load configuration file.";
        private const string sessionManagerMessage = "Load session manager with configuration file.";
        private const string startRecordingMessage = "Start recording session:";

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

                // Run message loop required for Windows hooks
                NativeMethods.DoWin32MessageLoop();
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
