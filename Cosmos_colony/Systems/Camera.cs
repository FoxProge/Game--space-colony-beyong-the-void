using Raylib_cs;
using Space_colony_game.World;
using System.Numerics;

namespace Space_colony_game.Systems
{
    /// <summary>
    /// Камера, управляющая просмотром игрового мира: позиция, зум и методы преобразования координат.
    /// Поддерживает скролл клавиатурой и мышью, перемещение к позиции и ограничение внутри границ карты.
    /// </summary>
    public class Camera
    {
        private const float ScrollSpeed = 650f;
        private const float EdgeScrollZone = 40f;
        private const float EdgeScrollMult = 0.9f;
        private const float ZoomSpeed = 0.1f;
        private const float ZoomMin = 0.4f;
        private const float ZoomMax = 2.0f;
        private const float ZoomDefault = 1.0f;

        /// <summary>Позиция левого верхнего угла просмотра в мировых координатах (пиксели).</summary>
        public Vector2 Position { get; private set; } = Vector2.Zero;

        /// <summary>Текущий коэффициент масштабирования (зум).</summary>
        public float Zoom { get; private set; } = ZoomDefault;

        /// <summary>Ширина вьюпорта в пикселях (экранная ширина для камеры).</summary>
        public int ViewportWidth { get; set; }

        /// <summary>Высота вьюпорта в пикселях (экранная высота для камеры).</summary>
        public int ViewportHeight { get; set; }

        /// <summary>
        /// Создаёт камеру с заданными размерами вьюпорта и центрирует её по центру карты.
        /// </summary>
        /// <param name="viewportWidth">Ширина вьюпорта в пикселях.</param>
        /// <param name="viewportHeight">Высота вьюпорта в пикселях.</param>
        public Camera(int viewportWidth, int viewportHeight)
        {
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;

            CenterOn(
                new Vector2(
                    WorldMap.Columns * WorldMap.TileSize / 2f,
                    WorldMap.Rows * WorldMap.TileSize / 2f
                )
            );
        }

        /// <summary>
        /// Обновляет состояние камеры: обработка клавиатурного скролла, скролла по краям экрана и масштабирования колесом мыши.
        /// </summary>
        /// <remarks>
        /// При зуме камера сохраняет мировую позицию под курсором, чтобы зум выглядел «по месту».
        /// </remarks>
        public void Update()
        {
            float dt = Raylib.GetFrameTime();
            Vector2 delta = Vector2.Zero;

            if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up))
                delta.Y -= ScrollSpeed * dt;
            if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down))
                delta.Y += ScrollSpeed * dt;
            if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left))
                delta.X -= ScrollSpeed * dt;
            if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right))
                delta.X += ScrollSpeed * dt;

            Vector2 mouse = Raylib.GetMousePosition();
            float edgeSpd = ScrollSpeed * EdgeScrollMult * dt;

            if (mouse.X < EdgeScrollZone)
                delta.X -= edgeSpd * (1f - mouse.X / EdgeScrollZone);
            if (mouse.X > ViewportWidth - EdgeScrollZone)
                delta.X += edgeSpd * (1f - (ViewportWidth - mouse.X) / EdgeScrollZone);
            if (mouse.Y < EdgeScrollZone)
                delta.Y -= edgeSpd * (1f - mouse.Y / EdgeScrollZone);
            if (mouse.Y > ViewportHeight - EdgeScrollZone)
                delta.Y += edgeSpd * (1f - (ViewportHeight - mouse.Y) / EdgeScrollZone);

            float wheel = Raylib.GetMouseWheelMove();
            if (wheel != 0)
            {
                var worldBeforeZoom = ScreenToWorld(mouse);
                Zoom = Math.Clamp(Zoom + wheel * ZoomSpeed, ZoomMin, ZoomMax);
                var worldAfterZoom = ScreenToWorld(mouse);
                Position += worldBeforeZoom - worldAfterZoom;
            }

            Move(delta);
        }

        /// <summary>
        /// Обновляет камеру, учитывая только клавиатурный ввод и скролл по краям (без обработки колеса мыши).
        /// </summary>
        public void UpdateKeyboardOnly()
        {
            float dt = Raylib.GetFrameTime();
            Vector2 delta = Vector2.Zero;

            if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up))
                delta.Y -= ScrollSpeed * dt;
            if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down))
                delta.Y += ScrollSpeed * dt;
            if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left))
                delta.X -= ScrollSpeed * dt;
            if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right))
                delta.X += ScrollSpeed * dt;

            Vector2 mouse = Raylib.GetMousePosition();
            float edgeSpd = ScrollSpeed * EdgeScrollMult * dt;

            if (mouse.X < EdgeScrollZone)
                delta.X -= edgeSpd * (1f - mouse.X / EdgeScrollZone);
            if (mouse.X > ViewportWidth - EdgeScrollZone)
                delta.X += edgeSpd * (1f - (ViewportWidth - mouse.X) / EdgeScrollZone);
            if (mouse.Y < EdgeScrollZone)
                delta.Y -= edgeSpd * (1f - mouse.Y / EdgeScrollZone);
            if (mouse.Y > ViewportHeight - EdgeScrollZone)
                delta.Y += edgeSpd * (1f - (ViewportHeight - mouse.Y) / EdgeScrollZone);

            Move(delta);
        }

        /// <summary>
        /// Перемещает камеру на заданный вектор (в мировых пикселях) и ограничивает позицию границами карты.
        /// </summary>
        /// <param name="delta">Смещение в мировых пикселях.</param>
        public void Move(Vector2 delta)
            => Position = ClampPosition(Position + delta);

        /// <summary>
        /// Центрирует камеру на указанной мировой позиции.
        /// </summary>
        /// <param name="worldPos">Мировая позиция (в пикселях), на которую нужно центрироваться.</param>
        public void CenterOn(Vector2 worldPos)
        {
            Position = ClampPosition(worldPos - new Vector2(
                ViewportWidth / (2f * Zoom),
                ViewportHeight / (2f * Zoom)));
        }

        /// <summary>
        /// Преобразует мировую позицию (пиксели) в экранные координаты с учётом позиции и зума камеры.
        /// </summary>
        /// <param name="world">Мировая координата (пиксели).</param>
        /// <returns>Экранная координата в пикселях.</returns>
        public Vector2 WorldToScreen(Vector2 world)
            => (world - Position) * Zoom;

        /// <summary>
        /// Преобразует экранную позицию в мировые координаты (обратное к WorldToScreen).
        /// </summary>
        /// <param name="screen">Экранная координата (пиксели).</param>
        /// <returns>Мировая координата в пикселях.</returns>
        public Vector2 ScreenToWorld(Vector2 screen)
            => screen / Zoom + Position;

        /// <summary>
        /// Ограничивает переданную позицию так, чтобы область просмотра камеры оставалась внутри границ карты.
        /// </summary>
        /// <param name="pos">Проверяемая мировая позиция левого верхнего угла просмотра.</param>
        /// <returns>Скорректированная позиция, удовлетворяющая границам карты.</returns>
        private Vector2 ClampPosition(Vector2 pos)
        {
            float viewW = ViewportWidth / Zoom;
            float viewH = ViewportHeight / Zoom;

            float maxX = WorldMap.WorldWidth - viewW;
            float maxY = WorldMap.WorldHeight - viewH;

            if (maxX < 0)
                pos.X = maxX * 0.5f;
            else
                pos.X = Math.Clamp(pos.X, 0, maxX);

            if (maxY < 0)
                pos.Y = maxY * 0.5f;
            else
                pos.Y = Math.Clamp(pos.Y, 0, maxY);

            return pos;
        }
    }
}
