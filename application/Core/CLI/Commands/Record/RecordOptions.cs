using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace Morr.Core.CLI.Commands.Record
{
    [Verb("record", HelpText = "Starts a new recording")]
    public class RecordOptions : ICommandOptions
    {
        // TODO: Add parameters such as debug and recording file location and run config location
    }
}
