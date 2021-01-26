namespace TelegramBotIQ
{
    internal static class Config
    {
        private const string TelegramBotTokenKey = "1354258883:AAG4sK8gh1e9zFuEp1mzID1Le169lX2xRbw"; 

        public static string TelegramBotToken { get; }
        
        static Config()
        {
            TelegramBotToken = TelegramBotTokenKey;
        }


    }
}
