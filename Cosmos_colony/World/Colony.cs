using Space_colony_game.Systems;
using Space_colony_game.World.Buildings;

namespace Space_colony_game.World
{
    public class Colony
    {
        public Resource Food { get; } = new() { Name = "Еда", Value = 120f, CriticalLevel = 20f, Max = 200};
        public Resource Metal { get; } = new() { Name = "Металл", Value = 65f, CriticalLevel = 10f, Max = 200 };
        public Resource Energy { get; } = new() { Name = "Энергия", Value = 80f, CriticalLevel = 10f, Max = 400};
        public Resource People { get; } = new() { Name = "Колонисты", Value = 50f, CriticalLevel = 10f, Max = 100 };
        public int PeopleStarving { get; set; } = 0;
        public int Day { get; private set; } = 1;
        public WeatherSystem WeatherSys { get; } = new WeatherSystem();
        public BuildingManager? Buildings { get; set; }
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
        private void SavePrevValues()
        {
            prevFood_ = Food.Value;
            prevMetal_ = Metal.Value;
            prevEnergy_ = Energy.Value;
            prevPeople_ = People.Value;
        }

        private void UpdateDeltas()
        {
            Food.Delta = Food.Value - prevFood_;
            Metal.Delta = Metal.Value - prevMetal_;
            Energy.Delta = Energy.Value - prevEnergy_;
            People.Delta = People.Value - prevPeople_;
        }
    }
}
