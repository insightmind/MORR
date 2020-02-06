using System;
using System.Collections.Generic;
using System.Text;

namespace MORR.Core.CLI.Interactive
{
    public interface IInteractiveCommandLine
    {
        void Launch(Action completionAction);
    }
}
