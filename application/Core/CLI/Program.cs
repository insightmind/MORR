using System;
using CommandLine;
using Morr.Core.CLI.CLI;
using Morr.Core.CLI.Execution;

namespace MORR.Core.CLI
{
    /// <summary>
    /// Command line tool entry point
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                  .WithParsed<Options>(new Executor().Execute);
        }
    }
}
