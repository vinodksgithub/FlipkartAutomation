using System.Text.Json.Serialization;

namespace FlipkartProj.Entities
{
    public class BookingIdItem
    {
        [JsonPropertyName("bookingid")]
        public int BookingId { get; set; }
    }
}