using System.Drawing;

namespace IMNOTEPIX.Framework.Drawables.TimerBars
{
    /// <summary>
    /// Timerbar styled like GTA Online player name rows.
    /// Left side shows the player name; right side shows an optional stat string.
    /// </summary>
    public class HudPlayerBar : HudBar
    {
        public string PlayerName { get; set; }
        public string RightText { get; set; }
        public Color RightTextColor { get; set; } = Color.White;

        public HudPlayerBar(string playerName, string rightText = "")
            : base("", thin: false)
        {
            PlayerName = playerName ?? string.Empty;
            RightText = rightText ?? string.Empty;
        }

        public override void Draw(float y)
        {
            base.Draw(y);

            y -= 0.0113f;

            if (!string.IsNullOrEmpty(PlayerName))
            {
                HudBarDraw.DrawText(
                    PlayerName,
                    HudBarDraw.TitleX,
                    y + 0.001f,
                    font: 0,
                    scale: 0.494f,
                    colour: Color.White,
                    justification: 1,
                    wrap: 0.867f);
            }

            if (!string.IsNullOrEmpty(RightText))
            {
                float textX = HudBarDraw.BaseX + (HudBarDraw.Width / 2f) - 0.008f;

                HudBarDraw.DrawText(
                    RightText,
                    textX,
                    y + 0.001f,
                    font: 0,
                    scale: 0.494f,
                    colour: RightTextColor,
                    justification: 2,
                    wrap: textX);
            }
        }
    }
}