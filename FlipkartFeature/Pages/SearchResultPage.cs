using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace FlipkartProj.Pages
{
    public class SearchResultsPage
    {
        private readonly IPage _page;

        public SearchResultsPage(IPage page)
        {
            _page = page;
        }

        // Locators
        private ILocator ProductCards => _page.Locator("a[href*='/p/itm'][href*='pid=']");
        private ILocator FirstResultTitle => ProductCards.First.Locator("div.RG5Slk");
        private ILocator ResultsHeader => _page.GetByText(
            new Regex(@"Showing \d+\s*[–-]\s*\d+ of \d+ results", RegexOptions.IgnoreCase));

        // Verifications
        public async Task<bool> AreSearchResultsDisplayedAsync()
        {
            try
            {
                await ProductCards.First.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 15000
                });
            }
            catch (TimeoutException)
            {
                return false;
            }

            return await ProductCards.CountAsync() > 0;
        }

        public async Task<int> GetResultCountAsync()
        {
            return await ProductCards.CountAsync();
        }

        public async Task<string> GetFirstResultTitleAsync()
        {
            await FirstResultTitle.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible
            });
            return (await FirstResultTitle.InnerTextAsync()).Trim();
        }
    }
}