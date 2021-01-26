using Newtonsoft.Json;

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeatherAPI
{
    public class Weather
    {
        public static async Task GetWeather(string cityname, TelegramBotClient bot, Message msg)
        {
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&lang=ru&appid=eea02a41d94bdf373c33d3442935f55e", msg.Text);

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            string response;

            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

            await bot.SendTextMessageAsync(msg.Chat.Id, String.Format(@$"Погода в {weatherResponse.Name}
Температура: {weatherResponse.Main.Temp} °С
Осадки: {weatherResponse.Weather[0].Description}"));

        }
    }
}
