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

        public static readonly BlipPreset Destination = new(
            BlipSprite.Standard,
            BlipColor.Yellow,
            "Destination",
            Scale: 1f,
            IsShortRange: false
        );

        public static readonly BlipPreset WeedStack = new(
            (BlipSprite)469,
            BlipColor.Green,
            "Weed",
            Scale: 0.75f
        );

        public static readonly BlipPreset GoldStack = new(
            (BlipSprite)618,
            (BlipColor)28,
            "Gold",
            Scale: 0.75f
        );

        public static readonly BlipPreset BookStack = new(
            BlipSprite.Standard,
            BlipColor.White,
            "Black Books",
            Scale: 0.75f
        );

        public static readonly BlipPreset ServerStack = new(
            BlipSprite.Standard,
            BlipColor.White,
            "Server Blades",
            Scale: 0.75f
        );

        public static readonly BlipPreset HardDriveStack = new(
            BlipSprite.Standard,
            BlipColor.White,
            "Hard Drives",
            Scale: 0.75f
        );

        public static readonly BlipPreset CashStack = new(
            (BlipSprite)272,
            BlipColor.Green,
            "Cash",
            Scale: 0.75f
        );

        public static readonly BlipPreset CokeStack = new(
            (BlipSprite)514,
            BlipColor.Grey,
            "Cocaine",
            Scale: 0.75f
        );

        public static readonly BlipPreset WashingMachine = new (
            BlipSprite.Standard,
            BlipColor.Green,
            "Washing Machine",
            Scale: 0.7f
            );

        /*
        public static readonly BlipPreset Template = new(
            BlipSprite.Standard,
            BlipColor.Red,
            "Template",
            Scale: 1f,
            IsShortRange: false,
            DisplayType: BlipDisplayType.Default
        );*/
    }
}