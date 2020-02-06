using MORR.Shared.Events;

namespace MORR.Modules.WindowManagement.Events
{
    /// <summary>
    ///     A window management event which all specific WindowEvents inherit from.
    /// </summary>
    public abstract class WindowEvent : Event
    {
        /// <summary>
        ///     The title of the interacted window
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The name of the process associated with the window
        /// </summary>
        public string ProcessName { get; set; }
    }
}