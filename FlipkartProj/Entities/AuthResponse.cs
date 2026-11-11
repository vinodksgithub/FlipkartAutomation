using System.Text.Json.Serialization;

namespace FlipkartProj.Entities
{
    public class AuthResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}