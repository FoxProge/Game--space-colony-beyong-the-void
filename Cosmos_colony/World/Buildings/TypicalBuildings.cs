
using Space_colony_game.UI.Menus;

namespace Space_colony_game.World.Buildings
{
    public class Farm : PoweredBuilding
    {
        private readonly Colony colony_;
        private const float BaseFoodPerDay = 12f;

        public Farm(Colony colony) { colony_ = colony; }

        public override void DrawBody(int x, int y, int w, int h)
            => DrawTextureOrFallback(Assets.Assets.TextureFarm, x, y, w, h,
                    new Raylib_cs.Color(220, 200, 50, 255)
               );

        protected override void ApplyProduction()
        {
            float weatherMod = colony_.WeatherSys.Forecast.Count > 0 &&
                               colony_.WeatherSys.Forecast[0].IsDangerous ? 0.4f : 1.0f;
            if (colony_.Food.Value <= colony_.Food.Max)
                colony_.Food.Value += BaseFoodPerDay * weatherMod * Efficiency * Scientist.GetEfficiencyBonus(0);
            colony_.Energy.Value -= PowerPerDay;
        }
    }

    public class Mine : PoweredBuilding
    {
        private readonly Colony _colony;
        private const float BaseMetalPerDay = 8f;

        public Mine(Colony colony) { _colony = colony; }

        public override void DrawBody(int x, int y, int w, int h)
            => DrawTextureOrFallback(Assets.Assets.TextureMine, x, y, w, h,
                    new Raylib_cs.Color(140, 90, 50, 255)
               );
        protected override void ApplyProduction()
        {
            if(_colony.Metal.Value <= _colony.Metal.Max)
                _colony.Metal.Value += BaseMetalPerDay * Efficiency * Scientist.GetEfficiencyBonus(1);
            _colony.Energy.Value -= PowerPerDay;
        }
    }

    public class SolarPanel : PoweredBuilding
    {
        private readonly Colony _colony;
        private const float BaseEnergyPerDay = 20f;

        public SolarPanel(Colony colony) { _colony = colony; }

        public override void DrawBody(int x, int y, int w, int h)
            => DrawTextureOrFallback(Assets.Assets.TextureSolarPanel, x, y, w, h,
                    new Raylib_cs.Color(220, 200, 50, 255)
               );

        protected override void ApplyProduction()
        {
            bool storm = _colony.WeatherSys.Forecast.Count > 0 &&
                         _colony.WeatherSys.Forecast[0].IsDangerous;
            if (!storm && _colony.Energy.Value <= _colony.Energy.Max)
                _colony.Energy.Value += BaseEnergyPerDay * Efficiency * Scientist.GetEfficiencyBonus(2);
        }
    }

    public class House : PoweredBuilding
    {
        private readonly Colony _colony;
        private bool _bonusApplied = false;
        public int Capacity { get; } = 10;

        public House(Colony colony) { _colony = colony; }

        public override void DrawBody(int x, int y, int w, int h)
            => DrawTextureOrFallback(Assets.Assets.TextureHouse, x, y, w, h,
                    new Raylib_cs.Color(220, 200, 50, 255)
               );
        protected override void ApplyProduction()
        {
            _colony.Energy.Value -= PowerPerDay;
            if (!_bonusApplied)
            {
                _colony.People.Max += Capacity;
                _bonusApplied = true;
            }
        }
    }

    public class Laboratory : PoweredBuilding
    {
        private readonly Colony _colony;
        public Laboratory(Colony colony)
        {
            _colony = colony;
            Scientist.LabBuilded += 1;
        }

        ~Laboratory()
        {
            Scientist.LabBuilded -= 1;
        }

        public override void DrawBody(int x, int y, int w, int h)
            => DrawTextureOrFallback(Assets.Assets.TextureLaboratory, x, y, w, h,
                    new Raylib_cs.Color(220, 200, 50, 255)
               );

        protected override void ApplyProduction()
        {
            _colony.Energy.Value -= PowerPerDay;
        }
    }

    public class Storage : PoweredBuilding
    {
        private readonly Colony _colony;
        private const float CapacityBonus = 100f;
        private bool _bonusApplied = false;

        public Storage(Colony colony) { _colony = colony; }

        public override void DrawBody(int x, int y, int w, int h)
            => DrawTextureOrFallback(Assets.Assets.TextureStorage, x, y, w, h,
                    new Raylib_cs.Color(220, 200, 50, 255)
               );
        protected override void ApplyProduction()
        {
            if (!_bonusApplied)
            {
                _colony.Food.Max += CapacityBonus;
                _colony.Metal.Max += CapacityBonus;
                _colony.Energy.Max += CapacityBonus;
                _bonusApplied = true;
            }
        }
    }
}
