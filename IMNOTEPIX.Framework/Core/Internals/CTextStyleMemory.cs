using GTA;
using System;
using System.Runtime.InteropServices;

namespace IMNOTEPIX.Framework.fCore.Internals
{
    /// <summary>
    /// Extends Memory (see Memory.cs) with the CTextStyle live-state address, so
    /// TextStyle.Apply()/Current can use the fast direct-write path when available instead
    /// of always going through SET_TEXT_* natives. Same Game.FindPattern approach already
    /// confirmed working for CTextFile.
    /// </summary>
    internal static unsafe partial class Memory
    {
        public static readonly IntPtr CTextStyle_ScriptStyle = ResolveCTextStyleScriptStyle();

        private static IntPtr ResolveCTextStyleScriptStyle()
        {
            IntPtr addr = FindAddress(() => Game.FindPattern("48 8D 0D ?? ?? ?? ?? 44 8A C3 E8 ?? ?? ?? ?? 83 25"));
            if (addr != IntPtr.Zero)
            {
                addr += *(int*)(addr + 3) + 7;
            }
            return addr;
        }
    }

    /// <summary>
    /// Direct port of RAGENativeUI's CTextStyle - the game's own live text-style struct.
    /// Uses a plain pointer cast for ScriptStyle (not System.Runtime.CompilerServices.Unsafe.AsRef -
    /// see the scrProgramRegistry fix earlier: that type needs an external NuGet package that
    /// isn't deployed alongside the mod DLL and threw FileNotFoundException at runtime).
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    internal unsafe struct CTextStyle
    {
        [FieldOffset(0x00)] public int Color;

        [FieldOffset(0x08)] public float Scale;
        [FieldOffset(0x0C)] public (float Start, float End) Wrap;
        [FieldOffset(0x14)] public int Font;

        [FieldOffset(0x1C)] public byte Justification;

        [FieldOffset(0x1E), MarshalAs(UnmanagedType.I1)] public bool DropShadow;
        [FieldOffset(0x1F), MarshalAs(UnmanagedType.I1)] public bool Outline;

        public static ref CTextStyle ScriptStyle => ref *(CTextStyle*)(void*)Memory.CTextStyle_ScriptStyle;

        public static readonly bool ScriptStyleAvailable = Memory.CTextStyle_ScriptStyle != System.IntPtr.Zero;
    }
}