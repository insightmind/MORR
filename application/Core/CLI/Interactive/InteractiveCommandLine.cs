using System;
using System.Threading;
using MORR.Shared.Utility;

namespace MORR.Core.CLI.Interactive
{
    internal class InteractiveCommandLine
    {
        private const string exitCommand = "exit";
        private const string commandLine = "=> ";
        private const string closingMessage = "Closing MORR. This may take some time!";

        public void Launch()
        {
            var userThread = new Thread(Start);
            userThread.Start();
        }

        private void Start()
        {
            do
            {
                Console.Write(commandLine);
            }

            while (Console.ReadLine() != exitCommand);

            Console.WriteLine(closingMessage);
            NativeMethods.StopMessageLoop();
        }
    }
}
