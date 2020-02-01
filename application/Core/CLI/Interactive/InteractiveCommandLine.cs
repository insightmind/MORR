using System;
using System.Threading;
using MORR.Shared.Utility;

namespace MORR.Core.CLI.Interactive
{
    internal class InteractiveCommandLine
    {
        private const char exitCommand = 'x';
        private const string startMessage = "Use 'x' and enter to stop the recording!";
        private const string closingMessage = "Closing MORR. This may take some time!";

        public void Launch()
        {
            var userThread = new Thread(Start);
            userThread.Start();
        }

        private void Start()
        {
            Console.WriteLine(startMessage);
            while (Console.Read() != exitCommand);
            Console.WriteLine(closingMessage);
            NativeMethods.StopMessageLoop();
        }
    }
}
