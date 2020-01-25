using MORR.Core.CLI.Output;
using MORR.Core.Configuration;
using MORR.Shared.Utility;
using System;
using System.IO;

namespace MORR.Core.CLI.Commands.Validate
{
    internal class ValidateCommand : ICLICommand<ValidateOptions>
    {
        private const string successMessage = "The configuration file is valid!";
        private const string failureMessage = "The configuration file is invalid!";
        private const string loadedFileMessage = "Load configuration file.";
        private const string assemblyMessage = "Load all assemblies.";
        private const string resolveConfigMessage = "Resolving Configuration";

        public int Execute(ValidateOptions options)
        {
            if (options == null)
            {
                return -1;
            }

            try
            {
                OutputFormatter.IsVerbose = options.IsVerbose;

                // Load Configuration File
                OutputFormatter.PrintDebug(loadedFileMessage);
                var filePath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // Start Configuration Manager and Bootstrapper
                OutputFormatter.PrintDebug(assemblyMessage);
                IConfigurationManager configurationManager = new ConfigurationManager();

                IBootstrapper bootstrapper = new Bootstrapper();
                bootstrapper.ComposeImports(configurationManager);

                // Resolve Configuration File
                OutputFormatter.PrintDebug(resolveConfigMessage);
                configurationManager.LoadConfiguration(filePath);

                Console.WriteLine(successMessage);
                return 0;
            }
            catch (InvalidConfigurationException exception)
            {
                OutputFormatter.PrintError(exception);
                Console.WriteLine(failureMessage);
                return 1;
            }
            catch (Exception exception) // I know this is not a recommend way to deal with exception, however this method receives a arbitrary amount of exception types.
            {
                OutputFormatter.PrintError(exception);
                return -1;
            }
        }
    }
}
