using System.Text.RegularExpressions;
using FlipkartProj.Config;
using Microsoft.Playwright;

namespace FlipkartProj.Pages
{
    public class HomePage
    {
        private readonly IPage _page;

        public HomePage(IPage page)
        {
            _page = page;
        }

        // Locators
        private ILocator LoginPopupHeader => _page.Locator("//div[text()='Log in for the best experience']");
        private ILocator SearchBox => _page.Locator("input[name='q']:not([readonly])");
        // Actions
        public async Task NavigateAsync()
        {
            await _page.GotoAsync(ConfigReader.Settings.BaseUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });
        }

        public async Task ClosePopupIfPresentAsync()
        {
            try
            {
                // Wait up to 5s for the popup to appear
                await LoginPopupHeader.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                });

                await _page.Keyboard.PressAsync("Escape");

                // Confirm it is gone
                await LoginPopupHeader.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Hidden,
                    Timeout = 5000
                });
            }
            catch (TimeoutException)
            {
                // Popup did not appear (or did not close), continue
            }
        }

        public async Task<bool> IsHomePageDisplayedAsync()
        {
            await SearchBox.First.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible
            });
            return await SearchBox.First.IsVisibleAsync()
                   && _page.Url.Contains("flipkart.com");
        }

        public async Task SearchAsync(string searchTerm)
        {
            await SearchBox.ClickAsync();
            await SearchBox.FillAsync(searchTerm);
            await SearchBox.PressAsync("Enter");

            // Wait until the results page has loaded
            await _page.WaitForURLAsync(new Regex(@"/search\?q="), new PageWaitForURLOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });
        }
    }
}