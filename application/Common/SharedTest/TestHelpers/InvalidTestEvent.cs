using MORR.Shared.Events;

namespace SharedTest.Events
{
    /*
     * Use this class in all SharedTests if you want an incorrectly typed event.
     * This Event is explicitly not related to TestEvent! 
     */
    public class InvalidTypeTestEvent : Event
    {
        /* This is a simple Event subclass for use in the SharedTest */
        public InvalidTypeTestEvent() { }
    }
}
