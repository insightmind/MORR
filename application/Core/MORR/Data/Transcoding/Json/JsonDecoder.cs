using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MORR.Core.Data.IntermediateFormat.Json;
using MORR.Core.Data.Transcoding.Exceptions;
using MORR.Shared.Events.Queue;
using MORR.Shared.Utility;

namespace MORR.Core.Data.Transcoding.Json
{
    public class JsonDecoder : DefaultDecodeableEventQueue<JsonIntermediateFormatSample>, IDecoder
    {
        public static Guid Identifier { get; } = new Guid("E943EACB-5AD1-49A7-92CE-C42E7AD8995B");

        public void Decode(FilePath path)
        {
            Task.Run(() => DecodeEvents(path));
        }

        private async void DecodeEvents(FilePath path)
        {
            await using var fileStream = File.OpenRead(path.ToString());
            var document = JsonDocument.Parse(fileStream).RootElement;

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
        }
    }
}