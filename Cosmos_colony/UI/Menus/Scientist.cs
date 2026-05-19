using Raylib_cs;
using Space_colony_game.World;
using System.Numerics;

namespace Space_colony_game.UI.Menus
{
    public class Scientist
    {
        private readonly Colony colony_;

        public bool IsOpen { get; private set; } = false;
        public static int LabBuilded { get; set; } = 0;

        private const int PanelW = 520;
        private const int PanelH = 360;
        private static int PanelX => (Raylib.GetScreenWidth() - PanelW) / 2;
        private static int PanelY => (Raylib.GetScreenHeight() - PanelH) / 2;

        private const int ListW = 190;
        private const int ItemH = 52;
        private static readonly int[] levels_ = new int[UpgradeCount];
        private const int UpgradeCount = 4;
        private const int MaxLevel = 5;
        private const int BaseMetalCost = 20;
        private const int BaseEnergyCost = 10;

        private int selectedIdx_ = 0;

        private static readonly UpgradeInfo[] Upgrades =
        {
        new("Фермы",         "Увеличивает производство\nеды на 10% за уровень.",
            new Color(60,  160,  60,  255), new Color(100, 220, 100, 255)),
        new("Шахты",         "Увеличивает добычу\nметалла на 10% за уровень.",
            new Color(140,  90,  50,  255), new Color(200, 160,  96, 255)),
        new("Солн. панели",  "Увеличивает выработку\nэнергии на 10% за уровень.",
            new Color(160, 140,  30,  255), new Color(240, 208,  64, 255)),
    };

        private static readonly Color BgPanel = new(10, 16, 24, 240);
        private static readonly Color BgList = new(6, 10, 18, 255);
        private static readonly Color BorderColor = new(30, 45, 61, 255);
        private static readonly Color ItemHover = new(20, 30, 50, 255);
        private static readonly Color ItemSelected = new(25, 50, 90, 255);
        private static readonly Color TextMain = new(200, 212, 224, 255);
        private static readonly Color TextMuted = new(100, 120, 140, 255);
        private static readonly Color CanAfford = new(60, 184, 122, 255);
        private static readonly Color CantAfford = new(224, 80, 80, 255);
        private static readonly Color BtnUpgrade = new(30, 100, 50, 255);
        private static readonly Color BtnUpgradeHov = new(40, 130, 65, 255);
        private static readonly Color BtnClose = new(80, 30, 30, 255);
        private static readonly Color BtnCloseHov = new(110, 40, 40, 255);
        private static readonly Color MaxLevelColor = new(220, 180, 30, 255);
        private static readonly Color BarBg = new(20, 30, 50, 255);
        private static readonly Color BarFill = new(60, 184, 122, 255);

        public Scientist(Colony colony)
        {
            colony_ = colony;
        }

        public static float GetEfficiencyBonus(int upgradeIdx)
        {
            if (upgradeIdx < 0 || upgradeIdx >= UpgradeCount) return 1f;
            return 1f + levels_[upgradeIdx] * 0.1f;
        }

        public void Open()
        {
            if (LabBuilded != 0)
                IsOpen = true;
        }
        public void Close() => IsOpen = false;

        public void Update()
        {
            if (!IsOpen) return;
            if (LabBuilded == 0)
            {
                Close();
                return;
            }

            var mouse = Raylib.GetMousePosition();

            if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                !Raylib.CheckCollisionPointRec(mouse, new Rectangle(PanelX, PanelY, PanelW, PanelH)))
            {
                Close();
                return;
            }

            if (!Raylib.IsMouseButtonPressed(MouseButton.Left)) return;

            for (int i = 0; i < Upgrades.Length; i++)
            {
                if (Raylib.CheckCollisionPointRec(mouse, GetListItemRect(i)))
                {
                    selectedIdx_ = i;
                    return;
                }
            }

            if (Raylib.CheckCollisionPointRec(mouse, GetUpgradeBtnRect()))
            {
                TryUpgrade(selectedIdx_);
                return;
            }

            if (Raylib.CheckCollisionPointRec(mouse, GetCloseBtnRect()))
                Close();
        }

        public void Draw()
        {
            if (!IsOpen) return;

            float px = MathF.Round(PanelX);
            float py = MathF.Round(PanelY);

            Raylib.DrawRectangleRec(new Rectangle(px, py, PanelW, PanelH), BgPanel);
            Raylib.DrawRectangleLinesEx(new Rectangle(px, py, PanelW, PanelH), 1f, BorderColor);

            DrawHeader();
            DrawList();
            DrawDetail();
            DrawCloseButton();
        }

