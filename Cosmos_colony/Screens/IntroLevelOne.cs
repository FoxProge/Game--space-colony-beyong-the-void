using Raylib_cs;
using System.Numerics;
using Space_colony_game.Core;


namespace Space_colony_game.Screens
{
    /// <summary>
    /// Вступительный экран первого уровня (Марс).
    /// Фаза 1 — сюжетный текст поверх фонового изображения.
    /// Фаза 2 — инструкция по управлению и особенностям игры.
    /// Переход между фазами и запуск уровня — ЛКМ / ENTER / ПРОБЕЛ.
    /// </summary>
    public class IntroLevelOne : IScreen
    {
        /// <summary>Менеджер экранов для перехода на уровень.</summary>
        private readonly ScreenManager screenManager_;

        /// <summary>Перечисление фаз экрана: сюжет и инструкция по управлению.</summary>
        private enum Phase { Story, HowToPlay }

        /// <summary>Текущая активная фаза экрана.</summary>
        private Phase phase_ = Phase.Story;

        /// <summary>Накопленное время для управления мерцанием подсказки.</summary>
        private float blinkTimer_ = 0f;

        /// <summary>Флаг видимости мерцающей подсказки в текущий момент.</summary>
        private bool blinkVisible_ = true;

        /// <summary>Интервал переключения видимости подсказки в секундах.</summary>
        private const float BlinkInterval = 0.55f;

        /// <summary>Заголовок сюжетного экрана.</summary>
        private const string StoryTitle = "Марсианская колония";

        /// <summary>Строки сюжетного текста. Пустая строка — межпараграфный отступ.</summary>
        private static readonly string[] StoryLines =
        {
            "2126 год. Земля переживает кризис невиданного масштаба:",
            "высокая плотность населения, истощение ресурсов,",
            "экологические проблемы.",
            "",
            "Правительство Земной Федерации решает создать колонию",
            "на Марсе, чтобы расширить жизненное пространство для",
            "людей и наладить добычу ресурсов.",
            "",
            "Вас назначают главой колониальной миссии — вы должны",
            "заложить фундамент первого внеземного города.",
            "",
            "Не подведите человечество!",
        };

        /// <summary>Заголовок экрана инструкции по управлению.</summary>
        private const string HowToTitle = "Как играть";

        /// <summary>
        /// Таблица управления: пары (название клавиши / описание действия).
        /// Отображается на экране инструкции в виде двух колонок.
        /// </summary>
        private static readonly (string Key, string Desc)[] Controls =
        {
            ("W / A / S / D", "перемещение камеры по карте"),
            ("Стрелки ↑ ↓ ← →", "альтернативное перемещение камеры"),
            ("Мышь к краю экрана", "плавное смещение камеры в ту сторону"),
            ("Колесо мыши", "приближение / отдаление"),
            ("ЛКМ", "выбор объекта или взаимодействие"),
            ("ПКМ", "контекстное меню здания"),
            ("Backspace", "пауза / продолжить"),
            ("ENTER на паузе", "выйти в главное меню"),
            ("ESC", "Выйти из игры"),
        };

        /// <summary>
        /// Строки блока «Особенности игры» под таблицей управления.
        /// Пустая строка — межстрочный отступ.
        /// </summary>
        private static readonly string[] HowToNotes =
        {
            "Особенности игры:",
            "",
            "  —  Учёный доступен только после постройки Лаборатории.",
            "  —  Здания потребляют энергию — следите за балансом.",
            "  —  Каждый новый день расходует запасы еды колонистов.",
            "  —  Погода влияет на производительность зданий.",
            "  —  Цели уровня видны в панели справа.",
        };

        /// <summary>Горизонтальный отступ панели от края экрана, масштабируемый.</summary>
        private static int PadX => Game.ScaleInt(130);

        /// <summary>Вертикальный отступ панели от края экрана, масштабируемый.</summary>
        private static int PadY => Game.ScaleInt(60);

