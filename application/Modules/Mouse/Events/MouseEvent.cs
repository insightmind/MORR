<<<<<<< HEAD
﻿using MORR.Shared;
using MORR.Shared.Events;
=======
﻿using MORR.Modules.Mouse.Producers;
using MORR.Shared;
using MORR.Shared.Events;
using System.Windows;
>>>>>>> Create initial structure

namespace MORR.Modules.Mouse.Events
{
    /// <summary>
    ///     A generic mouse event which all specific MouseEvents inherit from.
    /// </summary>
    public abstract class MouseEvent: Event
    {
        /// <summary>
        ///     The current position of the mouse
        /// </summary>
        private Point MousePosition;
    }
}
