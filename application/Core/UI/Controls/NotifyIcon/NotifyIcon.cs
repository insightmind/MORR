using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using MORR.Core.UI.Utility;
using Point = System.Windows.Point;

namespace MORR.Core.UI.Controls.NotifyIcon
{
    /// <summary>
    ///     Manages a tray icon
    /// </summary>
    public class NotifyIcon : Control, IDisposable
    {
        private static readonly WindowMessageSink MessageSink = new WindowMessageSink();
        private NativeMethods.NotifyIconData iconData;
        private bool isNotifyIconCreated;

        public NotifyIcon()
        {
            CreateDefaultIconData();

            if (MessageSink != null)
            {
                MessageSink.TaskbarCreated += OnTaskbarCreated;
                MessageSink.WindowMessage += OnWindowMessage;
            }

            CreateNotifyIcon();

            if (Application.Current != null)
            {
                Application.Current.Exit += OnExit;
            }
        }

        private void CreateNotifyIcon()
        {
            lock (this)
            {
                if (isNotifyIconCreated)
                {
                    return;
                }

                WriteIconData(NativeMethods.NotifyIconMessage.NIM_ADD,
                              NativeMethods.NotifyIconFlags.NIF_MESSAGE | NativeMethods.NotifyIconFlags.NIF_ICON |
                              NativeMethods.NotifyIconFlags.NIF_TIP | NativeMethods.NotifyIconFlags.NIF_GUID);
                isNotifyIconCreated = true;
            }
        }

        private void RemoveNotifyIcon()
        {
            lock (this)
            {
                if (!isNotifyIconCreated)
                {
                    return;
                }

                WriteIconData(NativeMethods.NotifyIconMessage.NIM_DELETE,
                              NativeMethods.NotifyIconFlags.NIF_MESSAGE | NativeMethods.NotifyIconFlags.NIF_GUID);
                isNotifyIconCreated = false;
            }
        }

        #region Event handlers

