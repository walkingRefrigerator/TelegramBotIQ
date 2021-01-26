using System;

namespace TelegramBotIQ
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TelegramBotEditor telegrambot = new TelegramBotEditor(Config.TelegramBotToken))
            {
                telegrambot.InfoBot();

                telegrambot.StartListening();

                Console.ReadLine();

                telegrambot.StopListening();
            }
        }

        
    }
}
