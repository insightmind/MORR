using MORR.Core.Recording;
using MORR.Shared.Utility;

namespace Morr.Core.CLI.Commands.Record
{
    internal class RecordCommand : ICLICommand<RecordOptions>
    {
        public int Execute(RecordOptions options)
        {
            if (options == null)
            {
                return -1;
            }    

            var configPath = new FilePath(options.ConfigPath);
            IRecordingManager recordingManager = new RecordingManager(configPath);

            recordingManager.StartRecording();

            // We probably do not want return immediately as the recording is still running.
            return 0;
        }
    }
}
