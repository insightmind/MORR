using System;
using System.ComponentModel.Composition;
using MORR.Modules.WindowManagement.Producers;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Modules.WindowManagement
{
    /// <summary>
    ///     The <see cref="WindowManagementModule" /> is responsible for recording all window related user interactions
    /// </summary>
    public class WindowManagementModule : Module
    {
        private bool isActive;

        /// <summary>
        ///     A single-writer-multiple-reader queue for WindowFocusEvent
        /// </summary>
        [Import]
        public WindowFocusEventProducer WindowFocusEventProducer { get; private set; }

        /// <summary>
        ///     A single-writer-multiple-reader queue for WindowMovementEvent
        /// </summary>
        [Import]
        public WindowMovementEventProducer WindowMovementEventProducer { get; private set; }

        /// <summary>
        ///     A single-writer-multiple-reader queue for WindowResizingEvent
        /// </summary>
        [Import]
        public WindowResizingEventProducer WindowResizingEventProducer { get; private set; }

        /// <summary>
        ///     A single-writer-multiple-reader queue for WindowStateChangedEvent
        /// </summary>
        [Import]
        public WindowStateChangedEventProducer WindowStateChangedEventProducer { get; private set; }

        public new static Guid Identifier { get; } = new Guid("FAB5BC0D-8B33-4DFD-9FA3-C58E0F1435B5");

        /// <summary>
        ///     if the module is active or not.
        ///     When a module is being activated, all the producers will start to capture user interacts.
        ///     When a module is being deactivated, all the producers will stop capturing user interacts.
        /// </summary>
        public new bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, StartCapture, StopCapture);
        }

        private void StartCapture()
        {
            WindowFocusEventProducer?.Open();
            WindowMovementEventProducer?.Open();
            WindowResizingEventProducer?.Open();
            WindowStateChangedEventProducer?.Open();
        }

        private void StopCapture()
        {
            WindowFocusEventProducer?.Open();
            WindowMovementEventProducer?.Open();
            WindowResizingEventProducer?.Open();
            WindowStateChangedEventProducer?.Open();
        }
    }
}