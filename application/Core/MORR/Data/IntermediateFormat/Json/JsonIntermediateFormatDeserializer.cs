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
    public class JsonIntermediateFormatDeserializer : ITransformingModule
    {
        private bool isActive;

        [ImportMany]
        private IEnumerable<ISupportDeserializationEventQueue<Event>> EventQueues { get; set; }

        [Import]
        private IDecodeableEventQueue<JsonIntermediateFormatSample> IntermediateFormatSampleQueue { get; set; }

        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, LinkAllQueues, delegate
            {
                /* TODO Cancel iteration */
            });
        }

        public Guid Identifier { get; } = new Guid("03496342-BBAE-46A7-BCBE-98FACA083B74");

        public void Initialize() { }

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
                if (sample.EventType == eventQueue.EventType)
                {
                    var @event = JsonSerializer.Deserialize(sample.SerializedData, sample.EventType);
                    eventQueue.Enqueue(@event);
                }
            }
        }
    }
}