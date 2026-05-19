using Raylib_cs;
using Space_colony_game.World;
using Space_colony_game.World.Buildings;
using System.Numerics;

namespace Space_colony_game.UI.Menus
{
    public class Engineer
    {
        private readonly Colony colony_;
        private readonly BuildMode buildMode_;

        public bool IsOpen { get; private set; } = false;

        private int selectedIdx_ = 0;
        private const int PanelW = 520;
        private const int PanelH = 360;
        private static int PanelX => (Raylib.GetScreenWidth() - PanelW) / 2;
        private static int PanelY => (Raylib.GetScreenHeight() - PanelH) / 2;

        private const int ListW = 180;
        private const int ItemH = 44;

        private static readonly Color BgPanel = new(10, 16, 24, 240);
        private static readonly Color BgList = new(6, 10, 18, 255);
        private static readonly Color BorderColor = new(30, 45, 61, 255);
        private static readonly Color ItemHover = new(20, 30, 50, 255);
        private static readonly Color ItemSelected = new(25, 50, 90, 255);
        private static readonly Color TextMain = new(200, 212, 224, 255);
        private static readonly Color TextMuted = new(100, 120, 140, 255);
        private static readonly Color TextCost = new(200, 160, 96, 255);
        private static readonly Color BtnBuild = new(30, 100, 50, 255);
        private static readonly Color BtnBuildHov = new(40, 130, 65, 255);
        private static readonly Color BtnClose = new(80, 30, 30, 255);
        private static readonly Color BtnCloseHov = new(110, 40, 40, 255);
        private static readonly Color CanAfford = new(60, 184, 122, 255);
        private static readonly Color CantAfford = new(224, 80, 80, 255);

        public Engineer(Colony colony, BuildMode buildMode)
        {
            colony_ = colony;
            buildMode_ = buildMode;
        }

        public void Open()
        {
            IsOpen = true;
            selectedIdx_ = 0;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public void Update()
        {
            if (!IsOpen) return;

            var mouse = Raylib.GetMousePosition();

            var panelRect = new Rectangle(PanelX, PanelY, PanelW, PanelH);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                !Raylib.CheckCollisionPointRec(mouse, panelRect))
            {
                Close();
                return;
            }

            if (!Raylib.IsMouseButtonPressed(MouseButton.Left)) return;

            for (int i = 0; i < BuildingType.All.Length; i++)
            {
                var itemRect = GetListItemRect(i);
                if (Raylib.CheckCollisionPointRec(mouse, itemRect))
                {
                    selectedIdx_ = i;
                    return;
                }
            }

            if (Raylib.CheckCollisionPointRec(mouse, GetBuildBtnRect()))
            {
                var type = BuildingType.All[selectedIdx_];
                if (colony_.Metal.Value >= type.CostMetal)
                {
                    buildMode_.Activate(type);
                    Close();
                }
                return;
            }

            if (Raylib.CheckCollisionPointRec(mouse, GetCloseBtnRect()))
            {
                Close();
            }
        }

        public void Draw()
        {
            if (!IsOpen) return;

            float x = MathF.Round(PanelX);
            float y = MathF.Round(PanelY);
            float w = MathF.Round(PanelW);
            float h = MathF.Round(PanelH);

            var panelRect = new Rectangle(x, y, w, h);

            Raylib.DrawRectangleRec(panelRect, BgPanel);
            Raylib.DrawRectangleLinesEx(panelRect, 1f, BorderColor);

            DrawHeader();
            DrawBuildingList();
            DrawBuildingDetail();
            DrawButtons();
        }

        private void DrawHeader()
        {
            const string title = "Инженер — Постройки";

            var sz = Raylib.MeasureTextEx(
                Assets.Assets.FontMedium, title,
                Assets.Assets.FontMediumSize, 1);

            float tx = MathF.Round(PanelX + (PanelW - sz.X) * 0.5f);
            float ty = MathF.Round(PanelY + 12);

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, title,
                new Vector2(tx, ty),
                Assets.Assets.FontMediumSize, 1, TextMain);

            float y = MathF.Round(PanelY + 44);

            Raylib.DrawLineV(
                new Vector2(MathF.Round(PanelX), y),
                new Vector2(MathF.Round(PanelX + PanelW), y),
                BorderColor);
        }

