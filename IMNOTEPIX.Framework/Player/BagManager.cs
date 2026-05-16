using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fPlayer
{
    public class BagManager
    {
        private const int FreemodeComponentId = 5;
        private const int StoryModeComponentId = 9;

        public enum BagVariantTypes
        {
            Invalid = -1,
            OriginalHeists,
            CasinoYungAncestor,
            CasinoRegular,
            CasinoMaintenance,
            CasinoBugstars,
            CasinoGeometric,
            CasinoPattern,
            CasinoGeometric2,
            CasinoAggressive1,
            CasinoAggressive2,
            CasinoAggressive3,
            DrugWarsChemicals,
            ChopShopWeaponsBag,
        }

        public struct BagData
        {
            public int Drawable;
            public int Texture;

            public BagData(int drawable, int texture)
            {
                Drawable = drawable;
                Texture = texture;
            }
        }

        // Static entries — drawable/texture do not depend on player model.
        private static readonly Dictionary<BagVariantTypes, BagData> BagLookup = new Dictionary<BagVariantTypes, BagData>
        {
            // OriginalHeists is intentionally absent here — resolved at call time in GetBagData() because it depends on the current player model.
            { BagVariantTypes.CasinoYungAncestor, new BagData(82,  9) },
            { BagVariantTypes.CasinoRegular,      new BagData(82,  0) },
            { BagVariantTypes.CasinoMaintenance,  new BagData(82,  3) },
            { BagVariantTypes.CasinoBugstars,     new BagData(82,  8) },
            { BagVariantTypes.CasinoGeometric,    new BagData(82, 13) },
            { BagVariantTypes.CasinoPattern,      new BagData(82, 12) },
            { BagVariantTypes.CasinoGeometric2,   new BagData(82, 15) },
            { BagVariantTypes.CasinoAggressive1,  new BagData(82, 10) },
            { BagVariantTypes.CasinoAggressive2,  new BagData(82, 11) },
            { BagVariantTypes.CasinoAggressive3,  new BagData(82, 14) },
            { BagVariantTypes.DrugWarsChemicals,  new BagData(82,  2) },
            { BagVariantTypes.ChopShopWeaponsBag, new BagData(82,  4) },
        };

        private static readonly Dictionary<BagVariantTypes, string> ModelLookup = new Dictionary<BagVariantTypes, string>
        {
            { BagVariantTypes.OriginalHeists,     "hei_p_m_bag_var22_arm_s"         },
            { BagVariantTypes.CasinoYungAncestor, "ch_p_m_bag_var01_arm_s"          },
            { BagVariantTypes.CasinoRegular,      "ch_p_m_bag_var02_arm_s"          },
            { BagVariantTypes.CasinoMaintenance,  "ch_p_m_bag_var03_arm_s"          },
            { BagVariantTypes.CasinoBugstars,     "ch_p_m_bag_var04_arm_s"          },
            { BagVariantTypes.CasinoGeometric,    "ch_p_m_bag_var05_arm_s"          },
            { BagVariantTypes.CasinoPattern,      "ch_p_m_bag_var06_arm_s"          },
            { BagVariantTypes.CasinoGeometric2,   "ch_p_m_bag_var07_arm_s"          },
            { BagVariantTypes.CasinoAggressive1,  "ch_p_m_bag_var08_arm_s"          },
            { BagVariantTypes.CasinoAggressive2,  "ch_p_m_bag_var09_arm_s"          },
            { BagVariantTypes.CasinoAggressive3,  "ch_p_m_bag_var10_arm_s"          },
            { BagVariantTypes.DrugWarsChemicals,  "xm3_p_xm3_m_bag_var22_arm_s"     },
            { BagVariantTypes.ChopShopWeaponsBag, "m23_2_p_m32_m_bag_var22_arm_s_g" },
        };

        /// <summary>
        /// Returns the torso/bag component slot for the given ped.
        /// Freemode peds use slot 5; story mode peds use slot 9.
        /// </summary>
        public static int GetComponentId(Ped ped)
        {
            // Checks the local player model — ped parameter kept for callsite consistency but local player is always the one wearing the bag.
            return Player.IsFreemodePed ? FreemodeComponentId : StoryModeComponentId;
        }

        /// <summary>
        /// Returns the drawable/texture data for the given bag variant.
        /// OriginalHeists is resolved against the current player model at call time.
        /// </summary>
        public static BagData GetBagData(BagVariantTypes type)
        {
            if (type == BagVariantTypes.OriginalHeists)
                return Player.IsStoryPed ? new BagData(1, 0) : new BagData(45, 0);

            if (BagLookup.TryGetValue(type, out var data))
                return data;

            throw new ArgumentException($"No BagData registered for variant '{type}'.");
        }

        public static BagVariantTypes GetBagVariantFromPed(Ped ped)
        {
            int componentId = GetComponentId(ped);
            int currentDrawable = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, ped, componentId);
            int currentTexture = Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, ped, componentId);

            // Check OriginalHeists separately since its data is model-dependent
            var original = GetBagData(BagVariantTypes.OriginalHeists);
            if (original.Drawable == currentDrawable && original.Texture == currentTexture)
                return BagVariantTypes.OriginalHeists;

            foreach (var entry in BagLookup)
            {
                if (entry.Value.Drawable == currentDrawable &&
                    entry.Value.Texture == currentTexture)
                    return entry.Key;
            }

            return BagVariantTypes.Invalid;
        }

        public static void ApplyBag(Ped ped, BagVariantTypes type)
        {
            var data = GetBagData(type);
            int componentId = GetComponentId(ped);

            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, componentId, data.Drawable, data.Texture, 0);
        }

        public static void RemoveBag(Ped ped)
        {
            int componentId = GetComponentId(ped);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, componentId, 0, 0, 0);
        }

        public static Prop CreateBagPropFromPed(Ped ped)
        {
            var variant = GetBagVariantFromPed(ped);
            if (variant == BagVariantTypes.Invalid)
                return null;

            if (!ModelLookup.TryGetValue(variant, out var modelName))
                return null;

            return GTA.World.CreateProp(modelName, ped.Position, false, false);
        }

        /// <summary>Number of valid (non-Invalid) bag variants.</summary>
        public static int BagVariantCount
            => Enum.GetValues(typeof(BagVariantTypes)).Length - 1;
    }
}
