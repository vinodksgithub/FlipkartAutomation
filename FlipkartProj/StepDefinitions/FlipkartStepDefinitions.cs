using FlipkartProj.Pages;
using NUnit.Framework;
using Reqnroll;

namespace FlipkartProj.StepDefinitions
{
    [Binding]
    public class HomePageSteps
    {
        private readonly HomePage _homePage;

        public HomePageSteps(HomePage homePage)
        {
            _homePage = homePage;
        }

        [Given("User launches webpage")]
        public async Task GivenUserLaunchesWebpage()
        {
            await _homePage.NavigateAsync();
        }

        [Given("User closes any popup open in homepage")]
        public async Task GivenUserClosesAnyPopupOpenInHomepage()
        {
            await _homePage.ClosePopupIfPresentAsync();
        }

        [Then("User should see home page")]
        public async Task ThenUserShouldSeeHomePage()
        {
            bool isDisplayed = await _homePage.IsHomePageDisplayedAsync();
            Assert.That(isDisplayed, Is.True, "Flipkart home page was not displayed");
        }

        [When("User Inputs {string} in search")]
        public async Task WhenUserInputsInSearch(string searchTerm)
        {
            await _homePage.SearchAsync(searchTerm);
        }
    }
}