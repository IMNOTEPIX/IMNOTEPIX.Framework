using GTA;
using GTA.Math;
using IMNOTEPIX.Framework.Funcs.Ped;

namespace IMNOTEPIX.Framework.Funcs.Combat
{
    /// <summary>
    /// Plain-data config passed to <see cref="CombatPed"/> and <see cref="CombatGroup"/>.
    /// Fill only what you need; every field has a sensible default.
    /// </summary>
    public sealed class CombatConfig
    {
        // ── Identity ──────────────────────────────────────────────────────────
        public Model Model { get; set; }
        public Vector3 Position { get; set; }
        public float Heading { get; set; }

        // ── Combat behavior ─────────────────────────────────────────────────
        public WeaponHash Weapon { get; set; } = WeaponHash.AssaultRifle;
        public fPed.CombatAbilityLevel AbilityLevel { get; set; } = fPed.CombatAbilityLevel.Average;
        public fPed.CombatMovement Movement { get; set; } = fPed.CombatMovement.WillAdvance;
        public fPed.CombatAttributes[] EnableAttributes { get; set; } = DefaultEnableAttributes;
        public fPed.CombatAttributes[] DisableAttributes { get; set; } = DefaultDisableAttributes;

        // ── Flags ─────────────────────────────────────────────────────────────
        public bool CanHurt { get; set; } = true;
        public bool CanWrithe { get; set; } = true;
        public bool IsCop { get; set; } = false;

        // ── Blip ─────────────────────────────────────────────────────────────
        public string BlipName { get; set; } = "Enemy";
        public BlipColor BlipColor { get; set; } = BlipColor.Red;
        public BlipSprite BlipSprite { get; set; } = (BlipSprite)270;
        public float BlipScale { get; set; } = 0.7f;

        // ── Defaults ──────────────────────────────────────────────────────────
        public static readonly fPed.CombatAttributes[] DefaultEnableAttributes =
        {
            fPed.CombatAttributes.CA_IS_A_GUARD,
            fPed.CombatAttributes.CA_DISABLE_FLEE_FROM_COMBAT,
            fPed.CombatAttributes.CA_USE_COVER
        };

        public static readonly fPed.CombatAttributes[] DefaultDisableAttributes =
        {
            fPed.CombatAttributes.CA_DISABLE_REACT_TO_BUDDY_SHOT,
            fPed.CombatAttributes.CA_PLAY_REACTION_ANIMS,
            fPed.CombatAttributes.CA_DISABLE_BULLET_REACTIONS
        };

        public static readonly fPed.CombatAttributes[] NTH_DefaultVaultGuardEnableAttributes =
        {
            fPed.CombatAttributes.CA_BLIND_FIRE_IN_COVER,
            fPed.CombatAttributes.CA_CAN_CHARGE,
            fPed.CombatAttributes.CA_CAN_USE_PEEKING_VARIATIONS,
            fPed.CombatAttributes.CA_DISABLE_BULLET_REACTIONS,
            fPed.CombatAttributes.CA_DISABLE_FLEE_FROM_COMBAT,
            fPed.CombatAttributes.CA_ENABLE_TACTICAL_POINTS_WHEN_DEFENSIVE,
            fPed.CombatAttributes.CA_IS_A_GUARD,
            fPed.CombatAttributes.CA_USE_COVER,
            fPed.CombatAttributes.CA_USE_PROXIMITY_FIRING_RATE,
            fPed.CombatAttributes.CA_DISABLE_ENTRY_REACTIONS,
        };

        // ── Presets ──────────────────────────────────────────────────────────
        public static CombatConfig NTH_VaultGuard(Vector3 Posistion, float Heading) => new()
        {
            Model = -681004504,
            Position = Posistion,
            Heading = Heading,
            Weapon = fMisc.GetRandomIntInRange(1, 4) switch
            {
                1 => WeaponHash.CombatPistol,
                2 => WeaponHash.CarbineRifle,
                3 => WeaponHash.PumpShotgun,
                _ => WeaponHash.CombatPistol
            },
            AbilityLevel = fMisc.GetRandomIntInRange(1, 3) switch
            {
                1 => fPed.CombatAbilityLevel.Average,
                2 => fPed.CombatAbilityLevel.Professional,
                _ => fPed.CombatAbilityLevel.Average
            },
            Movement = fMisc.GetRandomIntInRange(1, 4) switch
            {
                1 => fPed.CombatMovement.Defensive,
                2 => fPed.CombatMovement.WillAdvance,
                3 => fPed.CombatMovement.WillRetreat,
                _ => fPed.CombatMovement.Defensive
            },
            EnableAttributes = NTH_DefaultVaultGuardEnableAttributes,
            DisableAttributes = DefaultDisableAttributes,
            CanHurt = false,
            CanWrithe = true,
            BlipName = "Guard"
        };
    }
}
