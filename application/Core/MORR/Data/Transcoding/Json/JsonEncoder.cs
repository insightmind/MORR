using System.ComponentModel.Composition;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.Json
{
    public class JsonEncoder : IEncoder
    {
        [Import]
        private IEncodableEventQueue<JsonIntermediateFormatSample> IntermediateFormatSampleQueue { get; set; }

        [Import]
        private JsonEncoderConfiguration Configuration { get; set; }

        private readonly IFileSystem fileSystem;

        public ManualResetEvent EncodeFinished { get; } = new ManualResetEvent(false);

        public JsonEncoder() : this(new FileSystem()) { }

        public JsonEncoder(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void Encode(DirectoryPath recordingDirectoryPath)
        {
            Task.Run(() => EncodeEvents(recordingDirectoryPath));
        }

        private async void EncodeEvents(DirectoryPath recordingDirectoryPath)
        {
            await using var fileStream = GetFileStream(recordingDirectoryPath);
            // using statement with IDisposable will close writer at end of scope
            await using var writer = new Utf8JsonWriter(fileStream);
            writer.WriteStartArray();

            EncodeFinished.Reset();

            await foreach (var sample in IntermediateFormatSampleQueue.GetEvents())
            {
                writer.WriteStartObject();
                writer.WriteString(nameof(JsonIntermediateFormatSample.Type), sample.JsonEncodedType);
                // As there is no WriteRaw method on Utf8JsonWriter, we have to use a workaround to write the data
                writer.WritePropertyName(nameof(JsonIntermediateFormatSample.Data));
                sample.JsonEncodedData.WriteTo(writer);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            EncodeFinished.Set();
        }

        private Stream GetFileStream(DirectoryPath recordingDirectoryPath)
        {
            var fullPath = fileSystem.Path.Combine(recordingDirectoryPath.ToString(), Configuration.RelativeFilePath.ToString());
            return fileSystem.File.OpenWrite(fullPath);
        }
    }
}