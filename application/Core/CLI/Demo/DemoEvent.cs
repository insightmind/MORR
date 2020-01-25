using System;
using MORR.Shared.Events;

namespace MORR.Core.CLI.Demo
{
    public class DemoEvent : Event
    {
        public int Num { get; set; }
    }
}