using System.Composition;
using System.Text.Json;
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
            var instance = JsonSerializer.Deserialize<DesktopCaptureConfiguration>(configuration);
            MonitorIndex = instance.MonitorIndex;
            PromptUserForMonitorSelection = instance.PromptUserForMonitorSelection;
        }
    }
}