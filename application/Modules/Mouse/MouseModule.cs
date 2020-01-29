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
    [Export(typeof(IModule))]
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

        public static Guid Identifier => new Guid("EFF894B3-4DC9-4605-9937-F02F400B4A62");

        /// <summary>
        ///     if the module is enabled or not.
        ///     When a module is being enabled, all the mouse hook in the producers will be set.
        ///     When a module is being disabled, all the mouse hook in the producers will be released.
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, StartCapture,
                                          StopCapture);
        }

        Guid IModule.Identifier => Identifier;

        /// <summary>
        ///     Initialize the module unenabled with KeyboardInteractEventProducer.
        /// </summary>
        public void Initialize()
        {
            // retrieve all parameters from the MouseModuleConfiguration
            var samplingRate = MouseModuleConfiguration.SamplingRate;
            var threshold = MouseModuleConfiguration.Threshold;

            // initialize all producers
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