        private void DrawBuildingList()
        {
            var mouse = Raylib.GetMousePosition();

            float bgX = MathF.Round(PanelX);
            float bgY = MathF.Round(PanelY + 45);
            float bgW = MathF.Round(ListW);
            float bgH = MathF.Round(PanelH - 45);

            Raylib.DrawRectangleRec(
                new Rectangle(bgX, bgY, bgW, bgH),
                BgList);

            float splitX = MathF.Round(PanelX + ListW);

            Raylib.DrawLineV(
                new Vector2(splitX, bgY),
                new Vector2(splitX, MathF.Round(PanelY + PanelH)),
                BorderColor);

            for (int i = 0; i < BuildingType.All.Length; i++)
            {
                var type = BuildingType.All[i];
                var itemRect = GetListItemRect(i);

                
                float x = MathF.Round(itemRect.X);
                float y = MathF.Round(itemRect.Y);
                float w = MathF.Round(itemRect.Width);
                float h = MathF.Round(itemRect.Height);

                var rect = new Rectangle(x, y, w, h);

                bool hovered = Raylib.CheckCollisionPointRec(mouse, rect);
                bool selected = i == selectedIdx_;

                if (selected)
                    Raylib.DrawRectangleRec(rect, ItemSelected);
                else if (hovered)
                    Raylib.DrawRectangleRec(rect, ItemHover);

                float iconX = MathF.Round(x + 8);
                float iconY = MathF.Round(y + (ItemH - 24) * 0.5f);

                Raylib.DrawRectangleRec(
                    new Rectangle(iconX, iconY, 24, 24),
                    type.Color);

                Raylib.DrawRectangleLinesEx(
                    new Rectangle(iconX, iconY, 24, 24),
                    1f, new Color(0, 0, 0, 80));

                var abbSz = Raylib.MeasureTextEx(
                    Assets.Assets.FontSmall, type.Abbr,
                    Assets.Assets.FontSmallSize * 0.7f, 1);

                float ax = MathF.Round(iconX + (24 - abbSz.X) * 0.5f);
                float ay = MathF.Round(iconY + (24 - abbSz.Y) * 0.5f);

                Raylib.DrawTextEx(
                    Assets.Assets.FontSmall, type.Abbr,
                    new Vector2(ax, ay),
                    Assets.Assets.FontSmallSize * 0.7f, 1,
                    type.AbbrevColor);

                float nameX = MathF.Round(iconX + 30);
                float nameY = MathF.Round(y + 6);

                Raylib.DrawTextEx(
                    Assets.Assets.FontSmall, type.Name,
                    new Vector2(nameX, nameY),
                    Assets.Assets.FontSmallSize, 1, TextMain);

                string costStr = $"Металл: {type.CostMetal}";
                bool canAfford = colony_.Metal.Value >= type.CostMetal;

                float costY = MathF.Round(y + 24);

                Raylib.DrawTextEx(
                    Assets.Assets.FontSmall, costStr,
                    new Vector2(nameX, costY),
                    Assets.Assets.FontSmallSize * 0.85f, 1,
                    canAfford ? CanAfford : CantAfford);

                float lineY = MathF.Round(y + ItemH);

                Raylib.DrawLineV(
                    new Vector2(x, lineY),
                    new Vector2(MathF.Round(x + ListW), lineY),
                    BorderColor);
            }
        }

        private void DrawBuildingDetail()
        {
            var type = BuildingType.All[selectedIdx_];

            float detX = MathF.Round(PanelX + ListW + 14);
            float detY = MathF.Round(PanelY + 52f);
            float detW = MathF.Round(PanelW - ListW - 28);

            float iconSize = 64;

            Raylib.DrawRectangleRec(
                new Rectangle(detX, detY, iconSize, iconSize),
                type.Color);

            Raylib.DrawRectangleLinesEx(
                new Rectangle(detX, detY, iconSize, iconSize),
                1f, new Color(0, 0, 0, 120));

            var abbSz = Raylib.MeasureTextEx(
                Assets.Assets.FontMedium, type.Abbr,
                Assets.Assets.FontMediumSize, 1);

            float ax = MathF.Round(detX + (iconSize - abbSz.X) * 0.5f);
            float ay = MathF.Round(detY + (iconSize - abbSz.Y) * 0.5f);

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, type.Abbr,
                new Vector2(ax, ay),
                Assets.Assets.FontMediumSize, 1,
                type.AbbrevColor);

