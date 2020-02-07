using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Core.CLI.Output;
using MORR.Core.CLI.Interactive;

namespace CLITest.Interactive
{
    [TestClass]
    public class InteractiveCommandLineTest
    {
        private const int waitTimeInMilliseconds = 1000;

        private Mock<IConsoleFormatter> outputMock;

        [TestInitialize]
        public void BeforeTest()
        {
            outputMock = new Mock<IConsoleFormatter>();
        }

        [TestMethod]
        public void TestInteractive_Completion()
        {
            // Precondition
            Debug.Assert(outputMock != null);

            /* GIVEN */
            var autoResetEvent = new AutoResetEvent(false);
            var isComplete = false;

            outputMock
                .SetupSequence(output => output.Read())?
                .Returns("notX")?
                .Returns("x");

            var commandLine = new InteractiveCommandLine(outputMock.Object);

            /* WHEN */
            commandLine.Launch(() =>
            {
                isComplete = true;
                autoResetEvent.Set();
            });

            /* THEN */
            Assert.IsTrue(autoResetEvent.WaitOne(waitTimeInMilliseconds));
            Assert.IsTrue(isComplete);
            
            outputMock.Verify(output => output.Read(), Times.Exactly(2));
        }

        [TestMethod]
        public void TestInteractive_Retry()
        {
            // Precondition
            Debug.Assert(outputMock != null);

            /* GIVEN */
            var autoResetEvent = new AutoResetEvent(false);
            var isComplete = false;

            outputMock
                .SetupSequence(output => output.Read())?
                .Returns("notX")?
                .Returns("again")?
                .Returns("x")?
                .Returns("x"); // This one should be superfluous

            var commandLine = new InteractiveCommandLine(outputMock.Object);

            /* WHEN */
            commandLine.Launch(() =>
            {
                isComplete = true;
                autoResetEvent.Set();
            });

            /* THEN */
            Assert.IsTrue(autoResetEvent.WaitOne(waitTimeInMilliseconds));
            Assert.IsTrue(isComplete);

            outputMock.Verify(output => output.Read(), Times.Exactly(3));
        }

        [TestMethod]
        public void TestInteractive_NullOutputFormatter()
        {
            // Precondition
            Debug.Assert(outputMock != null);

            /* GIVEN */
            var autoResetEvent = new AutoResetEvent(false);
            var isComplete = false;

            var commandLine = new InteractiveCommandLine(null);

            /* WHEN */
            commandLine.Launch(() =>
            {
                isComplete = true;
                autoResetEvent.Set();
            });

            /* THEN */
            Assert.IsTrue(autoResetEvent.WaitOne(waitTimeInMilliseconds));
            Assert.IsTrue(isComplete);
        }
    }
}
