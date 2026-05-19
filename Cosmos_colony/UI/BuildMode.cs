using Raylib_cs;
using Space_colony_game.Systems;
using Space_colony_game.World;
using Space_colony_game.World.Buildings;
using System.Numerics;

namespace Space_colony_game.UI
{
    /// <summary>
    /// Управляет режимом строительства: отображение "призрака" здания, проверка возможности размещения и фактическая постановка здания.
    /// </summary>
    public class BuildMode
    {
        private readonly BuildingManager manager_;
        private readonly Camera camera_;

        /// <summary>Признак активности режима постройки.</summary>
        public bool IsActive { get; private set; } = false;

        /// <summary>Тип текущего "призрака" для размещения.</summary>
        public BuildingType? GhostType { get; private set; } = null;

        private bool justActivated_ = false;

        /// <summary>Колбек, вызываемый после успешного размещения здания. Передаётся созданный объект Building.</summary>
        public Action<Building>? OnPlace { get; set; }

        private int ghostCol_ = 0;
        private int ghostRow_ = 0;

        /// <summary>Создаёт режим постройки, привязанный к менеджеру построек и камере.</summary>
        public BuildMode(BuildingManager manager, Camera camera)
        {
            manager_ = manager;
            camera_ = camera;
        }

        /// <summary>Активирует режим постройки для заданного типа здания.</summary>
        /// <param name="type">Тип здания, который будет показан в виде "призрака".</param>
        public void Activate(BuildingType type)
        {
            GhostType = type;
            IsActive = true;
            justActivated_ = true;
        }

        /// <summary>Деактивирует режим постройки.</summary>
        public void Deactivate()
        {
            IsActive = false;
            GhostType = null;
        }

        /// <summary>
        /// Обновляет состояние режима постройки: вычисляет позицию "призрака", обрабатывает отмену правой кнопкой
        /// и попытку размещения левой кнопкой мыши.
        /// </summary>
        public void Update()
        {
            if (!IsActive || GhostType == null) return;

            var mouse = Raylib.GetMousePosition();
            var worldPos = camera_.ScreenToWorld(mouse);
            ghostCol_ = (int)(worldPos.X / WorldMap.TileSize);
            ghostRow_ = (int)(worldPos.Y / WorldMap.TileSize);

            if (justActivated_)
            {
                // пропускаем первый кадр после активации, чтобы избежать мгновенного клика
                justActivated_ = false;
                return;
            }

            if (Raylib.IsMouseButtonPressed(MouseButton.Right))
            {
                Deactivate();
                return;
            }

            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                string? error = manager_.CanPlace(GhostType, ghostCol_, ghostRow_);
                if (error == null)
                {
                    var instance = manager_.Place(GhostType, ghostCol_, ghostRow_);
                    OnPlace?.Invoke(instance);
                }
            }
        }

        /// <summary>
        /// Рисует "призрак" здания в текущей позиции с индикацией возможности размещения и выводом сообщения об ошибке.
        /// </summary>
        public void Draw()
        {
            if (!IsActive || GhostType == null) return;

            string? error = manager_.CanPlace(GhostType, ghostCol_, ghostRow_);
            bool canPlace = error == null;

            var screenF = camera_.WorldToScreen(
                new Vector2(
                    ghostCol_ * WorldMap.TileSize,
                    ghostRow_ * WorldMap.TileSize
                )
            );

            float wF = GhostType.SizeX * WorldMap.TileSize * camera_.Zoom;
            float hF = GhostType.SizeY * WorldMap.TileSize * camera_.Zoom;
            float x = MathF.Round(screenF.X);
            float y = MathF.Round(screenF.Y);
            float w = MathF.Round(wF);
            float h = MathF.Round(hF);

            int ix = (int)x;
            int iy = (int)y;
            int iw = (int)w;
            int ih = (int)h;

            Color fill = canPlace ? Assets.Assets.GhostGreen : Assets.Assets.GhostRed;
            Color border = canPlace ? Assets.Assets.GhostBorderGreen : Assets.Assets.GhostBorderRed;

            Raylib.DrawRectangle(ix, iy, iw, ih, fill);

            Raylib.DrawRectangleLinesEx(
                new Rectangle(x, y, w, h), 2f, border);

            if (iw > 20)
            {
                var sz = Raylib.MeasureTextEx(
                    Assets.Assets.FontSmall, GhostType.Abbr,
                    Assets.Assets.FontSmallSize, 1);

                float tx = MathF.Round(x + (w - sz.X) * 0.5f);
                float ty = MathF.Round(y + (h - sz.Y) * 0.5f);

                Raylib.DrawTextEx(
                    Assets.Assets.FontSmall, GhostType.Abbr,
                    new Vector2(tx, ty),
                    Assets.Assets.FontSmallSize, 1, Color.White);
            }

            if (!canPlace && error != null)
            {
                float textYF = y + h + 6;

                var sz = Raylib.MeasureTextEx(
                    Assets.Assets.FontSmall, error,
                    Assets.Assets.FontSmallSize, 1
                );

                float tx = MathF.Round(x + 4);
                float ty = MathF.Round(textYF);

                float bw = MathF.Round(sz.X + 8);
                float bh = MathF.Round(sz.Y + 4);
                float bx = MathF.Round(x);
                float by = MathF.Round(textYF - 2);

                Raylib.DrawRectangleRec(
                    new Rectangle(bx, by, bw, bh),
                    new Color(0, 0, 0, 160)
                );

                Raylib.DrawTextEx(
                    Assets.Assets.FontSmall, error,
                    new Vector2(tx, ty),
                    Assets.Assets.FontSmallSize, 1,
                    new Color(255, 120, 120, 255)
                );
            }
        }
    }
}
