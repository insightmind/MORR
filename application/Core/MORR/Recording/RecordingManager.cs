using System.Collections.Generic;
using MORR.Core.Configuration;
using MORR.Core.Modules;
using MORR.Core.Recording.Exceptions;
using MORR.Shared.Utility;

namespace MORR.Core.Recording
{
    public class RecordingManager : IRecordingManager
    {
        private readonly IModuleManager moduleManager;

        public RecordingManager(FilePath configurationPath) : this(configurationPath, new Bootstrapper(),
                                                                   new ConfigurationManager(), new ModuleManager()) { }


        public RecordingManager(FilePath configurationPath,
                                IBootstrapper bootstrapper,
                                IConfigurationManager configurationManager,
                                IModuleManager moduleManager)
        {
            this.moduleManager = moduleManager;
            bootstrapper.ComposeImports(moduleManager);
            bootstrapper.ComposeImports(configurationManager);

            configurationManager.LoadConfiguration(configurationPath);
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
            // TODO Implement
        }
    }
}