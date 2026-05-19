using Raylib_cs;
using Space_colony_game.World;
using System.Numerics;

namespace Space_colony_game.UI.Menus
{
    public class Logist
    {
        private readonly Colony _colony;

        public bool IsOpen { get; private set; } = false;

        private const int PanelW = 520;
        private const int PanelH = 360;
        private static int PanelX => (Raylib.GetScreenWidth() - PanelW) / 2;
        private static int PanelY => (Raylib.GetScreenHeight() - PanelH) / 2;

        private int _orderAmount = 5;
        private const int MinOrder = 1;
        private const int MaxOrder = 20;
        private const int FoodPerColonist = 5;

        private readonly List<(int Amount, int DaysLeft)> _inTransit = new();
        private const int DeliveryDays = 3;

        private string _lastMessage = "";
        private float _messageTimer = 0f;
        private const float MessageDuration = 2.0f;

        private static readonly Color BgPanel = new(10, 16, 24, 240);
        private static readonly Color BorderColor = new(30, 45, 61, 255);
        private static readonly Color TextMain = new(200, 212, 224, 255);
        private static readonly Color TextMuted = new(100, 120, 140, 255);
        private static readonly Color CanAfford = new(60, 184, 122, 255);
        private static readonly Color CantAfford = new(224, 80, 80, 255);
        private static readonly Color BtnOrder = new(30, 100, 50, 255);
        private static readonly Color BtnOrderHov = new(40, 130, 65, 255);
        private static readonly Color BtnClose = new(80, 30, 30, 255);
        private static readonly Color BtnCloseHov = new(110, 40, 40, 255);
        private static readonly Color BtnArrow = new(40, 60, 90, 255);
        private static readonly Color BtnArrowHov = new(60, 90, 140, 255);
        private static readonly Color InTransitBg = new(15, 25, 40, 255);

        public Logist(Colony colony)
        {
            _colony = colony;
        }

        public void Open() => IsOpen = true;
        public void Close() => IsOpen = false;

        public void OnDayPassed()
        {
            for (int i = _inTransit.Count - 1; i >= 0; i--)
            {
                var (amount, days) = _inTransit[i];
                if (days <= 1)
                {
                    _colony.People.Value += amount;
                    _inTransit.RemoveAt(i);
                    ShowMessage($"+{amount} колонистов прибыло!");
                }
                else
                {
                    _inTransit[i] = (amount, days - 1);
                }
            }

            if (_messageTimer > 0)
                _messageTimer -= 1f;
        }

        public void Update()
        {
            if (!IsOpen) return;

            var mouse = Raylib.GetMousePosition();

            if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                !Raylib.CheckCollisionPointRec(mouse, new Rectangle(PanelX, PanelY, PanelW, PanelH)))
            {
                Close();
                return;
            }

            if (!Raylib.IsMouseButtonPressed(MouseButton.Left)) return;

            if (Raylib.CheckCollisionPointRec(mouse, GetMinusBtnRect()))
            {
                _orderAmount = Math.Max(MinOrder, _orderAmount - 1);
                return;
            }

            if (Raylib.CheckCollisionPointRec(mouse, GetPlusBtnRect()))
            {
                _orderAmount = Math.Min(MaxOrder, _orderAmount + 1);
                return;
            }

            if (Raylib.CheckCollisionPointRec(mouse, GetOrderBtnRect()))
            {
                int totalCost = _orderAmount * FoodPerColonist;
                if (_colony.Food.Value >= totalCost && _colony.People.Value < _colony.People.Max)
                {
                    _colony.Food.Value -= totalCost;
                    _inTransit.Add((_orderAmount, DeliveryDays));
                    ShowMessage($"Заказ на {_orderAmount} колонистов отправлен!");
                }
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
            DrawColonistInfo();
            DrawOrderSection();
            DrawInTransit();
            DrawMessage();
            DrawCloseButton();
        }

        private void DrawHeader()
        {
            const string title = "Логист — Колонисты";
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

        private void DrawColonistInfo()
        {
            float x = MathF.Round(PanelX + 20);
            float y = MathF.Round(PanelY + 58);

            string colStr = $"Колонисты: {_colony.People.Value}";
            Raylib.DrawTextEx(Assets.Assets.FontMedium, colStr,
                new Vector2(x, y), Assets.Assets.FontMediumSize, 1, TextMain);

            float foodY = MathF.Round(y + 34);
            string foodStr = $"Еда: {(int)_colony.Food.Value}  (1 колонист = {FoodPerColonist} еды)";
            Raylib.DrawTextEx(Assets.Assets.FontSmall, foodStr,
                new Vector2(x, foodY), Assets.Assets.FontSmallSize, 1, TextMuted);

            float divY = MathF.Round(PanelY + 130);
            Raylib.DrawLineV(
                new Vector2(MathF.Round(PanelX + 10), divY),
                new Vector2(MathF.Round(PanelX + PanelW - 10), divY),
                BorderColor);
        }

        private void DrawOrderSection()
        {
            float x = MathF.Round(PanelX + 20);
            float y = MathF.Round(PanelY + 145);
            int cost = _orderAmount * FoodPerColonist;
            int can = 0;
            if (_colony.Food.Value >= cost) can += 1;
            if (_colony.People.Value < _colony.People.Max) can += 2;

            Raylib.DrawTextEx(Assets.Assets.FontSmall, "Заказать колонистов:",
                new Vector2(x, y), Assets.Assets.FontSmallSize, 1, TextMuted);

            var minusRect = GetMinusBtnRect();
            bool minusHov = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), minusRect);
            Raylib.DrawRectangleRec(minusRect, minusHov ? BtnArrowHov : BtnArrow);
            Raylib.DrawRectangleLinesEx(minusRect, 1f, BorderColor);
            DrawCentered("—", minusRect, Assets.Assets.FontMediumSize, TextMain);

