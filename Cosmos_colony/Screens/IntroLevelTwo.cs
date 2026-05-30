using Raylib_cs;
using System.Numerics;
using Space_colony_game.Core;


namespace Space_colony_game.Screens
{
    /// <summary>
    /// Вступительный экран второго уровня (Титан).
    /// Показывает сюжетную завязку поверх фонового изображения.
    /// Переход к уровню — ЛКМ / ENTER / ПРОБЕЛ.
    /// </summary>
    public class IntroLevelTwo : IScreen
    {
        /// <summary>Менеджер экранов для перехода на уровень.</summary>
        private readonly ScreenManager screenManager_;

        /// <summary>Накопленное время для управления мерцанием подсказки.</summary>
        private float blinkTimer_ = 0f;

        /// <summary>Флаг видимости мерцающей подсказки в текущий момент.</summary>
        private bool blinkVisible_ = true;

        /// <summary>Интервал переключения видимости подсказки в секундах.</summary>
        private const float BlinkInterval = 0.55f;

        /// <summary>Заголовок сюжетного экрана.</summary>
        private const string StoryTitle = "Титан. Миссия Мировая засуха";

        /// <summary>Подзаголовок с номером уровня, отображается под заголовком.</summary>
        private const string StorySubtitle = "— Уровень 2 —";

        /// <summary>Строки сюжетного текста. Пустая строка — межпараграфный отступ.</summary>
        private static readonly string[] StoryLines =
        {
            "После успешной марсианской миссии Земная Федерация",
            "решает продолжить колонизацию Солнечной системы.",
            "",
            "Было основано несколько колоний на разных спутниках,",
            "но возникла новая проблема: нехватка воды.",
            "",
            "Поэтому вас, как человека с успешным опытом",
            "колонизации, отправляют на планету с суровым",
            "климатом — на Титан.",
            "",
            "Цель этой миссии — создать базу-центр добычи и",
            "транспортировки воды в другиие колонии, удалённые от",
            "родной планеты.",
            "",
            "И снова от вас зависит жизнь всего человечества.",
        };

        /// <summary>Горизонтальный отступ панели от края экрана, масштабируемый.</summary>
        private static int PadX => Game.ScaleInt(130);

        /// <summary>Вертикальный отступ панели от края экрана, масштабируемый.</summary>
        private static int PadY => Game.ScaleInt(60);

        private static readonly Color PanelBg = new(10, 14, 22, 190);
        private static readonly Color PanelBorder = new(50, 90, 140, 155);
        private static readonly Color AccentTitle = new(80, 195, 255, 255);   // ледяной голубой
        private static readonly Color AccentSub = new(160, 130, 80, 210);   // тёплый янтарь
        private static readonly Color TextMain = new(200, 212, 228, 235);
        private static readonly Color TextHint = new(80, 210, 255, 190);
        private static readonly Color CornerColor = new(60, 160, 220, 200);
        private static readonly Color DividerTop = new(60, 160, 220, 180);
        private static readonly Color DividerBot = new(60, 160, 220, 80);

        /// <summary>
        /// Создаёт вступительный экран второго уровня.
        /// </summary>
        /// <param name="screenManager">Менеджер экранов, используемый для перехода на уровень.</param>
        public IntroLevelTwo(ScreenManager screenManager)
        {
            screenManager_ = screenManager;
        }

        public void OnEnter() { }
        public void OnExit() { }

        /// <summary>
        /// Обновляет состояние экрана: управляет мерцанием подсказки и обрабатывает ввод
        /// пользователя. При нажатии ЛКМ / ENTER / ПРОБЕЛ выполняет переход на второй уровень.
        /// </summary>
        public void Update()
        {
            blinkTimer_ += Raylib.GetFrameTime();
            if (blinkTimer_ >= BlinkInterval)
            {
                blinkTimer_ = 0f;
                blinkVisible_ = !blinkVisible_;
            }

            bool advance =
                Raylib.IsMouseButtonPressed(MouseButton.Left) ||
                Raylib.IsKeyPressed(KeyboardKey.Enter) ||
                Raylib.IsKeyPressed(KeyboardKey.Space);

            if (advance)
                screenManager_.GoTo(new LevelTwo(screenManager_, 2));
        }

        /// <summary>Рисует экран: фоновое изображение и сюжетное содержимое.</summary>
        public void Draw()
        {
            DrawBgImage();
            DrawContent();
        }

        /// <summary>
        /// Рисует фоновое изображение экрана, если оно загружено в <see cref="Assets.Assets.BackgroundScreenImage"/>.
        /// Тонирование холоднее, чем в главном меню — передаёт атмосферу Титана.
        /// </summary>
        public void DrawBgImage()
        {
            if (Assets.Assets.BackgroundScreenImage == null) return;

            Raylib.DrawTexturePro(
                Assets.Assets.BackgroundScreenImage.Value,
                new Rectangle(
                    0, 0,
                    Assets.Assets.BackgroundScreenImage.Value.Width,
                    Assets.Assets.BackgroundScreenImage.Value.Height),
                new Rectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight),
                Vector2.Zero, 0f,
                new Color(90, 95, 110, 255));
        }

