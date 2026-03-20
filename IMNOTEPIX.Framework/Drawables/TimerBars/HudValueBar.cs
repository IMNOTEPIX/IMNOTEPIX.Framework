using GTA.Native;
using System.Drawing;
using IMNOTEPIX.Framework.Funcs;

namespace IMNOTEPIX.Framework.Drawables.TimerBars
{
    /// <summary>
    /// Timerbar displaying a numeric value, optionally formatted as money.
    ///
    /// NOTE: base.Draw(y) is intentionally NOT called. HudValueBar uses a
    /// variable-width BarConfig — the bar widens as the number of digits grows.
    /// This means background, highlight, accent, and label are all rendered
    /// here rather than delegated to HudBar.Draw().
    /// </summary>
    public class HudValueBar : HudBar
    {
        public int Value { get; set; }
        public bool IsMoney { get; set; }
        public Color ValueColor { get; set; } = Color.White;

        public HudValueBar(string label, bool isMoney = false, bool thin = false)
            : base(label, thin)
        {
            IsMoney = isMoney;
        }

        public override void Draw(float y)
        {
            int length = Value.ToString().Length;
            BarConfig config = GetConfigForLength(length);

            float offset = Thin ? 0.012f : 0.008f;
            y += offset;

            fGraphics.RequestStreamedTextureDict("timerbars");
            if (!fGraphics.HasStreamedTextureDictLoaded("timerbars")) return;

            float baseX = HudBarDraw.BaseX;

            // Background
            fGraphics.DrawSprite("timerbars", "all_black_bg",
                baseX, y, config.Width, config.Height,
                0, 255, 255, 255, 140);

            // Highlight overlay
            if (Highlight.HasValue)
            {
                fGraphics.DrawSprite("timerbars", "all_white_bg",
                    baseX, y, config.Width, config.Height,
                    0,
                    Highlight.Value.R, Highlight.Value.G, Highlight.Value.B,
                    140);
            }

            // Accent line
            if (Accent.HasValue)
            {
                float accentX = baseX + (config.Width / 2f) - 0.001f;
                fGraphics.DrawRect(accentX, y,
                    0.002f, config.Height,
                    Accent.Value.R, Accent.Value.G, Accent.Value.B, 255,
                    false);
            }

            // Text positions
            float labelY = y - 0.010f;
            float valueY = y - 0.0165f;
            float labelX = baseX - (config.Width / 2f) + 0.030f;
            float valueX = baseX + (config.Width / 2f) - 0.008f;
            float wrapEnd = valueX + 0.005f;
            float wrapStart = labelX + 0.05f;

            // Label
            if (!string.IsNullOrEmpty(Label))
            {
                Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, Label);
                Function.Call(Hash.SET_TEXT_COLOUR, LabelColor.R, LabelColor.G, LabelColor.B, LabelColor.A);
                Function.Call(Hash.SET_TEXT_SCALE, 0f, 0.32f);
                Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, labelX, labelY, 0);
            }

            // Value
            if (IsMoney)
            {
                // ESDOLLA format: game-style money string with comma separators and $ sign.
                fHud.BeginTextCommandDisplayText("ESDOLLA");
                fHud.AddTextComponentInt(Value);
                Function.Call(Hash.ADD_TEXT_COMPONENT_FORMATTED_INTEGER, Value, 1);
                fHud.SetTextJustification(2);
                fHud.SetTextWrap(wrapStart, wrapEnd);
                fHud.SetTextFont(5);
                fHud.SetTextScale(0f, config.ValueScale);
                fHud.SetTextColour(ValueColor.R, ValueColor.G, ValueColor.B, ValueColor.A);
                fHud.EndTextCommandDisplayText(valueX, valueY);
            }
            else
            {
                // Plain number — rendered via DrawText with the value stringified.
                // Fixed: this path was previously commented out, leaving IsMoney=false
                // bars with no value drawn at all.
                HudBarDraw.DrawText(
                    Value.ToString("N0"),
                    valueX,
                    valueY,
                    font: 0,
                    scale: config.ValueScale,
                    colour: ValueColor,
                    justification: 2,
                    wrap: wrapEnd);
            }
        }

        /// <summary>
        /// Returns bar dimensions and text scale for a given digit count.
        /// The bar widens and text shrinks as values grow to prevent overflow.
        /// </summary>
        private static BarConfig GetConfigForLength(int length)
        {
            if (length < 7)
                return new BarConfig { Width = 0.165f, Height = 0.035f, ValueScale = 0.483f };
            if (length == 7)
                return new BarConfig { Width = 0.175f, Height = 0.034f, ValueScale = 0.45f };
            if (length == 8)
                return new BarConfig { Width = 0.185f, Height = 0.0325f, ValueScale = 0.43f };
            if (length == 9)
                return new BarConfig { Width = 0.19f, Height = 0.0325f, ValueScale = 0.4275f };
            // 10+ digits
            return new BarConfig { Width = 0.195f, Height = 0.0325f, ValueScale = 0.4275f };
        }

        private struct BarConfig
        {
            public float Width;
            public float Height;
            public float ValueScale;
        }
    }
}