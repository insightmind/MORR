using System;
using System.Collections.Generic;
using System.Text;

namespace Morr.Core.CMD.CLI
{
    internal class OutputFormatter
    {
        internal bool Verbose = false;

        internal void PrintMessage(string message, bool verbose)
        {
            if (verbose && !this.Verbose)
            {
                return;
            }

            // TODO: Implement this
        }

        internal void PrintException(Exception exception)
        {
            // TODO: Implement this
        }
    }
}
