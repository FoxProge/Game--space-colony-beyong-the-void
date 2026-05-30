using Raylib_cs;

namespace Space_colony_game.Assets
{
    /// <summary>
    /// Глобальный менеджер ассетов игры.
    /// Отвечает за загрузку и выгрузку всех графических ресурсов и шрифтов.
    /// РЕализован как статический доступ (singleton-паттерн)
    /// </summary>
    public static class Assets
    {
        /// <summary>Малый UI шрифт.</summary>
        public static Font FontSmall { get; private set; }
        /// <summary>Средний UI шрифт.</summary>
        public static Font FontMedium { get; private set; }
        /// <summary>Крупный UI шрифт.</summary>
        public static Font FontLarge { get; private set; }

        public static int FontSmallSize = 16;
        public static int FontMediumSize = 24;
        public static int FontLargeSize = 64;

        public static Texture2D FoodIcon { get; private set; }
        public static Texture2D MetalIcon { get; private set; }
        public static Texture2D EnergyIcon { get; private set; }
        public static Texture2D PeopleIcon { get; private set; }
        public static Texture2D[] TileGround { get; private set; } = [];
        public static Texture2D[] TileRock { get; private set; } = [];
        public static Texture2D[] TileSand { get; private set; } = [];
        public static Texture2D TileCrater { get; private set; }
        public static Texture2D? TextureFoundation { get; private set; }
        public static Texture2D? TextureFarm { get; private set; }
        public static Texture2D? TextureMine { get; private set; }
        public static Texture2D? TextureSolarPanel { get; private set; }
        public static Texture2D? TextureStorage { get; private set; }
        public static Texture2D? TextureHouse { get; private set; }
        public static Texture2D? TextureLaboratory { get; private set; }
        public static Texture2D? TextureSpaceShip { get; private set; }
        public static Texture2D? PlanetMars { get; private set; }
        public static Texture2D? PlanetTitan { get; private set; }
        public static Texture2D? BackgroundScreenImage { get; private set; }

        public static readonly Color BackgroundScreenColor = new(15, 15, 20, 255);
        public static readonly Color DefaultBorderColor = new(70, 80, 100, 255);
        public static readonly Color DefaultButtonColor = new(35, 35, 45, 255);
        public static readonly Color DefaultButtonHoveredColor = new(55, 110, 195, 255);
        public static readonly Color DefaultButtonBorderColor = Color.Gray;
        public static readonly Color DefaultButtonHoveredBorderColor = Color.White;
        public static readonly Color DefaultButtonLabelColor = Color.LightGray;
        public static readonly Color DefaultButtonHoveredLabelColor = Color.White;
        public static readonly Color ExitButtonHoveredColor = new(180, 50, 50, 255);
        public static readonly Color NewDayButtonBackgroundColor = new(22, 48, 82, 255);
        public static readonly Color NewDayButtonHoveredColor = new(30, 62, 108, 255);
        public static readonly Color NewDayButtonBorderColor = new(44, 82, 130, 255);
        public static readonly Color NewDayButtonHoveredBorderColor = new(44, 82, 130, 255);
        public static readonly Color NewDayButtonLabelColor = new(120, 176, 220, 255);
        public static readonly Color NewDayButtonHoveredLabelColor = new(170, 214, 248, 255);
        public static readonly Color NightColor = new(8, 12, 30, 255);
        public static readonly Color ColorGround = new(140, 115, 70, 255);
        public static readonly Color ColorRock = new(80, 75, 70, 255);
        public static readonly Color ColorSand = new(140, 150, 70, 255);
        public static readonly Color ColorIce = new(160, 200, 220, 255);
        public static readonly Color ColorCrater = new(50, 40, 40, 255);
        public static readonly Color ColorGrid = new(0, 0, 0, 30);
        public static readonly Color MiniMapBorderColor = new(44, 66, 96, 255);
        public static readonly Color MiniMapViewportColor = new(255, 255, 255, 80);
        public static readonly Color MiniMapViewportBorder = new(255, 255, 255, 180);
        public static readonly Color MiniMapBackgroundColor = new(10, 16, 24, 220);
        public static readonly Color GhostGreen = new(60, 220, 80, 160);
        public static readonly Color GhostRed = new(220, 60, 60, 160);
        public static readonly Color GhostBorderGreen = new(60, 220, 80, 255);
        public static readonly Color GhostBorderRed = new(220, 60, 60, 255);
        public static readonly Color AlertMsgBox = new(10, 16, 24, 180);
        public static readonly Color AlertMsgBoxBorder = new(30, 45, 61, 255);
        public static readonly Color AlertBuildingRuinedMsg = new(220, 80, 80, 255);
        public static readonly Color AlertBuildingDamagedMsg = new(180, 160, 100, 255);
        public static readonly Color AlertBuildingWithoutEnergyMsg = new(220, 180, 30, 255);
        public static readonly Color ContextMenuBgColor = new(10, 16, 24, 245);
        public static readonly Color ContextMenuBorderColor = new(44, 66, 96, 255);
        public static readonly Color ContextMenuHoverColor = new(25, 50, 90, 255);
        public static readonly Color ContextMenuTextColor = new(200, 212, 224, 255);
        public static readonly Color ContextMenuTextMuted = new(100, 120, 140, 255);
        public static readonly Color ContextMenuDisabled = new(60, 70, 80, 255);
        public static readonly Color ContextMenuRepairColor = new(60, 184, 122, 255);
        public static readonly Color ContextMenuDestroyColor = new(220, 80, 80, 255);
        public static readonly Color EngineerColor = new (40, 100, 190, 255);
        public static readonly Color ScientistColor = new (40, 140, 80, 255);
        public static readonly Color LogistColor = new (190, 145, 35, 255);
        public static readonly Color SelectionColor = new(60, 220, 80, 200);

