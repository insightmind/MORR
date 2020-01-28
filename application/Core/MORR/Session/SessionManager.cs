using System.Collections.Generic;
using System.ComponentModel.Composition;
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

        public bool IsRecording { get; private set; }

        public void StartRecording()
        {
            if (IsRecording)
            {
                throw new AlreadyRecordingException();
            }

            IsRecording = true;

            encoder.Encode();
            moduleManager.NotifyModulesOnSessionStart();
        }

        public void StopRecording()
        {
            if (!IsRecording)
            {
                throw new NotRecordingException();
            }

            IsRecording = false;

            moduleManager.NotifyModulesOnSessionStop();
        }

        public void Process(IEnumerable<FilePath> files)
        {
            if (decoder == null)
            {
                throw new InvalidConfigurationException("No decoder specified for processing operation.");
            }

            moduleManager.NotifyModulesOnSessionStart();

            foreach (var file in files)
            {
                decoder.Decode(file);
                encoder.Encode();
            }

            moduleManager.NotifyModulesOnSessionStop();
        }
    }
}