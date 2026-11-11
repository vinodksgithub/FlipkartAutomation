using System.Text.Json.Serialization;

namespace FlipkartProj.Entities
{
    public class Booking
    {
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastname")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("totalprice")]
        public int TotalPrice { get; set; }

        [JsonPropertyName("depositpaid")]
        public bool DepositPaid { get; set; }

        [JsonPropertyName("bookingdates")]
        public BookingDates BookingDates { get; set; } = new();

        [JsonPropertyName("additionalneeds")]
        public string AdditionalNeeds { get; set; } = string.Empty;
    }
}