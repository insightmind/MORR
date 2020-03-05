using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Shared.Events;

namespace SharedTest.Events
{
    [TestClass]
    public class EventTest
    {
        public class EventImp : Event { }

        [TestMethod]
        public void TestEvent_TimeStampInitiallySet()
        {
            /* GIVEN */
            var prevTimeStamp = DateTime.Now;

            /* WHEN */
            var @event = new EventImp();
            var afterTimeStamp = DateTime.Now;

            /* THEN */
            Assert.IsNotNull(@event.Timestamp);
            Assert.IsTrue(prevTimeStamp < @event.Timestamp);
            Assert.IsTrue(@event.Timestamp < afterTimeStamp);
        }

        [TestMethod]
        public void TestEvent_TimeStamp()
        {
            /* GIVEN */
            var @event = new EventImp();
            var timeStamp = DateTime.Now;

            /* WHEN */
            @event.Timestamp = timeStamp;

            /* THEN */
            Assert.AreEqual(timeStamp, @event.Timestamp);
        }

        [TestMethod]
        public void TestEvent_IssuingModule()
        {
            /* GIVEN */
            var @event = new EventImp();
            var issuingModule = new Guid();

            /* WHEN */
            @event.IssuingModule = issuingModule;

            /* THEN */
            Assert.AreEqual(issuingModule, @event.IssuingModule);
        }
    }
}
