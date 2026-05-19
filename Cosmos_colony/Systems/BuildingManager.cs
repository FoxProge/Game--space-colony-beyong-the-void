using Raylib_cs;
using Space_colony_game.World;
using Space_colony_game.World.Buildings;
using System.Numerics;

namespace Space_colony_game.Systems
{
    /// <summary>
    /// Менеджер построек: хранит и управляет всеми зданиями на карте,
    /// проверяет возможность размещения, создаёт экземпляры зданий и выполняет дневную обработку.
    /// </summary>
    public class BuildingManager
    {
        private readonly WorldMap map_;
        private readonly Colony colony_;
        private readonly List<Building> buildings_ = [];
        private int nextId_ = 1;

        /// <summary>Список всех построек (только для чтения).</summary>
        public IReadOnlyList<Building> Buildings => buildings_;

        /// <summary>Текущее выбранное здание (если есть).</summary>
        public Building? Selected { get; private set; } = null;

        /// <summary>Создаёт менеджер построек, привязанный к карте и колонии.</summary>
        /// <param name="map">Карта мира, используемая для проверок размещения.</param>
        /// <param name="colony">Объект колонии, для списания ресурсов и передачи событий.</param>
        public BuildingManager(WorldMap map, Colony colony)
        {
            map_ = map;
            colony_ = colony;
        }

        /// <summary>Создаёт и размещает основной корабль (MotherShip) в центре карты.</summary>
        public MotherShip SpawnMotherShip()
        {
            int col = WorldMap.Columns / 2 - MotherShip.Size / 2;
            int row = WorldMap.Rows / 2 - MotherShip.Size / 2;
            var ship = MotherShip.Create(col, row, BuildingType.MotherShipType);
            buildings_.Add(ship);
            return ship;
        }

        private static readonly Random rand_ = new();

        /// <summary>Обрабатывает один игровой день: питание населения, подачу энергии и производство зданий.</summary>
        public void ProcessDay()
        {
            var weather = colony_.WeatherSys.Forecast.Count > 0
                ? colony_.WeatherSys.Forecast[0].Type
                : WeatherType.Sunny;

            ApplyWeatherDamage(weather);

            float totalPowerNeeded = buildings_
                .OfType<PoweredBuilding>()
                .Where(b => !b.IsBroken)
                .Sum(b => b.PowerPerDay);

            colony_.Energy.CriticalLevel = totalPowerNeeded;
            bool energyAvailable = colony_.Energy.Value > totalPowerNeeded;

            if (colony_.Food.Value > 0)
            {
                colony_.Food.Value -= colony_.People.Value * 0.2f;
                colony_.PeopleStarving = 0;
            }
            else
            {
                colony_.Food.Value = 0;
                if (colony_.PeopleStarving < 3)
                    colony_.PeopleStarving += 1;
            }

            if (colony_.PeopleStarving == 3) colony_.People.Value -= 1;

            foreach (var b in buildings_.OfType<PoweredBuilding>())
                b.HasPower = energyAvailable || b.PowerPerDay == 0;

            foreach (var b in buildings_)
                b.OnDayPassed();
        }

        private void ApplyWeatherDamage(WeatherType weather)
        {
            if (weather is WeatherType.Sunny or WeatherType.Cloudy) return;

            foreach (var b in buildings_.OfType<PoweredBuilding>())
            {
                b.ApplyWeatherDamage(weather, rand_);
            }
        }

        /// <summary>
        /// Проверяет, можно ли разместить здание указанного типа в заданной клетке.
        /// Возвращает текст ошибки или null при успешной проверке.
        /// </summary>
        /// <param name="type">Тип здания для размещения.</param>
        /// <param name="col">Колонка карты (x) в тайлах.</param>
        /// <param name="row">Строка карты (y) в тайлах.</param>
        public string? CanPlace(BuildingType type, int col, int row)
        {
            if (col < 0 || row < 0 || col + type.SizeX > WorldMap.Columns || row + type.SizeY > WorldMap.Rows)
                return "За границей карты";

            for (int c = col; c < col + type.SizeX; c++)
            {
                for (int r = row; r < row + type.SizeY; r++)
                {
                    Tile tile = map_.GetTile(c, r);

                    if (!type.RequiresFoundation && !tile.Buildable)
                        return "Нельзя строить на скале или льду";

                    if (type.RequiresFoundation && !HasFoundationAt(c, r))
                        return "Нет фундамента";
                }
            }

            if (IsOccupied(col, row, type.SizeX, type.SizeY))
                return "Место занято";
            if (colony_.Metal.Value < type.CostMetal)
                return $"Недостаточно ресурсов: металл {type.CostMetal}";
            return null;
        }

