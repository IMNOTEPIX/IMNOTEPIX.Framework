using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fExtension
{
    public static class EntityExtensions
    {
        private struct FadeEntry
        {
            public Entity Entity;
            public bool FadeIn;             // true = fading toward alpha 255, false = toward alpha 0
            public bool DeleteOnComplete;

            public float FlickerDuration;   // seconds of free jitter before committing to the target
            public float PhaseOffset;       // desyncs simultaneous fades from each other

            public float Timer;             // f_0 equivalent - accumulated seconds
            public bool Finished;           // f_1 equivalent
        }

        private static readonly List<FadeEntry> _pending = new List<FadeEntry>();
        private static readonly Random _rng = new Random();

        public const float DefaultFadeDuration = 0.75f;

        /// <summary>
        /// Begins fading this entity in using Rockstar's flickering pulse pattern
        /// (reverse engineered from func_7326/func_7327/func_7329).
        /// </summary>
        public static void FadeIn(this Entity entity, float duration = DefaultFadeDuration)
        {
            if (entity == null || !entity.Exists()) return;

            RemovePending(entity);

            Function.Call(Hash.SET_ENTITY_ALPHA, entity, 0, false);

            _pending.Add(new FadeEntry
            {
                Entity = entity,
                FadeIn = true,
                DeleteOnComplete = false,
                FlickerDuration = duration,
                PhaseOffset = NextPhaseOffset(),
                Timer = 0f,
                Finished = false
            });
        }

        /// <summary>
        /// Begins fading this entity out to invisible using Rockstar's flickering pulse pattern,
        /// then optionally deletes it once it reaches alpha 0.
        /// </summary>
        public static void FadeOut(this Entity entity, float duration = DefaultFadeDuration, bool deleteOnComplete = false)
        {
            if (entity == null || !entity.Exists()) return;

            RemovePending(entity);

            _pending.Add(new FadeEntry
            {
                Entity = entity,
                FadeIn = false,
                DeleteOnComplete = deleteOnComplete,
                FlickerDuration = duration,
                PhaseOffset = NextPhaseOffset(),
                Timer = 0f,
                Finished = false
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

                if (!entry.Entity.Exists())
                {
                    _pending.RemoveAt(i);
                    continue;
                }

                int alpha = ComputeAlpha(ref entry);
                Function.Call(Hash.SET_ENTITY_ALPHA, entry.Entity, alpha, false);

                if (entry.Finished)
                {
                    if (entry.FadeIn)
                    {
                        Function.Call(Hash.RESET_ENTITY_ALPHA, entry.Entity);
                    }
                    else if (entry.DeleteOnComplete)
                    {
                        entry.Entity.Delete();
                    }

                    _pending.RemoveAt(i);
                    continue;
                }

                _pending[i] = entry;
            }
        }

        // ── Core math (func_7327 / func_7328, generalized for both directions) ─

        private static int ComputeAlpha(ref FadeEntry entry)
        {
            float dt = Game.LastFrameTime;
            entry.Timer += dt;

            float wave = (float)Math.Sin((entry.Timer * 1000f) + entry.PhaseOffset);

            bool pastDuration = entry.Timer >= entry.FlickerDuration;

            if (pastDuration)
            {
                // Past the free-jitter window: stop letting the wave swing back toward
                // the "wrong" side, only ever move toward the target (0 or 255) from here.
                wave = entry.FadeIn ? Math.Abs(wave) : -Math.Abs(wave);
            }

            float normalized = (wave + 1f) * 0.5f; // remap [-1,1] -> [0,1]
            int alpha = (int)Math.Round(normalized * 255f, MidpointRounding.AwayFromZero);

            if (pastDuration)
            {
                bool reachedTarget = entry.FadeIn ? alpha >= 255 : alpha <= 0;
                if (reachedTarget)
                {
                    entry.Finished = true;
                    alpha = entry.FadeIn ? 255 : 0;
                }
            }

            return alpha;
        }

        private static void RemovePending(Entity entity)
        {
            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                if (_pending[i].Entity == entity)
                    _pending.RemoveAt(i);
            }
        }

        private static float NextPhaseOffset()
        {
            // R* hardcodes 500; randomizing keeps the same feel while desyncing multiple entities fading at the same time so they don't flicker in lockstep.
            return (float)(_rng.NextDouble() * 1000.0);
        }
    }
}