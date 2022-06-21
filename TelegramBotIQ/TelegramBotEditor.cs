using System;
using System.Data;
using System.Text.RegularExpressions;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using WeatherAPI;
using ValuteAPI;
using ConvertPDF;

using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace TelegramBotIQ
{
    internal class TelegramBotEditor : IDisposable
    {
        private readonly string _telegramBotToken;

        private TelegramBotClient botClient;

        private UserDB userDB;

        private DB db;

        private DataTable table;

        public TelegramBotEditor(string telegramBotToken)
        {
            _telegramBotToken = telegramBotToken;

            botClient = new TelegramBotClient(_telegramBotToken) { Timeout = TimeSpan.FromSeconds(10) };
            userDB = new UserDB();
            db = new DB();

            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += Bot_OnCallbackQuery;
            //botClient.OnInlineQuery += Bot_OnInline;

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

        #region Стартовые обработчики

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
После создания аккаунта или если он уже есть, то надо выбрать кнопку <i>'Авторизация'</i>", ParseMode.Html, false, false, 0);

                var inlineFunc = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Авторизация"),
                            InlineKeyboardButton.WithCallbackData("Регистрация")
                        }
                    }
                );

                await botClient.SendTextMessageAsync(msg.Chat.Id, "Верификация", replyMarkup: inlineFunc);

                botClient.OnMessage -= Bot_OnStart;
            }
        }

        private async void Bot_OnCheck(object sender, CallbackQueryEventArgs e)
        {
            var callback = e.CallbackQuery;
            var text = e.CallbackQuery.Data;

            switch (text)
            {
                case "Авторизация":

                    await botClient.SendTextMessageAsync(callback.From.Id, "Введите имя пользователя");

                    botClient.OnMessage += Bot_AutoLogin;
                    botClient.OnCallbackQuery -= Bot_OnCheck;

                    break;

                case "Регистрация":

                    await botClient.SendTextMessageAsync(callback.From.Id, "Введите имя пользователя");

                    botClient.OnMessage += Bot_RegLogin;
                    botClient.OnCallbackQuery -= Bot_OnCheck;

                    break;
            }
        }

        #endregion

        #region Обработчик регистрации

        private async void Bot_RegLogin(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            var username = e.Message.Text; 

            Regex.Replace(username, @"[^0-9a-zA-Z]+", "");

            if (username == null)
            {
                await botClient.SendTextMessageAsync(msg.Chat.Id, "Ошибка при регистрации");

                botClient.OnCallbackQuery += Bot_OnCheck;
                botClient.OnMessage -= Bot_RegLogin;
            }

            userDB.SetUser(username);

            await botClient.SendTextMessageAsync(msg.From.Id, "Введите пароль");

            botClient.OnMessage += Bot_RegPass;
            botClient.OnMessage -= Bot_RegLogin;
        }

        private async void Bot_RegPass(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            var pass = e.Message.Text;

            if (pass != null)
            {
                userDB.SetPass(pass);

                db.RegSQL(userDB);

                await botClient.SendTextMessageAsync(msg.Chat.Id, @$"Вы успешно зарегистрировались! 
Логин: {userDB.GetUser()},
Пароль: {userDB.GetPass()}");
                await botClient.SendTextMessageAsync(msg.Chat.Id, "Нажмите /command, чтобы получить список доступных команд");

                botClient.OnMessage += Bot_OnMessage;
                botClient.OnCallbackQuery += Bot_OnCallbackQuery;
                botClient.OnCallbackQuery -= Bot_OnCheck;
                botClient.OnMessage -= Bot_RegPass;
            }
            else
            {
                await botClient.SendTextMessageAsync(msg.Chat.Id, "Ошибка при регистрации!");

                botClient.OnCallbackQuery += Bot_OnCheck;
                botClient.OnMessage -= Bot_RegPass;
            }

        }

        #endregion

        #region Обработчик авторизации

        private async void Bot_AutoLogin(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            var username = e.Message.Text;

            userDB.SetUser(username);

            await botClient.SendTextMessageAsync(msg.From.Id, "Введите пароль");

            botClient.OnMessage += Bot_AutoPass;
            botClient.OnMessage -= Bot_AutoLogin;
        }

        private async void Bot_AutoPass(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            var pass = e.Message.Text;

            userDB.SetPass(pass);

            table = db.AutoSQL(userDB);

            if (table.Rows.Count > 0)
            {
                await botClient.SendTextMessageAsync(msg.Chat.Id, "Вы успешно авторизованы!");
                await botClient.SendTextMessageAsync(msg.Chat.Id, "Нажмите /command, чтобы получить список доступных команд");

                botClient.OnMessage += Bot_OnMessage;
                botClient.OnCallbackQuery += Bot_OnCallbackQuery;
                botClient.OnMessage -= Bot_AutoPass;
                botClient.OnCallbackQuery -= Bot_OnCheck;
            }
            else
            {
                await botClient.SendTextMessageAsync(msg.Chat.Id, "Ошибка при авторизации!");

                botClient.OnCallbackQuery += Bot_OnCheck;
                botClient.OnMessage -= Bot_AutoPass;
            }
            
        }

        #endregion

        //private async void Bot_OnInline(object sender, InlineQueryEventArgs e)
        //{

        //    InlineQueryResultBase[] result =
        //    {
        //        new InlineQueryResultPhoto
        //        (
        //            id: "1",
        //            photoUrl: @"https://photo.sibnet.ru/upload/imgbig/124840052868.jpeg",
        //            thumbUrl: @"https://photo.sibnet.ru/upload/imgbig/124840052868.jpeg"
        //            )
        //        {
        //            Caption = "Фото"
        //        },

        //    };

        //    await botClient.AnswerInlineQueryAsync(e.InlineQuery.Id, result, 
        //                                   isPersonal: true,
        //                                   cacheTime: 0);

        //}

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

                case "PDF to Word":

                    await botClient.AnswerCallbackQueryAsync(callback.Id, "Отправьте файл");

                    botClient.OnMessage += Bot_OnMessageConvert;
                    botClient.OnMessage -= Bot_OnMessage;

                    break;

                case "Скачать файл":

                    await botClient.AnswerCallbackQueryAsync(callback.Id, "Отправьте название файла");

                    botClient.OnMessage += Bot_DowloadFile;
                    botClient.OnMessage -= Bot_OnMessage;

                    break;

                case "Удалить файл":

                    await botClient.AnswerCallbackQueryAsync(callback.Id, "Отправьте название файла");

                    botClient.OnMessage += Bot_DeleteFile;
                    botClient.OnMessage -= Bot_OnMessage;

                    break;
            }
        }
        #endregion

        private async void Bot_DowloadFile(object sender, MessageEventArgs e)
        {
            var msg = e.Message;

            try
            {
                await BotCommands.SendFile(msg, botClient, userDB);
            }
            catch
            {
                await botClient.SendTextMessageAsync(msg.Chat.Id, "Введено неверный формат файла");
            }
            finally
            {
                botClient.OnMessage += Bot_OnMessage;

                Dispose();
            }
        }

        private async void Bot_DeleteFile(object sender, MessageEventArgs e)
        {
            var msg = e.Message;

            try
            {
                await BotCommands.DeleteFile(msg, botClient, userDB);
            }
            catch
            {
                await botClient.SendTextMessageAsync(msg.Chat.Id, "Введено неверный формат файла");
            }
            finally
            {
                botClient.OnMessage += Bot_OnMessage;

                Dispose();
            }
        }

        #region Обработка входящих сообщений
        private async void  Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;

            if (msg.Type != MessageType.Sticker && msg.Type != MessageType.Text && msg.Type != MessageType.Document && msg.Type != MessageType.Video 
                && msg.Type != MessageType.Voice && msg.Type != MessageType.Photo && msg.Type != MessageType.Audio)
                    return;

            if (msg.Text == "/Out")
                await OutUser(msg);

                Console.WriteLine($"{msg.Chat.FirstName} {msg.Chat.LastName} отправил сообщение.");

            switch (msg.Type)
            {
                case MessageType.Sticker:
                    await BotCommands.MessageReplySticker(msg, botClient);
                    break;

                case MessageType.Text:
                    await BotCommands.MessageMovie(msg, botClient);
                    break;

                default:
                    await BotCommands.MessageReplyFile(msg, botClient, msg.Type, userDB);
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

        #region Обработчик для конвертора
        private async void Bot_OnMessageConvert(object sender, MessageEventArgs e)
        {
            var msg = e.Message;

            try
            {
                await ConvertPDF.Convert.ConvertPDFtoWord(botClient, msg);
            }
            catch
            {
                Console.WriteLine($"{msg.Chat.FirstName} {msg.Chat.LastName} Проблема с обработкой файла");
            }
            finally
            {
                botClient.OnMessage += Bot_OnMessage;

                Dispose();
            }
        }
        #endregion

        private async Task OutUser(Message msg)
        {
            var inlineFunc = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Авторизация"),
                            InlineKeyboardButton.WithCallbackData("Регистрация")
                        }
                    }
                );

            await botClient.SendTextMessageAsync(msg.Chat.Id, "Верификация", replyMarkup: inlineFunc);

            botClient.OnCallbackQuery += Bot_OnCheck;
            botClient.OnMessage -= Bot_OnStart;
            botClient.OnMessage -= Bot_OnMessage;
            botClient.OnCallbackQuery -= Bot_OnCallbackQuery;


        }

        public void Dispose()
        {
            botClient.OnMessage -= Bot_OnMessageConvert;
            botClient.OnMessage -= Bot_OnMessageWeather;
            botClient.OnMessage -= Bot_DowloadFile;
        }
    }
}
