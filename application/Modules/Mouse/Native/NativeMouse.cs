using System.Runtime.InteropServices;

namespace MORR.Modules.Mouse.Native
{
    internal class NativeMouse : INativeMouse
    {
        bool INativeMouse.GetCursorPos(out INativeMouse.POINT lpPoint)
        {
            return GetCursorPos(out lpPoint);
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out INativeMouse.POINT lpPoint);
    }
}