namespace IMNOTEPIX.Framework.Funcs.Ped
{
    using System.Collections.Generic;
    using GTA;
    using GTA.Math;
    using IMNOTEPIX.Framework.Funcs.Interior;

    using Ped = GTA.Ped;
    /// <summary>
    /// Convenience utilities for unmanaged List&lt;Ped&gt; collections.
    ///
    /// These exist for simple one-off cases where a full CombatGroup is overkill.
    /// If you are managing combat peds, use CombatGroup instead — it handles
    /// blip visibility, dead-ped pruning, and activation in one place.
    /// </summary>
    public static class PedListUtils
    {
        // -----------------------------------------------------------------
        // Blip visibility — matches CombatPed.UpdateBlipVisibility()
        // -----------------------------------------------------------------

        /// <summary>
        /// Updates blip alpha based on whether the ped shares the player's
        /// interior and room. Prunes dead peds from the list.
        /// Intended for use on a throttled interval, not every frame.
        /// </summary>
        public static void CheckPeds(List<Ped> pedList)
        {
            for (int i = pedList.Count - 1; i >= 0; i--)
            {
                var ped = pedList[i];
                if (ped == null || !ped.Exists())
                {
                    pedList.RemoveAt(i);
                    continue;
                }

                UpdateBlipVisibility(ped);

                if (ped.IsDead)
                {
                    ped.AttachedBlip?.Delete();
                    ped.MarkAsNoLongerNeeded();
                    pedList.RemoveAt(i);
                }
            }
        }

        // -----------------------------------------------------------------
        // Deletion
        // -----------------------------------------------------------------

        public static void DeletePeds(List<Ped> pedList)
        {
            foreach (var ped in pedList)
            {
                if (ped == null) continue;
                ped.AttachedBlip?.Delete();
                if (ped.Exists()) ped.Delete();
            }
            pedList.Clear();
        }

        public static void DeletePeds(Ped[] pedArray)
        {
            foreach (var ped in pedArray)
            {
                if (ped == null) continue;
                ped.AttachedBlip?.Delete();
                if (ped.Exists()) ped.Delete();
            }
        }

        // -----------------------------------------------------------------
        // Blip setup
        // -----------------------------------------------------------------

        /// <summary>
        /// Adds a minimap blip to any ped in the list that doesn't already
        /// have one. Non-blocking — skips the ped if the blip isn't ready
        /// rather than spinning.
        /// </summary>
        public static void SetBlips(
            List<Ped> pedList,
            string blipName = "Enemy",
            float scale = 0.7f,
            BlipColor color = BlipColor.Red,
            int spriteId = 270)
        {
            foreach (var ped in pedList)
            {
                if (ped == null || !ped.Exists()) continue;
                if (ped.AttachedBlip != null) continue;

                ped.AddBlip();

                // One check — no blocking loop.
                // If the blip still isn't ready we'll catch it next call.
                var blip = ped.AttachedBlip;
                if (blip == null || !blip.Exists()) continue;

                blip.Sprite = (BlipSprite)spriteId;
                blip.Color = color;
                blip.Name = blipName;
                blip.Scale = scale;
                blip.IsShortRange = true;
                blip.DisplayType = BlipDisplayType.MiniMapOnly;
            }
        }

        // -----------------------------------------------------------------
        // Creation
        // -----------------------------------------------------------------

        /// <summary>
        /// Creates a ped and adds it to the list.
        /// One yield instead of a blocking loop — if the ped still doesn't
        /// exist after a single frame the model failed to load.
        /// </summary>
        public static Ped CreateAndAdd(
            List<Ped> pedList,
            Model model,
            Vector3 position,
            float heading = 0f)
        {
            var ped = World.CreatePed(model, position, heading);

            // Matches the same non-blocking approach used in CombatPed.Create().
            if (ped != null && !ped.Exists())
                Script.Yield();

            if (ped != null && !pedList.Contains(ped))
                pedList.Add(ped);

            return ped;
        }

        // -----------------------------------------------------------------
        // Private
        // -----------------------------------------------------------------

        private static void UpdateBlipVisibility(Ped ped)
        {
            if (ped.AttachedBlip == null) return;

            bool sameInterior = fInterior.GetInteriorFromEntity(ped)
                             == fInterior.GetInteriorFromEntity(Game.Player.Character);
            bool sameRoom = fInterior.GetRoomKeyFromEntity(ped)
                             == fInterior.GetRoomKeyFromEntity(Game.Player.Character);

            ped.AttachedBlip.Alpha = (sameInterior && sameRoom) ? 255 : 0;
        }
    }
}