        private static bool loaded_ = false;

        /// <summary>
        /// Загружает базовые ресурсы игры (UI, иконки, шрифты).
        /// Должен вызываться один раз при запуске приложения.
        /// </summary>
        public static void Load()
        {
            if (loaded_) return;
            LoadFonts();
            BackgroundScreenImage = LoadTextureSafe("Assets/Sprites/BG_image.png");
            PlanetMars = LoadTextureSafe("Assets/Sprites/Planets/mars.png");
            PlanetTitan = LoadTextureSafe("Assets/Sprites/Planets/titan.png");
            FoodIcon = LoadTextureSafe("Assets/Sprites/food_icon.png");
            MetalIcon = LoadTextureSafe("Assets/Sprites/metal_icon.png");
            EnergyIcon = LoadTextureSafe("Assets/Sprites/energy_icon.png");
            PeopleIcon = LoadTextureSafe("Assets/Sprites/people_icon.png");
            loaded_ = true;
        }

        /// <summary>
        /// Полностью выгружает все загруженные ресурсы (текстуры и шрифты).
        /// Используется при завершении игры или смене сцены.
        /// </summary>
        public static void Unload()
        {
            if (!loaded_) return;
            Raylib.UnloadFont(FontSmall);
            Raylib.UnloadFont(FontMedium);
            Raylib.UnloadFont(FontLarge);

            UnloadTextureArray(TileGround);
            UnloadTextureArray(TileRock);
            UnloadTextureArray(TileSand);
            UnloadTextureSafe(TileCrater);
            UnloadTextureSafe(TextureFoundation);
            UnloadTextureSafe(TextureFarm);
            UnloadTextureSafe(TextureMine);
            UnloadTextureSafe(TextureSolarPanel);
            UnloadTextureSafe(TextureStorage);
            UnloadTextureSafe(TextureHouse);
            UnloadTextureSafe(TextureLaboratory);
            UnloadTextureSafe(TextureSpaceShip);
            UnloadTextureSafe(PlanetMars);
            UnloadTextureSafe(PlanetTitan);
            UnloadTextureSafe(BackgroundScreenImage);
            UnloadTextureSafe(FoodIcon);
            UnloadTextureSafe(MetalIcon);
            UnloadTextureSafe(EnergyIcon);
            UnloadTextureSafe(PeopleIcon);
            loaded_ = false;
            Console.WriteLine("\t[Assets]: All unloaded\n");
        }

