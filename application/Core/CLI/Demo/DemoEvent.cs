using System;
using MORR.Shared.Events;

namespace MORR.Core.CLI.Demo
{
    public class DemoEvent : Event
    {
        public int Num;

        public DemoEvent()
        {
            var random = new Random();
            Num = random.Next(0, 100000);
        }
    }
}
