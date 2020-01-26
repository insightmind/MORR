using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;

namespace MORR.Modules.Mouse
{
    [Export(typeof(MouseModuleConfiguration))]
    public class MouseModuleConfiguration : IConfiguration
    {
        /// <summary>
        ///     The time interval between invocation of method to record mouse position, in milliseconds.
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        ///     The minimal distance a mouse move must reach in a period to be recorded.
        ///     (A mouse move with distance less than the threshold will be ignored,
        ///     in other words, a new MouseMoveEvent will not be generated and
        ///     the mouse position will not be recorded, the distance will be computed in screen coordinates.)
        /// </summary>
        public int Threshold { get; set; }

        public string Identifier { get; } = "MouseModule";

        public void Parse(RawConfiguration configuration)
        {
            var element = JsonDocument.Parse(configuration.RawValue).RootElement;
            try
            {
                int period;
                int threshold;
                Int32.TryParse(element.GetProperty("Period").GetString(),out period);
                Int32.TryParse(element.GetProperty("Threshold").GetString(),out threshold);
                Period = period;
                Threshold = threshold;
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidConfigurationException("Failed to parse the period and the threshold for mouse module.");
            }
        }
    }
}