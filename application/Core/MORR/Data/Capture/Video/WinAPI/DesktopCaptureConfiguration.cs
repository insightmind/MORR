using System;
using System.Composition;
using MORR.Shared.Configuration;

namespace MORR.Core.Data.Capture.Video.WinAPI
{
    [Export(typeof(DesktopCaptureConfiguration))]
    [Export(typeof(IConfiguration))]
    public class DesktopCaptureConfiguration : IConfiguration
    {
        /// <summary>
        ///     The index of the monitor to capture.
        /// </summary>
        public byte MonitorIndex { get; set; }

        /// <summary>
        ///     Indicates whether the user should be prompted to manually select the monitor to capture.
        ///     <see langword="true" /> if the user should be prompted, <see langword="false" /> otherwise.
        /// </summary>
        public bool PromptUserForMonitorSelection { get; set; }

        public string Identifier { get; } = "DesktopCapture";

        public void Parse(string configuration)
        {
            // TODO Implement once format has been decided
            throw new NotImplementedException();
        }
    }
}