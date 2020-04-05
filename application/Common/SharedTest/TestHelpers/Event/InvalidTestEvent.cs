namespace SharedTest.TestHelpers.Event
{
    /*
     * Use this class in all SharedTests if you want an incorrectly typed event.
     * This Event is explicitly not related to TestEvent! 
     */
    public class InvalidTypeTestEvent : MORR.Shared.Events.Event
    {
        /* This is a simple Event subclass for use in the SharedTest */
        public InvalidTypeTestEvent() { }
    }
}