        /// <summary>
        /// Рисует содержимое экрана: панель, заголовок, подзаголовок, разделители,
        /// строки сюжетного текста и мерцающую подсказку о запуске уровня.
        /// </summary>
        private void DrawContent()
        {
            float panelX = PadX;
            float panelY = PadY;
            float panelW = Game.ScreenWidth - PadX * 2f;
            float panelH = Game.ScreenHeight - PadY * 2f;

            Raylib.DrawRectangle(
                (int)panelX, (int)panelY, (int)panelW, (int)panelH, PanelBg);
            Raylib.DrawRectangleLinesEx(
                new Rectangle(panelX, panelY, panelW, panelH), 1.5f, PanelBorder);

            int cs = Game.ScaleInt(18);
            DrawCorner(panelX, panelY, cs, CornerColor, false, false);
            DrawCorner(panelX + panelW, panelY, cs, CornerColor, true, false);
            DrawCorner(panelX, panelY + panelH, cs, CornerColor, false, true);
            DrawCorner(panelX + panelW, panelY + panelH, cs, CornerColor, true, true);

            float titleY = panelY + Game.ScaleInt(22);
            DrawCentered(Assets.Assets.FontLarge, StoryTitle, 48, titleY, AccentTitle);

            float subY = titleY + 56;
            DrawCentered(Assets.Assets.FontSmall, StorySubtitle,
                Assets.Assets.FontSmallSize, subY, AccentSub);

            float divY = panelY + Game.ScaleInt(92);
            DrawDivider(panelX, divY, panelW, DividerTop);

            float lineH = Assets.Assets.FontMediumSize + Game.ScaleInt(8);
            float textY = divY + Game.ScaleInt(26);
            float textX = panelX + Game.ScaleInt(60);

            foreach (string line in StoryLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    textY += lineH * 0.4f;
                    continue;
                }
                Raylib.DrawTextEx(
                    Assets.Assets.FontMedium, line,
                    new Vector2((int)MathF.Round(textX), (int)MathF.Round(textY)),
                    Assets.Assets.FontMediumSize, 1, TextMain);
                textY += lineH;
            }

            float botDivY = panelY + panelH - Game.ScaleInt(56);
            DrawDivider(panelX, botDivY, panelW, DividerBot);

            if (blinkVisible_)
                DrawCentered(Assets.Assets.FontSmall,
                    "[ ЛКМ / ENTER / ПРОБЕЛ — начать уровень 2 ]",
                    Assets.Assets.FontSmallSize,
                    panelY + panelH - Game.ScaleInt(36), TextHint);
        }

        /// <summary>
        /// Рисует световой уголок рамки в виде двух коротких линий под прямым углом.
        /// </summary>
        /// <param name="cx">Угловая точка X.</param>
        /// <param name="cy">Угловая точка Y.</param>
        /// <param name="size">Длина каждой линии уголка в пикселях.</param>
        /// <param name="color">Цвет уголка.</param>
        /// <param name="flipX">Если <c>true</c> — уголок направлен влево (правая сторона панели).</param>
        /// <param name="flipY">Если <c>true</c> — уголок направлен вверх (нижняя сторона панели).</param>
        private static void DrawCorner(
            float cx, float cy, int size, Color color,
            bool flipX, bool flipY)
        {
            float dx = flipX ? -size : size;
            float dy = flipY ? -size : size;

            Raylib.DrawLineEx(
                new Vector2(cx, cy + dy), new Vector2(cx, cy), 2f, color);
            Raylib.DrawLineEx(
                new Vector2(cx, cy), new Vector2(cx + dx, cy), 2f, color);
        }

        /// <summary>
        /// Рисует горизонтальный разделитель внутри панели с отступами от краёв.
        /// </summary>
        /// <param name="panelX">Позиция X левого края панели.</param>
        /// <param name="y">Позиция Y разделителя.</param>
        /// <param name="panelW">Ширина панели.</param>
        /// <param name="color">Цвет линии разделителя.</param>
        private static void DrawDivider(float panelX, float y, float panelW, Color color)
        {
            Raylib.DrawLineEx(
                new Vector2(panelX + Game.ScaleInt(30), y),
                new Vector2(panelX + panelW - Game.ScaleInt(30), y),
                1.5f, color);
        }

        /// <summary>
        /// Рисует текст по горизонтальному центру экрана на заданной высоте.
        /// </summary>
        /// <param name="font">Шрифт для отрисовки.</param>
        /// <param name="text">Строка текста.</param>
        /// <param name="size">Размер шрифта.</param>
        /// <param name="y">Позиция Y текста.</param>
        /// <param name="color">Цвет текста.</param>
        private static void DrawCentered(
            Font font, string text, float size, float y, Color color)
        {
            Vector2 measured = Raylib.MeasureTextEx(font, text, size, 1);
            float x = (Game.ScreenWidth - measured.X) / 2f;
            Raylib.DrawTextEx(font, text,
                new Vector2((int)MathF.Round(x), (int)MathF.Round(y)),
                size, 1, color);
        }
    }

}
