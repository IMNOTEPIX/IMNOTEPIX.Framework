using GTA;

// Compiler shim to enable C# 9 records on .NET Framework 4.8.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace IMNOTEPIX.Framework.fBlip
{
    /// <summary>Immutable description of how a blip should look and behave on the map.</summary>
    public record BlipPreset(
        BlipSprite Sprite,
        BlipColor Color,
        string Name = "",
        float Scale = 1f,
        bool IsShortRange = true,
        BlipDisplayType DisplayType = BlipDisplayType.Default
    );
}
