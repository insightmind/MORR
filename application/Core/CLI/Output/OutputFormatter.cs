using MORR.Core.Data.Sample.Metadata;
using MORR.Core.Data.Transcoding;
using MORR.Core.Data.Transcoding.Exceptions;
using MORR.Shared.Events.Queue;
using System;
using System.ComponentModel.Composition;
using System.Text;
using System.Text.Json;

namespace MORR.Core.CLI.Output
{
    [Export(typeof(IEncoder))]
    public class OutputFormatter : IOutputFormatter
    {
        private static readonly string DateFormatString = "HH:mm:ss.fff";

        [Import]
        private ITranscodeableEventQueue<MetadataSample> MetadataQueue { get; set; }

        public async void Encode()
        {
            if (MetadataQueue == null)
            {
                throw new EncodingException();
            }

            await foreach (var sample in MetadataQueue.GetEvents())
            {

                PrintSample(sample);
            }
        }

        private static void PrintSample(MetadataSample sample)
        {
            if (sample == null)
            {
                throw new EncodingException();
            }

            dynamic @event = JsonSerializer.Deserialize(sample.SerializedData, sample.EventType);
            var output = Encoding.UTF8.GetString(sample.SerializedData);
            var timestamp = DateTime.Now.ToString(DateFormatString);

            Console.WriteLine($"{timestamp}: {output}");
        }

        private static void PrintError(Exception exception)
        {
            Console.WriteLine($"ERROR: {exception.GetType()} {exception.Message}");
        }
    }
}