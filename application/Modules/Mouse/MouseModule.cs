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
    public class MouseModule : ICollectingModule
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

        public static Guid Identifier = new Guid("EFF894B3-4DC9-4605-9937-F02F400B4A62");

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

        Guid IModule.Identifier => Identifier;

        /// <summary>
        ///     Initialize the module with Configuration and Producers.
        /// </summary>
        public void Initialize()
        {
            // retrieve all parameters from the MouseModuleConfiguration
            var samplingRate = MouseModuleConfiguration.SamplingRate;
            var threshold = MouseModuleConfiguration.Threshold;

            // configure all producers with retrieved parameters
            MouseMoveEventProducer.Configure(samplingRate, threshold);
        }

        private void StartCapture()
        {
            MouseClickEventProducer.StartCapture();
            MouseScrollEventProducer.StartCapture();
            MouseMoveEventProducer.StartCapture();
        }

        private void StopCapture()
        {
            MouseClickEventProducer.StopCapture();
            MouseScrollEventProducer.StopCapture();
            MouseMoveEventProducer.StopCapture();
        }
    }
}