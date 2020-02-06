using MORR.Core.CLI.Output;
using MORR.Core.Configuration;
using MORR.Shared.Utility;
using System;
using System.Diagnostics;
using System.IO;

namespace MORR.Core.CLI.Commands.Validate
{
    internal class ValidateCommand : ICommand<ValidateOptions>
    {
        #region Constants

        private const string successMessage = "The configuration file is valid!";
        private const string failureMessage = "The configuration file is invalid!";
        private const string loadedFileMessage = "Load configuration file.";
        private const string assemblyMessage = "Load all assemblies.";
        private const string resolveConfigMessage = "Resolving Configuration";

        #endregion

        #region Dependencies

        private readonly IConfigurationManager configurationManager;
        private readonly IOutputFormatter outputFormatter;
        private readonly IBootstrapper bootstrapper;

        #endregion

        #region LifeCycle

        internal ValidateCommand(
            IConfigurationManager configurationManager, 
            IOutputFormatter outputFormatter,
            IBootstrapper bootstrapper) 
        {
            this.configurationManager = configurationManager;
            this.outputFormatter = outputFormatter;
            this.bootstrapper = bootstrapper;
        }

        #endregion

        #region Execution

        public int Execute(ValidateOptions options)
        {
            Debug.Assert(outputFormatter != null, nameof(outputFormatter) + " != null");
            Debug.Assert(bootstrapper != null, nameof(bootstrapper) + " != null");

            if (options == null)
            {
                return -1;
            }

            try
            {
                
                outputFormatter.IsVerbose = options.IsVerbose;

                // Load Configuration File
                outputFormatter.PrintDebug(loadedFileMessage);
                var filePath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // Start Configuration Manager and Bootstrapper
                outputFormatter.PrintDebug(assemblyMessage);
                bootstrapper.ComposeImports(configurationManager);

                // Resolve Configuration File
                outputFormatter.PrintDebug(resolveConfigMessage);
                configurationManager.LoadConfiguration(filePath);

                Console.WriteLine(successMessage);
                return 0;
            }
            catch (InvalidConfigurationException exception)
            {
                outputFormatter.PrintError(exception);
                Console.WriteLine(failureMessage);
                return 1;
            }
            catch (Exception exception)
            {
                outputFormatter.PrintError(exception);
                return -1;
            }
        }

        #endregion
    }
}
