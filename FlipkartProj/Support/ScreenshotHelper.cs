using Microsoft.Playwright;

namespace FlipkartProj.Support
{
    public static class ScreenshotHelper
    {
        // One run folder per test run, e.g. Screenshots/2026-11-01_16-30-05
        private static readonly string RunFolder = Path.Combine(
            AppContext.BaseDirectory, "Screenshots", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

        public static async Task<string> CaptureAsync(IPage page, string scenarioTitle, int stepNumber, string stepText)
        {
            var scenarioFolder = Path.Combine(RunFolder, Sanitize(scenarioTitle));
            Directory.CreateDirectory(scenarioFolder);

            var fileName = $"{stepNumber:00}_{Sanitize(stepText)}.png";
            var fullPath = Path.Combine(scenarioFolder, fileName);

            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = fullPath,
                FullPage = false
            });

            return fullPath;
        }

        private static string Sanitize(string text)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var clean = new string(text.Select(c => invalid.Contains(c) || c == ' ' ? '_' : c).ToArray());
            return clean.Length > 60 ? clean[..60] : clean;
        }
    }
}