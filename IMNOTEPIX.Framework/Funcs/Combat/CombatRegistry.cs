using GTA;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.Funcs.Combat
{
    /// <summary>
    /// Holds every active <see cref="CombatPed"/> and drives their per-tick updates.
    /// <see cref="CombatPed"/> registers itself automatically on construction.
    /// </summary>
    public static class CombatRegistry
    {
        public static IReadOnlyList<CombatPed> All => _peds;

        private static readonly List<CombatPed> _peds = new List<CombatPed>();

        private static float _lastCheckTime;
        private static float _lastActivateTime;
        private const float TickInterval = 700f;

        // ── Called by CombatPed constructor ───────────────────────────────────

        internal static void Register(CombatPed ped)
        {
            if (!_peds.Contains(ped))
                _peds.Add(ped);
        }

        // ── Per-tick entry points (call these from your Script.Tick) ──────────

        /// <summary>Throttled: checks alive state and blip visibility for every registered ped.</summary>
        public static void Tick()
        {
            if (Game.GameTime - _lastCheckTime < TickInterval) return;
            _lastCheckTime = Game.GameTime;

            foreach (var ped in _peds)
                ped?.Check();
        }

        /// <summary>Throttled: activates all peds that have been created.</summary>
        public static void ActivateAll()
        {
            foreach (var ped in _peds)
                ped?.Activate();
        }

        /// <summary>Throttled: activates any peds that have been created but not yet activated.</summary>
        public static void ActivatePending()
        {
            if (Game.GameTime - _lastActivateTime < TickInterval) return;
            _lastActivateTime = Game.GameTime;

            foreach (var ped in _peds)
                ped?.Activate();
        }

        /// <summary>Creates all registered peds that haven't spawned yet.</summary>
        public static void CreateAll()
        {
            foreach (var ped in _peds)
                ped?.Create();
        }

        /// <summary>Disposes and unregisters every ped.</summary>
        public static void DisposeAll()
        {
            foreach (var ped in _peds)
                ped?.Dispose();

            _peds.Clear();
        }
    }
}
