using System;
using System.Collections.Generic;
using System.Drawing;
using GTA.Native;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Represents a timer bar containing icons at the right side.
    /// </summary>
    /// <seealso cref="TimerBarIcon"/>
    public class IconsTimerBar : TimerBarBase
    {
        private IList<TimerBarIcon> icons;

        /// <summary>
        /// Shown from right to left: index 0 is the right-most icon, the last item is the left-most.
        /// </summary>
        public IList<TimerBarIcon> Icons
        {
            get => icons;
            set => icons = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IconsTimerBar(string label) : base(label)
        {
            Icons = new List<TimerBarIcon>();
        }

        /// <inheritdoc/>
        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            if (Icons.Count > 0)
            {
                x += TB.IconXOffset;
                y += TB.IconYOffset;

                foreach (TimerBarIcon icon in Icons)
                {
                    if (icon != null)
                    {
                        Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, icon.TextureDictionary, true);
                        if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, icon.TextureDictionary))
                        {
                            continue;
                        }

                        Color c = icon.Color;

                        Function.Call(Hash.DRAW_SPRITE, icon.TextureDictionary, icon.TextureName, x, y, icon.Size.Width, icon.Size.Height, 0.0f, c.R, c.G, c.B, c.A);
                        x -= icon.Spacing;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents an icon of a <see cref="IconsTimerBar"/>.
    /// </summary>
    public class TimerBarIcon
    {
        public static readonly SizeF DefaultSize = new SizeF(TB.IconWidth, TB.IconHeight);
        public static readonly float DefaultSpacing = TB.IconWidth * 0.75f;
        public static readonly Color DefaultColor = Color.White;

        private string textureDictionary;
        private string textureName;

        public string TextureDictionary
        {
            get => textureDictionary;
            set => textureDictionary = String.IsNullOrWhiteSpace(value) ? throw new ArgumentException($"{nameof(value)} is null, empty or white-space", nameof(value)) : value;
        }

        public string TextureName
        {
            get => textureName;
            set => textureName = String.IsNullOrWhiteSpace(value) ? throw new ArgumentException($"{nameof(value)} is null, empty or white-space", nameof(value)) : value;
        }

        /// <summary>
        /// Size of this icon, in relative coordinates.
        /// </summary>
        public SizeF Size { get; set; } = DefaultSize;

        /// <summary>
        /// Space between this icon and the one to its left, in relative coordinates along the X-axis.
        /// </summary>
        public float Spacing { get; set; } = DefaultSpacing;

        public Color Color { get; set; } = DefaultColor;

        public TimerBarIcon(string textureDictionary, string textureName)
        {
            TextureDictionary = textureDictionary;
            TextureName = textureName;
        }

        // built-in icons
        private const string TBTxd = "timerbars";
        public static TimerBarIcon Rocket => new TimerBarIcon(TBTxd, "rockets");
        public static TimerBarIcon Spike => new TimerBarIcon(TBTxd, "spikes");
        public static TimerBarIcon Boost => new TimerBarIcon(TBTxd, "boost");

        private const string IconsTxd = "timerbar_icons";
        public static TimerBarIcon Beast => new TimerBarIcon(IconsTxd, "pickup_beast");
        public static TimerBarIcon Random => new TimerBarIcon(IconsTxd, "pickup_random");
        public static TimerBarIcon SlowTime => new TimerBarIcon(IconsTxd, "pickup_slow_time");
        public static TimerBarIcon Swap => new TimerBarIcon(IconsTxd, "pickup_swap");
        public static TimerBarIcon Testosterone => new TimerBarIcon(IconsTxd, "pickup_testosterone");
        public static TimerBarIcon Thermal => new TimerBarIcon(IconsTxd, "pickup_thermal");
        public static TimerBarIcon Weed => new TimerBarIcon(IconsTxd, "pickup_weed");
        public static TimerBarIcon Hidden => new TimerBarIcon(IconsTxd, "pickup_hidden");
        public static TimerBarIcon Time => new TimerBarIcon(IconsTxd, "pickup_b_time");
    }
}