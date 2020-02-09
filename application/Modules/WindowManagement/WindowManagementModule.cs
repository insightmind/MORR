using System;
using System.ComponentModel.Composition;
using System.Data.Common;
using MORR.Modules.WindowManagement.Producers;
using MORR.Shared.Modules;
using MORR.Shared.Utility;

namespace MORR.Modules.WindowManagement
{
    /// <summary>
    ///     The <see cref="WindowManagementModule" /> is responsible for recording all window related user interactions
    /// </summary>
    public class WindowManagementModule : IModule
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
        
        public static Guid Identifier { get; } = new Guid("FAB5BC0D-8B33-4DFD-9FA3-C58E0F1435B5");
        Guid IModule.Identifier => Identifier;

        /// <summary>
        ///     if the module is active or not.
        ///     When a module is being activated, all the producers will start to capture user interacts.
        ///     When a module is being deactivated, all the producers will stop capturing user interacts.
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set => Utility.SetAndDispatch(ref isActive, value, StartCapture, StopCapture);
        }

        public void Initialize(bool isEnabled)
        {
            if (isEnabled)
            {
                WindowFocusEventProducer?.Open();
                WindowMovementEventProducer?.Open();
                WindowResizingEventProducer?.Open();
                WindowStateChangedEventProducer?.Open();
            }
            else
            {
                WindowFocusEventProducer?.Close();
                WindowMovementEventProducer?.Close();
                WindowResizingEventProducer?.Close();
                WindowStateChangedEventProducer?.Close();
            }
        }

        private void StartCapture()
        {
            WindowFocusEventProducer?.StartCapture();
            WindowMovementEventProducer?.StartCapture();
            WindowResizingEventProducer?.StartCapture();
            WindowStateChangedEventProducer?.StartCapture();
        }

        private void StopCapture()
        {
            WindowFocusEventProducer?.StopCapture();
            WindowMovementEventProducer?.StopCapture();
            WindowResizingEventProducer?.StopCapture();
            WindowStateChangedEventProducer?.StopCapture();
        }
    }
}