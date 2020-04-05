using System.Text.Json;

namespace MORR.Core.Data.IntermediateFormat.Json
{
    /// <summary>
    ///     A sample in JSON intermediate format.
    /// </summary>
    public class JsonIntermediateFormatSample : IntermediateFormatSample
    {
        /// <summary>
        ///     The data that is serialized in JSON-compatible format.
        /// </summary>
        public JsonDocument JsonEncodedData => JsonDocument.Parse(Data);

        /// <summary>
        ///     The type of the event that is serialized in JSON-compatible format.
        /// </summary>
        public JsonEncodedText JsonEncodedType => JsonEncodedText.Encode(Type.ToString());
    }
}