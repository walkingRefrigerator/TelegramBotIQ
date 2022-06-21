using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MovieAPI
{
    public class Movies
    {
        [JsonProperty("docs")]
        public List<MovieContent> MoviesData { get; set; }
    }

    public partial class MovieContent
    {

        [JsonProperty("poster")]
        public Poster Poster { get; set; }

        [JsonProperty("rating")]
        public Rating Rating { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("year")]
        public long Year { get; set; }

        [JsonProperty("alternativeName")]
        public string AlternativeName { get; set; }
    }

    public partial class Poster
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Rating
    {
        [JsonProperty("kp")]
        public long Kp { get; set; }

        [JsonProperty("imdb")]
        public double Imdb { get; set; }
    }
}
