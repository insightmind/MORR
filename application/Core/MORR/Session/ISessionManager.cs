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
        ///     Indicates whether a session is currently being recorded. <see langword="true" /> if a session is currently being
        ///     recorded, <see langword="false" /> otherwise.
        /// </summary>
        bool IsRecording { get; }

        /// <summary>
        ///     Starts a recording if no session is currently being recorded.
        /// </summary>
        void StartRecording();

        /// <summary>
        ///     Stops a recording if a session is currently being recorded.
        /// </summary>
        void StopRecording();

        /// <summary>
        ///     Starts the decoding process.
        /// </summary>
        void Process(IEnumerable<FilePath> files);
    }
}