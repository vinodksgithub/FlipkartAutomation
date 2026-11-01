using Microsoft.Extensions.Configuration;

namespace FlipkartProj.Config
{
    public static class ConfigReader
    {
        private static readonly Lazy<TestSettings> _settings = new(Load);

        public static TestSettings Settings => _settings.Value;

        private static TestSettings Load()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            return config.Get<TestSettings>()
                   ?? throw new InvalidOperationException("Could not read appsettings.json");
        }
    }
}