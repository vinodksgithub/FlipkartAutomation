namespace FlipkartProj.Config
{
    public class TestSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public bool Headless { get; set; } = false;
        public int SlowMo { get; set; } = 0;
        public int DefaultTimeoutMs { get; set; } = 30000;
        public string ApiBaseUrl { get; set; } = "https://restful-booker.herokuapp.com/";
        public string ApiUsername { get; set; } = "admin";
        public string ApiPassword { get; set; } = "password123";

    }
}