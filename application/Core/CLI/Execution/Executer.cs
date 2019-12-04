using MORR.Core;
using Morr.Core.CLI.CLI;
using MORR.Core.Data.Transcoding;

namespace Morr.Core.CLI.Execution
{
    /// <summary>
    /// The Executor handles, as its name suggests, the execution of a CMD command.
    ///
    /// It validates the options and configures the decoder/encoder as well as the pipeline.
    /// </summary>
    internal class Executor
    {
        /// <summary>
        /// Is used to load the pipeline modules.
        /// </summary>
        private readonly ModuleManager moduleManager;

        /// <summary>
        /// Decodes the given file depending its file specification.
        /// </summary>
        private IDecoder decoder;

        /// <summary>
        /// Executes the CMD tools functionality based on the given options.
        /// </summary>
        /// <param name="options">The options that were inputted by the user as arguments.</param>
        internal void Execute(Options options)
        {
            // TODO: Implement this
        }
    }
}
