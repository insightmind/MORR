using System;
using System.IO;
using MORR.Core.Recording;
using MORR.Shared.Utility;

namespace MORR.Core.CLI.Commands.Record
{
    internal class RecordCommand : ICLICommand<RecordOptions>
    {
        public int Execute(RecordOptions options)
        {
            if (options == null)
            {
                return -1;
            }

            try
            {
                var configPath = new FilePath(Path.GetFullPath(options.ConfigPath));

                IRecordingManager recordingManager = new RecordingManager(configPath);
                recordingManager.StartRecording();

                // Run message loop required for Windows hooks
                NativeMethods.DoWin32MessageLoop();
                // To prevent the generated video file from becoming corrupted, recording needs to be stopped manually
                //recordingManager.StopRecording();
                return 0;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine("ERROR: " + exception.Message);
                return -1;
            }
        }
    }
}
