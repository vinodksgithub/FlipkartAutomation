using FlipkartProj.Drivers;
using FlipkartProj.Support;
using Reqnroll;
using Reqnroll.BoDi;

namespace FlipkartProj.Hooks
{
    [Binding]
    public class TestHooks
    {
        private readonly IObjectContainer _container;
        private readonly ScenarioContext _scenarioContext;
        private PlaywrightDriver? _driver;
        private int _stepCounter;

        public TestHooks(IObjectContainer container, ScenarioContext scenarioContext)
        {
            _container = container;
            _scenarioContext = scenarioContext;
        }

        private bool IsApiScenario =>
            _scenarioContext.ScenarioInfo.CombinedTags.Any(t => t.Equals("api", StringComparison.OrdinalIgnoreCase));


        [BeforeScenario(Order = 0)]
        public async Task BeforeScenario()
        {
            if (IsApiScenario) return;          // no browser for API tests

            _driver = new PlaywrightDriver();
            await _driver.InitializeAsync();

            _container.RegisterInstanceAs(_driver);
            _container.RegisterInstanceAs(_driver.Page);
        }

        
        [AfterStep]
        public async Task AfterStep()
        {
            if (IsApiScenario || _driver is null) return;   // skip screenshots for API tests

            _stepCounter++;

            var stepText = _scenarioContext.StepContext.StepInfo.Text;
            var status = _scenarioContext.TestError == null ? "" : "_FAILED";

            var path = await ScreenshotHelper.CaptureAsync(
                _driver.Page,
                _scenarioContext.ScenarioInfo.Title,
                _stepCounter,
                stepText + status);

            Console.WriteLine($"Screenshot: {path}");
        }


        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_driver is not null)
                await _driver.DisposeAsync();
        }
    }
}