        private void DrawHeader()
        {
            const string title = "Учёный — Улучшения";
            var sz = Raylib.MeasureTextEx(Assets.Assets.FontMedium, title, Assets.Assets.FontMediumSize, 1);
            Raylib.DrawTextEx(Assets.Assets.FontMedium, title,
                new Vector2(MathF.Round(PanelX + (PanelW - sz.X) / 2f), MathF.Round(PanelY + 12)),
                Assets.Assets.FontMediumSize, 1, TextMain);

            float lineY = MathF.Round(PanelY + 44);
            Raylib.DrawLineV(
                new Vector2(MathF.Round(PanelX), lineY),
                new Vector2(MathF.Round(PanelX + PanelW), lineY),
                BorderColor);
        }

        private void DrawList()
        {
            var mouse = Raylib.GetMousePosition();

            Raylib.DrawRectangleRec(
                new Rectangle(MathF.Round(PanelX), MathF.Round(PanelY + 45),
                    MathF.Round(ListW), MathF.Round(PanelH - 45)),
                BgList
            );
            Raylib.DrawLineV(
                new Vector2(MathF.Round(PanelX + ListW), MathF.Round(PanelY + 45)),
                new Vector2(MathF.Round(PanelX + ListW), MathF.Round(PanelY + PanelH)),
                BorderColor
            );

            for (int i = 0; i < Upgrades.Length; i++)
            {
                var upg = Upgrades[i];
                var rect = GetListItemRect(i);
                bool hovered = Raylib.CheckCollisionPointRec(mouse, rect);
                bool selected = i == selectedIdx_;

                if (selected) Raylib.DrawRectangleRec(rect, ItemSelected);
                else if (hovered) Raylib.DrawRectangleRec(rect, ItemHover);

                float dotX = MathF.Round(rect.X + 12);
                float dotY = MathF.Round(rect.Y + rect.Height / 2f);
                Raylib.DrawCircleV(new Vector2(dotX, dotY), 5f, upg.DotColor);

                float nameX = MathF.Round(rect.X + 26);
                float nameY = MathF.Round(rect.Y + 8);
                Raylib.DrawTextEx(Assets.Assets.FontSmall, upg.Name,
                    new Vector2(nameX, nameY), Assets.Assets.FontSmallSize, 1, TextMain
                );

                int lvl = levels_[i];
                string lvlStr = lvl >= MaxLevel ? "МАКС" : $"Ур. {lvl}/{MaxLevel}";
                Color lvlCol = lvl >= MaxLevel ? MaxLevelColor : TextMuted;
                Raylib.DrawTextEx(Assets.Assets.FontSmall, lvlStr,
                    new Vector2(nameX, MathF.Round(nameY + 20)),
                    Assets.Assets.FontSmallSize * 0.85f, 1, lvlCol
                );

                float barX = nameX;
                float barY = MathF.Round(rect.Y + rect.Height - 10);
                float barW = ListW - 36;
                float barH = 4;
                Raylib.DrawRectangleRec(new Rectangle(barX, barY, barW, barH), BarBg);
                if (lvl > 0)
                    Raylib.DrawRectangleRec(
                        new Rectangle(barX, barY, MathF.Round(barW * lvl / MaxLevel), barH),
                        BarFill
                    );

                Raylib.DrawLineV(
                    new Vector2(rect.X, MathF.Round(rect.Y + rect.Height)),
                    new Vector2(MathF.Round(rect.X + ListW), MathF.Round(rect.Y + rect.Height)),
                    BorderColor
                );
            }
        }

