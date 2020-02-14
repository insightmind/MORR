using System;
using System.Runtime.InteropServices;

namespace MORR.Modules.Clipboard.Native
{
    internal class NativeClipboard : INativeClipboard
    {
        #region Enums

        public enum ClipboardFormat
        {
            CF_UNICODETEXT = 13
        }

        #endregion

        #region Clipboard text helper

        /// <summary>
        ///     Gets the text from the clipboard
        /// </summary>
        /// <param name="hwnd">Pointer to the window that currently has clipboard</param>
        /// <returns>String representing text from the clipboard</returns>
        public string GetClipboardText()
        {
            OpenClipboard(GetOpenClipboardWindow());

            //Gets pointer to clipboard data in the selected format
            var clipboardDataPointer = GetClipboardData((uint) ClipboardFormat.CF_UNICODETEXT);

            var clipboardLock = GlobalLock(clipboardDataPointer);

            var text = Marshal.PtrToStringAuto(clipboardLock);

            GlobalUnlock(clipboardLock);

            CloseClipboard();

            if (text == null)
            {
                throw new Exception("Failed to get clipboard text.");
            }

            return text;
        }

        short INativeClipboard.RegisterClass(ref INativeClipboard.WindowClass lpWndClass)
        {
            return RegisterClass(ref lpWndClass);
        }

        IntPtr INativeClipboard.CreateWindowEx(int dwExStyle,
                                               [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
                                               [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
                                               int dwStyle,
                                               int x,
                                               int y,
                                               int nWidth,
                                               int nHeight,
                                               IntPtr hWndParent,
                                               IntPtr hMenu,
                                               IntPtr hInstance,
                                               IntPtr lpParam)
        {
            return CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent,
                                  hMenu,
                                  hInstance, lpParam);
        }

        bool INativeClipboard.AddClipboardFormatListener(IntPtr hwnd)
        {
            return AddClipboardFormatListener(hwnd);
        }

        IntPtr INativeClipboard.DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        bool INativeClipboard.RemoveClipboardFormatListener(IntPtr hwnd)
        {
            return RemoveClipboardFormatListener(hwnd);
        }

        bool INativeClipboard.DestroyWindow(IntPtr hWnd)
        {
            return DestroyWindow(hWnd);
        }

        #endregion

        #region DllImports

        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr GetOpenClipboardWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);


        [DllImport("user32.dll", EntryPoint = "CreateWindowExW")]
        private static extern IntPtr CreateWindowEx(int dwExStyle,
                                                    [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
                                                    [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
                                                    int dwStyle,
                                                    int x,
                                                    int y,
                                                    int nWidth,
                                                    int nHeight,
                                                    IntPtr hWndParent,
                                                    IntPtr hMenu,
                                                    IntPtr hInstance,
                                                    IntPtr lpParam);

        [DllImport("user32.dll", EntryPoint = "RegisterClassW")]
        private static extern short RegisterClass(ref INativeClipboard.WindowClass lpWndClass);


        [DllImport("user32.dll")]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        #endregion
    }
}