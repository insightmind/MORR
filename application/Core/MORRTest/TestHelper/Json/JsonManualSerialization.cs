using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using SharedTest.TestHelpers.Event;

namespace MORRTest.TestHelper.Json
{
    public static class JsonManualSerialization
    {
        public static string GenerateSerializedEvents(IReadOnlyList<TestEvent> testEvents)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"[");

            for (var index = 0; index < testEvents.Count; index++)
            {
                stringBuilder.Append(GenerateManualSerializedVersion(testEvents[index], testEvents[index].GetType().FullName));

                if (index + 1 < testEvents.Count)
                {
                    stringBuilder.Append(",");
                }
            }

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }

        public static string GenerateManualSerializedVersion(TestEvent testEvent, string eventType)
        {
            return "{\"Type\":\"" + eventType + "\",\"Data\":{\"Identifier\":" + testEvent.Identifier + ",\"Timestamp\":" + JsonSerializer.Serialize(testEvent.Timestamp) + ",\"IssuingModule\":\"" + testEvent.IssuingModule + "\"}}";
        }
    }
}
