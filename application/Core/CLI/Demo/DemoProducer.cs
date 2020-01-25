using System;
using System.ComponentModel.Composition;
using System.Timers;
using MORR.Shared.Events;
using MORR.Shared.Events.Queue;
using MORR.Shared.Modules;

namespace MORR.Core.CLI.Demo
{
    [Export(typeof(IModule))]
    [Export(typeof(IReadOnlyEventQueue<DemoEvent>))]
    [Export(typeof(IReadOnlyEventQueue<Event>))]
    public class DemoProducer : BoundedMultiConsumerEventQueue<DemoEvent>, IModule
    {
        private Timer timer;
        private int produced;

        public bool IsActive
        {
            get => timer.Enabled;
            set => timer.Enabled = value;
        }

        public Guid Identifier => new Guid("9fff4e22-5815-4184-bf9c-e718e0f256a6");

        public void Initialize()
        {
            timer = new Timer(100);
            timer.AutoReset = true;
            timer.Enabled = false;
            timer.Elapsed += GenerateEvent;
            produced = 0;
        }

        private void GenerateEvent(Object source, ElapsedEventArgs args)
        {
            produced++;
            var @event = new DemoEvent {Num = produced, IssuingModule = Identifier};
            Enqueue(@event);
        }

    }
}
