using System;
using System.Collections.Generic;
using System.Composition;
using System.Text.Json;
using System.Threading.Tasks;
using MORR.Core.Data.Sample.Metadata;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;

namespace MORR.Core.Data.Transcoding.Metadata
{
    public class MetadataDeserializer : IMetadataDeserializer
    {
        [ImportMany]
        private IEnumerable<EventQueue<Event>> EventQueues { get; set; }

        [Import]
        private IReadOnlyEventQueue<MetadataSample> MetadataSampleQueue { get; set; }

        public bool IsEnabled { get; set; }
        public Guid Identifier { get; }

        public void Initialize()
        {
            foreach (var eventQueue in EventQueues)
            {
                Task.Run(() => LinkQueue(eventQueue));
            }
        }

        private async void LinkQueue(EventQueue<Event> eventQueue)
        {
            await foreach (var metadataSample in MetadataSampleQueue.GetEvents())
            {
                if (metadataSample.EventType == eventQueue.EventType)
                {
                    // The actual event type is known to be metadataSample.EventType at runtime
                    // We have to bypass type checking by using dynamic as we cannot cast to the specified type at compile time
                    dynamic @event =
                        JsonSerializer.Deserialize(metadataSample.SerializedData, metadataSample.EventType);
                    eventQueue.Enqueue(@event);
                }
            }
        }
    }
}