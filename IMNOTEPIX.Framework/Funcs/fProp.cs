using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.Funcs
{
    public static class fProp
    {
        /// <summary>
        /// Deletes all props in the list and clears it.
        /// </summary>
        public static void DeletePropsInList(List<Prop> propList)
        {
            foreach (var prop in propList)
            {
                if (prop != null)
                    prop.Delete();
            }
            propList.Clear();
        }

        /// <summary>
        /// Creates a prop at the given position/rotation and adds it to the list.
        /// Uses a single yield rather than a blocking loop — consistent with
        /// CombatPed.Create() and PedListUtils.CreateAndAdd().
        /// </summary>
        public static void CreatePropForList(
            List<Prop> propList,
            Model model,
            Vector3 pos,
            Vector3 rot,
            bool dynamic,
            bool placeOnGround = true)
        {
            Prop prop = World.CreateProp(model, pos, rot, dynamic, placeOnGround);

            if (prop != null && !prop.Exists())
                Script.Yield();

            if (prop != null && !propList.Contains(prop))
                propList.Add(prop);
        }

        /// <summary>
        /// Creates a prop with no position offset and adds it to the list.
        /// </summary>
        public static void CreatePropNoOffsetForList(
            List<Prop> propList,
            Model model,
            Vector3 pos,
            Vector3 rot,
            bool dynamic)
        {
            Prop prop = World.CreatePropNoOffset(model, pos, rot, dynamic);

            if (prop != null && !prop.Exists())
                Script.Yield();

            if (prop != null && !propList.Contains(prop))
                propList.Add(prop);
        }

        /// <summary>
        /// Direct pass-through to World.CreateProp.
        /// Prefer calling World.CreateProp directly for new code.
        /// </summary>
        [Obsolete("Call World.CreateProp directly.")]
        public static Prop CreateProp(
            Model model, Vector3 pos, Vector3 rot,
            bool dynamic, bool placeOnGround)
            => World.CreateProp(model, pos, rot, dynamic, placeOnGround);

        /// <summary>
        /// Direct pass-through to World.CreatePropNoOffset.
        /// Prefer calling World.CreatePropNoOffset directly for new code.
        /// </summary>
        [Obsolete("Call World.CreatePropNoOffset directly.")]
        public static Prop CreatePropNoOffset(
            Model model, Vector3 pos, Vector3 rot, bool dynamic)
            => World.CreatePropNoOffset(model, pos, rot, dynamic);
    }
}
