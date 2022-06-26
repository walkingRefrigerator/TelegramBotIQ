using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using ValuteAPI;
using WeatherAPI;

namespace TelegramBotIQ
{
    internal class TelegramBotEditor : IDisposable
    {
        private readonly string _telegramBotToken;
        private List<long> _queueWeatherId;
        private List<long> _queueMovieId;

        private TelegramBotClient botClient;

        public TelegramBotEditor(string telegramBotToken)
        {
            _telegramBotToken = telegramBotToken;
            _queueWeatherId = new List<long>{};
            _queueMovieId = new List<long>();

            botClient = new TelegramBotClient(_telegramBotToken) { Timeout = TimeSpan.FromSeconds(10) };

            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += Bot_OnCallbackQuery;

        }

        public void InfoBot()
        {
            var botInfo = botClient.GetMeAsync().Result;

            Console.WriteLine($"Bot name: {botInfo.FirstName}," + Environment.NewLine +
                $"Bot id: {botInfo.Id}");
        }

        public void StartListening()
        {
            botClient.StartReceiving();
        }

        public void StopListening()
        {
            botClient.StopReceiving();
        }

        #region Обработка нажатия кнопок
        private async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var callback = e.CallbackQuery;

            string buttonText = callback.Data;
            string name = $"{callback.From.FirstName} {callback.From.LastName}";
            Console.WriteLine($"{name} выбрал кнопку {buttonText}");

            switch (buttonText)
            {
                case "Курс валюты":

                    await Exchange_Rates.GetValut(botClient, callback.Message);

                    break;

                case "Погода":

                    await botClient.AnswerCallbackQueryAsync(callback.Id, "Отправьте название города");

                    botClient.OnMessage += Bot_OnMessageWeather;

                    if (!_queueWeatherId.Contains(callback.Message.Chat.Id)){
                        _queueWeatherId.Add(callback.Message.Chat.Id);
                    }

                    break;
            }
        }
        #endregion

        #region Обработка входящих сообщений
        private async void  Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;

            if (msg.Type != MessageType.Sticker && msg.Type != MessageType.Text && msg.Type != MessageType.Document && msg.Type != MessageType.Video 
                && msg.Type != MessageType.Voice && msg.Type != MessageType.Photo && msg.Type != MessageType.Audio)
                    return;

                Console.WriteLine($"{msg.Chat.FirstName} {msg.Chat.LastName} отправил сообщение.");

            switch (msg.Text)
            {
                case "/Movie":

                    await botClient.SendTextMessageAsync(e.Message.Chat.Id, "Отправьте год премьеры");

                    botClient.OnMessage += Bot_OnMovie;

                    if (!_queueMovieId.Contains(e.Message.Chat.Id))
                    {
                        _queueMovieId.Add(e.Message.Chat.Id);
                    }

                    break;
            }

            switch (msg.Type)
            {
                case MessageType.Sticker:
                    await BotCommands.MessageReplySticker(msg, botClient);
                    break;

                case MessageType.Text:
                    await BotCommands.MessageReplyText(msg, botClient);
                    break;
            }
        }
        #endregion

        #region Обработчик сообщений для погоды
        private async void Bot_OnMessageWeather(object sender, MessageEventArgs e)
        {
            if (_queueWeatherId.Contains(e.Message.Chat.Id))
            {
                var msg = e.Message;

                try
                {
                    await Weather.GetWeather(msg.Text, botClient, msg);
                }
                catch
                {
                    Console.WriteLine($"{msg.Chat.FirstName} {msg.Chat.LastName} ввел неверно имя города");
                }
                finally
                {
                    botClient.OnMessage -= Bot_OnMessageWeather;

                    _queueWeatherId.Remove(e.Message.From.Id);

                    Dispose();
                }
            }
        }
        #endregion

        private async void Bot_OnMovie(object sender, MessageEventArgs e)
        {
            if (_queueMovieId.Contains(e.Message.Chat.Id))
            {
                var msg = e.Message;

                try
                {
                    await BotCommands.SendMovies(msg, botClient);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error message: User: {e.Message.From.Username} ChatId: {e.Message.From.Id} " +
                        $"{ex.Message}");
                }
                finally
                {
                    botClient.OnMessage -= Bot_OnMovie;

                    _queueMovieId.Remove(e.Message.From.Id);

                    Dispose();
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
