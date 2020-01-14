using MORR.Core.Configuration;
using MORR.Core.Data.Transcoding;
using MORR.Shared.Modules;

namespace MORR.Core
{
    /// <summary>
    ///     The SessionManager deals with the coordination and references of the components
    ///     which are necessary to start and stop a session.
    /// </summary>
    public class MORR
    {
        /// <summary>
        ///     The ConfigurationManage which takes care of loading the configuration
        ///     into the application.
        /// </summary>
        internal ConfigurationManager configurationManager;

        /// <summary>
        ///     The specified encoder to save the recordings.
        /// </summary>
        internal IEncoder encoder;

        /// <summary>
        ///     This value indicates whether a recording session is currently running.
        /// </summary>
        public bool isRunning;

        /// <summary>
        ///     The ModuleManager which keeps track of all modules currently loaded
        ///     into the application.
        /// </summary>
        internal ModuleManager moduleManager;

        /// <summary>
        ///     The receiver collects all events which left the pipeline.
        /// </summary>
        internal IReceivingModule receiver;

        /// <summary>
        ///     Starts a new recording session.
        ///     This will not work if a session is already running.
        /// </summary>
        public void Start() { }

        /// <summary>
        ///     Stops and saves the running recording session.
        ///     This will not work if no session is currently running.
        /// </summary>
        public void Stop() { }
    }
}