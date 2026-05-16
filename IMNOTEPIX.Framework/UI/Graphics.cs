using GTA.Math;
using GTA.Native;
using System.Drawing;

namespace IMNOTEPIX.Framework.fUI
{
    public class Graphics
    {
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

        public static void ResetScriptGfxAlign()
            => Function.Call(Hash.RESET_SCRIPT_GFX_ALIGN);

        public static void SetScriptGfxAlignParams(float x, float y, float w, float h)
            => Function.Call(Hash.SET_SCRIPT_GFX_ALIGN_PARAMS, x, y, w, h);

        public static void SetScriptGfxAlign(int horizontalAlign, int verticalAlign)
            => Function.Call(Hash.SET_SCRIPT_GFX_ALIGN, horizontalAlign, verticalAlign);

        public static bool HasStreamedTextureDictLoaded(string textureDict)
            => Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, textureDict);

        public static void RequestStreamedTextureDict(string textureDict)
            => Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, textureDict, 0);

        public static void DrawRect(float x, float y, float width, float height, int r, int g, int b, int a, bool stereo = false)
           => Function.Call(Hash.DRAW_RECT, x, y, width, height, r, g, b, a, stereo);

        public static void DrawSprite(string textureDict, string textureName, float screenX, float screenY, float width, float height, float heading, int red, int green, int blue, int alpha)
           => Function.Call(Hash.DRAW_SPRITE, textureDict, textureName, screenX, screenY, width, height, heading, red, green, blue, alpha, false, 0);

        public static void DrawMarkerSphere(Vector3 xyz, float radius, int red, int green, int blue, float alpha)
           => Function.Call(Hash.DRAW_MARKER_SPHERE, xyz.X, xyz.Y, xyz.Z, radius, red, green, blue, alpha);

        /// <summary>
        /// Draws a marker. See MarkerTypes enum for available shapes.
        /// p19 controls LoD distance — 2 is the standard value.
        /// </summary>
        public static void DrawMarker(MarkerTypes type, Vector3 pos, Vector3 dir, Vector3 rot, Vector3 scale, int red, int green, int blue, int alpha, bool bobUpAndDown = false, bool faceCamera = false, int p19 = 2, bool rotateY = false, string textureDict = null, string textureName = null, bool drawOnEnts = false)
           => Function.Call(Hash.DRAW_MARKER, (int)type, pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, rot.X, rot.Y, rot.Z, scale.X, scale.Y, scale.Z, red, green, blue, alpha, bobUpAndDown, faceCamera, p19, rotateY, textureDict, textureName, drawOnEnts);

        public static void AnimpostFXPlay(string effectName, int duration, bool looped)
            => Function.Call(Hash.ANIMPOSTFX_PLAY, effectName, duration, looped);

        public static void AnimpostFXStopAll()
            => Function.Call(Hash.ANIMPOSTFX_STOP_ALL);

        public static void SetTimeCycleModifier(string modifierName)
            => Function.Call(Hash.SET_TIMECYCLE_MODIFIER, modifierName);

        public static void SetTransitionTimecycleModifier(string modName, float transitionTime)
            => Function.Call(Hash.SET_TRANSITION_TIMECYCLE_MODIFIER, modName, transitionTime);

        public static void ClearTimecycleModifier()
            => Function.Call(Hash.CLEAR_TIMECYCLE_MODIFIER);

        public static string ColorToHex(Color color)
            => $"{color.R:X2}{color.G:X2}{color.B:X2}";

        public static string ToColorHexString(Color color, string text)
            => $"<FONT COLOR='#{ColorToHex(color)}'>{text}</FONT>";

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
}
