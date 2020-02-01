using System;
using System.Diagnostics;
using System.Threading;
using CommandLine;
using MORR.Shared.Utility;

namespace MORR.Core.CLI.Commands
{
    internal abstract class Command<TOptions> where TOptions : CommandOptions
    {

        internal int Execute(TOptions options)
        {
            var userThread = new Thread(StartCLI);
            userThread.Start();

            return Run(options);
        }

        private void StartCLI() // Actually make separate class to handle this.
        {
            Console.Write("MORR => ");
            while (Console.ReadLine() != "exit")
            {
                Console.WriteLine("Unknown command!");
                Console.Write("MORR => ");
            }

            CallExit();
            Console.WriteLine("Closing MORR");
        }

        internal abstract int Run(TOptions options);

        internal void CallExit() => NativeMethods.StopMessageLoop();
    }
}
