using System;
using MORR.Shared.Modules;
using MORR.Modules.Mouse.Producers;
using System.ComponentModel.Composition;

namespace MORR.Modules.Mouse
{
    /// <summary>
    /// The <see cref="MouseModule"/> is responsible for recording all mouse related user interactions
    /// </summary>
    public class MouseModule : ICollectingModule
    {
        private bool isActive;

        /// <summary>
        /// A single-writer-multiple-reader queue for MouseClickEvent
        /// </summary>
        [Import]
        private MouseClickEventProducer MouseClickEventProducer { get; set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for MouseScrollEvent
        /// </summary>
        [Import]
        private MouseScrollEventProducer MouseScrollEventProducer { get; set; }

        /// <summary>
        /// A single-writer-multiple-reader queue for MouseMoveEvent
        /// </summary>
        [Import]
        private MouseMoveEventProducer MouseMoveEventProducer { get; set; }

        /// <summary>
        ///     if the module is enabled or not.
        ///     When a module is being enabled, the keyboard hook will be set.
        ///     When a module is being disabled, the keyboard hook will be released.
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                if (value)
                {
                    //TODO Set the hooks in all producers
                }
                else
                {
                    //TODO Release the hooks in all producers
                }
            }
        }

        public Guid Identifier => new Guid("EFF894B3-4DC9-4605-9937-F02F400B4A62");


        /// <summary>
        ///     Initialize the module unenabled with KeyboardInteractEventProducer.
        /// </summary>
        public void Initialize()
        {
            //TODO initialize all producers with parameters.
            isActive = false;
        }
    }
}
}
