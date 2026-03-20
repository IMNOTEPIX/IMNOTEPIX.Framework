using GTA;
using GTA.Math;
using IMNOTEPIX.Framework.Funcs.Interior;
using IMNOTEPIX.Framework.Funcs.Ped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMNOTEPIX.Framework.Funcs.Combat
{
    /// <summary>
    /// Lightweight manager for a list of raw <see cref="GTA.Ped"/> handles.
    /// Use <see cref="CombatPed"/> for new code; this exists for legacy support.
    /// </summary>
    public sealed class CombatGroup
    {
        public List<GTA.Ped> Peds { get; } = new List<GTA.Ped>();

        private float _lastCheckTime;
        private const float CheckInterval = 1000f;

        // ── Tick ──────────────────────────────────────────────────────────────

        public void Check()
        {
            if (Game.GameTime - _lastCheckTime < CheckInterval) return;
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

        // ── Spawn ─────────────────────────────────────────────────────────────

        /// <summary>Spawns a single ped and adds it to this group.</summary>
        public GTA.Ped Spawn(Model model, Vector3 position, float heading)
        {
            model.Request(500);

            var ped = World.CreatePed(model, position, heading);

            if (ped != null && ped.Exists() && !Peds.Contains(ped))
                Peds.Add(ped);

            return ped;
        }

        // ── Bulk helpers ──────────────────────────────────────────────────────

        public void AddBlips(string name = "Enemy", float scale = 0.7f,
                             BlipColor color = BlipColor.Red, int spriteId = 270)
        {
            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists() || ped.AttachedBlip != null) continue;

                ped.AddBlip();
                var blip = ped.AttachedBlip;
                if (blip == null || !blip.Exists()) continue;

                blip.Sprite = (BlipSprite)spriteId;
                blip.Color = color;
                blip.Name = name;
                blip.Scale = scale;
                blip.IsShortRange = true;
                blip.DisplayType = BlipDisplayType.MiniMapOnly;
            }
        }

        public void SetCombatAttributes(fPed.CombatAttributes attribute, bool enabled)
        {
            foreach (var ped in Peds)
                if (ped != null && ped.Exists())
                    fPed.SetPedCombatAttributes(ped, attribute, enabled);
        }

        public void SetMovement(fPed.CombatMovement movement)
        {
            foreach (var ped in Peds)
                if (ped != null && ped.Exists())
                    fPed.SetPedCombatMovement(ped, movement);
        }

        public void SetAbilityLevel(fPed.CombatAbilityLevel level)
        {
            foreach (var ped in Peds)
                if (ped != null && ped.Exists())
                    fPed.SetPedCombatAbility(ped, level);
        }

        public void RandomizeMovement()
        {
            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists()) continue;
                fPed.SetPedCombatMovement(ped, (fPed.CombatMovement)fMisc.GetRandomIntInRange(1, 4));
            }
        }

        public void RandomizeAbilityLevel(bool includePoor = true)
        {
            int min = includePoor ? 1 : 2;

            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists()) continue;
                fPed.SetPedCombatAbility(ped, (fPed.CombatAbilityLevel)fMisc.GetRandomIntInRange(min, 4));
            }
        }

        public void ApplyDefaultAttributes()
        {
            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists()) continue;

                fPed.SetPedConfigFlag(ped, fPed.PedConfigFlags.PCF_DontBlipCop, true);
                fPed.SetPedAsCop(ped, false);

                foreach (var attr in CombatConfig.DefaultDisableAttributes)
                    fPed.SetPedCombatAttributes(ped, attr, false);

                foreach (var attr in CombatConfig.DefaultEnableAttributes)
                    fPed.SetPedCombatAttributes(ped, attr, true);
            }
        }

        public void DeleteAll()
        {
            foreach (var ped in Peds)
            {
                if (ped == null || !ped.Exists()) continue;
                ped.AttachedBlip?.Delete();
                ped.Delete();
            }
            Peds.Clear();
        }

        // ── Private ───────────────────────────────────────────────────────────

        private static void UpdateBlipVisibility(GTA.Ped ped)
        {
            var blip = ped.AttachedBlip;
            if (blip == null) return;

            var player = Game.Player.Character;
            bool sameInterior = fInterior.GetInteriorFromEntity(ped) == fInterior.GetInteriorFromEntity(player);
            bool sameRoom = fInterior.GetRoomKeyFromEntity(ped) == fInterior.GetRoomKeyFromEntity(player);

            blip.Alpha = (sameInterior && sameRoom) ? 255 : 0;
        }
    }
}
