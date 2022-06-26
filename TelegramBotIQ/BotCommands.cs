using MovieAPI;
using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotIQ
{
    internal static class BotCommands
    {
        public static async Task MessageSendPhoto(ChatId id, TelegramBotClient bot)
        {
            var rnd1 = new Random();

            System.IO.FileStream file1 = System.IO.File.OpenRead(@$"photo teleg\({rnd1.Next(1, 87)}).jpg");

            InputOnlineFile myPhoto1 = new InputOnlineFile(file1, "Art.jpg");

            await bot.SendDocumentAsync(id, myPhoto1);

        }

        #region Ответ на стикер
        public static async Task MessageReplySticker(Message msg, TelegramBotClient bot)
        {
            await bot.SendTextMessageAsync(msg.Chat.Id, "Я это запомнил");
            await bot.SendStickerAsync(msg.Chat.Id, msg.Sticker.FileId);
        }
        #endregion

        #region Ответ на текст

        public static async Task MessageReplyText(Message msg, TelegramBotClient bot)
        {
            var text = msg?.Text;

            if (text == null)
                return;

            switch (text)
            {
                case "/start":

                await bot.SendTextMessageAsync(msg.From.Id, @"Приветствую.
Данный бот является мультимедийной платформой со множеством полезных функций.
Также бот способен конвертировать <b>pdf</b> файлы, присылать афишу, выступать облачным диском, хранящий ваши документы/фото/ауди/видео.
Для начала следует отправить боту любой документ, и бот его сохранит. 
Позже вы сможете попросить наших ботов вам их прислать.
Для вашей персональной безопасности вам следует зарегистрировать пользователя на нашем сервисе.
Для этого стоит выбрать кнопку <i>'Регистрация'</i> и ввести логин и пароль.
После создания аккаунта или если он уже есть, то надо выбрать кнопку.
Напишите /command", ParseMode.Html, false, false, 0);

                    break;

                case "/command":

                    await bot.SendTextMessageAsync(msg.Chat.Id, @"Список команд:
/command - список команд,
/Keyboard - запуск клавиатуры,
/Menu - погода и курс валюты,
/Movie - афиша фильмов
");
                    break;

                case "/Menu":

                    var inlineF = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Курс валюты"),
                            InlineKeyboardButton.WithCallbackData("Погода")
                        }
                    }
                    );

                    await bot.SendTextMessageAsync(msg.Chat.Id, "Мои возможности", replyMarkup: inlineF);

                    break;

                case "Что по деньгам?":

                    var rnd2 = new Random();
                    var rnd3 = new Random();
                    var rnd4 = new Random();

                    string[] masToken = new string[]
                    {
                        "CAACAgIAAxkBAAEBJmZfKlnDSDXEZ_-U_ASKNPdUKDGAxAACMgAD2WCVGMw6lSLNfp7BGgQ",
                        "CAACAgIAAxkBAAEBKF5fLQMFm3iTJGrtFXHcI9sCZuuTmQACGQAD2WCVGGRCXmaP41uFGgQ",
                        "CAACAgIAAxkBAAEBKGBfLQMKiVDXUjU6u7ryw7XVPd3J_QACMwAD2WCVGC_QRyb_j3doGgQ",
                        "CAACAgIAAxkBAAEBKGJfLQMYM3brnpDxPVIVpqaBE9SiSQACMQAD2WCVGA_PKglJ-6QuGgQ"
                    };

                    string[] masAnswer = new string[]
                    {
                        "Все отлично",
                        "Вчера 300$ отдал",
                        "Прекрасно",
                        "Все на флекс поставил"
                    };

                    await bot.SendTextMessageAsync(msg.Chat.Id, $"{masAnswer[rnd2.Next(0, 4)]}");
                    await bot.SendStickerAsync(msg.Chat.Id, $"{masToken[rnd3.Next(0, 4)]}");

                    break;

                case "Пришли фото":

                    var rnd1 = new Random();

                    FileStream file1 = System.IO.File.OpenRead(@$"photo teleg\({rnd1.Next(1, 87)}).jpg");

                    InputOnlineFile myPhoto1 = new InputOnlineFile(file1, "Art.jpg");

                    await bot.SendDocumentAsync(msg.Chat.Id, myPhoto1);

                    break;
            }
        }
        #endregion


        public static async Task SendMovies(Message msg, TelegramBotClient bot)
        {
            var movies = await KinopoiskApi.GetMoviesAsync(msg.Text);

            foreach (var movie in movies.MoviesData)
            {
                var poster = new InputOnlineFile(movie.Poster.Url);

                await bot.SendPhotoAsync(msg.Chat.Id, poster, @$"<b>{movie.Name} ({movie.AlternativeName})</b>
Рейтинги 
KP: <b>{movie.Rating.Kp}</b>
Imdb: <b>{movie.Rating.Imdb}</b>
Описание:
{movie.Description}", ParseMode.Html);
            }
        }
    }
}