            Raylib.DrawTextEx(
                Assets.Assets.FontSmall, "на карте",
                new Vector2(
                    MathF.Round(detX + (iconSize - 44) * 0.5f),
                    MathF.Round(detY + iconSize + 3)),
                Assets.Assets.FontSmallSize * 0.8f, 1,
                TextMuted);

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, type.Name,
                new Vector2(
                    MathF.Round(detX + iconSize + 12),
                    MathF.Round(detY + 4)),
                Assets.Assets.FontMediumSize, 1, TextMain);

            string sizeStr = $"Размер: {type.SizeX}x{type.SizeY}";

            Raylib.DrawTextEx(
                Assets.Assets.FontSmall, sizeStr,
                new Vector2(
                    MathF.Round(detX + iconSize + 12),
                    MathF.Round(detY + 32)),
                Assets.Assets.FontSmallSize, 1, TextMuted);

            DrawWrappedText(type.Description,
                detX,
                MathF.Round(detY + iconSize + 24),
                detW,
                Assets.Assets.FontSmallSize,
                TextMuted);

            float costY = MathF.Round(detY + iconSize + 90);

            Raylib.DrawTextEx(
                Assets.Assets.FontSmall,
                "Стоимость строительства:",
                new Vector2(detX, costY),
                Assets.Assets.FontSmallSize, 1, TextMuted);

            costY = MathF.Round(costY + 18);

            bool canAfford = colony_.Metal.Value >= type.CostMetal;

            string metalStr =
                $"Металл: {type.CostMetal}  (есть: {(int)colony_.Metal.Value})";

            Raylib.DrawTextEx(
                Assets.Assets.FontSmall, metalStr,
                new Vector2(detX, costY),
                Assets.Assets.FontSmallSize, 1,
                canAfford ? CanAfford : CantAfford);

            if (type.PowerPerDay > 0)
            {
                costY = MathF.Round(costY + 18);

                Raylib.DrawTextEx(
                    Assets.Assets.FontSmall,
                    $"Энергия: -{type.PowerPerDay}/день",
                    new Vector2(detX, costY),
                    Assets.Assets.FontSmallSize, 1, TextCost);
            }
        }

        private void DrawButtons()
        {
            var mouse = Raylib.GetMousePosition();

            var type = BuildingType.All[selectedIdx_];
            bool canAfford = colony_.Metal.Value >= type.CostMetal;

            var buildRect = GetBuildBtnRect();

            float bx = MathF.Round(buildRect.X);
            float by = MathF.Round(buildRect.Y);
            float bw = MathF.Round(buildRect.Width);
            float bh = MathF.Round(buildRect.Height);

            var bRect = new Rectangle(bx, by, bw, bh);

            bool buildHov =
                Raylib.CheckCollisionPointRec(mouse, bRect) && canAfford;

            var buildBg = !canAfford
                ? new Color(40, 40, 40, 255)
                : buildHov ? BtnBuildHov : BtnBuild;

            Raylib.DrawRectangleRec(bRect, buildBg);
            Raylib.DrawRectangleLinesEx(bRect, 1f, BorderColor);

            string buildLabel =
                canAfford ? "Построить →" : "Не хватает ресурсов";

            var bSz = Raylib.MeasureTextEx(
                Assets.Assets.FontSmall, buildLabel,
                Assets.Assets.FontSmallSize, 1);

            Raylib.DrawTextEx(
                Assets.Assets.FontSmall, buildLabel,
                new Vector2(
                    MathF.Round(bx + (bw - bSz.X) * 0.5f),
                    MathF.Round(by + (bh - bSz.Y) * 0.5f)),
                Assets.Assets.FontSmallSize, 1,
                canAfford ? TextMain : TextMuted);

            var closeRect = GetCloseBtnRect();

            float cx = MathF.Round(closeRect.X);
            float cy = MathF.Round(closeRect.Y);
            float cw = MathF.Round(closeRect.Width);
            float ch = MathF.Round(closeRect.Height);

            var cRect = new Rectangle(cx, cy, cw, ch);

            bool closeHov = Raylib.CheckCollisionPointRec(mouse, cRect);

            Raylib.DrawRectangleRec(cRect,
                closeHov ? BtnCloseHov : BtnClose);

            Raylib.DrawRectangleLinesEx(cRect, 1f, BorderColor);

            var cSz = Raylib.MeasureTextEx(
                Assets.Assets.FontSmall, "Закрыть",
                Assets.Assets.FontSmallSize, 1);

            Raylib.DrawTextEx(
                Assets.Assets.FontSmall, "Закрыть",
                new Vector2(
                    MathF.Round(cx + (cw - cSz.X) * 0.5f),
                    MathF.Round(cy + (ch - cSz.Y) * 0.5f)),
                Assets.Assets.FontSmallSize, 1, TextMain);
        }

        private Rectangle GetListItemRect(int i) => new(
            PanelX, PanelY + 45 + i * ItemH, ListW, ItemH
        );

        private Rectangle GetBuildBtnRect() => new(
            PanelX + ListW + 14,
            PanelY + PanelH - 44,
            PanelW - ListW - 28,
            34
        );

        private Rectangle GetCloseBtnRect() => new(
            PanelX + PanelW - 100,
            PanelY - 1,
            100,
            34
        );

        private static void DrawWrappedText(
            string text, float x, float y,
            float maxW, float size, Color color)
        {
            float lineH = size + 4;
            float curY = MathF.Round(y);

            foreach (var line in text.Split('\n'))
            {
                Raylib.DrawTextEx(
                    Assets.Assets.FontSmall, line,
                    new Vector2(MathF.Round(x), curY),
                    size, 1, color);

                curY = MathF.Round(curY + lineH);
            }
        }
    }
}
