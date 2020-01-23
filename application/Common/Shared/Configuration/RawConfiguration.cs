using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MORR.Shared.Configuration
{
    public class RawConfiguration
    {
        public string RawValue { get; }

        public RawConfiguration(string rawValue)
        {
            RawValue = rawValue;
        }
    }
}
