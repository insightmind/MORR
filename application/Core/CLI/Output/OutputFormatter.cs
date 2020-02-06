using MORR.Core.Data.Transcoding.Exceptions;
using MORR.Core.Data.IntermediateFormat;
using MORR.Shared.Events.Queue;
using System;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Shared.Utility;

namespace MORR.Core.CLI.Output
{
    public class OutputFormatter : IOutputFormatter
    {
        internal static bool IsVerbose = false;
        private static readonly string DebugPrefix = "DEBUG: ";
        private static readonly string ErrorPrefix = "ERROR: ";
        private static readonly string DateFormatString = "HH:mm:ss.fff";

        public ManualResetEvent EncodeFinished { get; } = new ManualResetEvent(false);

        [Import]
        private IEncodeableEventQueue<JsonIntermediateFormatSample> MetadataQueue { get; set; }

        public async void Encode(DirectoryPath recordingDirectoryPath)
        {
            if (MetadataQueue == null)
            {
                throw new EncodingException();
            }

            EncodeFinished.Reset();

            await foreach (var sample in MetadataQueue.GetEvents())
            {

                PrintSample(sample);
            }

            EncodeFinished.Set();
        }

        private static void PrintSample(IntermediateFormatSample sample)
        {
            if (sample == null)
            {
                throw new EncodingException();
            }

            var output = Encoding.UTF8.GetString(sample.Data);
            var timestamp = DateTime.Now.ToString(DateFormatString);

            Console.WriteLine($"{timestamp}: {output}");
        }

        internal static void PrintError(Exception exception)
        {
            Console.WriteLine(ErrorPrefix + exception.Message);
        }

        internal static void PrintDebug(String message)
        {
            if (IsVerbose)
            {
                Console.WriteLine(DebugPrefix + message);
            }
        }
    }
}