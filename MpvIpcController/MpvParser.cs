using System;
using System.IO;
using System.Text.Json;

namespace HanumanInstitute.MpvIpcController
{
    public static class MpvParser
    {
        /// <summary>
        /// Parses a MPV message and returns either a MpvJsonEvent or MpvJsonResponse.
        /// </summary>
        /// <param name="message">The JSON message to parse.</param>
        /// <returns>An MpvJsonEvent or MpvJsonResponse containing the strongly-typed message content.</returns>
        public static object? Parse(string message)
        {
            // using var reader = new JsonTextReader(new StringReader(message));
            var reader = JsonDocument.Parse(message);
            if (reader.RootElement.TryGetProperty("event", out var eventName))
            {
                // Event.
                // ex: { "event": "event_name" }
                var response = new MpvEvent()
                {
                    Event = eventName.GetString()
                };
                // Parse additional event args.
                foreach (var item in reader.RootElement.EnumerateObject())
                {
                    if (item.Name != "event")
                    {
                        response.Data.Add(item.Name, item.Value.GetRawText()); // ##
                    }
                }
                return response;
            }
            else if (reader.RootElement.TryGetProperty("request_id", out var requestId))
            {
                // Response to a message.
                // ex: { "error": "success", "data": 1.468135, "request_id": 100 }
                var response = new MpvResponse()
                {
                    RequestID = requestId.GetInt32()
                };
                if (reader.RootElement.TryGetProperty("error", out var error))
                {
                    response.Error = error.GetString();
                }
                if (reader.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in data.EnumerateObject())
                    {
                        response.Data.Add(item.Name, item.Value.GetRawText()); // ##
                    }
                }
                return response;
            }
            else
            {
                throw new InvalidDataException($"Unrecognized message: {message}");
            }
        }
    }
}
