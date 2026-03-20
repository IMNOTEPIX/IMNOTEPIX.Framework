using GTA;
using GTA.Native;
using IMNOTEPIX.Framework.Funcs.Interior;
using IMNOTEPIX.Framework.Funcs.Ped;
using System;

namespace IMNOTEPIX.Framework.Funcs.Combat
{
    /// <summary>
    /// Spawns, configures, and tracks one combat ped.
    /// Automatically registers itself with <see cref="CombatRegistry"/>.
    /// </summary>
    public sealed class CombatPed : IDisposable
    {
        // ── Public state ──────────────────────────────────────────────────────
        public GTA.Ped Handle { get; private set; }
        public CombatConfig Config { get; }
        public bool Exists => Handle != null && Handle.Exists();
        public bool IsAlive => Exists && Handle.IsAlive;

        // ── Private state ─────────────────────────────────────────────────────
        private readonly int _aiTeam = World.AddRelationshipGroup("aiteam").Hash;
        private readonly int _playerTeam = Function.Call<int>(Hash.GET_HASH_KEY, "PLAYER");
        private bool _activated;
        private bool _disposed;

        // ── Constructor ───────────────────────────────────────────────────────

        public CombatPed(CombatConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            CombatRegistry.Register(this);
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Spawns the ped into the world. Call once before <see cref="Activate"/>.</summary>
        public void Create(int modelLoadTimeout = 500)
        {
            if (Exists || _disposed) return;

            Config.Model.Request(modelLoadTimeout - 100);
            Handle = World.CreatePed(Config.Model, Config.Position, Config.Heading);

            if (Handle != null && !Handle.Exists())
                Script.Yield();

            if (!Exists)
                throw new InvalidOperationException($"Failed to create combat ped at {Config.Position}.");
        }

        /// <summary>Applies all config settings to the spawned ped. Runs once.</summary>
        public void Activate()
        {
            if (!Exists || _activated || _disposed) return;

            SetupBlip();
            SetupFlags();
            SetupRelationships();
            SetupCombat();
            SetupWeapon();

            _activated = true;
        }

        /// <summary>Called every tick by <see cref="CombatRegistry"/>. Updates blip, cleans dead ped.</summary>
        public void Check()
        {
            if (!Exists || _disposed) return;

            UpdateBlipVisibility();

            if (Handle.IsDead)
            {
                Handle.AttachedBlip?.Delete();
                Handle.MarkAsNoLongerNeeded();
            }
        }

        /// <summary>Deletes the ped and its blip from the world.</summary>
        public void Dispose()
        {
            if (_disposed) return;

            if (Exists)
            {
                Handle.AttachedBlip?.Delete();
                Handle.Delete();
                Handle = null;
            }

            Config.Model.MarkAsNoLongerNeeded();
            _disposed = true;
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private void SetupBlip()
        {
            if (Handle.AttachedBlip != null) return;

            Handle.AddBlip();

            var blip = Handle.AttachedBlip;
            if (blip == null || !blip.Exists()) return;

            blip.Sprite = Config.BlipSprite;
            blip.Color = Config.BlipColor;
            blip.Name = Config.BlipName;
            blip.Scale = Config.BlipScale;
            blip.IsShortRange = true;
            blip.DisplayType = BlipDisplayType.MiniMapOnly;
        }

        private void SetupFlags()
        {
            fPed.SetPedConfigFlag(Handle, 281, !Config.CanWrithe);
            fPed.SetPedConfigFlag(Handle, 188, !Config.CanHurt);
            fPed.SetPedConfigFlag(Handle, fPed.PedConfigFlags.PCF_DontBlipCop, !Config.IsCop);
            fPed.SetPedAsCop(Handle, Config.IsCop);
        }

        private void SetupRelationships()
        {
            fPed.SetRelationshipBetweenGroups(fPed.RelationshipTypes.Hate, _aiTeam, _playerTeam);
            fPed.SetRelationshipBetweenGroups(fPed.RelationshipTypes.Hate, _playerTeam, _aiTeam);
        }

        private void SetupCombat()
        {
            fPed.SetPedCombatAbility(Handle, Config.AbilityLevel);
            fPed.SetPedCombatMovement(Handle, Config.Movement);
            Handle.Armor = 200;

            foreach (var attr in Config.EnableAttributes)
                fPed.SetPedCombatAttributes(Handle, attr, true);

            foreach (var attr in Config.DisableAttributes)
                fPed.SetPedCombatAttributes(Handle, attr, false);
        }

        private void SetupWeapon()
        {
            Handle.Weapons.Give(Config.Weapon, 10000, true, true);
            Handle.Weapons.Select(Config.Weapon);
        }

        private void UpdateBlipVisibility()
        {
            var blip = Handle.AttachedBlip;
            if (blip == null) return;

            var player = Game.Player.Character;
            bool sameInterior = fInterior.GetInteriorFromEntity(Handle) == fInterior.GetInteriorFromEntity(player);
            bool sameRoom = fInterior.GetRoomKeyFromEntity(Handle) == fInterior.GetRoomKeyFromEntity(player);

            blip.Alpha = (sameInterior && sameRoom) ? 255 : 0;
        }
    }
}
