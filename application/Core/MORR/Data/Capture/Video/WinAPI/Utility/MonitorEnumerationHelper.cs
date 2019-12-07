//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Windows.Foundation;

// TODO This is mostly copied from the reference project and needs to be cleaned up
namespace MORR.Core.Data.Capture.Video.WinAPI.Utility
{
    internal class MonitorInfo
    {
        public bool IsPrimary { get; set; }
        public Vector2 ScreenSize { get; set; }
        public Rect MonitorArea { get; set; }
        public Rect WorkArea { get; set; }
        public string DeviceName { get; set; }
        public IntPtr Hmon { get; set; }
    }

    /// <summary>
    ///     Provides utility methods for enumerating monitors
    /// </summary>
    internal static class MonitorEnumerationHelper
    {
        private const int CCHDEVICENAME = 32;

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc,
                                                       IntPtr lprcClip,
                                                       EnumMonitorsDelegate lpfnEnum,
                                                       IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        public static IEnumerable<MonitorInfo> GetMonitors()
        {
            var result = new List<MonitorInfo>();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                                (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
                                {
                                    var monitor = new MonitorInfoEx();
                                    monitor.Size = Marshal.SizeOf(monitor);

                                    if (GetMonitorInfo(hMonitor, ref monitor))
                                    {
                                        result.Add(new MonitorInfo
                                        {
                                            ScreenSize = new Vector2(monitor.Monitor.right - monitor.Monitor.left,
                                                                     monitor.Monitor.bottom - monitor.Monitor.top),
                                            MonitorArea = new Rect(monitor.Monitor.left, monitor.Monitor.top,
                                                                   monitor.Monitor.right - monitor.Monitor.left,
                                                                   monitor.Monitor.bottom -
                                                                   monitor.Monitor.top),
                                            WorkArea = new Rect(monitor.WorkArea.left, monitor.WorkArea.top,
                                                                monitor.WorkArea.right - monitor.WorkArea.left,
                                                                monitor.WorkArea.bottom - monitor.WorkArea.top),
                                            IsPrimary = monitor.Flags > 0,
                                            Hmon = hMonitor,
                                            DeviceName = monitor.DeviceName
                                        });
                                    }

                                    return true;
                                },
                                IntPtr.Zero);
            return result;
        }

        private delegate bool EnumMonitorsDelegate(IntPtr hMonitor,
                                                   IntPtr hdcMonitor,
                                                   ref RECT lprcMonitor,
                                                   IntPtr dwData);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MonitorInfoEx
        {
            public int Size;
            public RECT Monitor;
            public RECT WorkArea;
            public uint Flags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string DeviceName;
        }
    }
}