using GTA;
using GTA.Math;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fGameplay.Combat
{
    /// <summary>
    /// Lightweight manager for a list of raw <see cref="GTA.Ped"/> handles.
    /// Use <see cref="CombatAgent"/> for new code; this exists for legacy support.
    /// </summary>
    public sealed class CombatGroup
    {
        public List<GTA.Ped> Peds { get; } = new List<GTA.Ped>();

        private float _lastCheckTime;
        private const float CheckInterval = 1000f;

        public void Check()
        {
            if (Game.GameTime - _lastCheckTime < CheckInterval)
                return;
            _lastCheckTime = Game.GameTime;

            for (int i = Peds.Count - 1; i >= 0; i--)
            {
                var ped = Peds[i];

                if (ped == null || !ped.Exists())
                {
                    Peds.RemoveAt(i);
                    continue;
                }

                UpdateBlipVisibility(ped);

                if (ped.IsDead)
                {
                    ped.AttachedBlip?.Delete();
                    ped.MarkAsNoLongerNeeded();
                    Peds.RemoveAt(i);
                }
            }
        }

        /// <summary>Spawns a single ped and adds it to this group.</summary>
        public GTA.Ped Spawn(Model model, Vector3 position, float heading)
        {
            model.Request(500);

            var ped = GTA.World.CreatePed(model, position, heading);

            if (ped != null && ped.Exists() && !Peds.Contains(ped))
                Peds.Add(ped);

            return ped;
        }

        public void AddBlips(string name = "Enemy", float scale = 0.7f,
                             BlipColor color = BlipColor.Red, int spriteId = 270)
        {
            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists() || ped.AttachedBlip != null)
                    continue;

                ped.AddBlip();
                var blip = ped.AttachedBlip;
                if (blip == null || !blip.Exists())
                    continue;

                blip.Sprite = (BlipSprite)spriteId;
                blip.Color = color;
                blip.Name = name;
                blip.Scale = scale;
                blip.IsShortRange = true;
                blip.DisplayType = BlipDisplayType.MiniMapOnly;
            }
        }

        public void SetCombatAttributes(fWorld.Ped.CombatAttributes attribute, bool enabled)
        {
            foreach (var ped in Peds)
                if (ped != null && ped.Exists())
                    fWorld.Ped.SetPedCombatAttributes(ped, attribute, enabled);
        }

        public void SetMovement(fWorld.Ped.CombatMovement movement)
        {
            foreach (var ped in Peds)
                if (ped != null && ped.Exists())
                    fWorld.Ped.SetPedCombatMovement(ped, movement);
        }

        public void SetAbilityLevel(fWorld.Ped.CombatAbilityLevel level)
        {
            foreach (var ped in Peds)
                if (ped != null && ped.Exists())
                    fWorld.Ped.SetPedCombatAbility(ped, level);
        }

        public void RandomizeMovement()
        {
            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists())
                    continue;
                fWorld.Ped.SetPedCombatMovement(ped, (fWorld.Ped.CombatMovement)fWorld.Misc.GetRandomIntInRange(1, 4));
            }
        }

        public void RandomizeAbilityLevel(bool includePoor = true)
        {
            int min = includePoor ? 1 : 2;

            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists())
                    continue;
                fWorld.Ped.SetPedCombatAbility(ped, (fWorld.Ped.CombatAbilityLevel)fWorld.Misc.GetRandomIntInRange(min, 4));
            }
        }

        public void ApplyDefaultAttributes()
        {
            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists())
                    continue;

                fWorld.Ped.SetPedConfigFlag(ped, fWorld.Ped.PedConfigFlags.PCF_DontBlipCop, true);
                fWorld.Ped.SetPedAsCop(ped, false);

                foreach (var attr in CombatConfig.DefaultDisableAttributes)
                    fWorld.Ped.SetPedCombatAttributes(ped, attr, false);

                foreach (var attr in CombatConfig.DefaultEnableAttributes)
                    fWorld.Ped.SetPedCombatAttributes(ped, attr, true);
            }
        }

        public void DeleteAll()
        {
            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists())
                    continue;
                ped.AttachedBlip?.Delete();
                ped.Delete();
            }
            Peds.Clear();
        }

        private static void UpdateBlipVisibility(GTA.Ped ped)
        {
            var blip = ped.AttachedBlip;
            if (blip == null)
                return;

            var player = Game.Player.Character;
            bool sameInterior = fWorld.Interior.Interior.GetInteriorFromEntity(ped) == fWorld.Interior.Interior.GetInteriorFromEntity(player);
            bool sameRoom = fWorld.Interior.Interior.GetRoomKeyFromEntity(ped) == fWorld.Interior.Interior.GetRoomKeyFromEntity(player);

            blip.Alpha = (sameInterior && sameRoom) ? 255 : 0;
        }
    }
}