            var countRect = GetCountRect();
            Raylib.DrawRectangleRec(countRect, new Color(6, 10, 18, 255));
            Raylib.DrawRectangleLinesEx(countRect, 1f, BorderColor);
            DrawCentered(_orderAmount.ToString(), countRect, Assets.Assets.FontMediumSize, TextMain);

            var plusRect = GetPlusBtnRect();
            bool plusHov = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), plusRect);
            Raylib.DrawRectangleRec(plusRect, plusHov ? BtnArrowHov : BtnArrow);
            Raylib.DrawRectangleLinesEx(plusRect, 1f, BorderColor);
            DrawCentered("+", plusRect, Assets.Assets.FontMediumSize, TextMain);

            float costY = MathF.Round(minusRect.Y + minusRect.Height + 10);
            string costStr = $"Стоимость: {cost} еды";
            Raylib.DrawTextEx(Assets.Assets.FontSmall, costStr,
                new Vector2(x, costY), Assets.Assets.FontSmallSize, 1,
                (can == 3) ? CanAfford : CantAfford);

            var orderRect = GetOrderBtnRect();
            var mouse = Raylib.GetMousePosition();
            bool orderHov = Raylib.CheckCollisionPointRec(mouse, orderRect) && (can == 3);
            Color orderBg = (can != 3) ? new Color(40, 40, 40, 255) : orderHov ? BtnOrderHov : BtnOrder;

            Raylib.DrawRectangleRec(orderRect, orderBg);
            Raylib.DrawRectangleLinesEx(orderRect, 1f, BorderColor);
            string orderLabel = "";
            switch(can)
            {
                case 0: case 1: orderLabel = "Нет свободного места"; break;
                case 2: orderLabel = "Недостаточно еды"; break;
                case 3: orderLabel = "заказать"; break;
            }
            DrawCentered(orderLabel, orderRect, Assets.Assets.FontSmallSize,
                (can == 3) ? TextMain : TextMuted);
        }

        private void DrawInTransit()
        {
            if (_inTransit.Count == 0) return;

            float x = MathF.Round(PanelX + 20);
            float y = MathF.Round(PanelY + 265);

            Raylib.DrawTextEx(Assets.Assets.FontSmall, "В пути:",
                new Vector2(x, y), Assets.Assets.FontSmallSize, 1, TextMuted);

            float itemY = y + Assets.Assets.FontSmallSize + 6;
            foreach (var (amount, days) in _inTransit)
            {
                var itemRect = new Rectangle(x, itemY, PanelW - 40, 24);
                Raylib.DrawRectangleRec(itemRect, InTransitBg);
                Raylib.DrawRectangleLinesEx(itemRect, 0.5f, BorderColor);

                string info = $"{amount} колонистов — через {days} дн.";
                Raylib.DrawTextEx(Assets.Assets.FontSmall, info,
                    new Vector2(x + 8, itemY + 4),
                    Assets.Assets.FontSmallSize, 1, CanAfford);

                itemY += 28;
            }
        }

        private void DrawMessage()
        {
            if (_messageTimer <= 0 || string.IsNullOrEmpty(_lastMessage)) return;

            var sz = Raylib.MeasureTextEx(Assets.Assets.FontSmall, _lastMessage,
                Assets.Assets.FontSmallSize, 1);
            float tx = MathF.Round(PanelX + (PanelW - sz.X) / 2f);
            float ty = MathF.Round(PanelY + PanelH - 56);

            Raylib.DrawTextEx(Assets.Assets.FontSmall, _lastMessage,
                new Vector2(tx, ty), Assets.Assets.FontSmallSize, 1, CanAfford);
        }

        private void DrawCloseButton()
        {
            var rect = GetCloseBtnRect();
            var mouse = Raylib.GetMousePosition();
            bool hovered = Raylib.CheckCollisionPointRec(mouse, rect);

            Raylib.DrawRectangleRec(rect, hovered ? BtnCloseHov : BtnClose);
            Raylib.DrawRectangleLinesEx(rect, 1f, BorderColor);
            DrawCentered("Закрыть", rect, Assets.Assets.FontSmallSize, TextMain);
        }

        private Rectangle GetMinusBtnRect() => new(
            MathF.Round(PanelX + 20),
            MathF.Round(PanelY + 172), 36, 36);

        private Rectangle GetCountRect() => new(
            MathF.Round(PanelX + 62),
            MathF.Round(PanelY + 172), 60, 36);

        private Rectangle GetPlusBtnRect() => new(
            MathF.Round(PanelX + 128),
            MathF.Round(PanelY + 172), 36, 36);

        private Rectangle GetOrderBtnRect() => new(
            MathF.Round(PanelX + 20),
            MathF.Round(PanelY + 254), 180, 34);

        private Rectangle GetCloseBtnRect() => new(
            MathF.Round(PanelX + PanelW - 100),
            MathF.Round(PanelY - 1), 100, 34
        );

        private static void DrawCentered(string text, Rectangle rect, float size, Color color)
        {
            var sz = Raylib.MeasureTextEx(Assets.Assets.FontSmall, text, size, 1);
            Raylib.DrawTextEx(Assets.Assets.FontSmall, text,
                new Vector2(
                    MathF.Round(rect.X + (rect.Width - sz.X) / 2f),
                    MathF.Round(rect.Y + (rect.Height - sz.Y) / 2f)),
                size, 1, color);
        }

        private void ShowMessage(string msg)
        {
            _lastMessage = msg;
            _messageTimer = MessageDuration;
        }
    }
}
