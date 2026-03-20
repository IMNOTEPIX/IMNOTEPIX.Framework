using GTA;
using System;

namespace IMNOTEPIX.Framework.Core
{
    /// <summary>
    /// Frame-rate-independent countdown timer driven by Game.LastFrameTime.
    /// Call Update() once per tick from your OnTick handler.
    ///
    /// Note: if Update() is called infrequently and a large frame delta causes
    /// _remainingMs to go significantly negative, the Expired event fires once
    /// and Loop restarts from the full Duration rather than compensating for
    /// the overflow. For most mission use cases this is acceptable.
    /// </summary>
    public class GameTimer
    {
        // ─────────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────────

        private float _durationMs;
        private float _remainingMs;
        private bool _running;

        /// <summary>
        /// Total duration in milliseconds.
        /// Setting this does not affect RemainingMilliseconds mid-run — call
        /// Reset() afterwards if you want to restart from the new duration.
        /// </summary>
        public float Duration
        {
            get => _durationMs;
            set => _durationMs = Math.Max(0f, value);
        }

        public bool Loop { get; set; }
        public bool IsRunning => _running;
        public float RemainingMilliseconds => _remainingMs;

        /// <summary>0.0–1.0 progress through the current cycle. 1.0 = just started.</summary>
        public float Progress => _durationMs > 0f
            ? Math.Max(0f, _remainingMs / _durationMs)
            : 0f;

        public event Action<GameTimer> Expired;

        // ─────────────────────────────────────────────────────────────────────
        // Construction
        // ─────────────────────────────────────────────────────────────────────

        public GameTimer(float durationMs, bool loop = false)
        {
            _durationMs = Math.Max(0f, durationMs);
            _remainingMs = _durationMs;
            Loop = loop;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Control
        // ─────────────────────────────────────────────────────────────────────

        public void Start() => _running = true;
        public void Stop() => _running = false;

        /// <summary>Resets remaining time to Duration without changing running state.</summary>
        public void Reset() => _remainingMs = _durationMs;

        /// <summary>Resets and starts. Equivalent to Reset() + Start().</summary>
        public void Restart()
        {
            _remainingMs = _durationMs;
            _running = true;
        }

        public void AddTime(float ms)
            => _remainingMs += ms;

        public void RemoveTime(float ms)
            => _remainingMs = Math.Max(0f, _remainingMs - ms);

        // ─────────────────────────────────────────────────────────────────────
        // Tick
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Advances the timer by one frame. Call once per OnTick.
        /// <paramref name="timeScale"/> can slow or speed the timer independently
        /// of the game's own time scale.
        /// </summary>
        public void Update(float timeScale = 1f)
        {
            if (!_running) return;

            _remainingMs -= Game.LastFrameTime * 1000f * timeScale;

            if (_remainingMs <= 0f)
            {
                Expired?.Invoke(this);

                if (Loop)
                    _remainingMs = _durationMs;
                else
                    Stop();
            }
        }
    }
}