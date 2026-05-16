using GTA;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fGameplay.Combat
{
    /// <summary>
    /// Holds every active <see cref="CombatAgent"/> and drives their per-tick updates.
    /// <see cref="CombatAgent"/> registers itself automatically on construction.
    /// </summary>
    public class CombatRegistry
    {
        public static IReadOnlyList<CombatAgent> All => _peds;

        private static readonly List<CombatAgent> _peds = new List<CombatAgent>();

        private static float _lastCheckTime;
        private static float _lastActivateTime;
        private const float TickInterval = 700f;

        internal static void Register(CombatAgent ped)
        {
            if (!_peds.Contains(ped))
                _peds.Add(ped);
        }

        /// <summary>Throttled: checks alive state and blip visibility for every registered ped.</summary>
        public static void Check()
        {
            if (Game.GameTime - _lastCheckTime < TickInterval)
                return;
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
            if (Game.GameTime - _lastActivateTime < TickInterval)
                return;
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
