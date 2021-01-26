namespace WeatherAPI
{
    public class WeatherResponse
    {
        public WeatherPhenomenon[] Weather { get; set; }
        public TemperatureInfo Main { get; set; }
        public string Name { get; set; }
    }
}
