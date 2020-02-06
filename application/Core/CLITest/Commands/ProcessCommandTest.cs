using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Core.CLI.Commands.Processing;
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
    public class ProcessCommandTest
    {
        private const int failCode = -1;
        private const int successCode = 0;

        private Mock<ISessionManager> managerMock;
        private Mock<IOutputFormatter> outputMock;

        [TestInitialize]
        public void BeforeTest()
        {
            managerMock = new Mock<ISessionManager>();
            outputMock = new Mock<IOutputFormatter>();
        }

        [TestMethod]
        public void TestProcessCommand_Successful()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);

            /* GIVEN */
            var command = new ProcessCommand(managerMock.Object, outputMock.Object);
            var options = new ProcessOptions
            {
                IsVerbose = false,
                ConfigPath = Assembly.GetExecutingAssembly().Location,
                InputFile = Assembly.GetExecutingAssembly().Location
            };

            /* WHEN */
            var returnCode = command.Execute(options);

            /* THEN */

            // We test if the command was successful and returned code 0.
            Assert.AreEqual(successCode, returnCode);
            managerMock.Verify(manager => manager.Process(It.IsAny<IEnumerable<FilePath>>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestProcessCommand_OnProcessError()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);

            /* GIVEN */
            managerMock.
                Setup(manager => manager.Process(It.IsAny<IEnumerable<FilePath>>()))?
                .Throws(new InvalidOperationException());

            var command = new ProcessCommand(managerMock.Object, outputMock.Object);
            var options = new ProcessOptions
            {
                IsVerbose = false,
                ConfigPath = Assembly.GetExecutingAssembly().Location,
                InputFile = Assembly.GetExecutingAssembly().Location
            };

            /* WHEN */
            var returnCode = command.Execute(options);

            /* THEN */

            // We test if the command failed and returned code -1.
            Assert.AreEqual(failCode, returnCode);

            managerMock.Verify(manager => manager.Process(It.IsAny<IEnumerable<FilePath>>()), Times.Once);
            outputMock.Verify(output => output.PrintError(It.IsAny<InvalidOperationException>()), Times.Once);
        }

        [TestMethod]
        public void TestProcessCommand_NullOptions()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);

            /* GIVEN */
            var command = new ProcessCommand(managerMock.Object, outputMock.Object);

            /* WHEN */
            var returnCode = command.Execute(null);

            /* THEN */

            // We test if the command was unsuccessful and returned code -1.
            Assert.AreEqual(failCode, returnCode);

            managerMock.Verify(manager => manager.Process(It.IsAny<IEnumerable<FilePath>>()), Times.Never);
        }

        [TestMethod]
        public void TestProcessCommand_IsVerbosePropagation()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);

            /* GIVEN */
            outputMock.SetupSet(output => output.IsVerbose = true);

            var command = new ProcessCommand(managerMock.Object, outputMock.Object);
            var options = new ProcessOptions
            {
                IsVerbose = true,
                ConfigPath = "C:/",
                InputFile = "C:/"
            };

            /* WHEN */
            command.Execute(options);

            /* THEN */
            outputMock.VerifySet(output => output.IsVerbose = true);
        }
    }
}
