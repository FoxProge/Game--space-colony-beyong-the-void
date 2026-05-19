using Space_colony_game.Systems;
using Space_colony_game.World.Buildings;

namespace Space_colony_game.World
{
    /// <summary>
    /// Представляет состояние колонии: ресурсы, дни, система погоды и связь с менеджером построек.
    /// Содержит логику перехода дня, обработки голода и генерации уведомлений для UI.
    /// </summary>
    public class Colony
    {
        /// <summary>Ресурс еды.</summary>
        public Resource Food { get; } = new() { Name = "Еда", Value = 120f, CriticalLevel = 20f, Max = 200};

        /// <summary>Ресурс металла.</summary>
        public Resource Metal { get; } = new() { Name = "Металл", Value = 65f, CriticalLevel = 10f, Max = 200 };

        /// <summary>Ресурс энергии.</summary>
        public Resource Energy { get; } = new() { Name = "Энергия", Value = 80f, CriticalLevel = 10f, Max = 400};

        /// <summary>Количество колонистов (как ресурс).</summary>
        public Resource People { get; } = new() { Name = "Колонисты", Value = 50f, CriticalLevel = 10f, Max = 100 };

        /// <summary>
        /// Счётчик последовательных дней голода (используется для расчёта смертности).
        /// </summary>
        public int PeopleStarving { get; set; } = 0;

        /// <summary>Текущий день в игре (начиная с 1).</summary>
        public int Day { get; private set; } = 1;

        /// <summary>Система погоды колонии (прогноз и состояния).</summary>
        public WeatherSystem WeatherSys { get; } = new WeatherSystem();

        /// <summary>Менеджер построек, привязанный к этой колонии (может быть null до инициализации уровня).</summary>
        public BuildingManager? Buildings { get; set; }

        /// <summary>
        /// Выполняет переход на следующий день: увеличивает счётчик дней, обновляет прогноз погоды,
        /// вызывает дневную обработку менеджера построек и обновляет дельты ресурсов.
        /// </summary>
        public void ProcessDay()
        {
            Day++;
            WeatherSys.AdvanceDay();

            if(Buildings != null)
            {
                SavePrevValues();
                Buildings.ProcessDay();
                ProcessStarvation();
                UpdateDeltas();
            }
        }

        /// <summary>
        /// Применяет правила голодания: если еды нет, увеличивает счётчик голода и при накоплении штрафов убавляет колонистов.
        /// </summary>
        private void ProcessStarvation()
        {
            if(Food.Value > 0)
            {
                PeopleStarving = 0;
                return;
            }

            PeopleStarving++;

            if (PeopleStarving <= 3)
                return;

            int starvingDays = PeopleStarving - 3;

            int deaths = starvingDays * starvingDays;
            deaths = Math.Min(deaths, (int)People.Value);

            People.Value -= deaths;
        }

        /// <summary>
        /// Возвращает список строковых уведомлений/alert'ов для отображения в UI,
        /// основанных на текущих значениях ресурсов и состоянии построек.
        /// </summary>
        public List<string> GetAlerts()
        {
            List<string> alerts = new List<string>();

            if (Food.IsCritical) alerts.Add("Еда на исходе!");
            if (Metal.IsCritical) alerts.Add("Металл на исходе!");
            if (Energy.IsCritical) alerts.Add("Энергия на исходе!");
            if (People.IsCritical) alerts.Add("Колонисты убывают!");
            if (PeopleStarving == 3) alerts.Add("Люди голодают!");

            if (Buildings == null) return alerts;

            bool hasBroken = Buildings.Buildings.Any(
                b => b is PoweredBuilding pb && pb.IsBroken
            );
            bool hasNoPower = Buildings.Buildings.Any(
                b => b is PoweredBuilding pb && !pb.IsBroken && !pb.HasPower && pb.PowerPerDay > 0
            );
            bool hasDamaged = Buildings.Buildings.Any(
                b => b is PoweredBuilding pb && pb.Health < 100 && !pb.IsBroken
            );

            if (hasBroken) alerts.Add("Есть разрушенные здания!");
            if (hasNoPower) alerts.Add("Зданиям не хватает энергии!");
            if (hasDamaged && !hasBroken) alerts.Add("Есть повреждённые здания");

            return alerts;
        }

        private float prevFood_, prevMetal_, prevEnergy_, prevPeople_;

        /// <summary>
        /// Сохраняет текущие значения ресурсов во временные поля для последующего расчёта дельт.
        /// </summary>
        private void SavePrevValues()
        {
            prevFood_ = Food.Value;
            prevMetal_ = Metal.Value;
            prevEnergy_ = Energy.Value;
            prevPeople_ = People.Value;
        }

        /// <summary>
        /// Вычисляет дельты (изменения) ресурсов после обработки дня и записывает их в соответствующие поля ресурсов.
        /// </summary>
        private void UpdateDeltas()
        {
            Food.Delta = Food.Value - prevFood_;
            Metal.Delta = Metal.Value - prevMetal_;
            Energy.Delta = Energy.Value - prevEnergy_;
            People.Delta = People.Value - prevPeople_;
        }
    }
}
