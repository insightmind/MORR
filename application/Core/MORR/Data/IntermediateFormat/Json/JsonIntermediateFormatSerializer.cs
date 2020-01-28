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
    [Export(typeof(IModule))]
    [Export(typeof(IEncodeableEventQueue<JsonIntermediateFormatSample>))]
    [Export(typeof(IEncodeableEventQueue<IntermediateFormatSample>))]
    public class JsonIntermediateFormatSerializer
        : DefaultEncodeableEventQueue<JsonIntermediateFormatSample>, ITransformingModule
    {
        private bool isActive;

        [ImportMany]
        private IEnumerable<IReadOnlyEventQueue<Event>> EventQueues { get; set; }

        public void Initialize() { }

        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, LinkAllQueues, delegate
            {
                /* TODO Cancel iteration */
            });
        }

        public Guid Identifier { get; } = new Guid("2D61FFB2-9CC1-4AAD-B1B9-A362FCF022A0");

        private void LinkAllQueues()
        {
            foreach (var eventQueue in EventQueues)
            {
                Task.Run(() => LinkSingleQueue(eventQueue));
            }
        }

        /// <summary>
        ///     Creates <see cref="IntermediateFormatSample" /> instances from <see cref="Event" /> instances using serialization
        ///     to JSON.
        /// </summary>
        /// <param name="event">The <see cref="Event" /> instance to convert</param>
        /// <returns>The <see cref="IntermediateFormatSample" /> that the event was converted to</returns>
        private JsonIntermediateFormatSample MakeSample(Event @event)
        {
            var eventType = @event.GetType();
            var serializedData = JsonSerializer.SerializeToUtf8Bytes(@event, eventType);

            var sample = new JsonIntermediateFormatSample
            {
                EventType = eventType,
                SerializedData = serializedData,
                IssuingModule = Identifier
            };

            return sample;
        }

        private async void LinkSingleQueue(IReadOnlyEventQueue<Event> eventQueue)
        {
            await foreach (var @event in eventQueue.GetEvents())
            {
                // TODO Cancel iteration once module is deactivated
                var sample = MakeSample(@event);
                Enqueue(sample);
            }
        }
    }
}