using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using MORR.Core.Data.Capture.Video.Desktop.Utility;
using MORR.Core.Session;
using MORR.Core.UI.Dialogs;
using MORR.Core.UI.Utility;
using MORR.Shared.Utility;

namespace MORR.Core.UI.ViewModels
{
    public class ApplicationViewModel : DependencyObject
    {
        private SessionManager sessionManager;

        public ApplicationViewModel()
        {
            DesktopCaptureNativeMethods.WindowRequestedHandler = CreateWindowForPicker;
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                var configurationPath = GetConfigurationPathFromCommandLine();
                sessionManager = new SessionManager(configurationPath);
            }
            catch (Exception)
            {
                ExitWithError(Properties.Resources.Error_Configuration_Invalid);
            }
        }

        private static void Exit()
        {
            Application.Current?.Shutdown();
        }

        private void OnOpenRecordingsDirectory(object _)
        {
            var recordingsFolder = sessionManager.RecordingsFolder;

            if (recordingsFolder == null)
            {
                ExitWithError(Properties.Resources.Error_No_Recordings_Directory);
            }

            Process.Start("explorer.exe", recordingsFolder.ToString());
        }

        private void OnExitApplication(object _)
        {
            if (IsRecording)
            {
                StopRecording();
            }

            Exit();
        }

        private void OnToggleRecording(object _)
        {
            if (!IsRecording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

        private void StartRecording()
        {
            if (!ShowDialogWithResult<InformationDialog>())
            {
                return;
            }

            IsRecording = true;

            try
            {
                sessionManager.StartRecording();
            }
            catch (Exception)
            {
                // We cannot recover from an error here, as the modules might be in an invalid state
                // We have to exit immediately as otherwise threads might throw new exceptions
                Environment.Exit(-1); // Exit with non-zero value to indicate error
            }
        }

        private void StopRecording()
        {
            IsRecording = false;
            sessionManager.StopRecording();

            if (ShowDialogWithResult<SaveDialog>())
            {
                // Recordings are automatically saved - no further action required
                return;
            }

            if (!ShowDialogWithResult<ConfirmationDialog>())
            {
                // If recording shouldn't actually be deleted, no further action is required
                return;
            }

            var recordingDirectory = sessionManager.CurrentRecordingDirectory?.ToString();

            if (recordingDirectory == null)
            {
                return;
            }

            Directory.Delete(recordingDirectory, true);
        }

        #region Utility

        private static FilePath GetConfigurationPathFromCommandLine()
        {
            var commandLineArguments = Environment.GetCommandLineArgs();

            if (commandLineArguments.Length < 2)
            {
                ExitWithError(Properties.Resources.Error_No_Configuration_Specified);
            }

            // The first argument is the current assembly, the second argument is the first actual command line argument
            var configurationPathArgument = commandLineArguments.ElementAt(1);
            return new FilePath(Path.GetFullPath(configurationPathArgument));
        }

        [DoesNotReturn]
        private static void ExitWithError(string errorMessage)
        {
            new ErrorDialog(errorMessage).ShowDialog();
            Exit();
        }

        private static bool ShowDialogWithResult<T>() where T : Window, new()
        {
            var dialogResult = new T().ShowDialog();
            return dialogResult ?? false;
        }

        private static DesktopCaptureNativeMethods.WindowHandleWrapper CreateWindowForPicker()
        {
            var window = new Window
            {
                WindowStyle = WindowStyle.None,
                Top = 0.0d,
                Left = 0.0d,
                Width = 0.0d,
                Height = 0.0d,
                ShowInTaskbar = false
            };

            window.Show();

            void CleanupCallback()
            {
                window.Close();
            }

            var windowInteropHelper = new WindowInteropHelper(window);
            return new DesktopCaptureNativeMethods.WindowHandleWrapper(windowInteropHelper.EnsureHandle(),
                                                                       CleanupCallback);
        }

        #endregion

        #region Commands

        private ICommand exitCommand;

        private ICommand openRecordingsDirectoryCommand;

        private ICommand toggleRecordingCommand;

        public ICommand OpenRecordingsDirectoryCommand =>
            openRecordingsDirectoryCommand ??= new RelayCommand(OnOpenRecordingsDirectory);

        public ICommand ExitCommand => exitCommand ??= new RelayCommand(OnExitApplication);

        public ICommand ToggleRecordingCommand => toggleRecordingCommand ??= new RelayCommand(OnToggleRecording);

        #endregion

        #region Dependency properties

        public bool IsRecording
        {
            get => (bool) GetValue(IsRecordingProperty);
            set => SetValue(IsRecordingProperty, value);
        }

        public static readonly DependencyProperty IsRecordingProperty =
            DependencyProperty.Register(nameof(IsRecording), typeof(bool), typeof(ApplicationViewModel),
                                        new PropertyMetadata(false));

        #endregion
    }
}