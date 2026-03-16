using AccelByte.Sdk.Api.Cloudsave.Model;
using System.Text.Json.Serialization;

namespace AccelByte.PluginArch.EventHandler.Demo.Server.Services
{
    public class PlayerJoinedEvent : ModelsGameRecordRequest
    {
        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = "";

        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; } = "";

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = "";
    }
}
