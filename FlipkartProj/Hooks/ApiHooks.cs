using FlipkartProj.Api;
using Reqnroll;
using Reqnroll.BoDi;

namespace FlipkartProj.Hooks
{
    [Binding]
    public class ApiHooks
    {
        private readonly IObjectContainer _container;
        private ApiDriver? _apiDriver;

        public ApiHooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario("api", Order = 0)]
        public async Task BeforeApiScenario()
        {
            _apiDriver = new ApiDriver();
            await _apiDriver.InitializeAsync();

            _container.RegisterInstanceAs(_apiDriver);
        }

        [AfterScenario("api")]
        public async Task AfterApiScenario()
        {
            if (_apiDriver is not null)
                await _apiDriver.DisposeAsync();
        }
    }
}