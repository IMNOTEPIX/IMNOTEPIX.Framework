using GTA;
using System;

namespace IMNOTEPIX.Framework.fCore
{
    /// <summary>
    /// Frame-rate-independent countdown timer driven by Game.LastFrameTime.
    /// Call Update() once per tick from your OnTick handler.
    /// </summary>
    /// 
    /// Note: if Update() is called infrequently and a large frame delta causes
    /// _remainingMs to go significantly negative, the Expired event fires once
    /// and Loop restarts from the full Duration rather than compensating for
    /// the overflow. For most mission use cases this is acceptable.
    public class GameTimer
    {
        private float durationMs;
        private float remainingMs;
        private bool running;

        /// <summary>
        /// Total duration in milliseconds.
        /// Setting this does not affect RemainingMilliseconds mid-run — call
        /// Reset() afterwards if you want to restart from the new duration.
        /// </summary>
        public float Duration
        {
            get => durationMs;
            set => durationMs = System.Math.Max(0f, value);
        }

        public bool Loop { get; set; }
        public bool IsRunning => running;
        public float RemainingMilliseconds => remainingMs;

        /// <summary>0.0–1.0 progress through the current cycle. 1.0 = just started.</summary>
        public float Progress => durationMs > 0f ? System.Math.Max(0f, 1f - (remainingMs / durationMs)) : 0f;

        public event Action<GameTimer> Expired;

        public GameTimer(float durationMs, bool loop = false)
        {
            this.durationMs = System.Math.Max(0f, durationMs);
            remainingMs = this.durationMs;
            Loop = loop;
        }

        public void Start() => running = true;
        public void Stop() => running = false;

        /// <summary>Resets remaining time to Duration without changing running state.</summary>
        public void Reset() => remainingMs = durationMs;

        /// <summary>Resets and starts. Equivalent to Reset() + Start().</summary>
        public void Restart()
        {
            remainingMs = durationMs;
            running = true;
        }

        public void AddTime(float ms)
            => remainingMs = System.Math.Min(durationMs, remainingMs + ms);

        public void RemoveTime(float ms)
            => remainingMs = System.Math.Max(0f, remainingMs - ms);

        /// <summary>
        /// Advances the timer by one frame. <b>Call once per OnTick.</b>
        /// <paramref name="timeScale"/> can slow or speed the timer independently of the game's own time scale.
        /// </summary>
        public void Update(float timeScale = 1f)
        {
            if (!running)
                return;

            remainingMs -= Game.LastFrameTime * 1000f * timeScale;

            if (remainingMs <= 0f)
            {
                Expired?.Invoke(this);

                if (Loop)
                    remainingMs = durationMs;
                else
                    Stop();
            }
        }
    }
}
