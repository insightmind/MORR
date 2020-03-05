using System.Data.Common;
using MORR.Shared.Events;

namespace SharedTest.Events
{
    /*
     * Use this class in all SharedTests
     */
    public class TestEvent : Event
    {
        /* THis identifier can be used to identify a event in test cases. */
        private readonly int identifier = -1;

        /* This is a simple Event subclass for use in the SharedTest */
        public TestEvent() { }

        public TestEvent(int identifier)
        {
            this.identifier = identifier;
        }
    }
}
