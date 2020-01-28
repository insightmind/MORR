using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.Json;
using System.Threading.Tasks;
using MORR.Core.Data.IntermediateFormat;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Core.Data.Capture.Metadata
{
    [Export(typeof(MetadataSampleProducer))]
    [Export(typeof(ITranscodeableEventQueue<IntermediateFormatSample>))]
    [Export(typeof(ITranscodeableEventQueue<Event>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MetadataSampleProducer : DefaultTranscodeableEventQueue<IntermediateFormatSample>
    {
        [ImportMany]
        private IEnumerable<IReadOnlyEventQueue<Event>> EventQueues { get; set; }

        /// <summary>
        ///     Creates <see cref="IntermediateFormatSample" /> instances from <see cref="Event" /> instances using serialization to JSON.
        /// </summary>
        /// <param name="event">The <see cref="Event" /> instance to convert</param>
        /// <returns>The <see cref="IntermediateFormatSample" /> that the event was converted to</returns>
        private static IntermediateFormatSample MakeMetadataSample(Event @event)
        {
            var eventType = @event.GetType();
            var serializedData = JsonSerializer.SerializeToUtf8Bytes(@event, eventType);

            var sample = new IntermediateFormatSample
            {
                EventType = eventType,
                SerializedData = serializedData,
                IssuingModule = MetadataCapture.Identifier
            };

            return sample;
        }

        private async void LinkQueue(IReadOnlyEventQueue<Event> eventQueue)
        {
            await foreach (var @event in eventQueue.GetEvents())
            {
                var sample = MakeMetadataSample(@event);
                Enqueue(sample);
            }
        }

        /// <summary>
        ///     Initializes the producer.
        /// </summary>
        public void Initialize()
        {
            foreach (var eventQueue in EventQueues)
            {
                Task.Run(() => LinkQueue(eventQueue));
            }
        }
    }
}