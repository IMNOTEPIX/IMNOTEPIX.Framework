using System;
using System.Collections.Generic;
using System.Drawing;
using GTA.Native;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Represents a timer bar containing a progress bar.
    /// Ported from RAGENativeUI's BarTimerBar. The obsolete Draw(int interval) overload
    /// (Rage.MathHelper, UIMenu.GetScreenResolutionMantainRatio, ResRectangle - all RPH-only,
    /// no SHVDN equivalent) was dropped since it was already superseded by Draw(float, float).
    /// </summary>
    /// <seealso cref="TimerBarMarker"/>
    public class BarTimerBar : TimerBarBase
    {
        private float percentage;
        private IList<TimerBarMarker> markers;

        /// <summary>
        /// Progress percentage, from <c>0.0f</c> to <c>1.0f</c>; values outside this range are clamped.
        /// </summary>
        public float Percentage
        {
            get => percentage;
            set => percentage = ClampPercentage(value);
        }

        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }

        public IList<TimerBarMarker> Markers
        {
            get => markers;
            set => markers = value ?? throw new ArgumentNullException(nameof(value));
        }

        // .NET Framework 4.8 (SHVDN's target) predates Math.Clamp (added in .NET Core 2.0 /
        // .NET Standard 2.1), so a manual clamp replaces Rage.MathHelper.Clamp here.
        private static float ClampPercentage(float value) => value < 0.0f ? 0.0f : (value > 1.0f ? 1.0f : value);

        public BarTimerBar(string label) : base(label)
        {
            Thin = true;

            ForegroundColor = HudColor.Red.GetColor();
            BackgroundColor = Color.FromArgb(120, ForegroundColor);

            Markers = new List<TimerBarMarker>();
        }

        /// <inheritdoc/>
        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            x += TB.BarXOffset;
            y += TB.BarYOffset;

            float w = TB.BarWidth;
            float h = TB.BarHeight;

            Function.Call(Hash.DRAW_RECT, x, y, w, h, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);

            float fillW = w * Percentage;
            float fillX = x - w * 0.5f + fillW * 0.5f;
            Function.Call(Hash.DRAW_RECT, fillX, y, fillW, h, ForegroundColor.R, ForegroundColor.G, ForegroundColor.B, ForegroundColor.A);

            if (Markers.Count > 0)
            {
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, TB.MarkersTextureDictionary, true);
                if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, TB.MarkersTextureDictionary))
                {
                    return;
                }

                float markerOrigX = x - w * 0.5f;
                foreach (TimerBarMarker m in Markers)
                {
                    float markerX = markerOrigX + w * m.Percentage;

                    Function.Call(Hash.DRAW_SPRITE, TB.MarkersTextureDictionary, TB.MarkerTextureName, markerX, y, w * 1.1f, h * 1.9f, 0.0f, m.Color.R, m.Color.G, m.Color.B, m.Color.A);
                }
            }
        }
    }

    /// <summary>
    /// Defines a <see cref="BarTimerBar"/> marker - a thin line over the progress bar at the specified <see cref="Percentage"/>.
    /// </summary>
    public struct TimerBarMarker : IEquatable<TimerBarMarker>
    {
        public float Percentage { get; }
        public Color Color { get; }

        public TimerBarMarker(float percentage, Color color)
        {
            Percentage = percentage < 0.0f ? 0.0f : (percentage > 1.0f ? 1.0f : percentage);
            Color = color;
        }

        public TimerBarMarker(float percentage) : this(percentage, Color.Black)
        {
        }

        public override int GetHashCode() => (Percentage, Color).GetHashCode();

        public override bool Equals(object other) => other is TimerBarMarker m && Equals(m);

        public bool Equals(TimerBarMarker other)
        {
            return Percentage == other.Percentage &&
                   Color == other.Color;
        }
    }
}