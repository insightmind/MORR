using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using MORR.Core.Configuration;
using MORR.Core.Data.Transcoding;
using MORR.Core.Modules;
using MORR.Core.Session.Exceptions;
using MORR.Shared.Hook;
using MORR.Shared.Utility;

namespace MORR.Core.Session
{
    public class SessionManager : ISessionManager
    {
        private const string dateFormat = "yyyy-MM-ddTHH-mm-ss";
        private IEnumerable<IEncoder> encoders = new IEncoder[0];
        private IEnumerable<IDecoder> decoders = new IDecoder[0];
        private readonly IModuleManager moduleManager;
        private readonly IConfigurationManager configurationManager;
        private bool isRecording;

        public SessionManager() : this(new Bootstrapper(), new ConfigurationManager(), new ModuleManager()) { }

        public SessionManager(IBootstrapper bootstrapper,
                              IConfigurationManager configurationManager,
                              IModuleManager moduleManager)
        {
            this.moduleManager = moduleManager;
            this.configurationManager = configurationManager;

            bootstrapper.ComposeImports(this);
            bootstrapper.ComposeImports(configurationManager);
            bootstrapper.ComposeImports(moduleManager);
        }

        [ImportMany]
        private IEnumerable<IEncoder> Encoders { get; set; }

        [ImportMany]
        private IEnumerable<IDecoder> Decoders { get; set; }

        [Import]
        private SessionConfiguration Configuration { get; set; }

        public DirectoryPath? CurrentRecordingDirectory { get; private set; }

        // Nullable to prevent issues with calling this before the configuration has been parsed
        public DirectoryPath? RecordingsFolder => Configuration?.RecordingDirectory;

        private DirectoryPath CreateNewRecordingDirectory()
        {
            var timeNow = DateTime.Now;
            var sessionId = Guid.NewGuid();
            var directory = Path.Combine(Configuration.RecordingDirectory.ToString(), timeNow.ToString(dateFormat) + "-" + sessionId.ToString());
            Directory.CreateDirectory(directory);
            return new DirectoryPath(directory);
        }

        public void StartRecording(FilePath configurationPath)
        {
            LoadUsingConfig(configurationPath);

            if (isRecording)
            {
                throw new AlreadyRecordingException();
            }

            isRecording = true;

            CurrentRecordingDirectory = CreateNewRecordingDirectory();

            moduleManager.NotifyModulesOnSessionStart();

            foreach (var encoder in encoders)
            {
                encoder.Encode(CurrentRecordingDirectory);
            }
        }

        public void StopRecording()
        {
            if (!isRecording)
            {
                throw new NotRecordingException();
            }

            isRecording = false;

            moduleManager.NotifyModulesOnSessionStop();

            foreach (var encoder in encoders)
            {
                // IEncoder.EncodeFinished will not be reset before IEncoder.Encode gets called again
                // We may therefore wait on this event sequentially without risk of blocking indefinitely
                encoder.EncodeFinished.WaitOne();
            }

            GlobalHook.IsActive = false;
        }

        public void Process(FilePath configurationPath, IEnumerable<DirectoryPath> recordings)
        {
            LoadUsingConfig(configurationPath);

            if (!decoders.Any())
            {
                throw new InvalidConfigurationException("No decoder specified for processing operation.");
            }

            moduleManager.NotifyModulesOnSessionStart();

            foreach (var recording in recordings)
            {
                CurrentRecordingDirectory = CreateNewRecordingDirectory();

                foreach (var decoder in decoders)
                {
                    decoder.Decode(recording);
                }

                foreach (var encoder in encoders)
                {
                    encoder.Encode(CurrentRecordingDirectory);
                }

                foreach (var decoder in decoders)
                {
                    decoder.DecodeFinished.WaitOne();
                }

                foreach (var encoder in encoders)
                {
                    encoder.EncodeFinished.WaitOne();
                }
            }

            moduleManager.NotifyModulesOnSessionStop();
        }

        private void LoadUsingConfig(FilePath configurationPath)
        {
            configurationManager.LoadConfiguration(configurationPath);

            encoders = Encoders.Where(x => Configuration.Encoders.Contains(x.GetType()));
            decoders = Decoders.Where(x => Configuration.Decoders?.Contains(x.GetType()) ?? false);

            moduleManager.InitializeModules();
        }
    }
}