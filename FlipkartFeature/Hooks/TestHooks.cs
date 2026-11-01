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
        private PlaywrightDriver _driver = default!;
        private int _stepCounter;

        public TestHooks(IObjectContainer container, ScenarioContext scenarioContext)
        {
            _container = container;
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _driver = new PlaywrightDriver();
            await _driver.InitializeAsync();

            _container.RegisterInstanceAs(_driver);
            _container.RegisterInstanceAs(_driver.Page);
        }

        [AfterStep]
        public async Task AfterStep()
        {
            _stepCounter++;

            var stepText = _scenarioContext.StepContext.StepInfo.Text;
            var status = _scenarioContext.TestError == null ? "" : "_FAILED";

            var path = await ScreenshotHelper.CaptureAsync(
                _driver.Page,
                _scenarioContext.ScenarioInfo.Title,
                _stepCounter,
                stepText + status);

            // Shows the screenshot path in the test output
            Console.WriteLine($"Screenshot: {path}");
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            await _driver.DisposeAsync();
        }
    }
}