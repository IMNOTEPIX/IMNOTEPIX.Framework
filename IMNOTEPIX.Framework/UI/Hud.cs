using GTA;
using GTA.Graphics;
using GTA.Native;
using GTA.UI;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IMNOTEPIX.Framework.fUI
{
    public class Hud
    {
        #region HelpText

        public static void BeginTextCommandDisplayHelp(IntPtr gxtEntry)
            => Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_HELP, gxtEntry);

        public static void BeginTextCommandDisplayHelp(string gxtEntry)
           => Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_HELP, gxtEntry);

        public static void AddTextComponentSubstringPlayerName(string text)
           => Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);

        /// <summary>
        /// p0     = help message shape/ID, usually 0
        /// loop   = stays on screen until cleared if true
        /// beep   = plays the help beep sound
        /// duration = display time in ms, -1 for default
        ///
        /// NOTE: parameter order fixed vs original — first param is shape/p0,
        /// last is duration. Original had these named and ordered incorrectly.
        /// </summary>
        public static void EndTextCommandDisplayHelp(int p0 = 0, bool loop = false, bool beep = true, int duration = -1)
           => Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_HELP, p0, loop, beep, duration);

        public static bool IsHelpMessageBeingDisplayed()
            => Function.Call<bool>(Hash.IS_HELP_MESSAGE_BEING_DISPLAYED);

        public static bool IsHelpMessageOnScreen()
            => Function.Call<bool>(Hash.IS_HELP_MESSAGE_ON_SCREEN);

        public static bool IsHelpMessageFadingOut()
            => Function.Call<bool>(Hash.IS_HELP_MESSAGE_FADING_OUT);

        /// <summary>
        /// Displays one or more strings as a help message.
        /// Passes through params — single strings are handled by the same overload.
        /// </summary>
        public static void DisplayHelpText(params string[] texts)
        {
            bool showing = IsHelpMessageBeingDisplayed() || IsHelpMessageOnScreen() || IsHelpMessageFadingOut();

            ClearAllHelpMessages();

            if (showing)
                ClearHelp(true);

            BeginTextCommandDisplayHelp(CellEmailBcon);
            foreach (var text in texts)
                AddTextComponentSubstringPlayerName(text);

            EndTextCommandDisplayHelp(0, false, !showing);
        }

        /// <summary>
        /// Displays a help message with explicit duration and loop control.
        /// </summary>
        public static void DisplayHelpText_Duration(int p0 = 0, bool loop = false, bool beep = true, int duration = 2500, params string[] texts)
        {
            if (IsHelpMessageBeingDisplayed() || IsHelpMessageOnScreen() || IsHelpMessageFadingOut() || Game.Player.Character.IsDead)
                return;

            ClearAllHelpMessages();
            ClearBrief();
            BeginTextCommandDisplayHelp(CellEmailBcon);
            foreach (var text in texts)
                AddTextComponentSubstringPlayerName(text);

            EndTextCommandDisplayHelp(p0, loop, beep, duration);
        }

        /// <summary>
        /// Displays a GXT-keyed help message.
        /// loop = true means the message persists until explicitly cleared.
        /// </summary>
        public static void DisplayGXTHelpText(string gxtEntry, bool loop = false, bool beep = true, int duration = -1)
        {
            bool showing = Function.Call<bool>(Hash.IS_HELP_MESSAGE_BEING_DISPLAYED) || Function.Call<bool>(Hash.IS_HELP_MESSAGE_ON_SCREEN) || Function.Call<bool>(Hash.IS_HELP_MESSAGE_FADING_OUT);

            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_HELP, gxtEntry);
            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_HELP, 0, loop, showing ? false : beep, duration);
        }

        public static void DisplayHelpTextThisFrameGXT(string helpText, bool p1 = true)
            => Function.Call(Hash.DISPLAY_HELP_TEXT_THIS_FRAME, helpText, p1);

        #endregion

        #region Misc

        public static void ClearAllHelpMessages()
            => Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);

        public static void ClearHelp(bool toggle)
            => Function.Call(Hash.CLEAR_HELP, toggle);

        public static void ClearBrief()
            => Function.Call(Hash.CLEAR_BRIEF);

        // ─────────────────────────────────────────────────────────────────────
        // CellEmailBcon — allocated once, never leaked
        //
        // Previously a property calling StringToCoTaskMemUTF8 on every access,
        // which called Marshal.AllocCoTaskMem and never freed it.
        // ─────────────────────────────────────────────────────────────────────

        public static IntPtr CellEmailBcon => StringToCoTaskMemUTF8("CELL_EMAIL_BCON");


        // ─────────────────────────────────────────────────────────────────────
        // UTF-8 interop — used internally for CellEmailBcon
        // ─────────────────────────────────────────────────────────────────────

        private static byte[] _strBuffer = new byte[100];

        /// <summary>
        /// Allocates a null-terminated UTF-8 string in unmanaged memory.
        /// Caller is responsible for freeing with Marshal.FreeCoTaskMem
        /// if the result is not a long-lived static (like CellEmailBcon).
        /// </summary>
        public unsafe static IntPtr StringToCoTaskMemUTF8(string s)
        {
            if (s == null)
                return IntPtr.Zero;

            int byteCount = Encoding.UTF8.GetByteCount(s);
            if (byteCount > _strBuffer.Length)
                _strBuffer = new byte[byteCount * 2];

            Encoding.UTF8.GetBytes(s, 0, s.Length, _strBuffer, 0);

            IntPtr ptr = Marshal.AllocCoTaskMem(byteCount + 1);
            if (ptr == IntPtr.Zero)
                throw new OutOfMemoryException();

            Marshal.Copy(_strBuffer, 0, ptr, byteCount);
            ((byte*)ptr.ToPointer())[byteCount] = 0;
            return ptr;
        }

        #endregion

        #region Notifications

        public static void ShowNotification(string message, bool isImportant = true, bool cacheMessage = true)
   => Notification.PostTicker(message, isImportant, cacheMessage);

        public static FeedPost PostMessageNotify(string text, TextureAsset textAsset, bool isImportant, FeedTextIcon icon, string senderName)
            => Notification.PostMessageText(text, textAsset, isImportant, icon, senderName);

        #endregion

        #region Subtitle

        public static void ClearSubtitles()
            => Function.Call(Hash.CLEAR_PRINTS);

        public static void ClearSmallSubtitles()
            => Function.Call(Hash.CLEAR_SMALL_PRINTS);

        public static void ClearAllSubtitles()
        {
            Function.Call(Hash.CLEAR_SMALL_PRINTS);
            Function.Call(Hash.CLEAR_PRINTS);
        }

        public static void ShowSubtitle(string message, int duration, bool drawImmediately = true)
            => Screen.ShowSubtitle(message, duration, drawImmediately);
        public static void ShowGXTSubtitle(string gxtEntry, int duration = 10)
        {
            Function.Call(Hash.BEGIN_TEXT_COMMAND_PRINT, gxtEntry);
            Function.Call(Hash.END_TEXT_COMMAND_PRINT, duration, true);
        }

        #endregion

        #region Additional Text

        /// <param name="p1">Unused in most cases — pass false (default).</param>
        public static void ClearAdditionalText(int p0, bool p1 = false)
            => Function.Call(Hash.CLEAR_ADDITIONAL_TEXT, p0, p1);

        /// <summary>
        /// Blocks (via Script.Yield) until both text checks pass.
        /// Intentional coroutine yield — not a blocking loop.
        /// </summary>
        public static void RequestAdditionalText(string gxt, int slot)
        {
            while (!Function.Call<bool>(Hash.HAS_THIS_ADDITIONAL_TEXT_LOADED, gxt, slot) || !Function.Call<bool>(Hash.HAS_ADDITIONAL_TEXT_LOADED, slot))
            {
                Function.Call(Hash.REQUEST_ADDITIONAL_TEXT, gxt, slot);
                GTA.Script.Yield();
            }
        }

        public static void RequestAdditionalTextBasic(string gxt, int slot)
            => Function.Call(Hash.REQUEST_ADDITIONAL_TEXT, gxt, slot);

        public static bool HasThisAdditionalTextLoaded(string gxt, int slot)
            => Function.Call<bool>(Hash.HAS_THIS_ADDITIONAL_TEXT_LOADED, gxt, slot);

        public static bool HasAdditionalTextLoaded(int slot)
            => Function.Call<bool>(Hash.HAS_ADDITIONAL_TEXT_LOADED, slot);

        #endregion

        #region Text Rendering

        public static void SetTextFont(int fontType)
            => Function.Call(Hash.SET_TEXT_FONT, fontType);

        public static void SetTextColour(int r, int g, int b, int a)
            => Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);

        public static void SetTextScale(float scale, float size)
            => Function.Call(Hash.SET_TEXT_SCALE, scale, size);

        public static void SetTextJustification(int justifyType)
            => Function.Call(Hash.SET_TEXT_JUSTIFICATION, justifyType);

        public static void SetTextRightJustify(bool toggle)
            => Function.Call(Hash.SET_TEXT_RIGHT_JUSTIFY, toggle);

        public static void SetTextWrap(float start, float end)
            => Function.Call(Hash.SET_TEXT_WRAP, start, end);

        public static void SetTextOutline()
            => Function.Call(Hash.SET_TEXT_OUTLINE);

        public static void SetTextDropShadow()
            => Function.Call(Hash.SET_TEXT_DROP_SHADOW);

        public static void SetTextDropshadow(int distance, int r, int g, int b, int a)
            => Function.Call(Hash.SET_TEXT_DROPSHADOW, distance, r, g, b, a);

        public static void SetTextEdge(int p0, int r, int g, int b, int a)
            => Function.Call(Hash.SET_TEXT_EDGE, p0, r, g, b, a);

        public static float GetTextHeight(float size, int font)
            => Function.Call<float>(Hash.GET_RENDERED_CHARACTER_HEIGHT, size, font);

        public static void BeginTextCommandGetWidth(string text)
            => Function.Call(Hash.BEGIN_TEXT_COMMAND_GET_SCREEN_WIDTH_OF_DISPLAY_TEXT, text);

        public static float EndTextCommandGetWidth(bool p0)
            => Function.Call<float>(Hash.END_TEXT_COMMAND_GET_SCREEN_WIDTH_OF_DISPLAY_TEXT, p0);

        public static void BeginTextCommandDisplayText(string text)
            => Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, text);

        public static void EndTextCommandDisplayText(float x, float y)
            => Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y, 0);

        public static void BeginTextCommandGetLineCount(string entry)
            => Function.Call(Hash.BEGIN_TEXT_COMMAND_GET_NUMBER_OF_LINES_FOR_STRING, entry);

        public static int EndTextCommandGetLineCount(float x, float y)
            => Function.Call<int>(Hash.END_TEXT_COMMAND_GET_NUMBER_OF_LINES_FOR_STRING, x, y);

        public static void AddTextComponentFormatInt(int value, bool commaSeparated)
            => Function.Call(Hash.ADD_TEXT_COMPONENT_FORMATTED_INTEGER, value, commaSeparated);

        public static void AddTextComponentInt(int value)
            => Function.Call(Hash.ADD_TEXT_COMPONENT_INTEGER, value);

        #endregion

        #region Minimap

        public static void ToggleNorthYanktonMap(bool toggle)
            => Function.Call(Hash.SET_MINIMAP_IN_PROLOGUE, toggle);

        public static void HideMinimapExteriorMapThisFrame()
            => Function.Call(Hash.HIDE_MINIMAP_EXTERIOR_MAP_THIS_FRAME);

        public static void HideMinimapInteriorMapThisFrame()
            => Function.Call(Hash.HIDE_MINIMAP_INTERIOR_MAP_THIS_FRAME);

        public static void DontTiltMinimapThisFrame()
            => Function.Call(Hash.DONT_TILT_MINIMAP_THIS_FRAME);

        public static void DontZoomMinimapWhenSnipingThisFrame()
            => Function.Call(Hash.DONT_ZOOM_MINIMAP_WHEN_SNIPING_THIS_FRAME);

        public static void DontZoomMinimapWhenRunningThisFrame()
            => Function.Call(Hash.DONT_ZOOM_MINIMAP_WHEN_RUNNING_THIS_FRAME);

        public static void FlashMinimapDisplay()
            => Function.Call(Hash.FLASH_MINIMAP_DISPLAY);

        public static void SetFakePauseMapPlayerPositionThisFrame(float x, float y)
            => Function.Call(Hash.SET_FAKE_PAUSEMAP_PLAYER_POSITION_THIS_FRAME, x, y);

        public static void HudSuppressWeaponWheelResultsThisFrame()
            => Function.Call(Hash.HUD_SUPPRESS_WEAPON_WHEEL_RESULTS_THIS_FRAME);

        public static void SetInsideVerySmallInterior(bool toggle)
            => Function.Call(Hash.SET_INSIDE_VERY_SMALL_INTERIOR, toggle);

        public static void ClearGPSMultiRoute()
            => Function.Call(Hash.CLEAR_GPS_MULTI_ROUTE);

        #endregion

        #region GTA Hud Components

        public static void RadarAndHud(bool radar, bool hud)
        {
            Function.Call(Hash.DISPLAY_RADAR, radar);
            Function.Call(Hash.DISPLAY_HUD, hud);
        }

        public static bool IsPauseMenuActive()
            => Function.Call<bool>(Hash.IS_PAUSE_MENU_ACTIVE);

        public static bool IsPauseMapInInteriorMode()
            => Function.Call<bool>(Hash.IS_PAUSEMAP_IN_INTERIOR_MODE);

        public static void ShowHudComponentThisFrame(HudComponent id)
            => Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, (int)id);

        public static void ShowHudComponentThisFrame(int id)
            => Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, id);

        public static void ShowScriptedHudComponentThisFrame(int id)
            => Function.Call(Hash.SHOW_SCRIPTED_HUD_COMPONENT_THIS_FRAME, id);

        public static void HideHudComponentThisFrame(HudComponent id)
            => Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, (int)id);

        public static void HideHudComponentThisFrame(int id)
            => Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, id);

        public static bool IsHudComponentActive(HudComponent id)
            => Function.Call<bool>(Hash.IS_HUD_COMPONENT_ACTIVE, (int)id);

        public static bool IsHudComponentActive(int id)
            => Function.Call<bool>(Hash.IS_HUD_COMPONENT_ACTIVE, id);

        public static void DisplayAmmoThisFrame(bool display)
            => Function.Call(Hash.DISPLAY_AMMO_THIS_FRAME, display);

        public static void HideHudMarkersThisFrame()
            => Function.Call(Hash.HIDE_HUDMARKERS_THIS_FRAME);

        public static unsafe void GetHudColour(int hudColorIndex, out int r, out int g, out int b, out int a)
        {
            int R = 0, G = 0, B = 0, A = 0;
            Function.Call(Hash.GET_HUD_COLOUR, hudColorIndex, &R, &G, &B, &A);
            r = R; g = G; b = B; a = A;
        }

        #endregion
    }
}
