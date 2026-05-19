using Raylib_cs;
using Space_colony_game.Core;

namespace Space_colony_game.Systems
{
    public class DayTransition
    {
        private const float FadeInOutDuration = 0.4f;
        private const float NightDuration = 0.2f;
        private enum Phase { Idle, FadeIn, Night, FadeOut }
        private Phase phase_ = Phase.Idle;
        private float timer_ = 0f;
        private int dayNum_ = 1;

        public bool IsPlaying => phase_ != Phase.Idle;
        public Action? OnMidnight {  get; set; }
        private bool midnightFired_ = false;

        public void Start(int nextDay)
        {
            if (IsPlaying) return;
            dayNum_ = nextDay;
            phase_ = Phase.FadeIn;
            timer_ = 0f;
            midnightFired_ = false;
        }

        public void Update()
        {
            if (phase_ == Phase.Idle) return;
            timer_ += Raylib.GetFrameTime();

            switch(phase_)
            {
                case Phase.FadeIn:
                    if(timer_ >= FadeInOutDuration)
                    {
                        phase_ = Phase.Night;
                        timer_ = 0f;
                    }
                    break;

                case Phase.Night:
                    if(!midnightFired_ && timer_ >= NightDuration * 0.3f)
                    {
                        midnightFired_ = true;
                        OnMidnight?.Invoke();
                    }

                    if(timer_ >= NightDuration)
                    {
                        phase_ = Phase.FadeOut;
                        timer_ = 0f;
                    }
                    break;

                case Phase.FadeOut:
                    if(timer_ >= FadeInOutDuration)
                    {
                        phase_ = Phase.Idle;
                        timer_ = 0f;
                    }
                    break;
            }
        }

        public void Draw()
        {
            if (phase_ == Phase.Idle) return;

            byte alpha = phase_ switch
            {
                Phase.FadeIn => (byte)(255 * EaseInOut(timer_ / FadeInOutDuration)),
                Phase.Night => (byte)255,
                Phase.FadeOut => (byte)(255 * EaseInOut(1f - timer_ / FadeInOutDuration)),
                _ => 0
            };

            Color overlay = Assets.Assets.NightColor with { A = alpha };
            Raylib.DrawRectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight, overlay);

            if (alpha > 180)
                DrawDayLabel(alpha);
        }

        private void DrawDayLabel(byte overlayAlpha)
        {
            byte textAlpha = (byte)Math.Clamp((overlayAlpha - 180) * 3, 0, 255);

            string line1 = "День";
            string line2 = dayNum_.ToString();

            int cx = Raylib.GetScreenWidth() / 2;
            int cy = Raylib.GetScreenHeight() / 2;

            Color numColor = new((byte)200, (byte)220, (byte)255, textAlpha);
            Color captColor = new((byte)140, (byte)160, (byte)200, (byte)(textAlpha * 0.6f));

            // --- число ---
            var sz2 = Raylib.MeasureTextEx(Assets.Assets.FontLarge, line2, 72, 1);

            float fx2 = cx - sz2.X / 2f;
            float fy2 = cy - sz2.Y / 2f - 10;

            int x2 = (int)MathF.Round(fx2);
            int y2 = (int)MathF.Round(fy2);

            Raylib.DrawTextEx(
                Assets.Assets.FontLarge, line2,
                new System.Numerics.Vector2(x2, y2),
                72, 1, numColor
            );

            // --- подпись ---
            var sz1 = Raylib.MeasureTextEx(Assets.Assets.FontMedium, line1, 18, 1);

            float fx1 = cx - sz1.X / 2f;
            float fy1 = cy - sz2.Y / 2f - 36;

            int x1 = (int)MathF.Round(fx1);
            int y1 = (int)MathF.Round(fy1);

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, line1,
                new System.Numerics.Vector2(x1, y1),
                18, 1, captColor
            );
        }

        private static float EaseInOut(float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return t * t * (3f - 2f * t);
        }
    }
}
