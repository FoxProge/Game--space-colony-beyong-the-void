
using Space_colony_game.Systems;

namespace Space_colony_game.World.Buildings
{
    public abstract class PoweredBuilding : Building
    {
        public int PowerPerDay => Type.PowerPerDay;
        public bool HasPower { get; set; } = true;
        public float Efficiency => Health switch
        {
            0 => 0f,
            <= 50 => 0.5f,
            _ => 1.0f
        };
        public bool IsBroken => Health <= 0;
        public override void OnDayPassed()
        {
            if (IsBroken || !HasPower) return;
            ApplyProduction();
        }
        protected abstract void ApplyProduction();
        public void Repair(Colony colony)
        {
            if (Health >= 100) return;

            int missing = 100 - Health;
            int repairAmt = Math.Min(missing, 10);
            int metalCost = 5;

            if (colony.Metal.Value < metalCost) return;

            colony.Metal.Value -= metalCost;
            Health = Math.Min(100, Health + repairAmt);
        }

        public void ApplyWeatherDamage(WeatherType weather, Random rng)
        {
            bool isSolar = this is SolarPanel;

            switch (weather)
            {
                case WeatherType.Storm:
                    // 25% шанс урона
                    if (rng.Next(100) < 25)
                    {
                        int dmg = isSolar
                            ? rng.Next(20, 31)   // 20-30 для солнечной панели
                            : rng.Next(5, 11);  // 5-10 для остальных
                        Health = Math.Max(0, Health - dmg);
                    }
                    break;

                case WeatherType.Blizzard:
                    // 50% шанс урона
                    if (rng.Next(100) < 50)
                    {
                        int dmg = isSolar
                            ? rng.Next(40, 51)   // 40-50 для солнечной панели
                            : rng.Next(20, 31);  // 20-30 для остальных
                        Health = Math.Max(0, Health - dmg);
                    }
                    break;
            }
        }
    }
}
