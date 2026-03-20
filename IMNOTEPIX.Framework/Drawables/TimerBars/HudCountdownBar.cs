using GTA;
using System;
using System.Drawing;

namespace IMNOTEPIX.Framework.Drawables.TimerBars
{
    /// <summary>
    /// Timerbar displaying a mm:ss countdown.
    /// Fires OnFinished and sets ShouldRemove when the timer reaches zero.
    /// HudBarController automatically removes the bar when ShouldRemove is true.
    /// </summary>
    public class HudCountdownBar : HudBar
    {
        private readonly int _duration;
        private int _endTime;
        private bool _finished;

        public bool ShouldRemove { get; private set; }
        public Action OnFinished { get; set; }
        public Color TimeColor { get; set; } = Color.White;

        public int RemainingTime => Math.Max(0, _endTime - Game.GameTime);
        public bool IsFinished => _finished;

        public HudCountdownBar(string label, int durationMs)
            : base(label, thin: false)
        {
            _duration = durationMs;
            _endTime = Game.GameTime + durationMs;
        }

        public void Restart()
        {
            _finished = false;
            _endTime = Game.GameTime + _duration;
            ShouldRemove = false;
        }

        public override void Draw(float y)
        {
            base.Draw(y);

            y -= 0.0113f;

            int remaining = RemainingTime;

            if (remaining <= 0 && !_finished)
            {
                _finished = true;
                ShouldRemove = true;
                OnFinished?.Invoke();
            }

            float textX = HudBarDraw.BaseX + (HudBarDraw.Width / 2f) - 0.008f;
            TimeSpan time = TimeSpan.FromMilliseconds(remaining);

            HudBarDraw.DrawText(
                time.ToString("mm\\:ss"),
                textX,
                y + 0.001f,
                font: 0,
                scale: 0.494f,
                colour: TimeColor,
                justification: 2,
                wrap: textX);
        }
    }
}