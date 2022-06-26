using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieAPI
{
    public static class KinopoiskApi
    {
        private static readonly string token = "WSFK9JT-K42MJN0-M95MQAV-A4WPKNP";

        public static async Task<Movies> GetMoviesAsync(string year)
        {
            var movies = new Movies();

            try
            {
                var url = @$"https://api.kinopoisk.dev/movie?token={token}&field=rating.kp&search=7-10&field=year&search={year}";

                var client = new HttpClient();
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();


                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        {
                            movies = JsonConvert.DeserializeObject<Movies>(responseBody);

                            break;
                        }
                }
            }
            catch(Exception ex) { }

            return movies;
        }
    }
}
