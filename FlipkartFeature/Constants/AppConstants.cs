namespace FlipkartFeature.Constants
{
    internal class AppConstants
    {
        public static class Urls
        {
            public const string Home = ""; // "https://www.flipkart.com";
        }

        public static class Timeouts
        {
            public const int Short = 5000;
            public const int Default = 30000;
            public const int Long = 60000;
        }

        public static class ScenarioKeys
        {
            public const string SelectedProduct = "SelectedProduct";
            public const string SearchTerm = "SearchTerm";
        }
    }
}
