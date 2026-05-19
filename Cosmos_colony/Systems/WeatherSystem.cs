using System;
using System.Collections.Generic;

namespace Space_colony_game.Systems
{
    /// <summary>
    /// Тип погоды для дня.
    /// </summary>
    public enum WeatherType { Sunny, Cloudy, Storm, Blizzard }

    /// <summary>
    /// Информация о погоде на один день: тип и температура.
    /// </summary>
    public class WeatherDay
    {
        /// <summary>Тип погоды.</summary>
        public WeatherType Type { get; init; } = WeatherType.Sunny;

        /// <summary>Температура дня (целое значение).</summary>
        public int Temp { get; init; } = 10;

        /// <summary>Признак опасной погоды (шторм или метель).</summary>
        public bool IsDangerous => Type is WeatherType.Storm or WeatherType.Blizzard;
    }

    /// <summary>
    /// Система погоды: генерирует прогноз на несколько дней и сдвигает прогноз при наступлении нового дня.
    /// </summary>
    public class WeatherSystem
    {
        private static readonly Random rand_ = new();

        /// <summary>
        /// Прогноз погоды. Индекс 0 — ближайший день.
        /// </summary>
        public List<WeatherDay> Forecast { get; private set; } = [];

        /// <summary>
        /// Создаёт систему погоды и заполняет начальный прогноз.
        /// </summary>
        public WeatherSystem() => GenerateForecast();

        /// <summary>
        /// Продвигает прогноз на один день: убирает первый элемент и добавляет новый сгенерированный день в конец.
        /// </summary>
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

        /// <summary>
        /// Случайным образом генерирует один день погоды (тип и температуру) на основе вероятностного распределения.
        /// </summary>
        /// <returns>Сформированный объект <see cref="WeatherDay"/>.</returns>
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
