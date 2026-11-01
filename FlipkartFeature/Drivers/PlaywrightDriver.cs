using Microsoft.Playwright;

namespace FlipkartProj.Drivers
{
    public class PlaywrightDriver : IAsyncDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;

        public IBrowserContext Context { get; private set; } = default!;
        public IPage Page { get; private set; } = default!;

        public async Task InitializeAsync(bool headless = false)
        {
            _playwright = await Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless,
                Channel = "chrome",          // uses installed Google Chrome
                SlowMo = 0,
                Args = new[] { "--start-maximized" }
            });

            Context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,   // needed for --start-maximized
                IgnoreHTTPSErrors = true
            });

            Context.SetDefaultTimeout(30000);
            Context.SetDefaultNavigationTimeout(60000);

            // Optional: start trace (saved on failure in the hook)
            await Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            Page = await Context.NewPageAsync();
        }

        public async Task StopTracingAsync(string? tracePath = null)
        {
            if (tracePath is null)
                await Context.Tracing.StopAsync();
            else
                await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
        }

        public async ValueTask DisposeAsync()
        {
            if (Context is not null) await Context.CloseAsync();
            if (_browser is not null) await _browser.CloseAsync();
            _playwright?.Dispose();
        }
    }
}