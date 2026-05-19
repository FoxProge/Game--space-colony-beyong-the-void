using Raylib_cs;
using Space_colony_game.Systems;
using System.Numerics;

namespace Space_colony_game.World
{
    /// <summary>
    /// Тип тайла карты мира.
    /// </summary>
    public enum TileType
    {
        Ground,
        Rock,
        Sand, 
        Ice,
        Crater
    }

    /// <summary>
    /// Одна клетка карты — содержит тип поверхности, индекс текстуры и флаг возможности строительства.
    /// </summary>
    public class Tile
    {
        /// <summary>Тип тайла.</summary>
        public TileType Type { get; init; }

        /// <summary>Индекс текстуры для данного типа (используется при рисовании наборов текстур).</summary>
        public int TextureId { get; init; } = 0;

        /// <summary>Возвращает true, если по этому тайлу можно строить (земля или песок).</summary>
        public bool Buildable => Type is TileType.Ground or TileType.Sand;
    }

    /// <summary>
    /// Карта мира: хранит сетку тайлов, генерирует её по seed и предоставляет методы отрисовки.
    /// </summary>
    public class WorldMap
    {
        /// <summary>Количество колонок карты (ширина в тайлах).</summary>
        public static readonly int Columns = 50;

        /// <summary>Количество строк карты (высота в тайлах).</summary>
        public static readonly int Rows = 50;

        /// <summary>Размер одного тайла в пикселях.</summary>
        public static readonly int TileSize = 64;

        /// <summary>Полная ширина мира в пикселях.</summary>
        public static int WorldWidth { get; private set; } =  Columns * TileSize;

        /// <summary>Полная высота мира в пикселях.</summary>
        public static int WorldHeight { get; private set; } =  Columns * TileSize;

        /// <summary>Матрица тайлов карты [col, row].</summary>
        public static readonly Tile[,] Tiles = new Tile[Columns, Rows];

        /// <summary>
        /// Создаёт карту мира и генерирует содержимое на основе seed.
        /// </summary>
        /// <param name="seed">Seed для генератора случайных чисел (по умолчанию 13).</param>
        public WorldMap(int seed = 13)
        {
            GenerateMap(seed);
        }

        /// <summary>
        /// Возвращает тайл по индексам колонки и строки; при выходе за границы возвращает тайл типа Rock.
        /// </summary>
        /// <param name="col">Колонка (x) в тайлах.</param>
        /// <param name="row">Строка (y) в тайлах.</param>
        /// <returns>Экземпляр <see cref="Tile"/> для запрошенной клетки.</returns>
        public Tile GetTile(int col, int row)
        {
            if (col < 0 || col >= Columns || row < 0 || row >= Rows)
                return new Tile { Type = TileType.Rock };
            return Tiles[col, row];
        }

        /// <summary>
        /// Рисует видимую часть карты, используя данные камеры для отсечения и преобразования координат.
        /// </summary>
        /// <param name="camera">Камера, задающая область просмотра и зум.</param>
        public void Draw(Camera camera)
        {
            int colFrom = Math.Max(0, (int)(camera.Position.X / TileSize));
            int colTo = Math.Min(
                Columns,
                (int)((camera.Position.X + camera.ViewportWidth / camera.Zoom) / TileSize) + 2
            );

            int rowFrom = Math.Max(0, (int)(camera.Position.Y / TileSize));
            int rowTo = Math.Min(
                Rows,
                (int)((camera.Position.Y + camera.ViewportHeight / camera.Zoom) / TileSize) + 2
            );

            for (int col = colFrom; col < colTo; col++)
            {
                for (int row = rowFrom; row < rowTo; row++)
                {
                    Tile tile = Tiles[col, row];

                    float wx = col * TileSize;
                    float wy = row * TileSize;

                    var screen = camera.WorldToScreen(new Vector2(wx, wy));

                    float fw = TileSize * camera.Zoom;
                    float fh = TileSize * camera.Zoom;

                    int left = (int)MathF.Round(screen.X);
                    int top = (int)MathF.Round(screen.Y);
                    int right = (int)MathF.Round(screen.X + fw);
                    int bottom = (int)MathF.Round(screen.Y + fh);

                    int sw = right - left;
                    int sh = bottom - top;

                    DrawTile(tile, left, top, sw, sh);
                }
            }
        }

