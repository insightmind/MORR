using System;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Core.Data.Transcoding.Exceptions;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.Json
{
    public class JsonDecoder : DefaultDecodableEventQueue<JsonIntermediateFormatSample>, IDecoder
    {
        [Import]
        private JsonDecoderConfiguration Configuration { get; set; }

        private static Guid Identifier { get; } = new Guid("E943EACB-5AD1-49A7-92CE-C42E7AD8995B");

        private readonly IFileSystem fileSystem;

        public ManualResetEvent DecodeFinished { get; } = new ManualResetEvent(false);

        [ImportingConstructor]
        public JsonDecoder() : this(new FileSystem()) { }

        public JsonDecoder(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void Decode(DirectoryPath recordingDirectoryPath)
        {
            Task.Run(() => DecodeEvents(recordingDirectoryPath));
        }

        private Stream GetFileStream(DirectoryPath recordingDirectoryPath)
        {
            var fullPath = fileSystem.Path.Combine(recordingDirectoryPath.ToString(), Configuration.RelativeFilePath.ToString());
            return fileSystem.File.OpenRead(fullPath);
        }

        private async void DecodeEvents(DirectoryPath recordingDirectoryPath)
        {
            await using var fileStream = GetFileStream(recordingDirectoryPath);
            var document = JsonDocument.Parse(fileStream).RootElement;

            Open();
            DecodeFinished.Reset();

            foreach (var eventElement in document.EnumerateArray())
            {
                var encodedType = eventElement.GetProperty(nameof(JsonIntermediateFormatSample.Type)).GetString();
                var type = Utility.GetTypeFromAnyAssembly(encodedType);

                if (type == null)
                {
                    throw new DecodingException($"Failed to parse event type {encodedType}.");
                }

                var encodedData = eventElement.GetProperty(nameof(JsonIntermediateFormatSample.Data)).GetRawText();
                var data = Encoding.UTF8.GetBytes(encodedData);

                var intermediateFormatSample = new JsonIntermediateFormatSample
                {
                    Type = type,
                    Data = data,
                    IssuingModule = Identifier
                };

                Enqueue(intermediateFormatSample);
            }

            Close();
            DecodeFinished.Set();
        }
    }
}