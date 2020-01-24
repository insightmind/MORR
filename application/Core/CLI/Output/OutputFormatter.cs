using System;
using System.ComponentModel.Composition;
using System.Text.Json;
using MORR.Core.Data.Sample.Metadata;
using MORR.Core.Data.Transcoding;
using MORR.Core.Data.Transcoding.Exceptions;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Core.CLI.Output
{
    [Export(typeof(IEncoder))]
    public class OutputFormatter : IOutputFormatter
    {
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

            var @event = JsonSerializer.Deserialize(sample.SerializedData, sample.EventType) as Event;
            Console.WriteLine($"{@event.Timestamp.ToShortTimeString()}: {sample.SerializedData}");
        }

        private static void PrintError(Exception exception)
        {
            Console.WriteLine($"ERROR: {exception.GetType()} {exception.Message}");
        }
    }
}