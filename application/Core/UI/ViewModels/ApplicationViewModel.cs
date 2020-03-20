using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private SessionManager sessionManager = null!;

        public ApplicationViewModel()
        {
            DesktopCaptureNativeMethods.WindowRequestedHandler = CreateWindowForPicker;
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                var configurationPath = GetConfigurationPath();
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

        private static FilePath GetConfigurationPath()
        {
            if (TryGetConfigurationPathFromCommandLine(out var path))
            {
                return path;
            }

            var defaultConfigurationBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (defaultConfigurationBasePath == null)
            {
                ExitWithError(Properties.Resources.Error_No_Configuration_Specified);
            }

            var defaultConfigurationPath = Path.Combine(defaultConfigurationBasePath, "config.morr");

            return new FilePath(Path.GetFullPath(defaultConfigurationPath));
        }

        private static bool TryGetConfigurationPathFromCommandLine([NotNullWhen(true)] out FilePath? path)
        {
            var commandLineArguments = Environment.GetCommandLineArgs();

            if (commandLineArguments.Length < 2 || (App.HasLanguageCommandLineArgument && commandLineArguments.Length < 3))
            {
                // If no command line argument has been specified or only one has been specified
                // and that argument is used to set the language, then the configuration path must be set to the default value
                path = null;
                return false;
            }

            // The first argument is the current assembly, the second argument is the first actual command line argument
            var configurationPathArgument = commandLineArguments.ElementAt(1);

            if (App.IsLanguageCommandLineArgument(configurationPathArgument))
            {
                // If the first specified argument is used to set the language, then the path must be the second argument
                configurationPathArgument = commandLineArguments.ElementAt(2);
            }

            path = new FilePath(Path.GetFullPath(configurationPathArgument));
            return true;
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

        private ICommand exitCommand = null!;

        private ICommand openRecordingsDirectoryCommand = null!;

        private ICommand toggleRecordingCommand = null!;

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