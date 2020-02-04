using System.Collections.Generic;
using MORR.Shared.Utility;

namespace MORR.Core.Session
{
    /// <summary>
    ///     A manager responsible for all aspects of recording and processing.
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        ///     The path to the directory containing the most recent recording or <see langword="null" /> if no recording has been
        ///     created yet.
        /// </summary>
        DirectoryPath? CurrentRecordingDirectory { get; }

        /// <summary>
        ///     The path to the top-level folder containing the recording subdirectories.
        /// </summary>
        DirectoryPath? RecordingsFolder { get; }

        /// <summary>
        ///     Starts a recording if no session is currently being recorded.
        /// </summary>
        void StartRecording();

        /// <summary>
        ///     Stops a recording if a session is currently being recorded.
        /// </summary>
        void StopRecording();

        /// <summary>
        ///     Processes the specified recordings.
        /// </summary>
        void Process(IEnumerable<DirectoryPath> recordings);
    }
}