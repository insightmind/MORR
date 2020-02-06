﻿using CommandLine;
using MORR.Shared.Utility;

namespace MORR.Core.CLI.Commands.Validate
{
    [Verb("validate", HelpText = "Validates if a given config.")]
    internal class ValidateOptions : CommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        public string ConfigPath { get; set; }
    }
}
