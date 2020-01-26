using System;
using System.Windows;
using System.Windows.Input;
using MORR.Core.Configuration;
using MORR.Core.Recording;
using Morr.Core.UI.Dialogs;
using MORR.Core.UI.ViewModels.Utility;
using MORR.Shared.Utility;

namespace MORR.Core.UI.ViewModels
{
    public class ApplicationViewModel : DependencyObject
    {
        private RecordingManager recordingManager;
        public ApplicationViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            var configurationSuccessful = true;

            try
            {
                recordingManager = new RecordingManager(new FilePath(Environment.GetCommandLineArgs()[0]));
            }
            catch (InvalidConfigurationException)
            {
                configurationSuccessful = false;
            }

            if (!configurationSuccessful)
            {
                // Invalid configuration
                new ErrorDialog("The configuration could not be loaded. \nPlease check the configuration file or contact an administrator.").ShowDialog();
                Exit();
            }
        }

        private static void Exit()
        {
            Application.Current?.Shutdown();
        }

        private static void OnOpenRecordingsDirectory(object _)
        {
            // TODO Get correct file path
            // Process.Start("explorer.exe", "file-path-here");
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
            if (new InformationDialog().ShowDialog() ?? false)
            {
                IsRecording = true;
                recordingManager.StopRecording();
                
            }
        }

        private void StopRecording()
        {
            recordingManager.StopRecording();

            IsRecording = false;
            if (new SaveDialog().ShowDialog() ?? false)
            {
                // TODO Save recording
            }
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