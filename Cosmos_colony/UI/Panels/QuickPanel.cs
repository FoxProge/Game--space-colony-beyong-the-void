using Raylib_cs;
using Space_colony_game.Core;

namespace Space_colony_game.UI.Panels
{
    /// <summary>
    /// Быстрая панель с кнопками открытия панелей специалистов (Инженер, Учёный, Логист).
    /// Размещает иконки-кнопки по центру панели и управляет их стилем.
    /// </summary>
    public class QuickPanel : Panel
    {
        private static readonly string[] SpecLabels =
        {
            "ИН",
            "УЧ",
            "ЛГ"
        };

        private static readonly Color[] SpecColors =
        {
            Assets.Assets.EngineerColor,
            Assets.Assets.ScientistColor,
            Assets.Assets.LogistColor,
        };

        /// <inheritdoc/>
        public override Rectangle Body { get; set; }

        /// <inheritdoc/>
        public override Color BackgroundColor { get; set; }

        /// <inheritdoc/>
        public override Color BorderColor { get; set; }

        /// <inheritdoc/>
        public override float BorderThickness { get; set; }

        /// <summary>Кнопка открытия панели инженера.</summary>
        public Button BtnOpenEngineer { get; }

        /// <summary>Кнопка открытия панели учёного.</summary>
        public Button BtnOpenScientist { get; }

        /// <summary>Кнопка открытия панели логиста.</summary>
        public Button BtnOpenLogistics { get; }

        private readonly Button[] _buttons;

        /// <summary>Создаёт QuickPanel с заданными размерами и инициализирует кнопки.</summary>
        public QuickPanel(float x, float y, float width, float height)
        {
            Body = new Rectangle(x, y, width, height);

            BackgroundColor = new Color(5, 10, 20, 255);
            BorderColor = new Color(30, 45, 61, 255);
            BorderThickness = 1f;

            float buttonSize = Game.ScaleF(40);

            BtnOpenEngineer = new Button(
                0,
                0,
                buttonSize,
                buttonSize,
                SpecLabels[0]
            );

            BtnOpenScientist = new Button(
                0,
                0,
                buttonSize,
                buttonSize,
                SpecLabels[1]
            );

            BtnOpenLogistics = new Button(
                0,
                0,
                buttonSize,
                buttonSize,
                SpecLabels[2]
            );

            _buttons =
            [
                BtnOpenEngineer,
                BtnOpenScientist,
                BtnOpenLogistics
            ];

            ConfigureButtons();
            UpdateLayout();
        }

        /// <summary>Настраивает цвета и стиль всех кнопок панели.</summary>
        private void ConfigureButtons()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                Button button = _buttons[i];

                button.BackgroundColor = new Color(13, 22, 34, 255);
                button.HoveredColor = new Color(20, 32, 48, 255);

                button.BorderColor = BorderColor;
                button.HoveredBorderColor = SpecColors[i];

                button.TextColor = Color.Gray;
                button.HoveredTextColor = Color.White;
            }
        }

        /// <summary>Вычисляет и устанавливает расположение кнопок внутри панели.</summary>
        private void UpdateLayout()
        {
            float spacing = Game.ScaleF(5);

            float buttonWidth = _buttons[0].Body.Width;
            float buttonHeight = _buttons[0].Body.Height;

            int count = _buttons.Length;

            float totalWidth =
                buttonWidth * count +
                spacing * (count - 1);

            float startX =
                Body.X + (Body.Width - totalWidth) / 2f;

            float startY =
                Body.Y + (Body.Height - buttonHeight) / 2f;

            for (int i = 0; i < count; i++)
            {
                float x =
                    startX + i * (buttonWidth + spacing);

                _buttons[i].Body = new Rectangle(
                    x,
                    startY,
                    buttonWidth,
                    buttonHeight
                );
            }
        }

        /// <summary>Рисует панель и все кнопки на ней.</summary>
        public override void Draw()
        {
            UpdateLayout();

            Raylib.DrawRectangleRec(
                Body,
                BackgroundColor
            );

            Raylib.DrawRectangleLinesEx(
                Body,
                BorderThickness,
                BorderColor
            );

            foreach (Button button in _buttons)
            {
                button.Draw();
            }
        }
    }
}