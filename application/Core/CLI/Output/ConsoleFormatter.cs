using System;

namespace MORR.Core.CLI.Output
{
    /// <summary>
    /// The ConsoleFormat formats any output to the CMD console. It prints
    /// additional indications for error or debug messages for the user.
    /// </summary>
    public class ConsoleFormatter : IConsoleFormatter
    {
        /// <summary>
        /// Defines whether the current output is verbose.
        /// </summary>
        public bool IsVerbose { get; set; } = false;

        private const string debugPrefix = "DEBUG: ";
        private const string errorPrefix = "ERROR: ";

        /// <summary>
        /// Prints an error using the exceptions description.
        /// </summary>
        /// <param name="exception">The exception used to infer the error output.</param>
        public void PrintError(Exception exception)
        {
            Console.WriteLine(errorPrefix + exception.Message);
        }

        /// <summary>
        /// Prints a debug message. This may not result in an output
        /// if the formatter is not in an verbose state.
        /// </summary>
        /// <param name="message">The debug message to be printed.</param>
        public void PrintDebug(string message)
        {
            if (IsVerbose) Print(debugPrefix + message);
        }

        /// <summary>
        /// Prints message no matter the state of the formatter.
        /// </summary>
        /// <param name="message"></param>
        public void Print(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Reads a single line.
        /// </summary>
        /// <returns>A single line in form of a string which was read by the formatter</returns>
        public string Read()
        {
            return Console.ReadLine();
        }
    }
}