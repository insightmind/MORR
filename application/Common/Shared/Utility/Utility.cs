using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Loader;
using System.Text;

namespace MORR.Shared.Utility
{
    public static class Utility
    {
        /// <summary>
        ///     Sets a boolean property and dispatches based on its value.
        /// </summary>
        /// <param name="variable">The property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="onTrue">
        ///     The action to execute if <paramref name="variable" /> was not set to <see langword="true" />
        ///     before calling this method, but <paramref name="value" /> is <see langword="true" />.
        /// </param>
        /// <param name="onFalse">
        ///     The action to execute if <paramref name="variable" /> was not set to <see langword="false" />
        ///     before calling this method, but <paramref name="value" /> is <see langword="false" />.
        /// </param>
        public static void SetAndDispatch(ref bool variable, bool value, Action onTrue, Action onFalse)
        {
            if (variable == value)
            {
                return;
            }

            variable = value;

            if (variable)
            {
                onTrue();
            }
            else
            {
                onFalse();
            }
        }

        /// <summary>
        ///     Attempts to load the type with the specified name from any currently loaded assembly.
        /// </summary>
        /// <param name="type">The name of the type to load.</param>
        /// <returns>The <see cref="Type" /> with the corresponding name or <see cref="null" /> on failure.</returns>
        public static Type? GetTypeFromAnyAssembly(string type)
        {
            return Type.GetType(type) ?? AssemblyLoadContext.All
                                                            .SelectMany(x => x.Assemblies)
                                                            .Select(x => x.GetType(type))
                                                            .FirstOrDefault(loadedType => loadedType != null);
        }

        /// <summary>
        ///     Get the title of a window from it's Hwnd
        /// </summary>
        /// <param name="hwnd"> Hwnd of the window</param>
        /// <returns>the title of the window in string</returns>
        public static string GetWindowTitleFromHwnd(IntPtr hwnd)
        {
            int length1 = NativeMethods.GetWindowTextLength(hwnd);
            StringBuilder sb1 = new StringBuilder(length1 + 1);
            NativeMethods.GetWindowText(hwnd, sb1, sb1.Capacity);
            return sb1.ToString();
        }

        /// <summary>
        ///     Get the The name of the process associated with a window
        ///     from the window's Hwnd
        /// </summary>
        /// <param name="hwnd">Hwnd of the window</param>
        /// <returns>the name of the process associated with the window</returns>
        public static string GetProcessNameFromHwnd(IntPtr hwnd)
        {
            NativeMethods.GetWindowThreadProcessId(hwnd, out var pid);
            Process process = Process.GetProcessById((int)pid);
            return process.ProcessName;
        }

        public static bool IsRectSizeEqual(Rectangle rectA, Rectangle rectB) 
        {
            return GetWindowWidth(rectA) == GetWindowWidth(rectB) && GetWindowHeight(rectA) == GetWindowHeight(rectB);
        }

        public static int GetWindowWidth(Rectangle rect) {
            return rect.Width - rect.X;
        }

        public static int GetWindowHeight(Rectangle rect)
        {
            return rect.Height - rect.Y;
        }
    }
}