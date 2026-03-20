using GTA.Math;
using GTA.Native;
using GTA.UI;
using System;
using System.Drawing;

namespace IMNOTEPIX.Framework.Funcs
{
    public static class fGraphics
    {
        // ─────────────────────────────────────────────────────────────────────
        // Screen / resolution
        // ─────────────────────────────────────────────────────────────────────

        public static bool GetIsWidescreen()
            => Function.Call<bool>(Hash.GET_IS_WIDESCREEN);

        public static float GetAspectRatio(bool b)
            => Function.Call<float>(Hash.GET_ASPECT_RATIO, b);

        /// <summary>
        /// Returns the current screen resolution.
        /// Fixed: original passed X/Y by value — native requires output pointers.
        /// </summary>
        public static unsafe void GetActiveScreenResolution(out int x, out int y)
        {
            int rx = 0, ry = 0;
            Function.Call(Hash.GET_ACTUAL_SCREEN_RESOLUTION, &rx, &ry);
            x = rx;
            y = ry;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GFX alignment
        // ─────────────────────────────────────────────────────────────────────

        public static void ResetScriptGfxAlign()
            => Function.Call(Hash.RESET_SCRIPT_GFX_ALIGN);

        public static void SetScriptGfxAlignParams(float x, float y, float w, float h)
            => Function.Call(Hash.SET_SCRIPT_GFX_ALIGN_PARAMS, x, y, w, h);

        public static void SetScriptGfxAlign(int horizontalAlign, int verticalAlign)
            => Function.Call(Hash.SET_SCRIPT_GFX_ALIGN, horizontalAlign, verticalAlign);

        // ─────────────────────────────────────────────────────────────────────
        // Texture streaming
        // ─────────────────────────────────────────────────────────────────────

        public static bool HasStreamedTextureDictLoaded(string textureDict)
            => Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, textureDict);

        public static void RequestStreamedTextureDict(string textureDict)
            => Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, textureDict, 0);

        // ─────────────────────────────────────────────────────────────────────
        // Drawing
        // ─────────────────────────────────────────────────────────────────────

        public static void DrawRect(
            float x, float y, float width, float height,
            int r, int g, int b, int a,
            bool stereo = false)
        {
            Function.Call(Hash.DRAW_RECT, x, y, width, height, r, g, b, a, stereo);
        }

        public static void DrawSprite(
            string textureDict, string textureName,
            float screenX, float screenY,
            float width, float height,
            float heading,
            int red, int green, int blue, int alpha)
        {
            Function.Call(Hash.DRAW_SPRITE,
                textureDict, textureName,
                screenX, screenY, width, height,
                heading, red, green, blue, alpha,
                false, 0);
        }

        public static void DrawMarkerSphere(
            Vector3 xyz, float radius,
            int red, int green, int blue, float alpha)
        {
            Function.Call(Hash.DRAW_MARKER_SPHERE,
                xyz.X, xyz.Y, xyz.Z, radius, red, green, blue, alpha);
        }

