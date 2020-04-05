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
        public Index MonitorIndex { get; set; }

        /// <summary>
        ///     Indicates whether the user should be prompted to manually select the monitor to capture.
        ///     <see langword="true" /> if the user should be prompted, <see langword="false" /> otherwise.
        /// </summary>
        public bool PromptUserForMonitorSelection { get; set; }

        public void Parse(RawConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException();
            }

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

        public override bool Equals(object? obj)
        {
            return (obj is DesktopCaptureConfiguration configuration)
                   && MonitorIndex.Value == configuration.MonitorIndex.Value
                   && PromptUserForMonitorSelection == configuration.PromptUserForMonitorSelection;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MonitorIndex.Value, PromptUserForMonitorSelection);
        }
    }
}