        private static void OnTooltipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NotifyIcon notifyIcon && e.NewValue is string tooltip)
            {
                notifyIcon.iconData.szTip = tooltip;
                notifyIcon.WriteIconData(NativeMethods.NotifyIconMessage.NIM_MODIFY,
                                         NativeMethods.NotifyIconFlags.NIF_TIP |
                                         NativeMethods.NotifyIconFlags.NIF_GUID);
            }
        }

        private static void IconUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NotifyIcon notifyIcon && e.NewValue is Uri uri)
            {
                notifyIcon.SetIcon(uri);
            }
        }

        private void OnWindowMessage(uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId != iconData.uCallbackMessage)
            {
                // Ignore messages not associated with this icon
                return;
            }

            switch (lParam.ToInt32())
            {
                case (int) NativeMethods.WindowMessages.WM_LBUTTONDOWN:
                case (int) NativeMethods.WindowMessages.WM_LBUTTONDBLCLK:
                {
                    // Invoke the command object on left click
                    if (Command != null && Command.CanExecute(CommandParameter))
                    {
                        Command.Execute(CommandParameter);
                    }
                }
                    break;

                case (int) NativeMethods.WindowMessages.WM_RBUTTONDOWN:
                {
                    // Open the context menu on right click
                    var cursorPosition = new NativeMethods.Win32Point();
                    NativeMethods.GetCursorPos(ref cursorPosition);

                    using (var hWndSource = new HwndSource(new HwndSourceParameters()))
                    {
                        // Adjust for non-standard DPI settings
                        var scaleFactor = new Point(hWndSource?.CompositionTarget?.TransformToDevice.M11 ?? 1,
                                                    hWndSource?.CompositionTarget?.TransformToDevice.M22 ?? 1);
                        cursorPosition = new NativeMethods.Win32Point
                        {
                            X = (int) (cursorPosition.X / scaleFactor.X), Y = (int) (cursorPosition.Y / scaleFactor.Y)
                        };
                    }

                    if (ContextMenu != null && MessageSink != null)
                    {
                        ContextMenu.DataContext = DataContext;
                        ContextMenu.Placement = PlacementMode.AbsolutePoint;
                        ContextMenu.HorizontalOffset = cursorPosition.X;
                        ContextMenu.VerticalOffset = cursorPosition.Y;
                        ContextMenu.IsOpen = true;

                        var handle = (PresentationSource.FromVisual(ContextMenu) as HwndSource)?.Handle ??
                                     MessageSink.WindowHandle;

                        NativeMethods.SetForegroundWindow(handle);
                    }
                }
                    break;
            }
        }

        private void OnTaskbarCreated()
        {
            // Recreate the icon if the taskbar gets recreated
            // This might happen if explorer.exe is manually restarted
            isNotifyIconCreated = false;
            CreateNotifyIcon();
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(NotifyIcon),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(NotifyIcon),
                                        new PropertyMetadata(null));

        public new static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.Register(nameof(ContextMenu), typeof(ContextMenu), typeof(NotifyIcon),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty TooltipProperty =
            DependencyProperty.Register(nameof(Tooltip), typeof(string), typeof(NotifyIcon),
                                        new PropertyMetadata(string.Empty, OnTooltipChanged));

        public static readonly DependencyProperty IconUriProperty =
            DependencyProperty.Register(nameof(IconUri), typeof(Uri), typeof(NotifyIcon),
                                        new PropertyMetadata(null, IconUriChanged));

        /// <summary>
        ///     The tooltip shown during hovering
        /// </summary>
        public string Tooltip
        {
            get => (string) GetValue(TooltipProperty);
            set => SetValue(TooltipProperty, value);
        }

        /// <summary>
        ///     The icon shown in the tray
        /// </summary>
        public Uri IconUri
        {
            get => (Uri) GetValue(IconUriProperty);
            set => SetValue(IconUriProperty, value);
        }

        /// <summary>
        ///     The command to execute on left click
        /// </summary>
        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        ///     The parameter of the command to execute on left click
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        ///     The context menu to show on right click
        /// </summary>
        public new ContextMenu ContextMenu
        {
            get => (ContextMenu) GetValue(ContextMenuProperty);
            set => SetValue(ContextMenuProperty, value);
        }

        #endregion

        #region Utility

        private void SetIcon(Uri uri)
        {
            var resourceInfo = Application.GetResourceStream(uri);

            if (resourceInfo == null)
            {
                return;
            }

            iconData.hIcon = new Icon(resourceInfo.Stream).Handle;
            WriteIconData(NativeMethods.NotifyIconMessage.NIM_MODIFY,
                          NativeMethods.NotifyIconFlags.NIF_ICON | NativeMethods.NotifyIconFlags.NIF_GUID);
        }

        private void CreateDefaultIconData()
        {
            iconData = new NativeMethods.NotifyIconData();

            iconData.cbSize = (uint) Marshal.SizeOf(iconData);
            iconData.hWnd = MessageSink?.WindowHandle ?? IntPtr.Zero;
            const int callbackMessageMin = 0x2;
            const int callbackMessageMax = 0x1FFFF;
            // The callback ID is used to identify the icon instance that sent the message and therefore needs to be unique
            iconData.uCallbackMessage = (uint) new Random().Next(callbackMessageMin, callbackMessageMax);
            iconData.uTimeoutOrVersion = (uint) NativeMethods.NotifyIconVersion.NOTIFY_ICON_VERSION_4;
            iconData.hIcon = IntPtr.Zero;
            iconData.dwState = NativeMethods.NotifyIconState.NIS_HIDDEN;
            iconData.dwStateMask = NativeMethods.NotifyIconState.NIS_HIDDEN;
            iconData.guidItem = Guid.NewGuid();
            iconData.szTip = iconData.szInfo = iconData.szInfoTitle = string.Empty;
        }

        private bool WriteIconData(NativeMethods.NotifyIconMessage message, NativeMethods.NotifyIconFlags flags)
        {
            iconData.uFlags = flags;
            return NativeMethods.Shell_NotifyIcon(message, ref iconData);
        }

        #endregion

        #region Dispose

        private bool isDisposed;

        private void OnExit(object sender, EventArgs e)
        {
            // Remove the icon from the tray when the application is closed
            Dispose();
        }

        ~NotifyIcon()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Frees all unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed || !disposing)
            {
                return;
            }

            lock (this)
            {
                isDisposed = true;

                if (Application.Current != null)
                {
                    Application.Current.Exit -= OnExit;
                }

                MessageSink?.Dispose();
                RemoveNotifyIcon();
            }
        }

        #endregion
    }
}