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
        private readonly IOutputFormatter outputFormatter;

        public InteractiveCommandLine(IOutputFormatter outputFormatter)
        {
            this.outputFormatter = outputFormatter;
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
            if (outputFormatter == null)
            {
                completionAction?.Invoke();
            }

            outputFormatter.Print(startMessage);
            while (outputFormatter.Read() != exitCommand);
            outputFormatter.Print(closingMessage);

            completionAction?.Invoke();
        }
    }
}
