using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IMNOTEPIX.Framework.fCore
{
    public static class AfterAbort
    {
        private const string LOG_CAT = "AfterAbort";
        private static readonly string PendingFile = @"scripts\IMNOTEPIX\Data\AfterAbortPending.txt";

        // key -> factory that produces a fresh coroutine each time it's started
        private static readonly Dictionary<string, Func<IEnumerator>> _handlers = new Dictionary<string, Func<IEnumerator>>();

        // key -> in-progress coroutine
        private static readonly Dictionary<string, IEnumerator> _active = new Dictionary<string, IEnumerator>();

        // key -> Environment.TickCount at which the coroutine may resume
        private static readonly Dictionary<string, int> _resumeAt = new Dictionary<string, int>();

        /// <summary>
        /// Register a coroutine handler. Inside it, use "yield return AfterAbort.WaitFor(ms);"
        /// to pause between game ticks WITHOUT calling SHVDN's Script.Wait(). Example:
        ///
        /// AfterAbort.Register("myKey", () => MyRoutine());
        ///
        /// static IEnumerator MyRoutine()
        /// {
        ///     DoSomething();
        ///     yield return AfterAbort.WaitFor(1000);
        ///     DoSomethingElseAfterOneSecond();
        /// }
        /// </summary>
        public static void Register(string key, Func<IEnumerator> handler)
            => _handlers[key] = handler;

        private static IEnumerator RunOnce(Action handler)
        {
            handler();
            yield break;
        }

        /// <summary>Yield this from a coroutine handler to pause for N milliseconds, non-blocking.</summary>
        public static object WaitFor(int milliseconds) => milliseconds;

        /// <summary>Call from your Aborted handler. Writes immediately to disk.</summary>
        public static void Request(string key)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PendingFile));
                File.AppendAllText(PendingFile, key + Environment.NewLine);
            }
            catch { /* best-effort */ }
        }

        /// <summary>
        /// Cancels a pending or in-progress request for the given key.
        /// Removes it from disk so it won't be picked up on the next Process(),
        /// and if it's already running, stops it immediately (no further MoveNext calls).
        /// </summary>
        public static void Unrequest(string key)
        {
            // stop it if it's already active
            _active.Remove(key);
            _resumeAt.Remove(key);

            // remove it from the pending file so StartNewlyPendingKeys() won't restart it
            try
            {
                if (!File.Exists(PendingFile)) return;

                var remaining = File.ReadAllLines(PendingFile)
                    .Where(k => !string.IsNullOrWhiteSpace(k) && k != key)
                    .Distinct()
                    .ToArray();

                if (remaining.Length > 0)
                    File.WriteAllLines(PendingFile, remaining);
                else
                    File.Delete(PendingFile);
            }
            catch (IOException) { /* best-effort, same as Request() */ }
        }

        /// <summary>
        /// Call every tick from any long-lived script. Never blocks - safe to call
        /// from OnTick even while other handlers are "waiting".
        /// </summary>
        public static void Process()
        {
            StartNewlyPendingKeys();

            if (_active.Count == 0) return;

            int now = Environment.TickCount;
            List<string> finished = null;

            foreach (var pair in _active.ToList())
            {
                string key = pair.Key;
                IEnumerator routine = pair.Value;

                // still "waiting"? skip without blocking anything.
                if (_resumeAt.TryGetValue(key, out int resumeAt) && unchecked(now - resumeAt) < 0)
                    continue;

                _resumeAt.Remove(key);

                bool moved;
                try { moved = routine.MoveNext(); }
                catch (Exception ex)
                {
                    Logger.Error(LOG_CAT, $"Handler '{key}' threw: {ex}");
                    moved = false;
                }

                if (!moved)
                {
                    if (finished == null) finished = new List<string>();
                    finished.Add(key);
                    continue;
                }

                if (routine.Current is int ms && ms > 0)
                    _resumeAt[key] = now + ms;
                // yield return null (or anything else) just means "continue next tick"
            }

            if (finished != null)
            {
                foreach (var key in finished) _active.Remove(key);
                SavePendingFile();
            }
        }

        private static void StartNewlyPendingKeys()
        {
            if (!File.Exists(PendingFile)) return;

            string[] keys;
            try
            {
                keys = File.ReadAllLines(PendingFile).Where(k => !string.IsNullOrWhiteSpace(k)).Distinct().ToArray();
            }
            catch (IOException) { return; }

            foreach (var key in keys)
            {
                if (_active.ContainsKey(key)) continue;                     // already running
                if (!_handlers.TryGetValue(key, out var factory)) continue; // no handler registered yet, leave pending

                try
                {
                    _active[key] = factory();
                }
                catch (Exception ex)
                {
                    Logger.Error(LOG_CAT, $"Handler '{key}' threw on start: {ex}");
                }
            }
        }

        private static void SavePendingFile()
        {
            try
            {
                if (!File.Exists(PendingFile)) return;

                // keep on disk: anything still actively running, and anything with no handler yet
                var keysStillOnDisk = File.ReadAllLines(PendingFile)
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Distinct()
                    .Where(k => _active.ContainsKey(k) || !_handlers.ContainsKey(k))
                    .ToArray();

                if (keysStillOnDisk.Length > 0)
                    File.WriteAllLines(PendingFile, keysStillOnDisk);
                else
                    File.Delete(PendingFile);
            }
            catch (IOException) { }
        }
    }
}