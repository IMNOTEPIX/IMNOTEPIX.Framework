using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fExtension
{
    public static class BlipExtensions
    {
        private struct FadeEntry
        {
            public Blip Blip;
            public int FromAlpha;
            public int ToAlpha;
            public float Duration;  // seconds
            public float Timer;     // accumulated seconds
        }

        private static readonly List<FadeEntry> _pending = new List<FadeEntry>();

        public const float DefaultFadeDuration = 0.75f;

        /// <summary>
        /// Begins fading this blip in from alpha 0 to 255 over the given duration.
        /// </summary>
        public static void FadeIn(this Blip blip, float duration = DefaultFadeDuration)
        {
            if (blip == null || !blip.Exists()) return;

            RemovePending(blip);

            Function.Call(Hash.SET_BLIP_ALPHA, blip, 0);

            _pending.Add(new FadeEntry
            {
                Blip = blip,
                FromAlpha = 0,
                ToAlpha = 255,
                Duration = duration,
                Timer = 0f
            });
        }

        /// <summary>
        /// Begins fading this blip out from its current alpha to 0 over the given duration.
        /// </summary>
        public static void FadeOut(this Blip blip, float duration = DefaultFadeDuration)
        {
            if (blip == null || !blip.Exists()) return;

            RemovePending(blip);

            int currentAlpha = Function.Call<int>(Hash.GET_BLIP_ALPHA, blip);

            _pending.Add(new FadeEntry
            {
                Blip = blip,
                FromAlpha = currentAlpha,
                ToAlpha = 0,
                Duration = duration,
                Timer = 0f
            });
        }

        /// <summary>
        /// Call this every tick from your script's OnTick.
        /// </summary>
        public static void Update()
        {
            if (_pending.Count == 0) return;

            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                var entry = _pending[i];

                if (!entry.Blip.Exists())
                {
                    _pending.RemoveAt(i);
                    continue;
                }

                entry.Timer += Game.LastFrameTime;

                float t = entry.Duration <= 0f ? 1f : Math.Min(entry.Timer / entry.Duration, 1f);
                int alpha = (int)Math.Round(entry.FromAlpha + (entry.ToAlpha - entry.FromAlpha) * t);

                Function.Call(Hash.SET_BLIP_ALPHA, entry.Blip, alpha);

                if (t >= 1f)
                {
                    _pending.RemoveAt(i);
                    continue;
                }

                _pending[i] = entry;
            }
        }

        private static void RemovePending(Blip blip)
        {
            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                if (_pending[i].Blip == blip)
                    _pending.RemoveAt(i);
            }
        }
    }
}