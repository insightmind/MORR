using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MORR.Core.Configuration;
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
            Initialize();
        }

        private void Initialize()
        {
            var commandLineArguments = Environment.GetCommandLineArgs();

            if (!commandLineArguments.Any())
            {
                throw new InvalidConfigurationException("");
            }

            if (!Environment.GetCommandLineArgs().Any())
            {
                ExitWithError(
                    "No configuration has been specified. Please specify a configuration file as the command line argument.");
            }

            // The first argument is the current assembly, the second argument is the first actual command line argument
            var configurationPathArgument = commandLineArguments.ElementAt(1);
            var configurationPath = new FilePath(Path.GetFullPath(configurationPathArgument));

            try
            {
                sessionManager = new SessionManager(configurationPath);
            }
            catch (Exception)
            {
                ExitWithError(
                    "The configuration could not be loaded correctly. \nPlease check the configuration file or contact an administrator.");
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
                ExitWithError("The recordings folder could not be found. \nPlease contact an administrator.");
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
            sessionManager.StartRecording();
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

            var recordingDirectory = sessionManager.CurrentRecordingDirectory?.ToString();

            if (recordingDirectory == null)
            {
                return;
            }

            Directory.Delete(recordingDirectory, true);
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