using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fGameplay.Combat
{
    /// <summary>
    /// Plain-data config passed to <see cref="CombatAgent"/> and <see cref="CombatGroup"/>.
    /// Fill only what you need; every field has a sensible default.
    /// </summary>
    public sealed class CombatConfig
    {
        public Model Model { get; set; }
        public Vector3 Position { get; set; }
        public float Heading { get; set; }

        public float Health { get; set; } = 200;
        public int Armor { get; set; }

        public WeaponHash Weapon { get; set; } = WeaponHash.AssaultRifle;
        public fWorld.Ped.CombatAbilityLevel AbilityLevel { get; set; } = fWorld.Ped.CombatAbilityLevel.Average;
        public fWorld.Ped.CombatMovement Movement { get; set; } = fWorld.Ped.CombatMovement.WillAdvance;
        public fWorld.Ped.CombatAttributes[] EnableAttributes { get; set; } = DefaultEnableAttributes;
        public fWorld.Ped.CombatAttributes[] DisableAttributes { get; set; } = DefaultDisableAttributes;

        public List<Tuple<CombatFloatAttributes, float>> CombatFloat { get; set; } = [];

        public bool CanHurt { get; set; } = true;
        public bool CanWrithe { get; set; } = true;
        public bool IsCop { get; set; } = false;

        public string BlipName { get; set; } = "Enemy";
        public BlipColor BlipColor { get; set; } = BlipColor.Red;
        public BlipSprite BlipSprite { get; set; } = (BlipSprite)270;
        public float BlipScale { get; set; } = 0.7f;

        public static readonly fWorld.Ped.CombatAttributes[] DefaultEnableAttributes =
        {
            fWorld.Ped.CombatAttributes.CA_IS_A_GUARD,
            fWorld.Ped.CombatAttributes.CA_DISABLE_FLEE_FROM_COMBAT,
            fWorld.Ped.CombatAttributes.CA_USE_COVER
        };

        public static readonly fWorld.Ped.CombatAttributes[] DefaultDisableAttributes =
        {
            fWorld.Ped.CombatAttributes.CA_DISABLE_REACT_TO_BUDDY_SHOT,
            fWorld.Ped.CombatAttributes.CA_PLAY_REACTION_ANIMS,
            fWorld.Ped.CombatAttributes.CA_DISABLE_BULLET_REACTIONS
        };

        public static readonly fWorld.Ped.CombatAttributes[] NTH_DefaultVaultGuardEnableAttributes =
        {
            fWorld.Ped.CombatAttributes.CA_BLIND_FIRE_IN_COVER,
            fWorld.Ped.CombatAttributes.CA_CAN_CHARGE,
            fWorld.Ped.CombatAttributes.CA_CAN_USE_PEEKING_VARIATIONS,
            fWorld.Ped.CombatAttributes.CA_DISABLE_BULLET_REACTIONS,
            fWorld.Ped.CombatAttributes.CA_DISABLE_FLEE_FROM_COMBAT,
            fWorld.Ped.CombatAttributes.CA_ENABLE_TACTICAL_POINTS_WHEN_DEFENSIVE,
            fWorld.Ped.CombatAttributes.CA_IS_A_GUARD,
            fWorld.Ped.CombatAttributes.CA_USE_COVER,
            fWorld.Ped.CombatAttributes.CA_USE_PROXIMITY_FIRING_RATE,
            fWorld.Ped.CombatAttributes.CA_DISABLE_ENTRY_REACTIONS,
        };

        public static CombatConfig NTH_VaultGuard(Vector3 Posistion, float Heading) => new()
        {
            Model = -681004504,
            Position = Posistion,
            Heading = Heading,
            Armor = 175,
            Weapon = fWorld.Misc.GetRandomIntInRange(1, 4) switch
            {
                1 => WeaponHash.CombatPistol,
                2 => WeaponHash.CarbineRifle,
                3 => WeaponHash.PumpShotgun,
                _ => WeaponHash.CombatPistol
            },
            AbilityLevel = fWorld.Misc.GetRandomIntInRange(1, 3) switch
            {
                1 => fWorld.Ped.CombatAbilityLevel.Average,
                2 => fWorld.Ped.CombatAbilityLevel.Professional,
                _ => fWorld.Ped.CombatAbilityLevel.Average
            },
            Movement = fWorld.Misc.GetRandomIntInRange(1, 4) switch
            {
                1 => fWorld.Ped.CombatMovement.Defensive,
                2 => fWorld.Ped.CombatMovement.WillAdvance,
                3 => fWorld.Ped.CombatMovement.WillRetreat,
                _ => fWorld.Ped.CombatMovement.Defensive
            },
            CombatFloat = new List<Tuple<CombatFloatAttributes, float>>
            {
                new Tuple<CombatFloatAttributes, float>(CombatFloatAttributes.BlindFireChance, 0.5f),
                new Tuple<CombatFloatAttributes, float>(CombatFloatAttributes.FightProficiency, 0.5f),
                new Tuple<CombatFloatAttributes, float>(CombatFloatAttributes.StrafeWhenMovingChance, 1.0f),
            },
            EnableAttributes = NTH_DefaultVaultGuardEnableAttributes,
            DisableAttributes = DefaultDisableAttributes,
            CanHurt = false,
            CanWrithe = true,
            BlipName = "Guard"
        };
    }
}
