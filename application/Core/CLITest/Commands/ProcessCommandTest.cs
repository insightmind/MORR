using System.Diagnostics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Core.CLI.Commands.Processing;
using MORR.Core.CLI.Output;
using MORR.Core.Session;

namespace CLITest.Commands
{
    [TestClass]
    public class ProcessCommandTest
    {
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
            Assert.AreEqual(0, returnCode);
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
            var returnCode = command.Execute(options);

            /* THEN */
            outputMock.VerifySet(output => output.IsVerbose = true);
        }
    }
}
