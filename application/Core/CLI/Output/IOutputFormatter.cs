using System;
namespace MORR.Core.CLI.Output
{
    internal interface IOutputFormatter
    {
        bool IsVerbose { get; set; }

        void PrintError(Exception exception);
        void PrintDebug(string message);
        void Print(string message);
    }
}
