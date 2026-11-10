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

        public async Task<IAPIResponse> UpdateLastNameAsync(int bookingId, string newLastName, string token)
        {
            var body = new Dictionary<string, object> { ["lastname"] = newLastName };

            var response = await _api.Request.PatchAsync($"booking/{bookingId}", new APIRequestContextOptions
            {
                DataObject = body,
                Headers = new Dictionary<string, string> { ["Cookie"] = $"token={token}" }
            });

            await LogAsync($"PATCH booking/{bookingId}", body, response);
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

        public async Task<IAPIResponse> GetAsync(int bookingId)
        {
            var response = await _api.Request.GetAsync($"booking/{bookingId}");
            await LogAsync($"GET booking/{bookingId}", null, response);
            return response;
        }

        public async Task<IAPIResponse> SearchByNameAsync(string firstName, string lastName)
        {
            var response = await _api.Request.GetAsync("booking", new APIRequestContextOptions
            {
                Params = new Dictionary<string, object>
                {
                    ["firstname"] = firstName,
                    ["lastname"] = lastName
                }
            });
            await LogAsync("GET booking (search by name)", null, response);
            return response;
        }

        public async Task<IAPIResponse> DeleteAsync(int bookingId, string token)
        {
            var response = await _api.Request.DeleteAsync($"booking/{bookingId}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string> { ["Cookie"] = $"token={token}" }
            });

            await LogAsync($"DELETE booking/{bookingId}", null, response);
            return response;
        }
    }
}