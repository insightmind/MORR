using System;
using CommandLine;
using Morr.Core.CMD.CLI;
using Morr.Core.CMD.Execution;

namespace MORR.Core.CMD
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
