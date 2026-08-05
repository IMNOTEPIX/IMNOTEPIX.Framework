using GTA;
using GTA.Math;
using GTA.Native;
using IMNOTEPIX.Framework.fExtension;
using System;
using System.Windows.Forms;

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

        private RelationshipGroup _aiTeam = new RelationshipGroup(RelationshipGroupHash.HatesPlayer);
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

            if (Exists)
                Handle.FadeIn();

            if (!Exists)
                throw new InvalidOperationException($"Failed to create combat ped at {Config.Position}.");
        }

        /// <summary>Applies all config settings to the spawned ped. Runs once.</summary>
        public void Activate()
        {
            if (!Exists || _activated || _disposed)
                return;

            CombatRegistry._alivePeds.Add(this);
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
                CombatRegistry._alivePeds.Remove(this);
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
            blip.FadeIn();
        }

        private void SetupFlags()
        {
            foreach (var flag in Config.EnableFlags)
                fWorld.Ped.SetPedConfigFlag(Handle, flag, true);
            foreach (var flag in Config.DisableFlags)
                fWorld.Ped.SetPedConfigFlag(Handle, flag, false);
            SetCop();
        }

        private void SetCop()
        {
            if (Config.IsCop)
            {
                fWorld.Ped.SetPedAsCop(Handle, true);
                fWorld.Ped.SetPedConfigFlag(Handle, fWorld.Ped.PedConfigFlags.PCF_DontBlipCop, false);
            }
        }

        private void SetupRelationships()
        {
            Handle.RelationshipGroup = _aiTeam;
            Handle.Task.CombatHatedTargetsAroundPed(1000f);
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

            foreach (var attr in Config.CombatFloat)
                Handle.SetCombatFloatAttribute(attr.Item1, attr.Item2);
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
            bool sameInterior = fWorld.Map.Interior.GetInteriorFromEntity(Handle) == fWorld.Map.Interior.GetInteriorFromEntity(player);
            bool sameRoom = fWorld.Map.Interior.GetRoomKeyFromEntity(Handle) == fWorld.Map.Interior.GetRoomKeyFromEntity(player);
            RaycastResult raycast = World.Raycast(GameplayCamera.Position, Handle.Position, IntersectFlags.Everything);

            if ((sameInterior && sameRoom) && fPlayer.Player.GetDistanceTo(Handle.Position) < 5f)
            {
                if (blip.Alpha == 0)
                    blip.FadeIn(0.3f);
            }
            else if (Handle.HasClearLineOfSightToAdjustForCover(player) || raycast.HitEntity == Handle)
            {
                if (blip.Alpha == 0)
                    blip.FadeIn(0.3f);
            }
            else
            {
                if (blip.Alpha == 255)
                    blip.FadeOut(0.3f);
            }
        }
    }
}
