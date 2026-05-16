using GTA;
using GTA.Native;
using GTA.UI;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// SHVDN Script that renders all registered HUD bars each tick.
    ///
    /// _activeBars is static so registrations survive SHVDN reloads.
    /// No Aborted handler is needed — on reload, the new HudBarController
    /// instance re-subscribes its Tick and the existing bar list is reused.
    /// </summary>
    public class HudBarController : GTA.Script
    {
        private static readonly List<HudBar> _activeBars = new List<HudBar>();

        public static bool ForceMoveUp { get; set; }

        public HudBarController()
        {
            Tick += HandleTick;
        }

        public static void Register(HudBar bar)
        {
            if (bar != null && !_activeBars.Contains(bar))
                _activeBars.Add(bar);
        }

        public static void Unregister(HudBar bar)
            => _activeBars.Remove(bar);

        public static void ClearAll()
            => _activeBars.Clear();

        public static bool IsRegistered(HudBar bar)
            => bar != null && _activeBars.Contains(bar);

        public static int Count => _activeBars.Count;

        private void HandleTick(object sender, EventArgs e)
        {
            if (!CanDraw())
                return;

            Graphics.RequestStreamedTextureDict("timerbars");
            if (!Graphics.HasStreamedTextureDictLoaded("timerbars"))
                return;

            HideDefaultHud();
            DrawBars();
        }

        private static bool CanDraw()
        {
            if (_activeBars.Count == 0)
                return false;
            if (!Function.Call<bool>(Hash.IS_HUD_PREFERENCE_SWITCHED_ON))
                return false;
            if (Function.Call<bool>(Hash.IS_HUD_HIDDEN))
                return false;
            if (Function.Call<bool>(Hash.IS_RADAR_HIDDEN))
                return false;
            if (Function.Call<bool>(Hash.IS_CUTSCENE_PLAYING))
                return false;
            return Game.Player.Character.IsAlive;
        }

        private static void HideDefaultHud()
        {
            // Hide the default SHVDN timer bar slots (6–9)
            Hud.HideHudComponentThisFrame(6);
            Hud.HideHudComponentThisFrame(7);
            Hud.HideHudComponentThisFrame(8);
            Hud.HideHudComponentThisFrame(9);

            Graphics.SetScriptGfxAlign(82, 66);
            Graphics.SetScriptGfxAlignParams(0, 0, HudBarDraw.AlignWidth, HudBarDraw.AlignHeight);
        }

        private static void DrawBars()
        {
            float y = (LoadingPrompt.IsActive || ForceMoveUp) ? HudBarDraw.BusyY : HudBarDraw.DefaultY;

            // Iterate backwards — allows safe removal of expired countdown bars
            for (int i = _activeBars.Count - 1; i >= 0; i--)
            {
                var bar = _activeBars[i];

                if (bar == null)
                {
                    _activeBars.RemoveAt(i);
                    continue;
                }

                bar.Draw(y);
                y -= bar.VerticalSpacing;

                if (bar is HudCountdownBar countdown && countdown.ShouldRemove)
                    _activeBars.RemoveAt(i);
            }

            Graphics.ResetScriptGfxAlign();
        }
    }
}
