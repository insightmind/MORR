using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Core.CLI.Commands.Record;
using MORR.Core.CLI.Interactive;
using MORR.Core.CLI.Output;
using MORR.Core.Session;
using MORR.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace CLITest.Commands
{
    [TestClass]
    public class RecordCommandTest
    {
        private Mock<ISessionManager> managerMock;
        private Mock<IOutputFormatter> outputMock;
        private Mock<IInteractiveCommandLine> commandLineMock;

        [TestInitialize]
        public void BeforeTest()
        {
            managerMock = new Mock<ISessionManager>();
            outputMock = new Mock<IOutputFormatter>();
            commandLineMock = new Mock<IInteractiveCommandLine>();
        }

        [Ignore] // Remove this later if an Interface for NativeMethods exists
        [TestMethod]
        public void TestProcessCommand_Successful()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(commandLineMock != null);

            /* GIVEN */
            var mockSequence = new MockSequence();
            managerMock.InSequence(mockSequence)?.Setup(manager => manager.StartRecording());
            managerMock.InSequence(mockSequence)?.Setup(manager => manager.StopRecording());

            commandLineMock
                .Setup(cli => cli.Launch(It.IsAny<Action>()))?
                .Callback((Action action) => action?.Invoke())?
                .Verifiable();

            var command = new RecordCommand(managerMock.Object, outputMock.Object, commandLineMock.Object);
            var options = new RecordOptions
            {
                IsVerbose = false,
                ConfigPath = Assembly.GetExecutingAssembly().Location
            };

            /* WHEN */
            var returnCode = command.Execute(options);

            /* THEN */

            // We test if the command was successful and returned code 0.
            Assert.AreEqual(0, returnCode);
            managerMock.VerifyAll();
            managerMock.Verify(manager => manager.StartRecording(), Times.Exactly(1));
            managerMock.Verify(manager => manager.StopRecording(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestProcessCommand_NullOptions()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(commandLineMock != null);

            /* GIVEN */
            var command = new RecordCommand(managerMock.Object, outputMock.Object, commandLineMock.Object);

            /* WHEN */
            var returnCode = command.Execute(null);

            /* THEN */

            // We test if the command was successful and returned code 0.
            Assert.AreEqual(-1, returnCode);
            managerMock.Verify(manager => manager.Process(It.IsAny<IEnumerable<FilePath>>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestProcessCommand_IsVerbosePropagation()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(commandLineMock != null);

            /* GIVEN */
            outputMock.SetupSet(output => output.IsVerbose = true);

            var command = new RecordCommand(managerMock.Object, outputMock.Object, commandLineMock.Object);
            var options = new RecordOptions
            {
                IsVerbose = true,
                ConfigPath = ""
            };

            /* WHEN */
            var returnCode = command.Execute(options);

            /* THEN */
            outputMock.VerifySet(output => output.IsVerbose = true);
        }
    }
}
