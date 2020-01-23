using System;
using System.IO;
using MORR.Core;
using MORR.Core.Configuration;
using MORR.Shared.Utility;

namespace Morr.Core.CLI.Commands.ValidateConfig
{
    internal class ValidateConfigCommand : ICLICommand<ValidateConfigOptions>
    {
        public int Execute(ValidateConfigOptions options)
        {
            if (options == null)
            {
                return -1;
            }

            try
            {
                var filePath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // We probably need to change this over to the session manager, but for now this should be fine.
                IConfigurationManager configurationManager = new ConfigurationManager();
                configurationManager.LoadConfiguration(filePath);

                IBootstrapper bootstrapper = new Bootstrapper();
                bootstrapper.ComposeImports(configurationManager);

                Console.WriteLine("The configuration file is valid!");
                return 0;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine("ERROR: " + exception.Message);
                return -1;
            }
            catch (InvalidConfigurationException exception)
            {
                Console.WriteLine("ERROR: Unable to parse config file (" + exception.Message + ")" );
                return -1;
            }
        }
    }
}
