using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

using TMDbLib.Client;

namespace MovieAPI
{
    public class Movie
    {
        public static async Task GetMovies(TelegramBotClient bot, Message msg)
        {
            TMDbClient Client = new TMDbClient(Config.MovieToken);

            var movies = Client.GetMoviePopularListAsync(language: "ru-Ru",
                page: 1,
                region: null);


            foreach (var mov in movies.Result.Results)
            {
                var movie = Client.GetMovieAsync(movieId: mov.Id,
                    language: "ru-Ru").Result;

                var url = string.Format("https://image.tmdb.org/t/p/w600_and_h900_bestv2/{0}", movie.PosterPath);

                InputOnlineFile file = new InputOnlineFile(url);

                await bot.SendPhotoAsync(msg.Chat.Id, file, caption: @$"<b><a href='{movie.Homepage}'>{mov.Title}</a></b>
<b>Рейтинг</b>: {movie.VoteAverage}
<b>Описание</b>: {mov.Overview}
<b>Дата премьеры</b>: {movie.ReleaseDate}
", parseMode: ParseMode.Html);
            }


        }
    }
}
