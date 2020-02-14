using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Core.CLI.Commands.Record;
using MORR.Core.CLI.Interactive;
using MORR.Core.CLI.Output;
using MORR.Core.CLI.Utility;
using MORR.Core.Session;
using System;
using System.Diagnostics;
using System.Reflection;
using MORR.Shared.Utility;

namespace CLITest.Commands
{
    [TestClass]
    public class RecordCommandTest
    {
        private const int failCode = -1;
        private const int successCode = 0;

        private Mock<ISessionManager> managerMock;
        private Mock<IConsoleFormatter> outputMock;
        private Mock<IInteractiveCommandLine> commandLineMock;
        private Mock<IMessageLoop> messageLoopMock;

        [TestInitialize]
        public void BeforeTest()
        {
            managerMock = new Mock<ISessionManager>();
            outputMock = new Mock<IConsoleFormatter>();
            commandLineMock = new Mock<IInteractiveCommandLine>();
            messageLoopMock = new Mock<IMessageLoop>();
        }

        [TestMethod]
        public void TestRecordCommand_Successful()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(commandLineMock != null);
            Debug.Assert(messageLoopMock != null);

            /* GIVEN */
            var mockSequence = new MockSequence();
            managerMock.InSequence(mockSequence)?.Setup(manager => manager.StartRecording());
            managerMock.InSequence(mockSequence)?.Setup(manager => manager.StopRecording());

            commandLineMock
                .Setup(cli => cli.Launch(It.IsAny<Action>()))?
                .Callback((Action action) => action?.Invoke())?
                .Verifiable();

            var command = new RecordCommand(managerMock.Object, outputMock.Object, commandLineMock.Object, messageLoopMock.Object);
            var options = new RecordOptions
            {
                IsVerbose = false,
                ConfigPath = Assembly.GetExecutingAssembly().Location
            };

            /* WHEN */
            var returnCode = command.Execute(options);

            /* THEN */

            // We test if the command was successful and returned code 0.
            Assert.AreEqual(successCode, returnCode);

            managerMock.VerifyAll();
            managerMock.Verify(manager => manager.StartRecording(), Times.Once);
            managerMock.Verify(manager => manager.StopRecording(), Times.Once);

            messageLoopMock.Verify(loop => loop.Start(), Times.Once);
            messageLoopMock.Verify(loop => loop.Stop(), Times.Once);
        }

        [TestMethod]
        public void TestRecordCommand_OnStartError()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(commandLineMock != null);
            Debug.Assert(messageLoopMock != null);

            /* GIVEN */
            managerMock
                .Setup(manager => manager.StartRecording())?
                .Throws(new InvalidOperationException());

            var command = new RecordCommand(managerMock.Object, outputMock.Object, commandLineMock.Object, messageLoopMock.Object);
            var options = new RecordOptions
            {
                IsVerbose = false,
                ConfigPath = Assembly.GetExecutingAssembly().Location
            };

            /* WHEN */
            var returnCode = command.Execute(options);

            /* THEN */

            // We test if the command failed and returned code -1.
            Assert.AreEqual(failCode, returnCode);

            managerMock.Verify(manager => manager.StartRecording(), Times.Once);
            managerMock.Verify(manager => manager.StopRecording(), Times.Never);

            outputMock.Verify(output => output.PrintError(It.IsAny<InvalidOperationException>()), Times.Once);

            messageLoopMock.Verify(loop => loop.Start(), Times.Never);
            messageLoopMock.Verify(loop => loop.Stop(), Times.Never);
        }

        [TestMethod]
        public void TestRecordCommand_NullOptions()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(commandLineMock != null);
            Debug.Assert(messageLoopMock != null);

            /* GIVEN */
            var command = new RecordCommand(managerMock.Object, outputMock.Object, commandLineMock.Object, messageLoopMock.Object);

            /* WHEN */
            var returnCode = command.Execute(null);

            /* THEN */

            // We test if the command was unsuccessful and returned code -1.
            Assert.AreEqual(-1, returnCode);

            managerMock.Verify(manager => manager.StartRecording(), Times.Never);
            managerMock.Verify(manager => manager.StopRecording(), Times.Never);
        }

        [TestMethod]
        public void TestRecordCommand_IsVerbosePropagation()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(commandLineMock != null);
            Debug.Assert(messageLoopMock != null);

            /* GIVEN */
            outputMock.SetupSet(output => output.IsVerbose = true);

            var command = new RecordCommand(managerMock.Object, outputMock.Object, commandLineMock.Object, messageLoopMock.Object);
            var options = new RecordOptions
            {
                IsVerbose = true,
                ConfigPath = ""
            };

            /* WHEN */
            command.Execute(options);

            /* THEN */
            outputMock.VerifySet(output => output.IsVerbose = true);
        }
    }
}
