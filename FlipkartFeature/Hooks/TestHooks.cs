using FlipkartProj.Drivers;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;

namespace FlipkartProj.Hooks
{
    [Binding]
    public class TestHooks
    {
        private readonly IObjectContainer _container;
        private readonly ScenarioContext _scenarioContext;
        private PlaywrightDriver _driver = default!;

        public TestHooks(IObjectContainer container, ScenarioContext scenarioContext)
        {
            _container = container;
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _driver = new PlaywrightDriver();
            await _driver.InitializeAsync();          // launches Chrome

            _container.RegisterInstanceAs(_driver);
            _container.RegisterInstanceAs(_driver.Page);   // makes IPage injectable
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_scenarioContext.TestError != null)
            {
                Directory.CreateDirectory("Screenshots");
                var name = _scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
                await _driver.Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = $"Screenshots/{name}.png",
                    FullPage = true
                });
            }

            await _driver.DisposeAsync();
        }
    }
}