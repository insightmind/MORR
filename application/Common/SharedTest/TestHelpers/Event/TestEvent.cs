namespace SharedTest.TestHelpers.Event
{
    /*
     * Use this class in all SharedTests
     */
    public class TestEvent : MORR.Shared.Events.Event
    {
        /* THis identifier can be used to identify a event in test cases. */
        public int Identifier { get; set; }

        /* This is a simple Event subclass for use in the SharedTest */
        public TestEvent() : this(-1) { }

        public TestEvent(int identifier)
        {
            Identifier = identifier;
        }

        public override bool Equals(object? obj)
        {
            return (obj is TestEvent @event)
                   && Identifier == @event.Identifier
                   && Timestamp.Equals(@event.Timestamp);
        }

        public override int GetHashCode() => Identifier;
    }
}
