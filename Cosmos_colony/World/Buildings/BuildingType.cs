using Raylib_cs;

namespace Space_colony_game.World.Buildings
{
    /// <summary>
    /// Идентификаторы типов зданий, используемые для создания и различения типов строений в игре.
    /// </summary>
    public enum BuildingTypeId
    {
        MotherShip,
        Foundation,
        Farm,
        Mine,
        SolarPanel,
        Storage,
        House,
        Laboratory
    }

    /// <summary>
    /// Описание типа здания: метаданные, стоимость, размеры и визуальные параметры.
    /// </summary>
    public class BuildingType
    {
        /// <summary>Идентификатор типа здания.</summary>
        public BuildingTypeId Id { get; init; }

        /// <summary>Отображаемое имя типа здания.</summary>
        public string Name { get; init; } = "";

        /// <summary>Текстовое описание и подсказка для игрока.</summary>
        public string Description { get; init; } = "";

        /// <summary>Краткое обозначение (аббревиатура) для отображения на миникарте/иконках.</summary>
        public string Abbr { get; init; } = "";

        /// <summary>Ширина здания в тайлах.</summary>
        public int SizeX { get; init; } = 1;

        /// <summary>Высота здания в тайлах.</summary>
        public int SizeY { get; init; } = 1;

        /// <summary>Стоимость в металле для постройки.</summary>
        public int CostMetal { get; init; } = 0;

        /// <summary>Стоимость в энергии для постройки (если используется).</summary>
        public int CostEnergy { get; init; } = 0;

        /// <summary>Потребление энергии в сутки (0 — не потребляет).</summary>
        public int PowerPerDay { get; init; } = 0;

        /// <summary>Требуется ли наличие фундамента для постройки этого типа.</summary>
        public bool RequiresFoundation { get; init; } = true;

        /// <summary>Основной цвет для отображения плитки/иконки здания.</summary>
        public Color Color { get; init; } = Color.Gray;

        /// <summary>Цвет текста аббревиатуры.</summary>
        public Color AbbrevColor { get; init; } = Color.White;

        // --- Предопределённые типы зданий ---

        /// <summary>Тип «Фундамент» (используется как основание для других зданий).</summary>
        public static readonly BuildingType Foundation = new()
        {
            Id = BuildingTypeId.Foundation,
            Name = "Фундамент",
            Description = "Основа для любого здания. \nУкладывается на грунт или песок",
            Abbr = "ФНД",
            SizeX = 1,
            SizeY = 1,
            CostMetal = 5,
            RequiresFoundation = false,
            Color = new Color(100, 100, 100, 255),
            AbbrevColor = new Color(180, 180, 180, 255)
        };

        /// <summary>Тип «Ферма» — производит еду.</summary>
        public static readonly BuildingType Farm = new()
        {
            Id = BuildingTypeId.Farm,
            Name = "Ферма",
            Description = "Производит еду для колонистов.\nЭффективность падает в бури на 60%.",
            Abbr = "ФРМ",
            SizeX = 2,
            SizeY = 2,
            CostMetal = 20,
            PowerPerDay = 5,
            Color = new Color(38, 100, 42, 255),
            AbbrevColor = new Color(100, 220, 100, 255),
        };

        /// <summary>Тип «Шахта» — добывает металл.</summary>
        public static readonly BuildingType Mine = new()
        {
            Id = BuildingTypeId.Mine,
            Name = "Шахта",
            Description = "Добывает металл из грунта.\nТребует операторов-колонистов.",
            Abbr = "ШХТ",
            SizeX = 3,
            SizeY = 2,
            CostMetal = 30,
            PowerPerDay = 8,
            Color = new Color(100, 70, 40, 255),
            AbbrevColor = new Color(200, 160, 96, 255),
        };

        /// <summary>Тип «Солнечная панель» — вырабатывает энергию днем.</summary>
        public static readonly BuildingType SolarPanel = new()
        {
            Id = BuildingTypeId.SolarPanel,
            Name = "Солн. панель",
            Description = "Производит энергию днём.\nНочью и в бури не работает.",
            Abbr = "СПН",
            SizeX = 1,
            SizeY = 1,
            CostMetal = 15,
            PowerPerDay = 0,   // сама производит энергию
            Color = new Color(160, 140, 30, 255),
            AbbrevColor = new Color(240, 208, 64, 255),
        };

        /// <summary>Тип «Склад» — увеличивает ёмкость ресурсов.</summary>
        public static readonly BuildingType Storage = new()
        {
            Id = BuildingTypeId.Storage,
            Name = "Склад",
            Description = "Увеличивает максимальный запас\nвсех ресурсов. Не потребляет энергию.",
            Abbr = "СКЛ",
            SizeX = 2,
            SizeY = 1,
            CostMetal = 25,
            PowerPerDay = 0,
            Color = new Color(120, 100, 60, 255),
            AbbrevColor = new Color(200, 170, 100, 255),
        };

        /// <summary>Тип «Жильё» — увеличивает вместимость колонистов.</summary>
        public static readonly BuildingType House = new()
        {
            Id = BuildingTypeId.House,
            Name = "Жильё",
            Description = "Размещает колонистов.\nВлияет на мораль и здоровье.",
            Abbr = "ЖЛЁ",
            SizeX = 2,
            SizeY = 2,
            CostMetal = 40,
            PowerPerDay = 6,
            Color = new Color(40, 120, 140, 255),
            AbbrevColor = new Color(96, 184, 224, 255),
        };

        /// <summary>Тип «Лаборатория» — открывает улучшения.</summary>
        public static readonly BuildingType Laboratory = new()
        {
            Id = BuildingTypeId.Laboratory,
            Name = "Лаборатория",
            Description = "Открывает улучшения зданий.\nТребует учёных.",
            Abbr = "ЛБР",
            SizeX = 2,
            SizeY = 3,
            CostMetal = 50,
            PowerPerDay = 10,
            Color = new Color(100, 40, 140, 255),
            AbbrevColor = new Color(180, 100, 220, 255),
        };

        /// <summary>Тип «Главный корабль» — стартовая база игрока.</summary>
        public static readonly BuildingType MotherShip = new()
        {
            Id = BuildingTypeId.MotherShip,
            Name = "Главный корабль",
            Description = "Стартовая база колонии. \nОткрывает доступ ко всем специалистам",
            Abbr = "МКР",
            SizeX = 4,
            SizeY = 4,
            CostMetal = 0,
            RequiresFoundation = false,
            Color = new Color(30, 60, 120, 255),
            AbbrevColor = new Color(100, 180, 255, 255),
        };

        /// <summary>Алиас для типа MotherShip.</summary>
        public static readonly BuildingType MotherShipType = MotherShip;

        /// <summary>Массив всех доступных типов для перечисления в интерфейсах.</summary>
        public static readonly BuildingType[] All =
        {
            Foundation, Farm, Mine, SolarPanel, Storage, House, Laboratory
        };
    }
}
