using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using MORR.Core.Configuration;
using MORR.Core.Data.Transcoding;
using MORR.Core.Modules;
using MORR.Core.Session.Exceptions;
using MORR.Core.Session.Crypto;
using MORR.Shared.Hook;
using MORR.Shared.Utility;

namespace MORR.Core.Session
{
    public class SessionManager : ISessionManager
    {
        private const string dateFormat = "yyyy-MM-ddTHH-mm-ss";
        private const string fileDivider = "--";
        private readonly IEnumerable<IEncoder> encoders;
        private readonly IEnumerable<IDecoder> decoders;
        private readonly IModuleManager moduleManager;
        private readonly IConfigurationManager configurationManager;
        private readonly IFileSystem fileSystem;

        public bool IsRecording { get; private set; }

        public SessionManager(FilePath configurationPath) : this(configurationPath, new Bootstrapper(), new ConfigurationManager(), new ModuleManager(), new FileSystem()) { }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public SessionManager(FilePath configurationPath,
                              IBootstrapper bootstrapper,
                              IConfigurationManager configurationManager,
                              IModuleManager moduleManager,
                              IFileSystem fileSystem)
        {
            this.moduleManager = moduleManager;
            this.configurationManager = configurationManager;
            this.fileSystem = fileSystem;

            GlobalHook.Initialize();

            bootstrapper.ComposeImports(this);
            bootstrapper.ComposeImports(configurationManager);
            bootstrapper.ComposeImports(moduleManager);

            configurationManager.LoadConfiguration(configurationPath);

            encoders = Encoders.Where(x => Configuration.Encoders.Contains(x.GetType()));
            decoders = Decoders.Where(x => Configuration.Decoders?.Contains(x.GetType()) ?? false);
        }

        [ImportMany]
        private IEnumerable<IEncoder> Encoders { get; set; }

        [ImportMany]
        private IEnumerable<IDecoder> Decoders { get; set; }

        [Import]
        private SessionConfiguration Configuration { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public DirectoryPath? CurrentRecordingDirectory { get; private set; }

        // Nullable to prevent issues with calling this before the configuration has been parsed
        public DirectoryPath? RecordingsFolder => Configuration?.RecordingDirectory;

        private DirectoryPath CreateNewRecordingDirectory()
        {
            var timeNow = DateTime.Now.ToString(dateFormat);
            var dir = Configuration.RecordingDirectory.ToString();
            var sessionId = Guid.NewGuid().ToString();
            var username = CryptoHelper.GenerateHash(Environment.UserName);

            var directory = fileSystem.Path.Combine(dir, timeNow + fileDivider + username + fileDivider + sessionId);
            fileSystem.Directory.CreateDirectory(directory);
            
            return new DirectoryPath(directory);
        }

        public void StartRecording()
        {
            if (IsRecording)
            {
                throw new AlreadyRecordingException();
            }

            moduleManager.InitializeModules();

            IsRecording = true;

            CurrentRecordingDirectory = CreateNewRecordingDirectory();

            moduleManager.NotifyModulesOnSessionStart();

            foreach (var encoder in encoders)
            {
                encoder.Encode(CurrentRecordingDirectory);
            }
        }

        public void StopRecording()
        {
            if (!IsRecording)
            {
                throw new NotRecordingException();
            }

            IsRecording = false;

            moduleManager.NotifyModulesOnSessionStop();

            foreach (var encoder in encoders)
            {
                // IEncoder.EncodeFinished will not be reset before IEncoder.Encode gets called again
                // We may therefore wait on this event sequentially without risk of blocking indefinitely
                encoder.EncodeFinished.WaitOne();
            }

            GlobalHook.IsActive = false;
        }

        public void Process(IEnumerable<DirectoryPath> recordings)
        {
            if (IsRecording)
            {
                throw new AlreadyRecordingException();
            }

            if (!decoders.Any())
            {
                throw new InvalidConfigurationException("No decoder specified for processing operation.");
            }

            moduleManager.InitializeModules();
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
    }
}