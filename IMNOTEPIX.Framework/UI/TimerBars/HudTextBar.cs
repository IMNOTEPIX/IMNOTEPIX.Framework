using System.Drawing;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Timerbar displaying a plain text value on the right side.
    /// </summary>
    public class HudTextBar : HudBar
    {
        public string Text { get; set; }
        public Color TextColor { get; set; } = Color.White;

        public HudTextBar(string label, string text = "") : base(label, thin: false)
        {
            Text = text ?? string.Empty;
        }

        public override void Draw(float y)
        {
            base.Draw(y);

            if (string.IsNullOrEmpty(Text))
                return;

            y -= 0.0113f;

            float textX = HudBarDraw.BaseX + (HudBarDraw.Width / 2f) - 0.008f;

            HudBarDraw.DrawText(Text, textX, y + 0.001f, font: 0, scale: 0.494f, colour: TextColor, justification: 2, wrap: textX);
        }
    }
}
