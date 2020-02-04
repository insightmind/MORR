using System.ComponentModel.Composition;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.Json
{
    public class JsonEncoder : IEncoder
    {
        [Import]
        private IEncodeableEventQueue<JsonIntermediateFormatSample> IntermediateFormatSampleQueue { get; set; }

        public void Encode(DirectoryPath directoryRecordingPath)
        {
            Task.Run(() => EncodeEvents(directoryRecordingPath));
        }

        private async void EncodeEvents(DirectoryPath recordingDirectoryPath)
        {
            await using var fileStream = GetFileStreamForOutput(recordingDirectoryPath);
            await using var writer = new Utf8JsonWriter(fileStream);
            writer.WriteStartArray();

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
        }

        private static FileStream GetFileStreamForOutput(DirectoryPath directoryPath)
        {
            var filePath = Path.Combine(directoryPath.ToString(), "event_data.json"); // TODO Make this configurable?
            var fileStream = File.OpenWrite(filePath);
            return fileStream;
        }
    }
}