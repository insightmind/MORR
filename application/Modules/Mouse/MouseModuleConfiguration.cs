using System;
using System.Composition;
using MORR.Shared.Configuration;

namespace MORR.Modules.Mouse
{
    [Export(typeof(MouseModuleConfiguration))]
    [Export(typeof(IConfiguration))]
    public class MouseModuleConfiguration : IConfiguration
    {
        /// <summary>
        ///     The time interval between invocation of method to record mouse position, in milliseconds.
        /// </summary>
        public int period { get; set; }

        /// <summary>
        ///     The minimal distance a mouse move must reach in a period to be recorded.
        ///     (A mouse move with distance less than the threshold will be ignored,
        ///     in other words, a new MouseMoveEvent will not be generated and
        ///     the mouse position will not be recorded)
        /// </summary>
        public double threshold { get; set; }

        public string Identifier { get; } = "MouseModule";

        public void Parse(string configuration)
        {
            // TODO Implement this .
            throw new NotImplementedException();
        }
    }
}