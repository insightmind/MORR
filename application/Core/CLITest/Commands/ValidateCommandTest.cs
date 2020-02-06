using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Core;
using MORR.Core.CLI.Commands.Record;
using MORR.Core.CLI.Commands.Validate;
using MORR.Core.CLI.Output;
using MORR.Core.Configuration;
using MORR.Shared.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace CLITest.Commands
{
    [TestClass]
    public class ValidateCommandTest
    {
        private Mock<IBootstrapper> bootstrapperMock;
        private Mock<IOutputFormatter> outputMock;
        private Mock<IConfigurationManager> managerMock;

        [TestInitialize]
        public void BeforeTest()
        {
            bootstrapperMock = new Mock<IBootstrapper>();
            outputMock = new Mock<IOutputFormatter>();
            managerMock = new Mock<IConfigurationManager>();
        }

        [TestMethod]
        public void TestProcessCommand_Successful()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(bootstrapperMock != null);

            /* GIVEN */
            var command = new ValidateCommand(managerMock.Object, outputMock.Object, bootstrapperMock.Object);
            var options = new ValidateOptions
            {
                IsVerbose = false,
                ConfigPath = Assembly.GetExecutingAssembly().Location
            };

            /* WHEN */
            var returnCode = command.Execute(options);

            /* THEN */

            // We test if the command was successful and returned code 0.
            Assert.AreEqual(0, returnCode);
            bootstrapperMock.Verify(bootstrapper => bootstrapper.ComposeImports(managerMock.Object), Times.Exactly(1));
            managerMock.Verify(manager => manager.LoadConfiguration(It.IsAny<FilePath>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestProcessCommand_InvalidConfig()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(bootstrapperMock != null);

            /* GIVEN */
            managerMock
                .Setup(manager => manager.LoadConfiguration(It.IsAny<FilePath>()))?
                .Throws(new InvalidConfigurationException());

            var command = new ValidateCommand(managerMock.Object, outputMock.Object, bootstrapperMock.Object);
            var options = new ValidateOptions
            {
                IsVerbose = false,
                ConfigPath = Assembly.GetExecutingAssembly().Location
            };

            /* WHEN */
            var returnCode = command.Execute(options);

            /* THEN */

            // We test if the command was successful and returned code 0.
            Assert.AreEqual(1, returnCode);
            bootstrapperMock.Verify(bootstrapper => bootstrapper.ComposeImports(managerMock.Object), Times.Exactly(1));
            managerMock.Verify(manager => manager.LoadConfiguration(It.IsAny<FilePath>()), Times.Exactly(1));
            outputMock.Verify(output => output.PrintError(It.IsAny<InvalidConfigurationException>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestProcessCommand_NullOptions()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(bootstrapperMock != null);

            /* GIVEN */
            var command = new ValidateCommand(managerMock.Object, outputMock.Object, bootstrapperMock.Object);

            /* WHEN */
            var returnCode = command.Execute(null);

            /* THEN */

            // We test if the command was successful and returned code 0.
            Assert.AreEqual(-1, returnCode);
            managerMock.Verify(manager => manager.LoadConfiguration(It.IsAny<FilePath>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestProcessCommand_IsVerbosePropagation()
        {
            // Preconditions
            Debug.Assert(managerMock != null);
            Debug.Assert(outputMock != null);
            Debug.Assert(bootstrapperMock != null);

            /* GIVEN */
            outputMock.SetupSet(output => output.IsVerbose = true);

            var command = new ValidateCommand(managerMock.Object, outputMock.Object, bootstrapperMock.Object);
            var options = new ValidateOptions
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
