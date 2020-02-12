using System;
using System.Diagnostics;
using System.IO;
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
        private static SessionManager sessionManager;

        public ApplicationViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                sessionManager = new SessionManager(new FilePath(Environment.GetCommandLineArgs()[1]));
            }
            catch (InvalidConfigurationException)
            {
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
            var recordingsFolder = sessionManager.RecordingsFolder;

            if (recordingsFolder == null)
            {
                new ErrorDialog("The recordings folder could not be found. \nPlease contact an administrator.").ShowDialog();
                Exit();  
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
            if (new InformationDialog().ShowDialog() ?? false)
            {
                IsRecording = true;
                sessionManager.StartRecording();
            }
        }

        private void StopRecording()
        {
            IsRecording = false;
            sessionManager.StopRecording();

            

            if (!new SaveDialog().ShowDialog() ?? false)
            {
                Directory.Delete(sessionManager.CurrentRecordingDirectory?.ToString(), true);
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