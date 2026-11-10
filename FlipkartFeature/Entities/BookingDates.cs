using System.Text.Json.Serialization;

namespace FlipkartProj.Entities
{
    public class BookingDates
    {
        [JsonPropertyName("checkin")]
        public string CheckIn { get; set; } = string.Empty;

        [JsonPropertyName("checkout")]
        public string CheckOut { get; set; } = string.Empty;
    }
}