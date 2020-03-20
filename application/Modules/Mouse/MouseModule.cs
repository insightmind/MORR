using System;
using System.ComponentModel.Composition;
using MORR.Modules.Mouse.Native;
using MORR.Modules.Mouse.Producers;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Modules.Mouse
{
    /// <summary>
    ///     The <see cref="MouseModule" /> is responsible for recording all mouse related user interactions
    /// </summary>
    public class MouseModule : IModule
    {
        private bool isActive;

        /// <summary>
        ///     A single-writer-multiple-reader queue for MouseClickEvent
        /// </summary>
        [Import]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private MouseClickEventProducer MouseClickEventProducer { get; set; }

        /// <summary>
        ///     A single-writer-multiple-reader queue for MouseScrollEvent
        /// </summary>
        [Import]
        private MouseScrollEventProducer MouseScrollEventProducer { get; set; }

        /// <summary>
        ///     A single-writer-multiple-reader queue for MouseMoveEvent
        /// </summary>
        [Import]
        private MouseMoveEventProducer MouseMoveEventProducer { get; set; }

        /// <summary>
        ///     Configuration of the MouseModule.
        /// </summary>
        [Import]
        private MouseModuleConfiguration MouseModuleConfiguration { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public static Guid Identifier { get; } = new Guid("EFF894B3-4DC9-4605-9937-F02F400B4A62");
        Guid IModule.Identifier => Identifier;

        /// <summary>
        ///     if the module is active or not.
        ///     When a module is being activated, all the producers will start to capture user interacts.
        ///     When a module is being deactivated, all the producers will stop capturing user interacts.
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, StartCapture,
                                          StopCapture);
        }

        /// <summary>
        ///     Initialize the module with Configuration and Producers.
        /// </summary>
        public void Initialize(bool isEnabled)
        {
            // configure all producers with retrieved parameters
            MouseMoveEventProducer.SamplingRateInHz = MouseModuleConfiguration.SamplingRateInHz;
            MouseMoveEventProducer.Threshold = MouseModuleConfiguration.Threshold;

            if (isEnabled)
            {
                MouseClickEventProducer?.Open();
                MouseScrollEventProducer?.Open();
                MouseMoveEventProducer?.Open();
            }
            else
            {
                MouseClickEventProducer?.Close();
                MouseScrollEventProducer?.Close();
                MouseMoveEventProducer?.Close();
            }
        }

        private void StartCapture()
        {
            MouseClickEventProducer?.StartCapture();
            MouseScrollEventProducer?.StartCapture();
            MouseMoveEventProducer?.StartCapture(new NativeMouse());
        }

        private void StopCapture()
        {
            MouseClickEventProducer?.StopCapture();
            MouseScrollEventProducer?.StopCapture();
            MouseMoveEventProducer?.StopCapture();
        }
    }
}