        private static readonly Color PanelBg = new(8, 12, 28, 185);
        private static readonly Color PanelBorder = new(55, 85, 150, 160);
        private static readonly Color AccentStory = new(210, 175, 70, 255);   // золото
        private static readonly Color AccentHowTo = new(70, 180, 255, 255);   // голубой
        private static readonly Color TextMain = new(200, 212, 228, 235);
        private static readonly Color TextKey = new(255, 205, 55, 240);
        private static readonly Color TextNote = new(160, 210, 160, 220);
        private static readonly Color TextHint = new(120, 170, 230, 195);
        private static readonly Color CornerColor = new(90, 140, 240, 190);
        private static readonly Color DividerStory = new(170, 130, 50, 180);
        private static readonly Color DividerHowTo = new(50, 140, 210, 180);

        /// <summary>
        /// Создаёт вступительный экран первого уровня.
        /// </summary>
        /// <param name="screenManager">Менеджер экранов, используемый для перехода на уровень.</param>
        public IntroLevelOne(ScreenManager screenManager)
        {
            screenManager_ = screenManager;
        }

        public void OnEnter() { }
        public void OnExit() { }

        /// <summary>
        /// Обновляет состояние экрана: управляет мерцанием подсказки и обрабатывает
        /// ввод пользователя. При нажатии ЛКМ / ENTER / ПРОБЕЛ переключает фазу
        /// с сюжета на инструкцию, а с инструкции — запускает уровень.
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

            if (!advance) return;

            if (phase_ == Phase.Story)
            {
                phase_ = Phase.HowToPlay;
                blinkTimer_ = 0f;
                blinkVisible_ = true;
            }
            else
            {
                screenManager_.GoTo(new LevelOne(screenManager_, 0));
            }
        }

        /// <summary>
        /// Рисует экран: фоновое изображение и содержимое текущей фазы
        /// (сюжет или инструкция по управлению).
        /// </summary>
        public void Draw()
        {
            DrawBgImage();

            if (phase_ == Phase.Story)
                DrawStory();
            else
                DrawHowToPlay();
        }

