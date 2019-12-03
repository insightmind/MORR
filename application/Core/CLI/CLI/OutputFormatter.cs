using System;

namespace Morr.Core.CLI.CLI
{
    /// <summary>
    /// Encapsulates the command line output behaviour and functionality of the CMD tool.
    /// </summary>
    internal class OutputFormatter
    {
        /// <summary>
        /// Defines whether the verbose output is configured or not.
        /// </summary>
        internal bool Verbose = false;

        /// <summary>
        /// Prints a message to the command line.
        /// </summary>
        /// <param name="message">The message which should be printed to the user.</param>
        /// <param name="verbose">Defines if the message is verbose and may be dismissed if verbose output is disabled by the user.</param>
        internal void PrintMessage(string message, bool verbose)
        {
            if (verbose && !this.Verbose)
            {
                return;
            }

            // TODO: Implement this
        }

        /// <summary>
        /// Prints an exception in a user-friendly to the command line.
        /// </summary>
        /// <param name="exception">The exception which context should be printed to the user.</param>
        internal void PrintException(Exception exception)
        {
            // TODO: Implement this
        }
    }
}
