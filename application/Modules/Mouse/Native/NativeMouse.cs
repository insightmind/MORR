using System.Runtime.InteropServices;

namespace MORR.Modules.Mouse.Native
{
    internal class NativeMouse : INativeMouse
    {
        INativeMouse.POINT INativeMouse.GetCursorPos()
        {
            INativeMouse.POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out INativeMouse.POINT lpPoint);
    }
}