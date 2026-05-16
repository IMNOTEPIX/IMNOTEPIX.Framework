using GTA;

namespace IMNOTEPIX.Framework.fBlip
{
    /// <summary>Ready-made <see cref="BlipPreset"/> instances for common mission blips.</summary>
    public class BlipPresets
    {
        public static readonly BlipPreset CashCart = new(
            (BlipSprite)272,
            BlipColor.Green,
            "Cash",
            Scale: 0.9f
        );

        public static readonly BlipPreset GoldCart = new(
            (BlipSprite)618,
            (BlipColor)28,
            "Gold",
            Scale: 0.8f
        );

        public static readonly BlipPreset DiamondsCart = new(
            (BlipSprite)617,
            (BlipColor)26,
            "Diamonds",
            Scale: 0.8f
        );

        public static readonly BlipPreset CocaineCart = new(
            (BlipSprite)514,
            BlipColor.Grey,
            "Cocaine",
            Scale: 0.8f
        );

        public static readonly BlipPreset VaultCart = new(
            BlipSprite.Standard,
            BlipColor.Green,
            "Cash",
            Scale: 0.75f
        );

        public static readonly BlipPreset Destination = new(
            BlipSprite.Standard,
            BlipColor.Yellow,
            "Destination",
            Scale: 1f,
            IsShortRange: false
        );

        public static readonly BlipPreset Torchable = new(
            BlipSprite.Standard,
            BlipColor.Red,
            Scale: 0.75f,
            DisplayType: BlipDisplayType.MiniMapOnly
        );

        public static readonly BlipPreset Template = new(
            BlipSprite.Standard,
            BlipColor.Red,
            "Template",
            Scale: 1f,
            IsShortRange: false,
            DisplayType: BlipDisplayType.Default
        );
    }
}
