using System;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;

namespace MORR.Core.Data.Capture.Video.Desktop
{
    public class DesktopCaptureConfiguration : IConfiguration
    {
        /// <summary>
        ///     The index of the monitor to capture.
        /// </summary>
        public Index MonitorIndex { get; private set; }

        /// <summary>
        ///     Indicates whether the user should be prompted to manually select the monitor to capture.
        ///     <see langword="true" /> if the user should be prompted, <see langword="false" /> otherwise.
        /// </summary>
        public bool PromptUserForMonitorSelection { get; private set; }

        public void Parse(RawConfiguration configuration)
        {
            var element = JsonDocument.Parse(configuration.RawValue).RootElement;

            if (!element.TryGetProperty(nameof(MonitorIndex), out var indexElement) ||
                !indexElement.TryGetInt32(out var monitorIndex))
            {
                throw new InvalidConfigurationException("Failed to parse monitor index.");
            }

            MonitorIndex = Index.FromStart(monitorIndex);

            if (!element.TryGetProperty(nameof(PromptUserForMonitorSelection), out var promptElement))
            {
                throw new InvalidConfigurationException("Failed to parse prompt behaviour.");
            }

            PromptUserForMonitorSelection = promptElement.GetBoolean();
        }
    }
}