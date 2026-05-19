using Raylib_cs;
using System.Numerics;

namespace Space_colony_game.UI.Menus
{
    public class ContextMenu
    {
        private enum ItemAction { Repair, Destroy }
        private abstract record MenuItem(string Label);
        private record SpecialistItem(string Label, int Index) : MenuItem(Label);
        private record ActionItem(string Label, ItemAction Action, bool Enabled, Color Color)
            : MenuItem(Label);
        private Vector2 position_;
        private const float ItemH = 36f;
        private const float MenuW = 220f;
        private const float PadX = 12f;
        private readonly List<MenuItem> items_ = new();
        private static readonly Color[] SpecDotColors =
        {
            Assets.Assets.EngineerColor,
            Assets.Assets.ScientistColor,
            Assets.Assets.LogistColor,
        };
        public bool IsOpen { get; private set; } = false;
        public Action<int>? OnSelectSpecialist { get; set; }
        public Action? OnRepair { get; set; }
        public Action? OnDestroy { get; set; }

        public void OpenForSpaceShip(Vector2 screenPosition)
        {
            position_ = ClampToScreen(screenPosition, 5);
            items_.Clear();
            string[] names = { "Инженер", "Ученый", "Логист" };
            for (int i = 0; i < names.Length; i++)
                items_.Add(new SpecialistItem(names[i], i));
            IsOpen = true;
        }

        public void OpenForBuilding(Vector2 screenPosition, bool canRepair, bool canDestroy)
        {
            position_ = ClampToScreen(screenPosition, 2);
            items_.Clear();
            items_.Add(new ActionItem(
                "Починить", ItemAction.Repair,
                canRepair, Assets.Assets.ContextMenuRepairColor
            ));
            items_.Add(new ActionItem(
                "Снести", ItemAction.Destroy,
                canDestroy, Assets.Assets.ContextMenuDestroyColor
            ));
            IsOpen = true;
        }

        public void Close() => IsOpen = false;

        public void Update()
        {
            if (!IsOpen) return;

            var mouse = Raylib.GetMousePosition();
            var menuRect = GetMenuRect();

            if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                !Raylib.CheckCollisionPointRec(mouse, menuRect))
            {
                Close();
                return;
            }

            if (Raylib.IsMouseButtonPressed(MouseButton.Right))
            {
                Close();
                return;
            }

            if (!Raylib.IsMouseButtonPressed(MouseButton.Left)) return;

            for (int i = 0; i < items_.Count; i++)
            {
                if (!Raylib.CheckCollisionPointRec(mouse, GetItemRect(i))) continue;

                switch (items_[i])
                {
                    case SpecialistItem s:
                        OnSelectSpecialist?.Invoke(s.Index);
                        break;
                    case ActionItem a when a.Enabled:
                        if (a.Action == ItemAction.Repair) OnRepair?.Invoke();
                        if (a.Action == ItemAction.Destroy) OnDestroy?.Invoke();
                        break;
                }
                Close();
                return;
            }
        }

        public void Draw()
        {
            if (!IsOpen) return;

            var mouse = Raylib.GetMousePosition();
            var menuRect = GetMenuRect();

            Raylib.DrawRectangleRec(menuRect, Assets.Assets.ContextMenuBgColor);
            Raylib.DrawRectangleLinesEx(menuRect, 1f, Assets.Assets.ContextMenuBorderColor);

            string header = items_.Any(i => i is SpecialistItem)
                ? "Специалисты" : "Действия";

            Raylib.DrawTextEx(Assets.Assets.FontSmall, header,
                new Vector2(position_.X + PadX, position_.Y + 8),
                Assets.Assets.FontSmallSize, 1,
                Assets.Assets.ContextMenuTextMuted);

            Raylib.DrawLineV(
                new Vector2(position_.X, position_.Y + 28),
                new Vector2(position_.X + MenuW, position_.Y + 28),
                Assets.Assets.ContextMenuBorderColor);

            for (int i = 0; i < items_.Count; i++)
            {
                var itemRect = GetItemRect(i);
                bool hovered = Raylib.CheckCollisionPointRec(mouse, itemRect);
                bool enabled = items_[i] is not ActionItem a || a.Enabled;

                if (hovered && enabled)
                    Raylib.DrawRectangleRec(itemRect, Assets.Assets.ContextMenuHoverColor);

                DrawItem(items_[i], itemRect, enabled);

                if (i < items_.Count - 1)
                    Raylib.DrawLineV(
                        new Vector2(itemRect.X + PadX, itemRect.Y + ItemH),
                        new Vector2(itemRect.X + MenuW - PadX, itemRect.Y + ItemH),
                        new Color(30, 45, 61, 180));
            }
        }

        private void DrawItem(MenuItem item, Rectangle rect, bool enabled)
        {
            float textY = rect.Y + (ItemH - Assets.Assets.FontSmallSize) / 2f;

            switch (item)
            {
                case SpecialistItem s:
                    {
                        Color dot = s.Index < SpecDotColors.Length
                            ? SpecDotColors[s.Index] : SpecDotColors[0];
                        Raylib.DrawCircleV(
                            new Vector2(rect.X + PadX + 5, rect.Y + ItemH / 2f), 4f, dot);
                        Raylib.DrawTextEx(Assets.Assets.FontSmall, s.Label,
                            new Vector2(rect.X + PadX + 16, textY),
                            Assets.Assets.FontSmallSize, 1, Assets.Assets.ContextMenuTextColor);
                        break;
                    }
                case ActionItem a:
                    {
                        Color col = enabled ? a.Color : Assets.Assets.ContextMenuDisabled;

                        Raylib.DrawRectangle(
                            (int)(rect.X + PadX), (int)(rect.Y + ItemH / 2f - 5),
                            10, 10, col with { A = enabled ? (byte)180 : (byte)60 });

                        Raylib.DrawTextEx(Assets.Assets.FontSmall, a.Label,
                            new Vector2(rect.X + PadX + 16, textY),
                            Assets.Assets.FontSmallSize, 1, col);

                        if (!enabled && a.Action == ItemAction.Repair)
                        {
                            var hint = "здание не повреждено";
                            var hSz = Raylib.MeasureTextEx(
                                Assets.Assets.FontSmall, hint,
                                Assets.Assets.FontSmallSize * 0.8f, 1);
                            Raylib.DrawTextEx(Assets.Assets.FontSmall, hint,
                                new Vector2(rect.X + MenuW - hSz.X - PadX, textY + 2),
                                Assets.Assets.FontSmallSize * 0.8f, 1,
                                Assets.Assets.ContextMenuDisabled);
                        }
                        break;
                    }
            }
        }

        private Rectangle GetMenuRect()
        {
            float totalH = 30 + items_.Count * ItemH;
            return new Rectangle(position_.X, position_.Y, MenuW, totalH);
        }

        private Rectangle GetItemRect(int i) =>
            new(position_.X, position_.Y + 30 + i * ItemH, MenuW, ItemH);

        private static Vector2 ClampToScreen(Vector2 pos, int itemCount)
        {
            float totalH = 30 + itemCount * ItemH;
            float x = Math.Min(pos.X, Raylib.GetScreenWidth() - MenuW - 4);
            float y = Math.Min(pos.Y, Raylib.GetScreenHeight() - totalH - 4);
            return new Vector2(Math.Max(0, x), Math.Max(0, y));
        }
    }
}
