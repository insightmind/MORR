using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.Json;
using System.Threading.Tasks;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Core.Data.IntermediateFormat.Json
{
    public class JsonIntermediateFormatDeserializer : IModule
    {
        private bool isActive;

        [ImportMany]
        private IEnumerable<ISupportDeserializationEventQueue<Event>> EventQueues { get; set; } = null!;

        [Import]
        private IDecodableEventQueue<JsonIntermediateFormatSample> IntermediateFormatSampleQueue { get; set; } = null!;

        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, LinkAllQueues, delegate
            {
                /*
                 * Cancel should happen automatically as this is a transforming module and awaits
                 * closing of input event queues.
                 */
            });
        }

        public Guid Identifier { get; } = new Guid("03496342-BBAE-46A7-BCBE-98FACA083B74");

        private void LinkAllQueues()
        {
            foreach (var eventQueue in EventQueues)
            {
                Task.Run(() => LinkSingleQueue(eventQueue));
            }
        }

        private async void LinkSingleQueue(ISupportDeserializationEventQueue<Event> eventQueue)
        {
            await foreach (var sample in IntermediateFormatSampleQueue.GetEvents())
            {
                if (sample.Type == eventQueue?.EventType)
                {
                    var @event = JsonSerializer.Deserialize(sample.Data, sample.Type);
                    eventQueue?.Enqueue(@event);
                }
            }

            eventQueue.Close();
        }
    }
}