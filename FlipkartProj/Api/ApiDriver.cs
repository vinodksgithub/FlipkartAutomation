using FlipkartProj.Config;
using Microsoft.Playwright;

namespace FlipkartProj.Api
{
    public class ApiDriver : IAsyncDisposable
    {
        private IPlaywright? _playwright;

        public IAPIRequestContext Request { get; private set; } = default!;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();

            Request = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = ConfigReader.Settings.ApiBaseUrl,
                Timeout = ConfigReader.Settings.DefaultTimeoutMs,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Accept"] = "application/json"
                }
            });
        }

        public async ValueTask DisposeAsync()
        {
            if (Request is not null) await Request.DisposeAsync();
            _playwright?.Dispose();
        }
    }
}