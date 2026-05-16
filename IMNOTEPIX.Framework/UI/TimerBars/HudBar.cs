using System.Drawing;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Base class for all HUD timer bars.
    /// Draws the black background sprite, optional highlight overlay,
    /// optional right-edge accent line, and the label text.
    ///
    /// Subclasses that need variable-width rendering (e.g. HudValueBar) may
    /// skip calling base.Draw() and reimplement background drawing manually —
    /// if so, they must also handle highlight, accent, and label themselves.
    /// </summary>
    public abstract class HudBar
    {
        public string Label { get; set; }
        public bool Thin { get; protected set; }
        public Color LabelColor { get; set; } = Color.White;
        public Color? Highlight { get; set; }
        public Color? Accent { get; set; }

        public float VerticalSpacing
            => Thin ? HudBarDraw.ThinMargin : HudBarDraw.Margin;

        protected HudBar(string label, bool thin = false)
        {
            Label = label ?? string.Empty;
            Thin = thin;
        }

        public virtual void Draw(float y)
        {
            float offset = Thin ? 0.012f : 0.008f;
            y += offset;

            float height = Thin ? HudBarDraw.ThinHeight : HudBarDraw.Height;

            Graphics.DrawSprite("timerbars", "all_black_bg", HudBarDraw.BaseX, y, HudBarDraw.Width, height, 0, 255, 255, 255, 140);

            if (Highlight.HasValue)
                Graphics.DrawSprite("timerbars", "all_white_bg", HudBarDraw.BaseX, y, HudBarDraw.Width, height, 0, Highlight.Value.R, Highlight.Value.G, Highlight.Value.B, 140);

            if (Accent.HasValue)
            {
                float accentX = HudBarDraw.BaseX + (HudBarDraw.Width / 2f) - 0.001f;
                Graphics.DrawRect(accentX, y, 0.002f, height, Accent.Value.R, Accent.Value.G, Accent.Value.B, 255, false);
            }

            if (!string.IsNullOrEmpty(Label))
                HudBarDraw.DrawText(Label, HudBarDraw.TitleX, y - 0.011f, font: 0, scale: 0.302f, colour: LabelColor, justification: 1, wrap: 0.867f);
        }
    }
}
