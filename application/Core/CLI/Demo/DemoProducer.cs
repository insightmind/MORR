using System;
using System.ComponentModel.Composition;
using System.Timers;
using MORR.Shared.Events.Queue;
using MORR.Shared.Modules;

namespace MORR.Core.CLI.Demo
{
    [Export(typeof(IModule))]
    [Export(typeof(IReadOnlyEventQueue<DemoEvent>))]
    public class DemoProducer : BoundedMultiConsumerEventQueue<DemoEvent>, IModule
    {
        private Timer timer;

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
        }

        private void GenerateEvent(Object source, ElapsedEventArgs args)
        {
            Enqueue(new DemoEvent());
        }

    }
}
