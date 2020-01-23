using System;
using System.IO;
using MORR.Core.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core.CLI.Commands.Validate
{
    internal class ValidateCommand : ICLICommand<ValidateOptions>
    {
        public int Execute(ValidateOptions options)
        {
            if (options == null)
            {
                return -1;
            }

            try
            {
                var filePath = new FilePath(Path.GetFullPath(options.ConfigPath));

                IConfigurationManager configurationManager = new ConfigurationManager();

                IBootstrapper bootstrapper = new Bootstrapper();
                bootstrapper.ComposeImports(configurationManager);

                configurationManager.LoadConfiguration(filePath);

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