        private void DrawDetail()
        {
            var upg = Upgrades[selectedIdx_];
            int lvl = levels_[selectedIdx_];

            float dx = MathF.Round(PanelX + ListW + 14);
            float dy = MathF.Round(PanelY + 56);

            float iconSize = 56f;
            Raylib.DrawRectangleRec(new Rectangle(dx, dy, iconSize, iconSize), upg.Color);
            Raylib.DrawRectangleLinesEx(new Rectangle(dx, dy, iconSize, iconSize), 1f, new Color(0, 0, 0, 120));

            Raylib.DrawTextEx(Assets.Assets.FontMedium, upg.Name,
                new Vector2(MathF.Round(dx + iconSize + 12), MathF.Round(dy + 4)),
                Assets.Assets.FontMediumSize, 1, TextMain
            );

            string lvlStr = lvl >= MaxLevel ? "МАКСИМАЛЬНЫЙ УРОВЕНЬ" : $"Уровень {lvl} / {MaxLevel}";
            Color lvlCol = lvl >= MaxLevel ? MaxLevelColor : TextMuted;
            Raylib.DrawTextEx(Assets.Assets.FontSmall, lvlStr,
                new Vector2(MathF.Round(dx + iconSize + 12), MathF.Round(dy + 34)),
                Assets.Assets.FontSmallSize, 1, lvlCol
            );

            float descY = MathF.Round(dy + iconSize + 14);
            DrawWrappedText(upg.Description, dx, descY);

            float bonusY = MathF.Round(descY + 52);
            string bonusStr = $"Текущий бонус: +{lvl * 10}%  эффективности";
            Raylib.DrawTextEx(Assets.Assets.FontSmall, bonusStr,
                new Vector2(dx, bonusY), Assets.Assets.FontSmallSize, 1, CanAfford
            );

            if (lvl < MaxLevel)
            {
                float costY = MathF.Round(bonusY + 24);
                int metal = GetMetalCost(lvl);
                int energy = GetEnergyCost(lvl);
                bool canAfford = colony_.Metal.Value >= metal && colony_.Energy.Value >= energy;

                string costStr = $"Стоимость: металл {metal}  энергия {energy}";
                Raylib.DrawTextEx(Assets.Assets.FontSmall, costStr,
                    new Vector2(dx, costY), Assets.Assets.FontSmallSize, 1,
                    canAfford ? CanAfford : CantAfford
                );

                var btnRect = GetUpgradeBtnRect();
                var mouse = Raylib.GetMousePosition();
                bool hov = Raylib.CheckCollisionPointRec(mouse, btnRect) && canAfford;
                Color btnBg = !canAfford ? new Color(40, 40, 40, 255) : hov ? BtnUpgradeHov : BtnUpgrade;

                Raylib.DrawRectangleRec(btnRect, btnBg);
                Raylib.DrawRectangleLinesEx(btnRect, 1f, BorderColor);

                string btnLabel = canAfford ? $"Улучшить → ур. {lvl + 1}" : "Не хватает ресурсов";
                var bSz = Raylib.MeasureTextEx(Assets.Assets.FontSmall, btnLabel, Assets.Assets.FontSmallSize, 1);
                Raylib.DrawTextEx(Assets.Assets.FontSmall, btnLabel,
                    new Vector2(
                        MathF.Round(btnRect.X + (btnRect.Width - bSz.X) / 2f),
                        MathF.Round(btnRect.Y + (btnRect.Height - bSz.Y) / 2f)),
                    Assets.Assets.FontSmallSize, 1, canAfford ? TextMain : TextMuted
                );
            }
        }

        private void DrawCloseButton()
        {
            var rect = GetCloseBtnRect();
            bool hovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect);
            Raylib.DrawRectangleRec(rect, hovered ? BtnCloseHov : BtnClose);
            Raylib.DrawRectangleLinesEx(rect, 1f, BorderColor);
            var sz = Raylib.MeasureTextEx(Assets.Assets.FontSmall, "Закрыть", Assets.Assets.FontSmallSize, 1);
            Raylib.DrawTextEx(Assets.Assets.FontSmall, "Закрыть",
                new Vector2(
                    MathF.Round(rect.X + (rect.Width - sz.X) / 2f),
                    MathF.Round(rect.Y + (rect.Height - sz.Y) / 2f)),
                Assets.Assets.FontSmallSize, 1, TextMain
            );
        }

        private void TryUpgrade(int idx)
        {
            if (levels_[idx] >= MaxLevel) return;

            int metal = GetMetalCost(levels_[idx]);
            int energy = GetEnergyCost(levels_[idx]);

            if (colony_.Metal.Value < metal) return;
            if (colony_.Energy.Value < energy) return;

            colony_.Metal.Value -= metal;
            colony_.Energy.Value -= energy;
            levels_[idx]++;
        }

        private static int GetMetalCost(int currentLevel) => BaseMetalCost * (currentLevel + 1);
        private static int GetEnergyCost(int currentLevel) => BaseEnergyCost * (currentLevel + 1);

        private Rectangle GetListItemRect(int i) => new(
            MathF.Round(PanelX),
            MathF.Round(PanelY + 45 + i * ItemH),
            MathF.Round(ListW),
            MathF.Round(ItemH));

        private Rectangle GetUpgradeBtnRect() => new(
            MathF.Round(PanelX + ListW + 14),
            MathF.Round(PanelY + PanelH - 44),
            MathF.Round(PanelW - ListW - 28),
            34
        );

        private Rectangle GetCloseBtnRect() => new(
            MathF.Round(PanelX + PanelW - 100),
            MathF.Round(PanelY - 1),
            100, 34
        );

        private static void DrawWrappedText(string text, float x, float y)
        {
            float lineH = Assets.Assets.FontSmallSize + 4;
            float curY = MathF.Round(y);
            foreach (var line in text.Split('\n'))
            {
                Raylib.DrawTextEx
                    (Assets.Assets.FontSmall, line,
                    new Vector2(MathF.Round(x), curY),
                    Assets.Assets.FontSmallSize, 1, new Color(100, 120, 140, 255));
                    curY = MathF.Round(curY + lineH
                );
            }
        }

        private record UpgradeInfo(string Name, string Description, Color Color, Color DotColor);
    }
}
