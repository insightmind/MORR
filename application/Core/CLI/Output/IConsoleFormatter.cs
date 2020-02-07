using System;
namespace MORR.Core.CLI.Output
{
    /// <summary>
    /// The IConsoleFormatter abstracts from the default
    /// Console output allowing to insert different behaviors on printing
    /// and a hierarchical printing options.
    /// </summary>
    public interface IConsoleFormatter
    {
        /// <summary>
        /// Defines whether the current output is verbose.
        /// </summary>
        bool IsVerbose { get; set; }

        /// <summary>
        /// Prints an error using the exceptions description.
        /// </summary>
        /// <param name="exception">The exception used to infer the error output.</param>
        void PrintError(Exception exception);

        /// <summary>
        /// Prints a debug message. This may not result in an output
        /// if the formatter is not in an verbose state.
        /// </summary>
        /// <param name="message">The debug message to be printed.</param>
        void PrintDebug(string message);

        /// <summary>
        /// Prints message no matter the state of the formatter.
        /// </summary>
        /// <param name="message"></param>
        void Print(string message);

        /// <summary>
        /// Reads a single line.
        /// </summary>
        /// <returns>A single line in form of a string which was read by the formatter</returns>
        string Read();
    }
}