        /// <summary>
        /// Загружает текстуры мира в зависимости от уровня планеты.
        /// </summary>
        /// <param name="level">
        /// Индекс планеты:
        /// 0 = Mars
        /// 1 = Titan
        /// </param>
        public static void LoadTextures(int level)
        {
            TextureFoundation = LoadTextureSafe("Assets/Sprites/Buildings/foundation.png");
            if (level == 0)
            {
                TileGround = LoadTextureArray(
                    "Assets/Sprites/Tiles/mars_ground1.png",
                    "Assets/Sprites/Tiles/mars_ground2.png",
                    "Assets/Sprites/Tiles/mars_ground3.png");
                TileRock = LoadTextureArray(
                    "Assets/Sprites/Tiles/mars_rock1.png",
                    "Assets/Sprites/Tiles/mars_rock2.png");
                TileSand = LoadTextureArray(
                    "Assets/Sprites/Tiles/mars_sand1.png",
                    "Assets/Sprites/Tiles/mars_sand2.png",
                    "Assets/Sprites/Tiles/mars_sand3.png");
                TileCrater = LoadTextureSafe("Assets/Sprites/Tiles/mars_crater.png");
                TextureFarm = LoadTextureSafe("Assets/Sprites/Buildings/mars_farm.png");
                TextureMine = LoadTextureSafe("Assets/Sprites/Buildings/mars_mine.png");
                TextureSolarPanel = LoadTextureSafe("Assets/Sprites/Buildings/mars_solar_panel.png");
                TextureStorage = LoadTextureSafe("Assets/Sprites/Buildings/mars_storage.png");
                TextureHouse = LoadTextureSafe("Assets/Sprites/Buildings/mars_house.png");
                TextureLaboratory = LoadTextureSafe("Assets/Sprites/Buildings/mars_laboratory.png");
                TextureSpaceShip = LoadTextureSafe("Assets/Sprites/Buildings/mars_space_ship.png");
            }
            else if (level == 1)
            {
                TileGround = LoadTextureArray(
                    "Assets/Sprites/Tiles/titan_ground1.png",
                    "Assets/Sprites/Tiles/titan_ground2.png",
                    "Assets/Sprites/Tiles/titan_ground3.png");
                TileRock = LoadTextureArray(
                    "Assets/Sprites/Tiles/titan_rock1.png",
                    "Assets/Sprites/Tiles/titan_rock2.png");
                TileSand = LoadTextureArray(
                    "Assets/Sprites/Tiles/titan_sand1.png",
                    "Assets/Sprites/Tiles/titan_sand2.png",
                    "Assets/Sprites/Tiles/titan_sand3.png");
                TileCrater = LoadTextureSafe("Assets/Sprites/Tiles/titan_crater.png");
                TextureFarm = LoadTextureSafe("Assets/Sprites/Buildings/titan_farm.png");
                TextureMine = LoadTextureSafe("Assets/Sprites/Buildings/titan_mine.png");
                TextureSolarPanel = LoadTextureSafe("Assets/Sprites/Buildings/titan_solar_panel.png");
                TextureStorage = LoadTextureSafe("Assets/Sprites/Buildings/titan_storage.png");
                TextureHouse = LoadTextureSafe("Assets/Sprites/Buildings/titan_house.png");
                TextureLaboratory = LoadTextureSafe("Assets/Sprites/Buildings/titan_laboratory.png");
                TextureSpaceShip = LoadTextureSafe("Assets/Sprites/Buildings/titan_space_ship.png");
            }
        }

        /// <summary>
        /// Выгрузкавсех текстур, загруженных в методе LoadTextures
        /// </summary>
        public static void UnloadTextures()
        {
            if(TileGround.Length > 0)
                UnloadTextureArray(TileGround);
            if (TileRock.Length > 0)
                UnloadTextureArray(TileRock);
            if(TileSand.Length > 0)
                UnloadTextureArray(TileSand);
            UnloadTextureSafe(TileCrater);
            UnloadTextureSafe(TextureFoundation);
            UnloadTextureSafe(TextureFarm);
            UnloadTextureSafe(TextureMine);
            UnloadTextureSafe(TextureSolarPanel);
            UnloadTextureSafe(TextureStorage);
            UnloadTextureSafe(TextureHouse);
            UnloadTextureSafe(TextureLaboratory);
            UnloadTextureSafe(TextureSpaceShip);
            Console.WriteLine("\t[Assets]: Unload all textures\n");
        }

        /// <summary>
        /// Загружает массив текстур из списка путей.
        /// </summary>
        /// <param name="paths">Список путей к текстурам.</param>
        /// <returns>Массив загруженных текстур.</returns>
        private static Texture2D[] LoadTextureArray(params string[] paths)
        {
            List<Texture2D> textures = new List<Texture2D>();

            foreach(string path in paths)
            {
                Texture2D texture = LoadTextureSafe(path);
                textures.Add(texture);
            }
            return textures.ToArray();
        }

