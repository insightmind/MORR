using System;

namespace MORR.Core.CLI.Interactive
{
    /// <summary>
    /// The interactive command line interface supports adding a custom
    /// separate built in command line interface in an addition to the
    /// default interface provided by CMD.exe.
    /// </summary>
    public interface IInteractiveCommandLine
    {
        /// <summary>
        /// Launches the internal command line.
        /// </summary>
        /// <param name="completionAction">The action called on completion.</param>
        void Launch(Action completionAction);
    }
}
