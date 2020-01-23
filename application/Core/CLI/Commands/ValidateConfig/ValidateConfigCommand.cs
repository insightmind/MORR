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

            var filePath = new FilePath(options.ConfigPath);

            // We probably need to change this over to the session manager, but for now this should be fine.
            IConfigurationManager configurationManager = new ConfigurationManager();
            configurationManager.LoadConfiguration(filePath);

            return 0;
        }
    }
}
