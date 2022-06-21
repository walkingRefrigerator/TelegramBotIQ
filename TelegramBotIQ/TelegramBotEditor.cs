using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ValuteAPI;
using WeatherAPI;

namespace TelegramBotIQ
{
    internal class TelegramBotEditor : IDisposable
    {
        private readonly string _telegramBotToken;

        private TelegramBotClient botClient;

        public TelegramBotEditor(string telegramBotToken)
        {
            _telegramBotToken = telegramBotToken;

            botClient = new TelegramBotClient(_telegramBotToken) { Timeout = TimeSpan.FromSeconds(10) };

            botClient.OnMessage += Bot_OnStart;
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

        private async void Bot_OnStart(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var msg = e.Message;

            if (msg.Text == "/start")
            {
                await botClient.SendTextMessageAsync(e.Message.From.Id, @"Приветствую.
Данный бот является мультимедийной платформой со множеством полезных функций.
Также бот способен конвертировать <b>pdf</b> файлы, присылать афишу, выступать облачным диском, хранящий ваши документы/фото/ауди/видео.
Для начала следует отправить боту любой документ, и бот его сохранит. 
Позже вы сможете попросить наших ботов вам их прислать.
Для вашей персональной безопасности вам следует зарегистрировать пользователя на нашем сервисе.
Для этого стоит выбрать кнопку <i>'Регистрация'</i> и ввести логин и пароль.
После создания аккаунта или если он уже есть, то надо выбрать кнопку.
Напишите /command", ParseMode.Html, false, false, 0);

                botClient.OnMessage -= Bot_OnStart;

                botClient.OnMessage += Bot_OnMessage;
            }
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
                    botClient.OnMessage -= Bot_OnMessage;

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
                botClient.OnMessage += Bot_OnMessage;

                Dispose();
            }


        }
        #endregion

        public void Dispose()
        {
        }
    }
}
