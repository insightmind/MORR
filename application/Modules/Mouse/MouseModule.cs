using System;
using System.ComponentModel.Composition;
using MORR.Modules.Mouse.Producers;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Modules.Mouse
{
    /// <summary>
    ///     The <see cref="MouseModule" /> is responsible for recording all mouse related user interactions
    /// </summary>
    public class MouseModule : Module
    {
        private bool isActive;

        /// <summary>
        ///     A single-writer-multiple-reader queue for MouseClickEvent
        /// </summary>
        [Import]
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

        public new static Guid Identifier { get; } = new Guid("EFF894B3-4DC9-4605-9937-F02F400B4A62");

        /// <summary>
        ///     if the module is active or not.
        ///     When a module is being activated, all the producers will start to capture user interacts.
        ///     When a module is being deactivated, all the producers will stop capturing user interacts.
        /// </summary>
        public new bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, StartCapture,
                                          StopCapture);
        }

        /// <summary>
        ///     Initialize the module with Configuration and Producers.
        /// </summary>
        public new void Initialize()
        {
            // configure all producers with retrieved parameters
            MouseMoveEventProducer.SamplingRateInHz = MouseModuleConfiguration.SamplingRateInHz;
            MouseMoveEventProducer.Threshold = MouseModuleConfiguration.Threshold;
        }

        private void StartCapture()
        {
            MouseClickEventProducer?.Open();
            MouseScrollEventProducer?.Open();
            MouseMoveEventProducer?.Open();
        }

        private void StopCapture()
        {
            MouseClickEventProducer?.Close();
            MouseScrollEventProducer?.Close();
            MouseMoveEventProducer?.Close();
        }
    }
}