using FlipkartProj.Entities;
using Microsoft.Playwright;

namespace FlipkartProj.Api
{
    public class BookingClient
    {
        private readonly ApiDriver _api;

        public BookingClient(ApiDriver api)
        {
            _api = api;
        }

        public async Task<IAPIResponse> CreateAsync(Booking booking)
        {
            var response = await _api.Request.PostAsync("booking", new APIRequestContextOptions
            {
                DataObject = booking
            });

            await LogAsync("POST booking", booking, response);
            return response;
        }

        private static async Task LogAsync(string title, object? requestBody, IAPIResponse response)
        {
            Console.WriteLine($"--- {title} ---");
            if (requestBody is not null)
                Console.WriteLine($"Request : {System.Text.Json.JsonSerializer.Serialize(requestBody)}");
            Console.WriteLine($"Status  : {response.Status}");
            Console.WriteLine($"Response: {await response.TextAsync()}");
        }
    }
}