using System;
using MORR.Shared.Modules;
using MORR.Modules.Mouse.Producers;
using System.Composition;

namespace MORR.Modules.Mouse
{
    /// <summary>
    /// The <see cref="MouseModule"/> is responsible for recording all mouse related user interactions
    /// </summary>
    public class MouseModule : ICollectingModule
    {
        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Identifier => throw new NotImplementedException();

        /// <summary>
        /// A single-writer-multiple-reader queue for MouseClickEvent
        /// </summary>
        [Import]
        public MouseClickEventProducer MouseClickEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for MouseScrollEvent
        /// </summary>
        [Import]
        public MouseScrollEventProducer MouseScrollEventProducer { get; private set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for MouseMoveEvent
        /// </summary>
        [Import]
        public MouseMoveEventProducer MouseMoveEventProducer { get; private set; }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
