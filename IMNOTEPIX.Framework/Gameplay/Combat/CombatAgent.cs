using GTA;
using GTA.Native;
using System;

namespace IMNOTEPIX.Framework.fGameplay.Combat
{
    /// <summary>
    /// Spawns, configures, and tracks one combat ped.
    /// Automatically registers itself with <see cref="CombatRegistry"/>.
    /// </summary>
    public sealed class CombatAgent : IDisposable
    {
        public GTA.Ped Handle { get; private set; }
        public CombatConfig Config { get; }
        public bool Exists => Handle != null && Handle.Exists();
        public bool IsAlive => Exists && Handle.IsAlive;

        private readonly int _aiTeam = GTA.World.AddRelationshipGroup("aiteam").Hash;
        private readonly int _playerTeam = Function.Call<int>(Hash.GET_HASH_KEY, "PLAYER");
        private bool _activated;
        private bool _disposed;


        public CombatAgent(CombatConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            CombatRegistry.Register(this);
        }

        /// <summary>Spawns the ped into the world. Call once before <see cref="Activate"/>.</summary>
        public void Create(int modelLoadTimeout = 500)
        {
            if (Exists || _disposed)
                return;

            Config.Model.Request(modelLoadTimeout - 100);
            Handle = GTA.World.CreatePed(Config.Model, Config.Position, Config.Heading);

            if (Handle != null && !Handle.Exists())
                GTA.Script.Yield();

            if (!Exists)
                throw new InvalidOperationException($"Failed to create combat ped at {Config.Position}.");
        }

        /// <summary>Applies all config settings to the spawned ped. Runs once.</summary>
        public void Activate()
        {
            if (!Exists || _activated || _disposed)
                return;

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
            if (!Exists || _disposed)
                return;

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
            if (_disposed)
                return;

            if (Exists)
            {
                Handle.AttachedBlip?.Delete();
                Handle.Delete();
                Handle = null;
            }

            Config.Model.MarkAsNoLongerNeeded();
            _disposed = true;
        }

        private void SetupBlip()
        {
            if (Handle.AttachedBlip != null)
                return;

            Handle.AddBlip();

            var blip = Handle.AttachedBlip;
            if (blip == null || !blip.Exists())
                return;

            blip.Sprite = Config.BlipSprite;
            blip.Color = Config.BlipColor;
            blip.Name = Config.BlipName;
            blip.Scale = Config.BlipScale;
            blip.IsShortRange = true;
            blip.DisplayType = BlipDisplayType.MiniMapOnly;
        }

        private void SetupFlags()
        {
            fWorld.Ped.SetPedConfigFlag(Handle, 281, !Config.CanWrithe);
            fWorld.Ped.SetPedConfigFlag(Handle, 188, !Config.CanHurt);
            fWorld.Ped.SetPedConfigFlag(Handle, fWorld.Ped.PedConfigFlags.PCF_DontBlipCop, !Config.IsCop);
            fWorld.Ped.SetPedAsCop(Handle, Config.IsCop);
        }

        private void SetupRelationships()
        {
            fWorld.Ped.SetRelationshipBetweenGroups(fWorld.Ped.RelationshipTypes.Hate, _aiTeam, _playerTeam);
            fWorld.Ped.SetRelationshipBetweenGroups(fWorld.Ped.RelationshipTypes.Hate, _playerTeam, _aiTeam);
        }

        private void SetupCombat()
        {
            fWorld.Ped.SetPedCombatAbility(Handle, Config.AbilityLevel);
            fWorld.Ped.SetPedCombatMovement(Handle, Config.Movement);
            Handle.Armor = Config.Armor;
            Handle.MaxHealthFloat = Math.Max(200, Config.Health);
            Handle.HealthFloat = Config.Health;

            foreach (var attr in Config.EnableAttributes)
                fWorld.Ped.SetPedCombatAttributes(Handle, attr, true);

            foreach (var attr in Config.DisableAttributes)
                fWorld.Ped.SetPedCombatAttributes(Handle, attr, false);
        }

        private void SetupWeapon()
        {
            Handle.Weapons.Give(Config.Weapon, 10000, true, true);
            Handle.Weapons.Select(Config.Weapon);
        }

        private void UpdateBlipVisibility()
        {
            var blip = Handle.AttachedBlip;
            if (blip == null)
                return;

            var player = Game.Player.Character;
            bool sameInterior = fWorld.Interior.Interior.GetInteriorFromEntity(Handle) == fWorld.Interior.Interior.GetInteriorFromEntity(player);
            bool sameRoom = fWorld.Interior.Interior.GetRoomKeyFromEntity(Handle) == fWorld.Interior.Interior.GetRoomKeyFromEntity(player);

            blip.Alpha = (sameInterior && sameRoom) ? 255 : 0;
        }
    }
}
