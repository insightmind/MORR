using System;
using MORR.Shared.Modules;
using MORR.Modules.WindowManagement.Producers;
using System.ComponentModel.Composition;

namespace MORR.Modules.WindowManagement
{
    /// <summary>
    /// The <see cref="WindowManagementModule"/> is responsible for recording all window related user interactions
    /// </summary>
    public class WindowManagementModule : ICollectingModule
    {
        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        /// <summary>
        /// A single-writer-multiple-reader queue for WindowFocusEvent
        /// </summary>
        [Import]
        public WindowFocusEventProducer WindowFocusEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for WindowMovementEvent
        /// </summary>
        [Import]
        public WindowMovementEventProducer WindowMovementEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for WindowResizingEvent
        /// </summary>
        [Import]
        public WindowResizingEventProducer WindowResizingEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for WindowStateChangedEvent
        /// </summary>
        [Import]
        public WindowStateChangedEventProducer WindowStateChangedEventProducer { get; private set; }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
