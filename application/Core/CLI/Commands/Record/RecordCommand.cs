using System;
using System.IO;
using MORR.Core.CLI.Output;
using MORR.Core.Recording;
using MORR.Shared.Utility;

namespace MORR.Core.CLI.Commands.Record
{
    internal class RecordCommand : ICLICommand<RecordOptions>
    {
        private const string loadedFileMessage = "Load configuration file.";
        private const string recordingManagerMessage = "Load recording manager with configuration file.";
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

                // Load Recording Manager
                OutputFormatter.PrintDebug(recordingManagerMessage);
                IRecordingManager recordingManager = new RecordingManager(configPath);

                // Start Recording
                OutputFormatter.PrintDebug(startRecordingMessage);
                recordingManager.StartRecording();

                // Run message loop required for Windows hooks
                NativeMethods.DoWin32MessageLoop();
                return 0;
            }
            catch (ArgumentException exception)
            {
                OutputFormatter.PrintError(exception);
                return -1;
            }
        }
    }
}