        /// <summary>Размещает здание указанного типа, списывает стоимость металла и возвращает созданный объект.</summary>
        /// <param name="type">Тип строения.</param>
        /// <param name="col">Колонка карты для размещения.</param>
        /// <param name="row">Строка карты для размещения.</param>
        public Building Place(BuildingType type, int col, int row)
        {
            colony_.Metal.Value -= type.CostMetal;

            Building building = type.Id switch
            {
                BuildingTypeId.Foundation => new Foundation
                { Type = type, Col = col, Row = row, Id = nextId_++ },
                BuildingTypeId.Farm => new Farm(colony_)
                { Type = type, Col = col, Row = row, Id = nextId_++ },
                BuildingTypeId.Mine => new Mine(colony_)
                { Type = type, Col = col, Row = row, Id = nextId_++ },
                BuildingTypeId.SolarPanel => new SolarPanel(colony_)
                { Type = type, Col = col, Row = row, Id = nextId_++ },
                BuildingTypeId.Storage => new Storage(colony_)
                { Type = type, Col = col, Row = row, Id = nextId_++ },
                BuildingTypeId.House => new House(colony_)
                { Type = type, Col = col, Row = row, Id = nextId_++ },
                BuildingTypeId.Laboratory => new Laboratory(colony_)
                { Type = type, Col = col, Row = row, Id = nextId_++ },
                _ => throw new ArgumentException($"Неизвестный тип здания: {type.Id}")
            };

            buildings_.Add(building);
            return building;
        }

        /// <summary>Пытается удалить здание по идентификатору. Возвращает true при успешном удалении.</summary>
        public bool TryDestroy(int buildingId)
        {
            Building? building = buildings_.FirstOrDefault(b => b.Id == buildingId);
            if (building == null) return false;

            if (!building.CanBeDestroyed) return false;

            buildings_.Remove(building);
            return true;
        }

        /// <summary>Возвращает здание, расположенное по мировым координатам (в пикселях), либо null.</summary>
        public Building? GetAt(float worldX, float worldY)
        {
            int col = (int)(worldX / WorldMap.TileSize);
            int row = (int)(worldY / WorldMap.TileSize);

            foreach (var b in buildings_)
                if (b.Type.Id != BuildingTypeId.Foundation &&
                    col >= b.Col && col < b.Col + b.SizeX &&
                    row >= b.Row && row < b.Row + b.SizeY)
                    return b;

            foreach (var b in buildings_)
                if (b.Type.Id == BuildingTypeId.Foundation &&
                    col >= b.Col && col < b.Col + b.SizeX &&
                    row >= b.Row && row < b.Row + b.SizeY)
                    return b;


            return null;
        }

        /// <summary>Обрабатывает клик по мировым координатам: выбирает здание или возвращает найденный объект.</summary>
        /// <param name="worldPos">Мировая позиция клика (в пикселях).</param>
        /// <param name="rightClick">Признак правого клика (не приводит к выделению при true).</param>
        public Building? HandleClick(Vector2 worldPos, bool rightClick)
        {
            int col = (int)(worldPos.X / WorldMap.TileSize);
            int row = (int)(worldPos.Y / WorldMap.TileSize);

            foreach (var b in buildings_)
            {
                if (col >= b.Col && col < b.Col + b.SizeX &&
                    row >= b.Row && row < b.Row + b.SizeY)
                {
                    if (!rightClick)
                        Selected = (Selected == b) ? null : b;
                    return b;
                }
            }

            if (!rightClick) Selected = null;
            return null;
        }

        /// <summary>Рисует все здания и рамку выделенного объекта (если есть).</summary>
        public void Draw(Camera camera)
        {
            foreach (var b in buildings_)
                b.Draw(camera);

            if (Selected != null)
                DrawSelectionBorder(Selected, camera);
        }

        private static void DrawSelectionBorder(Building b, Camera camera)
        {
            float wx = b.Col * WorldMap.TileSize;
            float wy = b.Row * WorldMap.TileSize;

            Vector2 screen = camera.WorldToScreen(new Vector2(wx, wy));

            float fw = b.SizeX * WorldMap.TileSize * camera.Zoom;
            float fh = b.SizeY * WorldMap.TileSize * camera.Zoom;

            int left = (int)MathF.Round(screen.X);
            int top = (int)MathF.Round(screen.Y);
            int right = (int)MathF.Round(screen.X + fw);
            int bottom = (int)MathF.Round(screen.Y + fh);

            int w = right - left;
            int h = bottom - top;

            var rectOuter = new Rectangle(
                left - 2,
                top - 2,
                w + 4,
                h + 4
            );

            var rectInner = new Rectangle(
                left - 4,
                top - 4,
                w + 8,
                h + 8
            );

            Raylib.DrawRectangleLinesEx(
                rectOuter,
                2f,
                new Color(60, 220, 80, 255)
            );

            Raylib.DrawRectangleLinesEx(
                rectInner,
                1f,
                new Color(60, 220, 80, 120)
            );
        }

        /// <summary>Проверяет наличие фундамента в указанной клетке.</summary>
        public bool HasFoundationAt(int c, int r)
        {
            foreach (var b in buildings_)
            {
                if (b.Type.Id != BuildingTypeId.Foundation) continue;
                if (c >= b.Col && c < b.Col + b.SizeX &&
                    r >= b.Row && r < b.Row + b.SizeY)
                    return true;
            }
            return false;
        }

        private bool IsOccupied(int col, int row, int w, int h)
        {
            for (int c = col; c < col + w; c++)
                for (int r = row; r < row + h; r++)
                    foreach (var b in buildings_)
                    {
                        if (b.Type.Id == BuildingTypeId.Foundation) continue;
                        if (c >= b.Col && c < b.Col + b.SizeX &&
                            r >= b.Row && r < b.Row + b.SizeY)
                            return true;
                    }
            return false;
        }
    }
}
