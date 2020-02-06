namespace MORR.Core.CLI.Utility
{
    /// <summary>
    /// A IMessageLoop allows to instantiate a maybe thread blocking loop to catch messages.
    ///
    /// It therefore pretty much only needs loop characteristics, however it may be blocking.
    /// </summary>
    public interface IMessageLoop
    {
        /// <summary>
        /// Describes whether the MessageLoop is running or not.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the message loop.
        /// This may block the calling thread.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the message loop.
        /// Execution may continue in both calling thread and thread which was used to start the loop.
        /// </summary>
        void Stop();
    }
}
