using MORR.Core.Recording;
using MORR.Shared.Utility;
using System;
using System.IO;
using MORR.Core.CLI.Output;

namespace MORR.Core.CLI.Commands.Process
{
    internal class ProcessCommand : ICLICommand<ProcessOptions>
    {
        private const string loadedFileMessage = "Load configuration file.";
        private const string loadInputMessage = "Load input file.";
        private const string recordingManagerMessage = "Load recording manager with configuration file.";
        private const string startProcessingMessage = "Start processing session:";

        public int Execute(ProcessOptions options)
        {
            if (options == null)
            {
                return -1;
            }

            try
            {
                OutputFormatter.IsVerbose = options.IsVerbose;

                // Load configuration file
                OutputFormatter.PrintDebug(loadedFileMessage);
                var configPath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // Load input file
                OutputFormatter.PrintDebug(loadInputMessage);
                var inputPath = new FilePath(Path.GetFullPath(options.InputFile));

                // Start recording manager
                OutputFormatter.PrintDebug(recordingManagerMessage);
                IRecordingManager recordingManager = new RecordingManager(configPath);

                // Start processing
                OutputFormatter.PrintDebug(startProcessingMessage);
                recordingManager.Process(new[] { inputPath });

                while (true) { }
            }
            catch (ArgumentException exception)
            {
                OutputFormatter.PrintError(exception);
                return -1;
            }
        }
    }
}