        /// <summary>
        /// Рисует фоновое изображение экрана, если оно загружено в <see cref="Assets.Assets.BackgroundScreenImage"/>.
        /// Затемнение сильнее, чем в главном меню — для читаемости текста поверх фона.
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
                new Color(100, 100, 100, 255));
        }

        /// <summary>
        /// Рисует фазу сюжета: панель, заголовок, разделитель и строки сюжетного текста.
        /// В нижней части панели — мерцающая подсказка о продолжении.
        /// </summary>
        private void DrawStory()
        {
            float panelX = PadX;
            float panelY = PadY;
            float panelW = Game.ScreenWidth - PadX * 2f;
            float panelH = Game.ScreenHeight - PadY * 2f;

            DrawPanel(panelX, panelY, panelW, panelH, DividerStory, CornerColor, AccentStory);

            float titleY = panelY + Game.ScaleInt(22);
            DrawCentered(Assets.Assets.FontLarge, StoryTitle, 48, titleY, AccentStory);

            float divY = panelY + Game.ScaleInt(80);
            DrawDivider(panelX, divY, panelW, DividerStory);

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

            if (blinkVisible_)
                DrawCentered(Assets.Assets.FontSmall,
                    "[ ЛКМ / ENTER / ПРОБЕЛ — продолжить ]",
                    Assets.Assets.FontSmallSize,
                    panelY + panelH - Game.ScaleInt(36), TextHint);
        }

        /// <summary>
        /// Рисует фазу инструкции: панель, заголовок, таблицу управления
        /// и блок «Особенности игры». В нижней части — мерцающая подсказка о запуске уровня.
        /// </summary>
        private void DrawHowToPlay()
        {
            float panelX = PadX;
            float panelY = PadY;
            float panelW = Game.ScreenWidth - PadX * 2f;
            float panelH = Game.ScreenHeight - PadY * 2f;

            DrawPanel(panelX, panelY, panelW, panelH, DividerHowTo, AccentHowTo, AccentHowTo);

            float titleY = panelY + Game.ScaleInt(22);
            DrawCentered(Assets.Assets.FontLarge, HowToTitle, 48, titleY, AccentHowTo);

            float divY = panelY + Game.ScaleInt(80);
            DrawDivider(panelX, divY, panelW, DividerHowTo);

            float colKey = panelX + Game.ScaleInt(60);
            float colDesc = panelX + Game.ScaleInt(340);
            float rowH = Assets.Assets.FontMediumSize + Game.ScaleInt(10);
            float tableY = divY + Game.ScaleInt(22);

            for (int i = 0; i < Controls.Length; i++)
            {
                float rowY = tableY + i * rowH;
                if (i % 2 == 0)
                    Raylib.DrawRectangle(
                        (int)(panelX + Game.ScaleInt(20)), (int)(rowY - 3),
                        (int)(panelW - Game.ScaleInt(40)), (int)(rowH - 2),
                        new Color(255, 255, 255, 10));

                Raylib.DrawTextEx(Assets.Assets.FontMedium, Controls[i].Key,
                    new Vector2((int)MathF.Round(colKey), (int)MathF.Round(rowY)),
                    Assets.Assets.FontMediumSize, 1, TextKey);

                Raylib.DrawTextEx(Assets.Assets.FontMedium, Controls[i].Desc,
                    new Vector2((int)MathF.Round(colDesc), (int)MathF.Round(rowY)),
                    Assets.Assets.FontMediumSize, 1, TextMain);
            }

            float noteDivY = tableY + Controls.Length * rowH + Game.ScaleInt(10);
            DrawDivider(panelX, noteDivY, panelW, new Color(50, 140, 210, 90));

            float noteY = noteDivY + Game.ScaleInt(12);
            float noteH = Assets.Assets.FontSmallSize + Game.ScaleInt(6);
            foreach (string line in HowToNotes)
            {
                if (string.IsNullOrEmpty(line))
                {
                    noteY += noteH * 0.3f;
                    continue;
                }
                Raylib.DrawTextEx(Assets.Assets.FontSmall, line,
                    new Vector2((int)(panelX + Game.ScaleInt(60)), (int)MathF.Round(noteY)),
                    Assets.Assets.FontSmallSize, 1, TextNote);
                noteY += noteH;
            }

            if (blinkVisible_)
                DrawCentered(Assets.Assets.FontSmall,
                    "[ ЛКМ / ENTER / ПРОБЕЛ — начать уровень ]",
                    Assets.Assets.FontSmallSize,
                    panelY + panelH - Game.ScaleInt(36),
                    new Color(100, 220, 130, 200));
        }

        /// <summary>
        /// Рисует полупрозрачную панель с тёмным фоном, рамкой и световыми уголками в sci-fi стиле.
        /// </summary>
        /// <param name="x">Позиция X левого верхнего угла панели.</param>
        /// <param name="y">Позиция Y левого верхнего угла панели.</param>
        /// <param name="w">Ширина панели.</param>
        /// <param name="h">Высота панели.</param>
        /// <param name="divider">Цвет разделителя (не используется напрямую, передаётся для единообразия).</param>
        /// <param name="corners">Цвет световых уголков рамки.</param>
        /// <param name="accent">Акцентный цвет текущей фазы.</param>
        private static void DrawPanel(
            float x, float y, float w, float h,
            Color divider, Color corners, Color accent)
        {
            Raylib.DrawRectangle(
                (int)x, (int)y, (int)w, (int)h, PanelBg);

            Raylib.DrawRectangleLinesEx(
                new Rectangle(x, y, w, h), 1.5f, PanelBorder);

            int cs = Game.ScaleInt(18);
            DrawCorner(x, y, cs, corners, false, false); // TL
            DrawCorner(x + w, y, cs, corners, true, false); // TR
            DrawCorner(x, y + h, cs, corners, false, true);  // BL
            DrawCorner(x + w, y + h, cs, corners, true, true);  // BR
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
