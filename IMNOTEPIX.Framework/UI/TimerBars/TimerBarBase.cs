using System;
using System.Drawing;
using GTA.Native;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Defines the base class for timer bars.
    /// Ported from RAGENativeUI's TimerBarBase. The obsolete Draw(int interval) overload
    /// (which drew via UIMenu.GetScreenResolutionMantainRatio/ResText/Sprite - all RPH/
    /// RAGENativeUI-only helpers with no SHVDN equivalent) was dropped, since it was already
    /// superseded by Draw(float, float) in the original. Everything else is 1:1, using
    /// GTA.Native.Function.Call in place of RPH's N.* wrapper.
    /// </summary>
    public abstract class TimerBarBase
    {
        public static readonly TextStyle DefaultLabelStyle = TextStyle.Default.With(
            scale: TB.LabelSize,
            wrap: (0.0f, TB.LabelInitialWrapEnd),
            justification: TextJustification.Right,
            color: HudColor.White.GetColor()
        );

        public static readonly PointF DefaultLabelOffset = new PointF(0.0f, 0.0f);

        private string label;

        public string Label
        {
            get => label;
            set => label = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <remarks>
        /// Note, the <c>End</c> value of <see cref="TextStyle.Wrap"/> is slightly modified
        /// when applying the style based on the current aspect ratio.
        /// </remarks>
        public TextStyle LabelStyle { get; set; } = DefaultLabelStyle;

        public PointF LabelOffset { get; set; } = DefaultLabelOffset;

        /// <summary>
        /// If not <c>null</c>, the timer bar background is drawn with an overlay of the specified color.
        /// </summary>
        public Color? Highlight { get; set; }

        /// <summary>
        /// If not <c>null</c>, a thin line of the specified color is drawn at the right border of the timer bar.
        /// </summary>
        public Color? Accent { get; set; }

        /// <summary>
        /// If <c>false</c>, uses the default height (like <see cref="TextTimerBar"/>);
        /// otherwise, a slightly smaller height (like <see cref="BarTimerBar"/>).
        /// </summary>
        protected internal bool Thin { get; set; }

        public TimerBarBase(string label)
        {
            Label = label;
        }

        /// <summary>
        /// Draws this timer bar at the specified position.
        /// </summary>
        /// <param name="x">The X-position in relative coordinates.</param>
        /// <param name="y">The Y-position in relative coordinates.</param>
        public virtual void Draw(float x, float y)
        {
            DrawBackground(x, y);
            DrawLabel(x, y);
        }

        private void DrawBackground(float x, float y)
        {
            x += TB.BgXOffset;
            y += Thin ? TB.BgSmallYOffset : TB.BgDefaultYOffset;

            float w = TB.BgWidth;
            float h = Thin ? TB.BgSmallHeight : TB.BgDefaultHeight;

            if (Highlight.HasValue)
            {
                Color c = Highlight.Value;
                Function.Call(Hash.DRAW_SPRITE, TB.BgTextureDictionary, TB.BgHighlightTextureName, x, y, w, h, 0.0f, c.R, c.G, c.B, c.A);
            }

            Function.Call(Hash.DRAW_SPRITE, TB.BgTextureDictionary, TB.BgTextureName, x, y, w, h, 0.0f, 255, 255, 255, 140);

            if (Accent.HasValue)
            {
                Color c = Accent.Value;
                Function.Call(Hash.DRAW_RECT, x - TB.BgXOffset + TB.AccentXOffset, y, TB.AccentWidth, h, c.R, c.G, c.B, c.A);
            }
        }

        private void DrawLabel(float x, float y)
        {
            TextStyle s = LabelStyle;

            var wrap = s.Wrap;
            // NOTE: verify IS_WIDESCREEN is the exact Hash member name in your SHVDN build.
            if (!Function.Call<bool>(Hash.GET_IS_WIDESCREEN))
            {
                wrap.End -= 0.02f;
            }
            wrap.End -= 0.03f * WrapEndMultiplier;
            s.Wrap = wrap;

            TextCommands.Display(Label, s, x + LabelOffset.X, y + LabelOffset.Y);
        }

        private static float WrapEndMultiplier
        {
            get
            {
                float aspectRatio = Function.Call<float>(Hash.GET_ASPECT_RATIO, false);

                var screenWidth = new OutputArgument();
                var screenHeight = new OutputArgument();
                // NOTE: verify _GET_ACTIVE_SCREEN_RESOLUTION is the exact Hash member name in your SHVDN build.
                Function.Call(Hash.GET_ACTUAL_SCREEN_RESOLUTION, screenWidth, screenHeight);
                float screenRatio = (float)screenWidth.GetResult<int>() / screenHeight.GetResult<int>();

                aspectRatio = Math.Min(aspectRatio, screenRatio);
                if (screenRatio > 3.5f && aspectRatio > 1.7f)
                {
                    return 0.4f;
                }
                else if (aspectRatio > 1.7f)
                {
                    return 0.0f;
                }
                else if (aspectRatio > 1.5f)
                {
                    return 0.2f;
                }
                else if (aspectRatio > 1.3f)
                {
                    return 0.3f;
                }
                else
                {
                    return 0.4f;
                }
            }
        }
    }
}