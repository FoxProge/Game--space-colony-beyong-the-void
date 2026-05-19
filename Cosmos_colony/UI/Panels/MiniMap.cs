using Raylib_cs;
using Space_colony_game.World;
using Space_colony_game.Systems;
using System.Numerics;

namespace Space_colony_game.UI.Panels
{
    /// <summary>
    /// Мини-карта — отображает уменьшенную карту мира, здания и текущую видимую область камеры.
    /// Поддерживает перетаскивание кликом по мини-карте для навигации камеры.
    /// </summary>
    public class MiniMap : Panel
    {
        /// <inheritdoc/>
        public override Rectangle Body { get; set; }

        /// <inheritdoc/>
        public override Color BackgroundColor { get; set; }

        /// <inheritdoc/>
        public override Color BorderColor { get; set; }

        /// <inheritdoc/>
        public override float BorderThickness { get; set; }

        /// <summary>Цвет заливки прямоугольника видимой области камеры на мини-карте.</summary>
        public Color ViewportColor { get; set; }

        /// <summary>Цвет рамки прямоугольника видимой области камеры на мини-карте.</summary>
        public Color ViewportBorder { get; set; }

        private readonly Camera camera_;

        /// <summary>Опциональная ссылка на менеджер построек — используется для отрисовки зданий на мини-карте.</summary>
        public BuildingManager? Buildings { get; set; }

        private bool isDragging_ = false;

        /// <summary>Создаёт мини-карту с указанными размерами и привязкой к камере.</summary>
        public MiniMap(float x, float y, float width, float height, Camera camera)
        {
            camera_ = camera;
            Body = new(x, y, width, height);
            ViewportColor = Assets.Assets.MiniMapViewportColor;
            ViewportBorder = Assets.Assets.MiniMapViewportBorder;
            BackgroundColor = Assets.Assets.MiniMapBackgroundColor;
            BorderColor = Assets.Assets.MiniMapBorderColor;
            BorderThickness = 3f;
        }

        /// <summary>
        /// Обновляет состояние мини-карты — обрабатывает клики и перетаскивание для перемещения камеры.
        /// Должен вызываться каждый кадр перед отрисовкой.
        /// </summary>
        public void Update()
        {
            Vector2 mouse = Raylib.GetMousePosition();

            if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                Raylib.CheckCollisionPointRec(mouse, Body))
            {
                isDragging_ = true;
                NavigateTo(mouse);
            }

            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
                isDragging_ = false;

            if (isDragging_ && Raylib.IsMouseButtonDown(MouseButton.Left))
                NavigateTo(mouse);
        }

        /// <summary>Рисует мини-карту: фон, тайлы, здания и прямоугольник области просмотра.</summary>
        public override void Draw()
        {
            Raylib.DrawRectangleRec(Body, BackgroundColor);
            DrawMiniTile();
            DrawBuildingsMini();
            DrawViewportRect();
            Raylib.DrawRectangleLinesEx(Body, BorderThickness, BorderColor);
        }

        /// <summary>Рисует представление зданий на мини-карте, если установлен <see cref="Buildings"/>.</summary>
        private void DrawBuildingsMini()
        {
            if (Buildings == null) return;


            float scaleX = Body.Width / WorldMap.WorldWidth;
            float scaleY = Body.Height / WorldMap.WorldHeight;

            foreach (var b in Buildings.Buildings)
            {
                float fx = Body.X + b.Col * WorldMap.TileSize * scaleX;
                float fy = Body.Y + b.Row * WorldMap.TileSize * scaleY;
                float fw = Math.Max(2f, b.SizeX * WorldMap.TileSize * scaleX);
                float fh = Math.Max(2f, b.SizeY * WorldMap.TileSize * scaleY);

                int left = (int)MathF.Round(fx);
                int top = (int)MathF.Round(fy);
                int right = (int)MathF.Round(fx + fw);
                int bottom = (int)MathF.Round(fy + fh);

                Raylib.DrawRectangleRec(
                    new Rectangle(
                        left,
                        top,
                        right - left,
                        bottom - top),
                    b.Type.Color with { A = 220 }
                );
            }
        }

        /// <summary>Рисует прямоугольник, соответствующий текущему просмотру камеры.</summary>
        private void DrawViewportRect()
        {
            float scaleX = Body.Width / WorldMap.WorldWidth;
            float scaleY = Body.Height / WorldMap.WorldHeight;

            float viewW = camera_.ViewportWidth / camera_.Zoom;
            float viewH = camera_.ViewportHeight / camera_.Zoom;

            viewW = MathF.Min(viewW, WorldMap.WorldWidth);
            viewH = MathF.Min(viewH, WorldMap.WorldHeight);

            float fx = Body.X + camera_.Position.X * scaleX;
            float fy = Body.Y + camera_.Position.Y * scaleY;
            float fw = viewW * scaleX;
            float fh = viewH * scaleY;

            fx = MathF.Max(Body.X, fx);
            fy = MathF.Max(Body.Y, fy);

            fw = MathF.Min(fw, Body.Width);
            fh = MathF.Min(fh, Body.Height);

            int left = (int)MathF.Round(fx);
            int top = (int)MathF.Round(fy);
            int right = (int)MathF.Round(fx + fw);
            int bottom = (int)MathF.Round(fy + fh);

            var vr = new Rectangle(
                left,
                top,
                right - left,
                bottom - top
            );

            Raylib.DrawRectangleRec(vr, ViewportColor);
            Raylib.DrawRectangleLinesEx(vr, 1f, ViewportBorder);
        }

        /// <summary>Рисует миниатюрное представление тайлов карты внутри мини-карты.</summary>
        private void DrawMiniTile()
        {
            float cellW = Body.Width / WorldMap.Columns;
            float cellH = Body.Height / WorldMap.Rows;

            for (int col = 0; col < WorldMap.Columns; col++)
            {
                for (int row = 0; row < WorldMap.Rows; row++)
                {
                    Tile tile = WorldMap.Tiles[col, row];
                    var color = WorldMap.TileColor(tile.Type);

                    Raylib.DrawRectangleRec(
                        new Rectangle(
                            Body.X + col * cellW,
                            Body.Y + row * cellH,
                            cellW + 0.5f,   // +0.5 убирает зазоры между клетками
                            cellH + 0.5f),
                        color);
                }
            }
        }

        /// <summary>
        /// Переводит координату экрана в нормализованные координаты мини-карты и центрирует камеру на соответствующую мировую позицию.
        /// </summary>
        private void NavigateTo(Vector2 screenPoint)
        {
            float nx = (screenPoint.X - Body.X) / Body.Width;
            float ny = (screenPoint.Y - Body.Y) / Body.Height;

            nx = Math.Clamp(nx, 0, 1);
            ny = Math.Clamp(ny, 0, 1);

            var worldPos = new Vector2(
                nx * WorldMap.WorldWidth,
                ny * WorldMap.WorldHeight);

            camera_.CenterOn(worldPos);
        }
    }
}
