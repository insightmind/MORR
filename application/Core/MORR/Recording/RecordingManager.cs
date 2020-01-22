using System.Collections.Generic;
using System.Composition;
using System.Linq;
using MORR.Core.Configuration;
using MORR.Core.Data.Transcoding;
using MORR.Core.Modules;
using MORR.Core.Recording.Exceptions;
using MORR.Shared.Utility;

namespace MORR.Core.Recording
{
    public class RecordingManager : IRecordingManager
    {
        private readonly IModuleManager moduleManager;
        private readonly IEncoder encoder;
        private readonly IDecoder decoder;

        [ImportMany]
        private IEnumerable<IEncoder> Encoders { get; set; }

        [ImportMany]
        private IEnumerable<IDecoder> Decoders { get; set; }

        [Import]
        private RecordingConfiguration Configuration { get; set; }

        public RecordingManager(FilePath configurationPath) : this(configurationPath, new Bootstrapper(),
                                                                   new ConfigurationManager(), new ModuleManager()) { }

        public RecordingManager(FilePath configurationPath,
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
            decoder = Decoders.Single(x => x.GetType() == Configuration.Decoder);

            moduleManager.InitializeModules();
        }

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