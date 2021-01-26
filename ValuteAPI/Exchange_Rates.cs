using Newtonsoft.Json;

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace ValuteAPI
{
    public class Exchange_Rates
    {
        public static async Task GetValut(TelegramBotClient botClient, Message msg)
        {
            string url = "https://www.cbr-xml-daily.ru/daily_json.js";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse webresponse = (HttpWebResponse)webRequest.GetResponse();

            string response;

            using (StreamReader streamReader = new StreamReader(webresponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            ExchangeResponse exchange = JsonConvert.DeserializeObject<ExchangeResponse>(response);

            await botClient.SendTextMessageAsync(msg.Chat.Id, String.Format(@$"Курс валюты:
Доллар - {exchange.Valute.USD.Value}
Евро - {exchange.Valute.EUR.Value}"));

        }

    }
}
