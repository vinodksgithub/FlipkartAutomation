using FlipkartProj.Pages;
using NUnit.Framework;
using Reqnroll;

namespace FlipkartProj.StepDefinitions
{
    [Binding]
    public class SearchResultsSteps
    {
        private readonly SearchResultsPage _searchResultsPage;

        public SearchResultsSteps(SearchResultsPage searchResultsPage)
        {
            _searchResultsPage = searchResultsPage;
        }

        [Then("User should see search results")]
        public async Task ThenUserShouldSeeSearchResults()
        {
            bool displayed = await _searchResultsPage.AreSearchResultsDisplayedAsync();
            Assert.That(displayed, Is.True, "Search results were not displayed");
        }

        [Then("User should See first result as {string}")]
        public async Task ThenUserShouldSeeFirstResultAs(string expectedText)
        {
            string actualTitle = await _searchResultsPage.GetFirstResultTitleAsync();

            Assert.That(actualTitle, Does.Contain(expectedText).IgnoreCase,
                $"First result was '{actualTitle}', expected it to contain '{expectedText}'");
        }
    }
}