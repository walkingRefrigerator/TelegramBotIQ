namespace MovieAPI
{
    internal static class Config
    {
        private const string MovieTokenKey = "2a0806aaa686c226e1a982fbe4444958";

        public static string MovieToken { get; }

        static Config()
        {
            MovieToken = MovieTokenKey;
        }

    }
}
