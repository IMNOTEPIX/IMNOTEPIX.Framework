using GTA.Native;
using System.Drawing;
using IMNOTEPIX.Framework.Funcs;

namespace IMNOTEPIX.Framework.Drawables.TimerBars
{
    public static class HudBarDraw
    {
        public const float AlignWidth = 0.952f;
        public const float AlignHeight = 0.949f;

        public const float BaseX = 0.84f;
        public const float TitleX = 0.741f;

        public const float DefaultY = 0.90f;
        public const float BusyY = 0.84f;

        public const float Width = 0.165f;
        public const float Height = 0.035f;
        public const float ThinHeight = 0.028f;
        public const float Margin = 0.0399f;
        public const float ThinMargin = 0.0319f;

        public static void DrawText(
            string text,
            float x,
            float y,
            int font,
            float scale,
            Color colour,
            int justification,
            float wrap,
            bool shadow = false,
            bool outline = false)
        {
            fHud.BeginTextCommandDisplayText("STRING");
            fHud.AddTextComponentSubstringPlayerName(text);
            fHud.SetTextJustification(justification);
            fHud.SetTextWrap(0f, wrap);
            fHud.SetTextFont(font);
            fHud.SetTextScale(0f, scale);
            fHud.SetTextColour(colour.R, colour.G, colour.B, colour.A);
            if (outline) fHud.SetTextOutline();
            if (shadow) fHud.SetTextDropShadow();
            fHud.EndTextCommandDisplayText(x, y);
        }

        public static void DrawNumber(
            int value,
            float x,
            float y,
            int font,
            float scale,
            Color colour,
            int justification,
            float wrap,
            bool asMoney = false)
        {
            fHud.BeginTextCommandDisplayText(asMoney ? "HUD_CASH" : "NUMBER");
            fHud.AddTextComponentInt(value);
            if (asMoney)
                fHud.AddTextComponentFormatInt(value, true);
            fHud.SetTextJustification(justification);
            fHud.SetTextWrap(0f, wrap);
            fHud.SetTextFont(font);
            fHud.SetTextScale(0f, scale);
            fHud.SetTextColour(colour.R, colour.G, colour.B, colour.A);
            fHud.EndTextCommandDisplayText(x, y);
        }
    }
}
