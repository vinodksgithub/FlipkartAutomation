using FlipkartProj.Entities;
using Microsoft.Playwright;

namespace FlipkartProj.Support
{
    public class BookingContext
    {
        public Booking Submitted { get; set; } = new();
        public int BookingId { get; set; }
        public IAPIResponse? LastResponse { get; set; }

        public string? Token { get; set; }
        public string UpdatedLastName { get; set; } = string.Empty;
    }
}