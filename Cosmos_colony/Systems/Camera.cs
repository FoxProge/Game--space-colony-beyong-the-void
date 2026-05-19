using Raylib_cs;
using Space_colony_game.World;
using System.Numerics;

namespace Space_colony_game.Systems
{
    public class Camera
    {
        private const float ScrollSpeed = 650f;
        private const float EdgeScrollZone = 40f;
        private const float EdgeScrollMult = 0.9f;
        private const float ZoomSpeed = 0.1f;
        private const float ZoomMin = 0.4f;
        private const float ZoomMax = 2.0f;
        private const float ZoomDefault = 1.0f;

        public Vector2 Position { get; private set; } = Vector2.Zero;
        public float Zoom { get; private set; } = ZoomDefault;
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }

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

        public void Move(Vector2 delta)
            => Position = ClampPosition(Position + delta);

        public void CenterOn(Vector2 worldPos)
        {
            Position = ClampPosition(worldPos - new Vector2(
                ViewportWidth / (2f * Zoom),
                ViewportHeight / (2f * Zoom)));
        }

        public Vector2 WorldToScreen(Vector2 world)
            => (world - Position) * Zoom;

        public Vector2 ScreenToWorld(Vector2 screen)
            => screen / Zoom + Position;

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
