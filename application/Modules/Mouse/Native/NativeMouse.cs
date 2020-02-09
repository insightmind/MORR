using System.Drawing;
using System.Runtime.InteropServices;

namespace MORR.Modules.Mouse.Native
{
    internal sealed class NativeMouse
    {
        #region DllImports

        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        #endregion

        /// <summary>
        ///     The POINT is of two int(32 bits) for the usage in MSLLHOOKSTRUCT.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.X, p.Y);
            }

            public static implicit operator POINT(Point p)
            {
                return new POINT(p.X, p.Y);
            }

            public static implicit operator System.Windows.Point(POINT p)
            {
                return new System.Windows.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Windows.Point p)
            {
                return new POINT((int)p.X, (int)p.Y);
            }
        }
    }
}
