using System;
using System.IO;
using System.Threading.Tasks;

using MovieAPI;

using ConvertPDF;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;

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
        public static async Task MessageReplyText(Message msg, TelegramBotClient bot, UserDB user)
        {
            var text = msg?.Text;

            if (text == null)
                return;

            switch (text)
            {
                case "/command":

                    await bot.SendTextMessageAsync(msg.Chat.Id, @"Список команд:
/command - список команд,
/Info - ссылки на разработчика,
/Keyboard - запуск клавиатуры,
/Menu - погода и курс валюты,
/Movie - афиша фильмов,
/MyFile - отправка ваших документов с облака,
/Convert - конвертирование PDF в WORD,
/Out - выход из пользователя.
");
                    break;


                case "/Convert":

                    var inlineFunc = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("PDF to Word")
                        }
                    }
                    );

                    await bot.SendTextMessageAsync(msg.Chat.Id, "Мои возможности", replyMarkup: inlineFunc);

                    break;

                case "/Movie":

                    await SendMovies(msg, bot);

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

                case "/Info":

                    var inline = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("VK", "https://vk.com/y.frolovprogramfck"),
                            InlineKeyboardButton.WithUrl("Telegram", "https://t.me/Yuryfralav")
                        }
                    }
                        );

                    await bot.SendTextMessageAsync(msg.Chat.Id, "Ссылки на моего разработчика", replyMarkup: inline);

                    break;

                case "/Keyboard":

                    var keyboaerd1 = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Пришли фото"),
                            new KeyboardButton("Мой контакт") {RequestContact = true}
                        },
                        new[]
                        {
                            new KeyboardButton("Что по деньгам?"),
                            new KeyboardButton("Геолокация") {RequestLocation = true}
                        }

                    });

                    await bot.SendTextMessageAsync(msg.Chat.Id, "Message", replyMarkup: keyboaerd1);

                    break;

                case "/MyFile":

                    var keyboaerd2 = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Пришли мои документы"),
                            new KeyboardButton("Пришли мои фото")
                        },
                        new[]
                        {
                            new KeyboardButton("Пришли мои голосовые"),
                            new KeyboardButton("Пришли мои видео")
                        },
                        new[]
                        {
                            new KeyboardButton("Пришли мои аудио")
                        }
                    });

                    await bot.SendTextMessageAsync(msg.Chat.Id, "Message", replyMarkup: keyboaerd2);

                    break;

                case "Пришли мои документы":

                    await SendOfFileInfo(msg, bot, user, MessageType.Document);

                    break;

                case "Пришли мои фото":

                    await SendOfPhoto(msg, bot, user, MessageType.Photo);

                    break;

                case "Пришли мои голосовые":

                    await SendOfVoice(msg, bot, user, MessageType.Voice);

                    break;

                case "Пришли мои видео":

                    await SendOfFileInfo(msg, bot, user, MessageType.Video);

                    break;

                case "Пришли мои аудио":

                    await SendOfFileInfo(msg, bot, user, MessageType.Audio);

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

        #region Сохранение файлов типа Docs
        public static async Task MessageReplyFile(Message msg, TelegramBotClient bot, MessageType type, UserDB user)
        {
            string file_id = "";
            string directpath = "";
            string path = "";

            switch (type)
            {
                case MessageType.Audio:
                    file_id = msg.Audio.FileId;

                    directpath = String.Format(@"TelegramBotIQ Users File\{0}\{1}", user.GetUser(), type.ToString());
                    path = String.Format(@"TelegramBotIQ Users File\{0}\{1}\{2}.mp3", user.GetUser(), type.ToString(), msg.Audio.Title);

                    break;

                case MessageType.Photo:
                    file_id = msg.Photo[0].FileId;

                    directpath = String.Format(@"TelegramBotIQ Users File\{0}\{1}", user.GetUser(), type.ToString());
                    path = String.Format(@"TelegramBotIQ Users File\{0}\{1}\{2}.png", user.GetUser(), type.ToString(), msg.Photo[0]);

                    break;

                case MessageType.Voice:
                    file_id = msg.Voice.FileId;

                    directpath = String.Format(@"TelegramBotIQ Users File\{0}\{1}", user.GetUser(), type.ToString());
                    path = String.Format(@"TelegramBotIQ Users File\{0}\{1}\{2}", user.GetUser(), type.ToString(), msg.Voice.FileId);

                    break;

                case MessageType.Video:
                    file_id = msg.Video.FileId;

                    directpath = String.Format(@"TelegramBotIQ Users File\{0}\{1}", user.GetUser(), type.ToString());
                    path = String.Format(@"TelegramBotIQ Users File\{0}\{1}\{2}.mp4", user.GetUser(), type.ToString(), msg.Video.FileId);

                    break;

                case MessageType.Document:
                    file_id = msg.Document.FileId;

                    directpath = String.Format(@"TelegramBotIQ Users File\{0}\{1}", user.GetUser(), type.ToString());
                    path = String.Format(@"TelegramBotIQ Users File\{0}\{1}\{2}", user.GetUser(), type.ToString(), msg.Document.FileName);

                    break;
            }

            

            var dir = Directory.CreateDirectory(directpath);

            try
            {
                using (var filestream = System.IO.File.OpenWrite(path))
                {
                    var filedowload = await bot.GetInfoAndDownloadFileAsync(
                        fileId: file_id,
                        destination: filestream
                        );
                }

                await bot.SendTextMessageAsync(msg.Chat.Id, "Ваш файл сохранен!");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                await bot.SendTextMessageAsync(msg.Chat.Id, "Возникла ошибка :(" + Environment.NewLine +
                    "Не беспокойтесь, разрабу дадим по шапке");
            }
        }
        #endregion

        public static async Task SendOfPhoto(Message msg, TelegramBotClient bot, UserDB user, MessageType type)
        {
            string filepath = String.Format(@"TelegramBotIQ Users File\{0}\{1}", user.GetUser(), type.ToString());

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(filepath);

                foreach (var file in directoryInfo.GetFiles())
                {
                    FileStream file1 = System.IO.File.OpenRead(@$"{file.FullName}");

                    InputOnlineFile inputOnline = new InputOnlineFile(file1, file1.Name);

                    await bot.SendDocumentAsync(msg.Chat.Id, inputOnline);

                }
            }
            catch
            {
                await bot.SendTextMessageAsync(msg.Chat.Id, "Папка с файлами пуста");
            }
        }

        public static async Task SendOfVoice(Message msg, TelegramBotClient bot, UserDB user, MessageType type)
        {
            string filepath = String.Format(@"TelegramBotIQ Users File\{0}\{1}", user.GetUser(), type.ToString());

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(filepath);

                foreach (var file in directoryInfo.GetFiles())
                {
                    FileStream file1 = System.IO.File.OpenRead(@$"{file.FullName}");

                    InputOnlineFile inputOnline = new InputOnlineFile(file1, file1.Name);

                    await bot.SendVoiceAsync(msg.Chat.Id, inputOnline);

                }
            }
            catch
            {
                await bot.SendTextMessageAsync(msg.Chat.Id, "Папка с файлами пуста");
            }
        }

        public static async Task SendOfFileInfo(Message msg, TelegramBotClient bot, UserDB user, MessageType type)
        {
            string filepath = String.Format(@"TelegramBotIQ Users File\{0}\{1}", user.GetUser(), type.ToString());

            //string[] allFoundFiles = Directory.GetFiles("C:/Windows/", "gogo.txt", SearchOption.AllDirectories);

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(filepath);

                foreach (var file in directoryInfo.GetFiles())
                {
                    //FileStream file1 = System.IO.File.OpenRead(@$"{file.FullName}");

                    //InputOnlineFile inputOnline = new InputOnlineFile(file1, file1.Name);

                    //await bot.SendDocumentAsync(msg.Chat.Id, inputOnline);

                    await bot.SendTextMessageAsync(msg.Chat.Id, file.Name);
                }

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Скачать файл"),
                            InlineKeyboardButton.WithCallbackData("Удалить файл")
                        }
                });

                await bot.SendTextMessageAsync(msg.Chat.Id, "Выберите действие", replyMarkup: inlineKeyboard);

            }
            catch
            {
                await bot.SendTextMessageAsync(msg.Chat.Id, "Папка с файлами пуста");
            }
        }

        public static async Task SendFile(Message msg, TelegramBotClient bot, UserDB user)
        {
            string[] allFoundFiles = Directory.GetFiles(@$"TelegramBotIQ Users File\{user.GetUser()}\", msg.Text, SearchOption.AllDirectories);

            foreach (var filepath in allFoundFiles)
            {
                FileStream file = System.IO.File.OpenRead(filepath);

                InputOnlineFile onlineFile = new InputOnlineFile(file, msg.Text);

                if (filepath.Contains(MessageType.Document.ToString()))
                    await bot.SendDocumentAsync(msg.Chat.Id, onlineFile);
                else if (filepath.Contains(MessageType.Audio.ToString()))
                    await bot.SendAudioAsync(msg.Chat.Id, onlineFile);
                else if (filepath.Contains(MessageType.Video.ToString()))
                    await bot.SendVideoAsync(msg.Chat.Id, onlineFile);

                return;
            }

            await bot.SendTextMessageAsync(msg.Chat.Id, "Введено неверное имя файла");
        }

        public static async Task DeleteFile(Message msg, TelegramBotClient bot, UserDB user)
        {
            string[] allFoundFiles = Directory.GetFiles(@$"TelegramBotIQ Users File\{user.GetUser()}\", msg.Text, SearchOption.AllDirectories);

            foreach (var filepath in allFoundFiles)
            {   
                System.IO.File.Delete(filepath);

                await bot.SendTextMessageAsync(msg.Chat.Id, "Файл удален!");

                return;
            }

            await bot.SendTextMessageAsync(msg.Chat.Id, "Введено неверное имя файла");
        }

        public static async Task SendMovies(Message msg, TelegramBotClient bot)
        {
            var movies = await KinopoiskApi.GetMoviesAsync("2022");

            foreach (var movie in movies.MoviesData)
            {
                var poster = new InputOnlineFile(movie.Poster.Url);

                await bot.SendPhotoAsync(msg.Chat.Id, poster, $"<br>" +
                    $"<b>{movie.Name} ({movie.AlternativeName})</b><br>" +
                    $"Рейтинги" +
                    $"KP:<b>{movie.Rating.Kp}</b><br>" +
                    $"Imdb:<b>{movie.Rating.Imdb}</b><br>" +
                    $"Описание:<br>" +
                    $"{movie.Description}");
            }
        }
    }
}
