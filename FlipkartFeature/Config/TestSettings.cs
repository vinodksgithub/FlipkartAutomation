namespace FlipkartProj.Config
{
    public class TestSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public bool Headless { get; set; } = false;
        public int SlowMo { get; set; } = 0;
        public int DefaultTimeoutMs { get; set; } = 30000;
    }
}