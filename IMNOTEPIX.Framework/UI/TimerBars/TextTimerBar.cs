using System;
using System.Drawing;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Represents a timer bar with text located at the right side.
    /// Ported from RAGENativeUI's TextTimerBar. The obsolete Draw(int interval) overload
    /// (UIMenu.GetScreenResolutionMantainRatio, ResText, Common.EFont - all RPH/RAGENativeUI-
    /// only, no SHVDN equivalent) was dropped since it was already superseded by Draw(float, float).
    /// </summary>
    public class TextTimerBar : TimerBarBase
    {
        public static readonly TextStyle DefaultTextStyle = TextStyle.Default.With(
            scale: TB.TextSize,
            wrap: (0.0f, TB.TextWrapEnd),
            justification: TextJustification.Right,
            color: HudColor.White.GetColor()
        );

        public static readonly PointF DefaultTextOffset = new PointF(0.0f, TB.TextYOffset);

        private string text;

        public string Text
        {
            get => text;
            set => text = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TextStyle TextStyle { get; set; } = DefaultTextStyle;

        public PointF TextOffset { get; set; } = DefaultTextOffset;

        public TextTimerBar(string label, string text) : base(label)
        {
            Text = text;
        }

        /// <inheritdoc/>
        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            TextCommands.Display(Text, TextStyle, x + TextOffset.X, y + TextOffset.Y);
        }
    }
}