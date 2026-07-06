using GTA;
using GTA.Native;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fExtension
{
    public static class EntityExtensions
    {
        private struct FadeEntry
        {
            public Entity Entity;
            public float Alpha;
            public float Duration;
            public bool DeleteOnComplete;

            // Flicker state
            public bool Flickering;
            public float ElapsedTime;
            public float FlickerTime;   // timer within current flicker pulse
            public int FlickerCount;    // how many pulses have fired
        }

        private static readonly List<FadeEntry> _pending = new List<FadeEntry>();

        public const float DefaultFadeDuration = 0.75f;

        // ── Extension methods ──────────────────────────────────────────────

        /// <summary>
        /// Begins fading this entity in using Rockstar's flickering pulse pattern.
        /// </summary>
        public static void FadeIn(this Entity entity, float duration = DefaultFadeDuration)
        {
            if (entity == null || !entity.Exists()) return;

            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                if (_pending[i].Entity == entity)
                    _pending.RemoveAt(i);
            }

            Function.Call(Hash.SET_ENTITY_ALPHA, entity, 0, false);

            _pending.Add(new FadeEntry
            {
                Entity = entity,
                Alpha = 0f,
                Duration = duration,
                ElapsedTime = 0f,
                Flickering = true,
                FlickerTime = 0f,
                FlickerCount = 0
            });
        }

        /// <summary>
        /// Begins fading this entity out to invisible using a flickering pulse, then optionally deletes it.
        /// </summary>
        public static void FadeOut(this Entity entity, float duration = DefaultFadeDuration, bool deleteOnComplete = false)
        {
            if (entity == null || !entity.Exists()) return;

            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                if (_pending[i].Entity == entity)
                    _pending.RemoveAt(i);
            }

            var currentAlpha = Function.Call<int>(Hash.GET_ENTITY_ALPHA, entity);

            _pending.Add(new FadeEntry
            {
                Entity = entity,
                Alpha = currentAlpha,
                Duration = -duration,
                DeleteOnComplete = deleteOnComplete,
                Flickering = true,      // flicker before the smooth ramp down
                ElapsedTime = 0f,
                FlickerTime = 0f,
                FlickerCount = 0
            });
        }

        // ── Tick ──────────────────────────────────────────────────────────

        /// <summary>
        /// Call this every tick from your script's OnTick.
        /// </summary>
        public static void Update()
        {
            if (_pending.Count == 0) return;

            float dt = Game.LastFrameTime;

            const int FlickerPulses = 4;
            const float FlickerPulseDur = 0.08f;
            const int FlickerLowAlpha = 20;
            const int FlickerHighAlpha = 90;

            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                var entry = _pending[i];

                if (!entry.Entity.Exists())
                {
                    _pending.RemoveAt(i);
                    continue;
                }

                bool fadingIn = entry.Duration > 0f;
                float step = (255f / System.Math.Abs(entry.Duration)) * dt;

                // ── Fade in (with flicker phase) ──────────────────────────────
                if (fadingIn)
                {
                    if (entry.Flickering)
                    {
                        entry.ElapsedTime += dt;
                        entry.FlickerTime += dt;

                        if (entry.FlickerTime >= FlickerPulseDur)
                        {
                            entry.FlickerTime -= FlickerPulseDur;
                            entry.FlickerCount++;
                        }

                        if (entry.FlickerCount < FlickerPulses * 2)
                        {
                            int targetAlpha = (entry.FlickerCount % 2 == 0)
                                ? FlickerLowAlpha
                                : FlickerHighAlpha;

                            Function.Call(Hash.SET_ENTITY_ALPHA, entry.Entity, targetAlpha, false);
                            _pending[i] = entry;
                            continue;
                        }

                        // Flicker done — hand off to smooth ramp
                        entry.Flickering = false;
                        entry.Alpha = FlickerHighAlpha;
                    }

                    entry.Alpha += step;

                    if (entry.Alpha >= 255f)
                    {
                        Function.Call(Hash.RESET_ENTITY_ALPHA, entry.Entity);
                        _pending.RemoveAt(i);
                        continue;
                    }
                }
                // ── Fade out (flicker then smooth ramp down) ──────────────────────
                else
                {
                    if (entry.Flickering)
                    {
                        entry.ElapsedTime += dt;
                        entry.FlickerTime += dt;

                        if (entry.FlickerTime >= FlickerPulseDur)
                        {
                            entry.FlickerTime -= FlickerPulseDur;
                            entry.FlickerCount++;
                        }

                        if (entry.FlickerCount < FlickerPulses * 2)
                        {
                            // Fade-out flicker is reversed: starts high, pulses down
                            int targetAlpha = (entry.FlickerCount % 2 == 0)
                                ? FlickerHighAlpha
                                : FlickerLowAlpha;

                            Function.Call(Hash.SET_ENTITY_ALPHA, entry.Entity, targetAlpha, false);
                            _pending[i] = entry;
                            continue;
                        }

                        // Flicker done — hand off to smooth ramp down
                        entry.Flickering = false;
                        entry.Alpha = FlickerHighAlpha;
                    }

                    entry.Alpha -= step;

                    if (entry.Alpha <= 0f)
                    {
                        Function.Call(Hash.SET_ENTITY_ALPHA, entry.Entity, 0, false);

                        if (entry.DeleteOnComplete)
                            entry.Entity.Delete();

                        _pending.RemoveAt(i);
                        continue;
                    }
                }

                Function.Call(Hash.SET_ENTITY_ALPHA, entry.Entity, (int)entry.Alpha, false);
                _pending[i] = entry;
            }
        }
    }
}