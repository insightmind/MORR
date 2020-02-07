using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.CLI.Utility;

namespace CLITest.Utility
{
    [TestClass]
    public class MessageLoopTest
    {
        private const int waitTimeInMilliseconds = 1000;

        [TestMethod]
        public void TestMessageLoop()
        {
            /* GIVEN */
            var autoResetEvent = new AutoResetEvent(false);
            var messageLoop = new MessageLoop();

            /* WHEN */
            var cancelThread = new Thread(() =>
            {
                while (!messageLoop.IsRunning)
                {
                     /*
                      * We need to wait until it actually started.
                      * However this will stop if the autoResetEvent is not fulfilled in time.
                      */
                }

                autoResetEvent.Set();
                messageLoop.Stop();
            });

            cancelThread.Start();
            messageLoop.Start();

            /* THEN */
            Assert.IsTrue(autoResetEvent.WaitOne(waitTimeInMilliseconds));
        }
    }
}
