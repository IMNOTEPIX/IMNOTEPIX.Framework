using System.Drawing;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    public class HudBarDraw
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

        public static void DrawText(string text, float x, float y, int font, float scale, Color colour, int justification, float wrap, bool shadow = false, bool outline = false)
        {
            Hud.BeginTextCommandDisplayText("STRING");
            Hud.AddTextComponentSubstringPlayerName(text);
            Hud.SetTextJustification(justification);
            Hud.SetTextWrap(0f, wrap);
            Hud.SetTextFont(font);
            Hud.SetTextScale(0f, scale);
            Hud.SetTextColour(colour.R, colour.G, colour.B, colour.A);
            if (outline)
                Hud.SetTextOutline();
            if (shadow)
                Hud.SetTextDropShadow();
            Hud.EndTextCommandDisplayText(x, y);
        }

        public static void DrawNumber(int value, float x, float y, int font, float scale, Color colour, int justification, float wrap, bool asMoney = false)
        {
            Hud.BeginTextCommandDisplayText(asMoney ? "HUD_CASH" : "NUMBER");
            Hud.AddTextComponentInt(value);
            if (asMoney)
                Hud.AddTextComponentFormatInt(value, true);
            Hud.SetTextJustification(justification);
            Hud.SetTextWrap(0f, wrap);
            Hud.SetTextFont(font);
            Hud.SetTextScale(0f, scale);
            Hud.SetTextColour(colour.R, colour.G, colour.B, colour.A);
            Hud.EndTextCommandDisplayText(x, y);
        }
    }
}
