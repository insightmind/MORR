using System;
using System.Collections.Generic;
using System.Text;

namespace Morr.Core.CLI.Commands.ValidateConfig
{
    class ValidateConfigCommand: ICLICommand<ValidateConfigOptions>
    {
        public int Execute(ValidateConfigOptions options)
        {
            return 1;
        }
    }
}