        /// <summary>
        /// Освобождает массив текстур.
        /// </summary>
        /// <param name="array">Массив текстур для выгрузки.</param>
        private static void UnloadTextureArray(Texture2D[] array)
        {
            foreach (Texture2D texture in array)
                UnloadTextureSafe(texture);
        }

        /// <summary>
        /// Безопасная загрузка текстуры из файла.
        /// Если файл отсутствует — возвращается default Texture2D (Id = 0).
        /// </summary>
        /// <param name="path">Путь к файлу текстуры.</param>
        /// <returns>Загруженная текстура или пустая (Id = 0).</returns>
        private static Texture2D LoadTextureSafe(string path)
        {
            if(!File.Exists(path))
            {
                Console.WriteLine($"\t[Assets] Не найдена: {Path.GetFullPath(path)}\n");
                return default; // Id == 0 — сигнал что текстуры нет
            }

            Texture2D texture = Raylib.LoadTexture(path);
            Raylib.SetTextureFilter(texture, TextureFilter.Point);
            Console.WriteLine($"\t[Assets]: Текстура загружена: {path}\n");
            return texture;
        }

        /// <summary>
        /// Безопасно выгружает текстуру, если она существует и валидна.
        /// </summary>
        /// <param name="texture">Текстура для освобождения.</param>
        private static void UnloadTextureSafe(Texture2D? texture)
        {
            if (texture.HasValue && texture.Value.Id != 0)
                Raylib.UnloadTexture(texture.Value);
        }

        /// <summary>
        /// Загружает шрифты с поддержкой кириллицы и дополнительных Unicode символов.
        /// </summary>
        private static void LoadFonts()
        {
            int[] codepoints = BuildCodepoints(
                32, 126, 1040, 1103,
                //       ё      Ё    ↑      ↓     →     ←     —    °
                extra: [1025, 1105, 8593, 8595, 8594, 8592, 8212, 176]
            );

            const string path = "Assets/Fonts/Roboto-Regular.ttf";
            FontSmall = LoadCyrillicSymbols(path, FontSmallSize, codepoints);
            FontMedium = LoadCyrillicSymbols(path, FontMediumSize, codepoints);
            FontLarge = LoadCyrillicSymbols(path, FontLargeSize, codepoints);
        }

        /// <summary>
        /// Загружает шрифт с заданным размером и набором codepoints.
        /// </summary>
        /// <param name="path">Путь к TTF файлу.</param>
        /// <param name="size">Размер шрифта.</param>
        /// <param name="codepoints">Unicode символы для загрузки.</param>
        /// <returns>Загруженный шрифт или default font при ошибке.</returns>
        private static Font LoadCyrillicSymbols(string path, int size, int[] codepoints)
        {
            string fullPath = Path.GetFullPath(path);

            Console.WriteLine($"\t[Assets]: Загружаю шрифт {size}px: {fullPath}\n");

            if (!File.Exists(path))
            {
                Console.WriteLine($"\t[Assets] Не найден: {fullPath}\n");
                return Raylib.GetFontDefault();
            }

            Font font = Raylib.LoadFontEx(path, size, codepoints, codepoints.Length);

            if(font.GlyphCount == 0)
            {
                Console.WriteLine($"\t[Assets]: LoadFontEx вернул пустой шрифт для размера {size}px.\n");
                return Raylib.GetFontDefault();
            }

            Raylib.SetTextureFilter(font.Texture, TextureFilter.Bilinear);
            Console.WriteLine($"\t[Assets]: OK — глифов: {font.GlyphCount}\n");
            return font;
        }

        /// <summary>
        /// Формирует массив Unicode codepoints из диапазонов и дополнительных символов.
        /// </summary>
        /// <param name="from1">Начало первого диапазона.</param>
        /// <param name="to1">Конец первого диапазона.</param>
        /// <param name="from2">Начало второго диапазона.</param>
        /// <param name="to2">Конец второго диапазона.</param>
        /// <param name="extra">Дополнительные символы.</param>
        /// <returns>Массив Unicode кодов.</returns>
        private static int[] BuildCodepoints(
            int from1, int to1,
            int from2, int to2,
            int[]? extra = null)
        {
            List<int> list = new List<int>();

            for(int i = from1; i <= to1; i++)
                list.Add(i);
            for(int i = from2; i <= to2; i++)
                list.Add(i);
            if(extra != null)
                foreach (int cp in extra)
                    list.Add(cp);
            return list.ToArray();
        }
    }
}