        /// <summary>
        /// Draws a marker. See MarkerTypes enum for available shapes.
        /// p19 controls LoD distance — 2 is the standard value.
        /// </summary>
        public static void DrawMarker(
            MarkerTypes type,
            Vector3 pos,
            Vector3 dir,
            Vector3 rot,
            Vector3 scale,
            int red, int green, int blue, int alpha,
            bool bobUpAndDown = false,
            bool faceCamera = false,
            int p19 = 2,
            bool rotateY = false,
            string textureDict = null,
            string textureName = null,
            bool drawOnEnts = false)
        {
            Function.Call(Hash.DRAW_MARKER,
                (int)type,
                pos.X, pos.Y, pos.Z,
                dir.X, dir.Y, dir.Z,
                rot.X, rot.Y, rot.Z,
                scale.X, scale.Y, scale.Z,
                red, green, blue, alpha,
                bobUpAndDown, faceCamera, p19, rotateY,
                textureDict, textureName, drawOnEnts);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Fading marker
        //
        // The original implementation used two private static fields (toggle1,
        // integer1) shared across all call sites. Calling this for two different
        // markers in the same frame corrupted both fade states.
        //
        // Replaced with FadingMarker — an instance that owns its own state.
        // Usage:
        //   private FadingMarker _myMarker = new FadingMarker(alpha: 200, fadeDistance: 20f);
        //   // per tick:
        //   _myMarker.Draw(type, pos, dir, rot, scale, r, g, b);
        // ─────────────────────────────────────────────────────────────────────

        // ─────────────────────────────────────────────────────────────────────
        // Post FX
        // ─────────────────────────────────────────────────────────────────────

        public static void AnimpostFXPlay(string effectName, int duration, bool looped)
            => Function.Call(Hash.ANIMPOSTFX_PLAY, effectName, duration, looped);

        public static void AnimpostFXStopAll()
            => Function.Call(Hash.ANIMPOSTFX_STOP_ALL);

        // ─────────────────────────────────────────────────────────────────────
        // Timecycle modifiers
        // ─────────────────────────────────────────────────────────────────────

        public static void SetTimeCycleModifier(string modifierName)
            => Function.Call(Hash.SET_TIMECYCLE_MODIFIER, modifierName);

        public static void SetTransitionTimecycleModifier(string modName, float transitionTime)
            => Function.Call(Hash.SET_TRANSITION_TIMECYCLE_MODIFIER, modName, transitionTime);

        public static void ClearTimecycleModifier()
            => Function.Call(Hash.CLEAR_TIMECYCLE_MODIFIER);

        // ─────────────────────────────────────────────────────────────────────
        // Color helpers
        // ─────────────────────────────────────────────────────────────────────

        public static string ColorToHex(Color color)
            => $"{color.R:X2}{color.G:X2}{color.B:X2}";

        public static string ToColorHexString(Color color, string text)
            => $"<FONT COLOR='#{ColorToHex(color)}'>{text}</FONT>";

        // ─────────────────────────────────────────────────────────────────────
        // Marker types enum
        // ─────────────────────────────────────────────────────────────────────

        public enum MarkerTypes
        {
            UpsideDownCone = 0,
            VerticalCylinder,
            ThickChevronUp,
            ThinChevronUp,
            CheckeredFlagRect,
            CheckeredFlagCircle,
            VerticalCircle,
            PlaneModel,
            LostMCDark,
            LostMCLight,
            Number0,
            Number1,
            Number2,
            Number3,
            Number4,
            Number5,
            Number6,
            Number7,
            Number8,
            Number9,
            ChevronUpx1,
            ChevronUpx2,
            ChevronUpx3,
            HorizontalCircleFat,
            ReplayIcon,
            HorizontalCircleSkinny,
            HorizontalCircleSkinnyArrow,
            HorizontalSplitArrowCircle,
            DebugSphere,
            DollarSign,
            HorizontalBars,
            WolfHead,
            QuestionMark,
            PlaneSymbol,
            HelicopterSymbol,
            BoatSymbol,
            CarSymbol,
            MotorcycleSymbol,
            BikeSymbol,
            TruckSymbol,
            ParachuteSymbol,
            Thruster_Jetpack,
            SawbladeSymbol,
            Box
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FadingMarker — replaces the broken static-field fade implementation
    //
    // Each instance owns its own fade state so multiple markers can fade
    // independently in the same frame without corrupting each other.
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// A marker that fades in when the player enters range and fades out when
    /// they leave. Create one instance per marker location and call Draw() each tick.
    /// </summary>
    public class FadingMarker
    {
        private readonly int _targetAlpha;
        private readonly float _fadeDistance;
        private readonly int _fadeStep;

        private int _currentAlpha;
        private bool _playerInRange;

        public FadingMarker(int alpha = 200, float fadeDistance = 20f, int fadeStep = 2)
        {
            _targetAlpha = alpha;
            _fadeDistance = fadeDistance;
            _fadeStep = Math.Max(1, fadeStep);
            _currentAlpha = 0;
        }

        /// <summary>
        /// Updates alpha and draws the marker. Call once per OnTick.
        /// </summary>
        public void Draw(
            fGraphics.MarkerTypes type,
            Vector3 pos,
            Vector3 dir,
            Vector3 rot,
            Vector3 scale,
            int red, int green, int blue,
            bool bobUpAndDown = false,
            bool faceCamera = false,
            int p19 = 2,
            bool rotateY = false,
            string textureDict = null,
            string textureName = null,
            bool drawOnEnts = false)
        {
            // Don't update alpha while wheels/pause menu are open
            bool menuOpen = fHud.IsHudComponentActive(HudComponent.WeaponWheel)
                         || fHud.IsHudComponentActive(HudComponent.RadioStationsWheel)
                         || fHud.IsPauseMenuActive();

            if (!menuOpen)
            {
                bool inRange = fPlayer.GetDistanceTo(pos) < _fadeDistance;

                if (inRange && _currentAlpha < _targetAlpha)
                    _currentAlpha = Math.Min(_targetAlpha, _currentAlpha + _fadeStep);
                else if (!inRange && _currentAlpha > 0)
                    _currentAlpha = Math.Max(0, _currentAlpha - _fadeStep);
            }

            if (_currentAlpha <= 0) return;

            fGraphics.DrawMarker(type, pos, dir, rot, scale,
                red, green, blue, _currentAlpha,
                bobUpAndDown, faceCamera, p19, rotateY,
                textureDict, textureName, drawOnEnts);
        }
    }
}