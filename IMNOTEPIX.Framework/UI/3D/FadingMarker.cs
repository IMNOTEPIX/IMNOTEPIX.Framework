using GTA.Math;
using GTA.UI;
using System;

namespace IMNOTEPIX.Framework.fUI._3D
{
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
    //
    //
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
        public void Draw(Graphics.MarkerTypes type, Vector3 pos, Vector3 dir, Vector3 rot, Vector3 scale, int red, int green, int blue, bool bobUpAndDown = false, bool faceCamera = false, int p19 = 2, bool rotateY = false, string textureDict = null, string textureName = null, bool drawOnEnts = false)
        {
            // Don't update alpha while wheels/pause menu are open
            bool menuOpen = Hud.IsHudComponentActive(HudComponent.WeaponWheel) || Hud.IsHudComponentActive(HudComponent.RadioStationsWheel) || Hud.IsPauseMenuActive();

            if (!menuOpen)
            {
                bool inRange = fPlayer.Player.GetDistanceTo(pos) < _fadeDistance;

                if (inRange && _currentAlpha < _targetAlpha)
                    _currentAlpha = Math.Min(_targetAlpha, _currentAlpha + _fadeStep);
                else if (!inRange && _currentAlpha > 0)
                    _currentAlpha = Math.Max(0, _currentAlpha - _fadeStep);
            }

            if (_currentAlpha <= 0) return;

            Graphics.DrawMarker(type, pos, dir, rot, scale, red, green, blue, _currentAlpha, bobUpAndDown, faceCamera, p19, rotateY, textureDict, textureName, drawOnEnts);
        }
    }
}
