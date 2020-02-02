using System;
using System.Threading;

namespace MORR.Core.CLI.Interactive
{
    internal class InteractiveCommandLine
    {
        private const char exitCommand = 'x';
        private const string startMessage = "Use 'x' and enter to stop the current process!";
        private const string closingMessage = "Closing MORR. This may take some time!";
        private Action? cancelAction;

        /// <summary>
        /// Launches the interactive command line.
        /// It is launched on a separate thread so interactive usage can be provided.
        /// </summary>
        public void Launch(Action cancelAction)
        {
            this.cancelAction = cancelAction;
            var userThread = new Thread(Start);
            userThread.Start();
        }

        /// <summary>
        /// Starts the actual interactive command line.
        /// </summary>
        private void Start()
        {
            Console.WriteLine(startMessage);
            while (Console.Read() != exitCommand);
            Console.WriteLine(closingMessage);

            cancelAction?.Invoke();
        }
    }
}
