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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Capture;

namespace MORR.Core.Data.Capture.Video.Desktop.Utility
{
    /// <summary>
    ///     Provides utility methods for interacting with GraphicsCapture objects.
    /// </summary>
    internal static class GraphicsCaptureHelper
    {
        private static readonly Guid GraphicsCaptureItemGuid = new Guid("79C3F95B-31F7-4EC2-A464-632EF5D30760");

        /// <summary>
        ///     Indicates whether the device supports creating a <see cref="GraphicsCaptureItem" /> by API instead of by using the
        ///     <see cref="GraphicsCapturePicker" />. <see langword="true" /> if creation by API is supported,
        ///     <see langword="false" />
        ///     otherwise.
        /// </summary>
        internal static bool CanCreateItemWithoutPicker
        {
            get
            {
                var contractName = typeof(UniversalApiContract).FullName;
                return contractName != null && ApiInformation.IsApiContractPresent(contractName, 8);
            }
        }

        /// <summary>
        ///     Initializes a <see cref="GraphicsCapturePicker" /> with a window.
        /// </summary>
        /// <param name="picker">The <see cref="GraphicsCapturePicker" /> to initialize.</param>
        /// <param name="hWnd">The handle of the window to initialize the picker with.</param>
        internal static void SetWindow(this GraphicsCapturePicker picker, IntPtr hWnd)
        {
            // Cast via object as direct cast is not supported for imported interface
            var interop = picker as object as IInitializeWithWindow;
            interop?.Initialize(hWnd);
        }

        /// <summary>
        ///     Creates a <see cref="GraphicsCaptureItem" /> for a provided monitor. This requires
        ///     <see cref="CanCreateItemWithoutPicker" /> to be <see langword="true" />.
        /// </summary>
        /// <param name="hMon">The handle of the monitor to create the item for.</param>
        /// <returns>The created <see cref="GraphicsCaptureItem" />.</returns>
        internal static GraphicsCaptureItem? CreateItemForMonitor(IntPtr hMon)
        {
            var factory = WindowsRuntimeMarshal.GetActivationFactory(typeof(GraphicsCaptureItem));
            var interop = factory as IGraphicsCaptureItemInterop;

            var itemPointer = interop?.CreateForMonitor(hMon, GraphicsCaptureItemGuid);

            if (itemPointer != null)
            {
                var item = Marshal.GetObjectForIUnknown(itemPointer.Value) as GraphicsCaptureItem;
                Marshal.Release(itemPointer.Value);
                return item;
            }

            return null;
        }

        [ComImport]
        [System.Runtime.InteropServices.Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(true)]
        private interface IInitializeWithWindow
        {
            void Initialize(IntPtr hWnd);
        }

        [ComImport]
        [System.Runtime.InteropServices.Guid("3628E81B-3CAC-4C60-B7F4-23CE0E0C3356")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(true)]
        private interface IGraphicsCaptureItemInterop
        {
            IntPtr CreateForWindow([In] IntPtr window,
                                   [In] ref Guid iid);

            IntPtr CreateForMonitor([In] IntPtr monitor,
                                    [In] ref Guid iid);
        }
    }
}