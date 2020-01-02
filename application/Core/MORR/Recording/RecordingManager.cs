using System;
using MORR.Core.Data.Capture.Video;
using MORR.Core.Data.Transcoding;
using MORR.Core.Modules;
using MORR.Core.Recording.Exceptions;

namespace MORR.Core.Recording
{
    public class RecordingManager : IRecordingManager
    {
        private IDecoder decoder;
        private IEncoder encoder;
        private IMetadataCapture metadataCapture;
        private IMetadataDeserializer metadataDeserializer;
        private IVideoCapture videoCapture;

        public RecordingManager()
        {
            var bootstrapper = new Bootstrapper();
            // TODO Should the concrete types be loaded from configuration?
            var moduleManager = new ModuleManager();
            var configurationManager = new ConfigurationManager();

            bootstrapper.ComposeImports(moduleManager);
            bootstrapper.ComposeImports(configurationManager);

            configurationManager.LoadConfiguration("some/path"); // TODO Decide on path convention
            moduleManager.InitializeModules();

            encoder.MetadataSampleRequested += metadataCapture.NextSample;
            encoder.VideoSampleRequested += videoCapture.NextSample;
        }

        public bool IsRecording { get; private set; }

        public void StartRecording()
        {
            if (IsRecording)
            {
                throw new AlreadyRecordingException();
            }

            IsRecording = true;

            encoder.Encode(); // TODO This probably also needs a path
        }

        public void StopRecording()
        {
            if (!IsRecording)
            {
                throw new NotRecordingException();
            }

            IsRecording = false;
            // TODO This requires the encoder to be an IModule - having both an Encode method and an IsEnabled field is somewhat convoluted
            encoder.IsEnabled = false;
        }

        public void StartDecoding()
        {
            throw new NotImplementedException();
        }

        public void StopDecoding()
        {
            throw new NotImplementedException();
        }
    }
}