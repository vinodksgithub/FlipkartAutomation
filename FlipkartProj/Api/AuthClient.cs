using FlipkartProj.Config;
using FlipkartProj.Entities;
using Microsoft.Playwright;

namespace FlipkartProj.Api
{
    public class AuthClient
    {
        private readonly ApiDriver _api;

        public AuthClient(ApiDriver api)
        {
            _api = api;
        }

        public async Task<string> GetTokenAsync()
        {
            var response = await _api.Request.PostAsync("auth", new APIRequestContextOptions
            {
                DataObject = new
                {
                    username = ConfigReader.Settings.ApiUsername,
                    password = ConfigReader.Settings.ApiPassword
                }
            });

            // Password is not logged on purpose
            Console.WriteLine($"--- POST auth --- Status: {response.Status}");

            var body = await response.JsonAsync<AuthResponse>();

            if (string.IsNullOrEmpty(body?.Token))
                throw new InvalidOperationException(
                    $"Authentication failed: {body?.Reason ?? await response.TextAsync()}");

            return body.Token;
        }
    }
}