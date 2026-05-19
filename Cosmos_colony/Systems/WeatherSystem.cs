
namespace Space_colony_game.Systems
{
    public enum WeatherType { Sunny, Cloudy, Storm, Blizzard }
    public class WeatherDay
    {
        public WeatherType Type { get; init; } = WeatherType.Sunny;
        public int Temp { get; init; } = 10;
        public bool IsDangerous => Type is WeatherType.Storm or WeatherType.Blizzard;
    }

    public class WeatherSystem
    {
        private static readonly Random rand_ = new();
        public List<WeatherDay> Forecast { get; private set; } = [];

        public WeatherSystem() => GenerateForecast();

        public void AdvanceDay()
        {
            if(Forecast.Count > 0) Forecast.RemoveAt(0);
            Forecast.Add(GenerateDay());
        }

        private void GenerateForecast()
        {
            Forecast.Clear();
            for (int i = 0; i < 7; i++) Forecast.Add(GenerateDay());
        }

        private static WeatherDay GenerateDay()
        {
            int roll = rand_.Next(100);
            var type = roll switch
            {
                < 50 => WeatherType.Sunny,
                < 80 => WeatherType.Cloudy,
                < 95 => WeatherType.Storm,
                _ => WeatherType.Blizzard
            };
            int temp = type switch
            {
                WeatherType.Sunny => rand_.Next(5, 22),
                WeatherType.Cloudy => rand_.Next(-2, 12),
                WeatherType.Storm => rand_.Next(-8, 5),
                WeatherType.Blizzard => rand_.Next(-20, -5),
                _ => 0
            };
            return new WeatherDay { Type = type, Temp = temp };
        }
    }
}
