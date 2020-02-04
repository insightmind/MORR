using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using MORR.Core.Configuration;
using MORR.Core.Data.Transcoding;
using MORR.Core.Modules;
using MORR.Core.Session.Exceptions;
using MORR.Shared.Utility;

namespace MORR.Core.Session
{
    public class SessionManager : ISessionManager
    {
        private readonly IDecoder? decoder;
        private readonly IEncoder encoder;
        private readonly IModuleManager moduleManager;
        private bool isRecording;

        public SessionManager(FilePath configurationPath) : this(configurationPath, new Bootstrapper(),
                                                                 new ConfigurationManager(), new ModuleManager()) { }

        public SessionManager(FilePath configurationPath,
                              IBootstrapper bootstrapper,
                              IConfigurationManager configurationManager,
                              IModuleManager moduleManager)
        {
            this.moduleManager = moduleManager;
            bootstrapper.ComposeImports(this);
            bootstrapper.ComposeImports(configurationManager);
            bootstrapper.ComposeImports(moduleManager);

            configurationManager.LoadConfiguration(configurationPath);

            encoder = Encoders.Single(x => x.GetType() == Configuration.Encoder);
            decoder = Decoders.SingleOrDefault(x => x.GetType() == Configuration.Decoder);

            moduleManager.InitializeModules();
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
            var sessionId = Guid.NewGuid();
            var directory = Path.Combine(Configuration.RecordingDirectory.ToString(), sessionId.ToString());
            Directory.CreateDirectory(directory);
            return new DirectoryPath(directory);
        }

        public void StartRecording()
        {
            if (isRecording)
            {
                throw new AlreadyRecordingException();
            }

            isRecording = true;

            CurrentRecordingDirectory = CreateNewRecordingDirectory();

            encoder.Encode(CurrentRecordingDirectory);
            moduleManager.NotifyModulesOnSessionStart();
        }

        public void StopRecording()
        {
            if (!isRecording)
            {
                throw new NotRecordingException();
            }

            isRecording = false;

            moduleManager.NotifyModulesOnSessionStop();
        }

        public void Process(IEnumerable<DirectoryPath> recordings)
        {
            if (decoder == null)
            {
                throw new InvalidConfigurationException("No decoder specified for processing operation.");
            }

            moduleManager.NotifyModulesOnSessionStart();

            foreach (var recording in recordings)
            {
                CurrentRecordingDirectory = CreateNewRecordingDirectory();
                decoder.Decode(recording);
                encoder.Encode(CurrentRecordingDirectory);
            }

            moduleManager.NotifyModulesOnSessionStop();
        }
    }
}