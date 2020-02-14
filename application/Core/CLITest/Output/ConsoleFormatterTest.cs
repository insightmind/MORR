using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.CLI.Output;

namespace CLITest.Output
{
    [TestClass]
    public class ConsoleFormatterTest
    {
        private const string newLine = "\n";

        [TestMethod]
        public void TestConsole_Read()
        {
            /* GIVEN */
            const string stringInput = "InputString";
            Console.SetIn(new StringReader(stringInput));
            var consoleFormatter = new ConsoleFormatter();

            /* WHEN */
            var readString = consoleFormatter.Read();

            /* THEN */
            Assert.AreEqual(stringInput, readString);

        }

        [TestMethod]
        public void TestConsole_Print()
        {
            /* GIVEN */
            const string stringOutput = "OutputString";
            var writer = new StringWriter();
            Console.SetOut(writer);
            var consoleFormatter = new ConsoleFormatter();

            /* WHEN */
            consoleFormatter.Print(stringOutput);

            /* THEN */
            Assert.AreEqual(stringOutput, writer.ToString().Trim());
        }

        [TestMethod]
        public void TestConsole_PrintDebug()
        {
            /* GIVEN */
            const string stringOutput = "OutputString";
            var writer = new StringWriter();
            Console.SetOut(writer);

            var consoleFormatter = new ConsoleFormatter
            {
                IsVerbose = true
            };

            /* WHEN */
            consoleFormatter.PrintDebug(stringOutput);

            /* THEN */
            Assert.AreEqual("DEBUG: " + stringOutput, writer.ToString().Trim());
        }

        [TestMethod]
        public void TestConsole_PrintDebug_NotInDebug()
        {
            /* GIVEN */
            const string stringOutput = "OutputString";
            var writer = new StringWriter();
            Console.SetOut(writer);

            var consoleFormatter = new ConsoleFormatter
            {
                IsVerbose = false
            };

            /* WHEN */
            consoleFormatter.PrintDebug(stringOutput);

            /* THEN */
            Assert.AreEqual("", writer.ToString());
        }

        [TestMethod]
        public void TestConsole_PrintError()
        {
            /* GIVEN */
            const string errorMessage = "Error Message";
            var exception = new Exception(errorMessage);
            var writer = new StringWriter();
            Console.SetOut(writer);

            var consoleFormatter = new ConsoleFormatter();

            /* WHEN */
            consoleFormatter.PrintError(exception);

            /* THEN */
            Assert.AreEqual("ERROR: " + errorMessage, writer.ToString().Trim());
        }
    }
}
