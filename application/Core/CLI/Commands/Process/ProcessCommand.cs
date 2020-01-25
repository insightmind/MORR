using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MORR.Core.Recording;
using MORR.Shared.Utility;

namespace MORR.Core.CLI.Commands.Process
{
    internal class ProcessCommand : ICLICommand<ProcessOptions>
    {
        public int Execute(ProcessOptions options)
        {
            if (options == null)
            {
                return -1;
            }

            try
            {
                var configPath = new FilePath(Path.GetFullPath(options.ConfigPath));
                var inputPath = new FilePath(Path.GetFullPath(options.InputFile));

                IRecordingManager recordingManager = new RecordingManager(configPath);
                recordingManager.Process(new []{ inputPath });

                while (true) { }
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine("ERROR: " + exception.Message);
                return -1;
            }
        }
    }
}
