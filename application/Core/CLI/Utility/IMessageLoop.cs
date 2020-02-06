namespace MORR.Core.CLI.Utility
{
    public interface IMessageLoop
    {
        bool IsRunning { get; }

        void Start();
        void Stop();
    }
}
