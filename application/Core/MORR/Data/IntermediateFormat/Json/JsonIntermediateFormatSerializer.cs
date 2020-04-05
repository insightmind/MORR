using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Core.Data.IntermediateFormat.Json
{
    public class JsonIntermediateFormatSerializer : DefaultEncodableEventQueue<JsonIntermediateFormatSample>, IModule
    {
        private bool isActive;
        private CountdownEvent resetCounter = new CountdownEvent(0);

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        [ImportMany]
        private IEnumerable<IReadOnlyEventQueue<Event>> EventQueues { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public void Initialize(bool isEnabled)
        {
            if (isEnabled)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, LinkAllQueues, () => { });
        }

        public Guid Identifier { get; } = new Guid("2D61FFB2-9CC1-4AAD-B1B9-A362FCF022A0");

        private void LinkAllQueues()
        {
            var enabledQueues = EventQueues.Where(queue => !queue.IsClosed);
            resetCounter = new CountdownEvent(enabledQueues.Count());

            foreach (var eventQueue in enabledQueues)
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
                Type = eventType,
                Data = serializedData,
                IssuingModule = Identifier
            };

            return sample;
        }

        private async void LinkSingleQueue(IReadOnlyEventQueue<Event> eventQueue)
        {
            await foreach (var @event in eventQueue.GetEvents())
            {
                var sample = MakeSample(@event);
                Enqueue(sample);
            }

            if (resetCounter.Signal())
            {
                Close();
            }
        }
    }
}