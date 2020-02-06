using System;
namespace MORR.Core.CLI.Output
{
    public interface IOutputFormatter
    {
        bool IsVerbose { get; set; }

        void PrintError(Exception exception);
        void PrintDebug(string message);
        void Print(string message);

        string Read();
    }
}
