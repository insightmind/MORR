using System;
using System.Threading;
using MORR.Core.CLI.Output;

namespace MORR.Core.CLI.Interactive
{
    public class InteractiveCommandLine: IInteractiveCommandLine
    {
        private const string exitCommand = "x";
        private const string startMessage = "Use 'x' and enter to stop the current process!";
        private const string closingMessage = "Closing MORR. This may take some time!";

        private Action? completionAction;
        private readonly IConsoleFormatter consoleFormatter;

        public InteractiveCommandLine(IConsoleFormatter consoleFormatter)
        {
            this.consoleFormatter = consoleFormatter;
        }

        /// <summary>
        /// Launches the interactive command line.
        /// It is launched on a separate thread so interactive usage can be provided.
        /// </summary>
        public void Launch(Action completionAction)
        {
            this.completionAction = completionAction;

            var userThread = new Thread(Start);
            userThread.Start();
        }

        /// <summary>
        /// Starts the actual interactive command line.
        /// </summary>
        private void Start()
        {
            if (consoleFormatter == null)
            {
                completionAction?.Invoke();
            }

            consoleFormatter?.Print(startMessage);
            while (consoleFormatter?.Read() != exitCommand);
            consoleFormatter?.Print(closingMessage);

            completionAction?.Invoke();
        }
    }
}
