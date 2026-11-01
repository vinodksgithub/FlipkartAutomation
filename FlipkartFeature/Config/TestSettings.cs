namespace FlipkartProj.Config
{
    public class TestSettings
    {
        public string BaseUrl { get; set; } = "https://www.flipkart.com";
        public string Browser { get; set; } = "chrome";
        public bool Headless { get; set; } = false;
        public int SlowMo { get; set; } = 0;
        public int DefaultTimeoutMs { get; set; } = 30000;
    }
}