        /// <summary>Генерирует карту по заданному seed — заполняет массив Tiles.</summary>
        /// <param name="seed">Seed генератора случайных чисел.</param>
        private void GenerateMap(int seed)
        {
            var rng = new Random(seed);

            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    int roll = rng.Next(100);
                    TileType type = roll switch
                    {
                        < 65 => TileType.Ground,
                        < 83 => TileType.Sand,
                        < 96 => TileType.Rock,
                        < 99 => TileType.Crater,
                        _ => TileType.Rock,
                    };

                    int texIdx = type switch
                    {
                        TileType.Ground => rng.Next(3),
                        TileType.Rock => rng.Next(2),
                        TileType.Sand => rng.Next(3),
                        _ => 0,
                    };

                    Tiles[col, row] = new Tile { Type = type, TextureId = texIdx };
                }
            }

            for (int col = 22; col <= 27; col++)
                for (int row = 22; row <= 27; row++)
                    Tiles[col, row] = new Tile
                    {
                        Type = TileType.Ground,
                        TextureId = new Random(col * 100 + row).Next(3)
                    };
        }

        /// <summary>Рисует один тайл на экране, используя текстуры или fallback-цвета.</summary>
        private static void DrawTile(Tile tile, int x, int y, int w, int h)
        {
            var dest = new Rectangle(x, y, w, h);

            switch (tile.Type)
            {
                case TileType.Ground:
                    DrawWithFallback(Assets.Assets.TileGround, tile.TextureId, dest, Assets.Assets.ColorGround);
                    break;
                case TileType.Rock:
                    DrawWithFallback(Assets.Assets.TileRock, tile.TextureId, dest, Assets.Assets.ColorRock);
                    break;
                case TileType.Sand:
                    DrawWithFallback(Assets.Assets.TileSand, tile.TextureId, dest, Assets.Assets.ColorSand);
                    break;
                case TileType.Crater:
                    if (Assets.Assets.TileCrater.Id != 0)
                        Raylib.DrawTexturePro(Assets.Assets.TileCrater,
                            new Rectangle(0, 0, Assets.Assets.TileCrater.Width, Assets.Assets.TileCrater.Height),
                            dest, Vector2.Zero, 0f, Color.White);
                    else
                        Raylib.DrawRectangleRec(dest, Assets.Assets.ColorCrater);
                    break;
                case TileType.Ice:
                    Raylib.DrawRectangleRec(dest, Assets.Assets.ColorIce);
                    break;
            }

            if (w >= 16)
                Raylib.DrawRectangleLinesEx(dest, 0.5f, Assets.Assets.ColorGrid);

        }

        /// <summary>
        /// Пытается отрисовать текстуру из массива textures по индексу; при отсутствии текстур используется fallback-цвет.
        /// </summary>
        private static void DrawWithFallback(
            Texture2D[] textures, int idx, Rectangle dest, Color fallback)
        {
            if (textures.Length > 0)
            {
                var tex = textures[idx % textures.Length];
                if (tex.Id != 0)
                {
                    Raylib.DrawTexturePro(tex,
                        new Rectangle(0, 0, tex.Width, tex.Height),
                        dest, Vector2.Zero, 0f, Color.White);
                    return;
                }
            }
            Raylib.DrawRectangleRec(dest, fallback);
        }

        /// <summary>
        /// Возвращает цвет для заданного типа тайла (используется при рисовании миникарты и отладке).
        /// </summary>
        /// <param name="type">Тип тайла.</param>
        public static Color TileColor(TileType type) => type switch
        {
            TileType.Ground => Assets.Assets.ColorGround,
            TileType.Rock => Assets.Assets.ColorRock,
            TileType.Sand => Assets.Assets.ColorSand,
            TileType.Ice => Assets.Assets.ColorIce,
            TileType.Crater => Assets.Assets.ColorCrater,
            _ => Assets.Assets.ColorGround,
        };
    }
}
