using Raylib_cs;
using Space_colony_game.World;
using Space_colony_game.World.Buildings;
using System.Numerics;

namespace Space_colony_game.Systems
{
    /// <summary>
    /// Менеджер выделения объектов на карте.
    /// Обрабатывает клики мыши — выбор зданий левой кнопкой и контекстные правые клики по кораблю/зданиям.
    /// Предоставляет событие для обработки правого клика по MotherShip и по другим зданиям.
    /// </summary>
    public class SelectionManager
    {
        private readonly BuildingManager buildings_;
        private readonly Camera camera_;
        private float selTimer_ = 0f;

        /// <summary>
        /// Текущее выбранное здание или <c>null</c>, если ничего не выбрано.
        /// </summary>
        public Building? Selected { get; private set; } = null;

        /// <summary>
        /// Вызывается при правом клике по MotherShip; передаются экранные координаты клика.
        /// </summary>
        public Action<Vector2>? OnMotherShipRightClick { get; set; }

        /// <summary>
        /// Вызывается при правом клике по зданию (не по фундаменту); передаются объект здания и экранные координаты клика.
        /// </summary>
        public Action<Building, Vector2>? OnBuildingRightClick { get; set; }

        /// <summary>
        /// Снимает выделение с текущего объекта.
        /// </summary>
        public void Deselect() => Selected = null;

        /// <summary>
        /// Создаёт менеджер выделения.
        /// </summary>
        /// <param name="buildings">Менеджер построек (источник объектов для выбора).</param>
        /// <param name="camera">Камера для преобразования координат экрана/мира.</param>
        public SelectionManager(BuildingManager buildings, Camera camera)
        {
            buildings_ = buildings;
            camera_ = camera;
        }

        /// <summary>
        /// Обрабатывает ввод мыши:
        /// - при нажатии ЛКМ выполняет выбор здания под курсором (устанавливает <see cref="Selected"/> и запускает анимацию подсветки);
        /// - при нажатии ПКМ вызывает соответствующее событие (<see cref="OnMotherShipRightClick"/> или <see cref="OnBuildingRightClick"/>).
        /// </summary>
        public void Update()
        {
            var mouse = Raylib.GetMousePosition();

            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                var worldPos = camera_.ScreenToWorld(mouse);
                var building = buildings_.GetAt(worldPos.X, worldPos.Y);

                if (building != null)
                {
                    Selected = building;
                    selTimer_ = 5.5f;
                }
                else
                {
                    Selected = null;
                    selTimer_ = 0f;
                }
            }

            if (Raylib.IsMouseButtonPressed(MouseButton.Right))
            {
                var worldPos = camera_.ScreenToWorld(mouse);
                var building = buildings_.GetAt(worldPos.X, worldPos.Y);

                if (building is MotherShip)
                    OnMotherShipRightClick?.Invoke(mouse);
                else if (building != null && building.Type.Id != BuildingTypeId.Foundation)
                    OnBuildingRightClick?.Invoke(building, mouse);
            }

            if (Selected != null)
                selTimer_ += Raylib.GetFrameTime();
        }

        /// <summary>
        /// Рисует подсветку выбранного здания: рамку и угловые маркеры с пульсацией.
        /// Ничего не делает, если <see cref="Selected"/> равен <c>null</c>.
        /// </summary>
        public void Draw()
        {
            if (Selected == null) return;

            var screen = camera_.WorldToScreen(new Vector2(
                Selected.Col * WorldMap.TileSize,
                Selected.Row * WorldMap.TileSize));

            float sw = Selected.SizeX * WorldMap.TileSize * camera_.Zoom;
            float sh = Selected.SizeY * WorldMap.TileSize * camera_.Zoom;

            float pulse = (float)(Math.Sin(selTimer_ * 3.0) * 0.5 + 0.5);
            byte alpha = (byte)(150 + pulse * 105);
            var color = Assets.Assets.SelectionColor with { A = alpha };

            var rect = new Rectangle(screen.X - 2, screen.Y - 2, sw + 2, sh + 2);
            Raylib.DrawRectangleLinesEx(rect, 2f, color);

            int cornerLen = (int)(Math.Min(sw, sh) * 0.2f);
            cornerLen = Math.Max(6, Math.Min(cornerLen, 16));
            int left = (int)MathF.Round(rect.X);
            int top = (int)MathF.Round(rect.Y);
            int right = (int)MathF.Round(rect.X + rect.Width);
            int bottom = (int)MathF.Round(rect.Y + rect.Height);


            DrawCorner(left - 5, top - 5, cornerLen, 1, 1, color);
            DrawCorner(right + 5, top - 5, cornerLen, -1, 1, color);
            DrawCorner(left - 5, bottom + 5, cornerLen, 1, -1, color);
            DrawCorner(right + 5, bottom + 5, cornerLen, -1, -1, color);
        }

        /// <summary>
        /// Рисует угловой маркер — две линии, исходящие из точки.
        /// </summary>
        /// <param name="x">X-координата опорной точки в пикселях.</param>
        /// <param name="y">Y-координата опорной точки в пикселях.</param>
        /// <param name="len">Длина каждой линии маркера в пикселях.</param>
        /// <param name="dx">Направление по X (1 или -1).</param>
        /// <param name="dy">Направление по Y (1 или -1).</param>
        /// <param name="color">Цвет линий.</param>
        private static void DrawCorner(int x, int y, int len, int dx, int dy, Color color)
        {
            Raylib.DrawLineEx(new Vector2(x, y), new Vector2(x + dx * len, y), 2, color);
            Raylib.DrawLineEx(new Vector2(x, y), new Vector2(x, y + dy * len), 2, color);
        }
    }
}
