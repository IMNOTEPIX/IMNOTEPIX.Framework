using GTA;
using System;
using System.Runtime.InteropServices;

namespace IMNOTEPIX.Framework.fCore.Internals
{
    /// <summary>
    /// Extension of Memory (see Memory.cs) adding scrProgramRegistry resolution and the
    /// "ingamehud" script-global pattern scans this was missing. Everything here was already
    /// present in the RAGENativeUI source you sent - this just wires it up using the same
    /// Game.FindPattern approach already confirmed working for the CTextFile port, instead
    /// of the unsafe Unsafe.AsRef&lt;float&gt; pointer arithmetic the RPH original used for reading
    /// script globals. SHVDN exposes GTA.Native.GlobalVariable for that instead (see
    /// TimerBarScriptGlobals.cs), so no raw global-variable-address pointer math was needed here.
    ///
    /// Requires: the existing Memory class declaration in Memory.cs needs the `partial` keyword
    /// added (`internal static unsafe partial class Memory`) so this file's members merge into it.
    /// </summary>
    internal static unsafe partial class Memory
    {
        // ------------------------------------------------------------
        // scrProgramRegistry - needed to reach into a loaded script's bytecode
        // (e.g. "ingamehud") to pattern-scan for its global-variable indices.
        // ------------------------------------------------------------

        private static readonly (IntPtr Instance, IntPtr Find) scrProgramRegistryAddrs = ResolveScrProgramRegistry();

        public static IntPtr scrProgramRegistry_sm_Instance => scrProgramRegistryAddrs.Instance;
        public static IntPtr scrProgramRegistry_Find => scrProgramRegistryAddrs.Find;

        private static (IntPtr, IntPtr) ResolveScrProgramRegistry()
        {
            IntPtr addr = FindAddress(() => Game.FindPattern("48 8D 0D ?? ?? ?? ?? 8B D0 44 8B F0 89 85"));
            if (addr == IntPtr.Zero)
            {
                return (IntPtr.Zero, IntPtr.Zero);
            }

            IntPtr instance = addr + *(int*)(addr + 3) + 7;
            IntPtr find = addr + *(int*)(addr + 19) + 23;
            return (instance, find);
        }

        // ------------------------------------------------------------
        // "ingamehud" script-global indices - resolved by pattern-scanning the
        // ingamehud script's own bytecode (not the exe) via FindPatternInScript below.
        // Kept as public static readonly fields, same shape as the original.
        // ------------------------------------------------------------

        private static readonly (int PrevTotalHeightGlobalId, int SharedGlobalId, int SharedTimerbarsTotalHeightOffset) timershudIds = ResolveTimershudIds();
        private static readonly int timershudInstructionalButtonsNumRowsOffset = ResolveTimershudInstructionalButtonsNumRowsOffset();

        public static int TimerbarsPrevTotalHeightGlobalId => timershudIds.PrevTotalHeightGlobalId;
        public static int TimershudSharedGlobalId => timershudIds.SharedGlobalId;
        public static int TimershudSharedTimerbarsTotalHeightOffset => timershudIds.SharedTimerbarsTotalHeightOffset;
        public static int TimershudSharedInstructionalButtonsNumRowsOffset => timershudInstructionalButtonsNumRowsOffset;

        private static (int, int, int) ResolveTimershudIds()
        {
            // b2802 introduced three new script opcodes after CALL (0x5D), shifting these
            // byte values by +3 in builds from 2802 onward.
            int fix = GetExeBuildNumber() >= 2802 ? 3 : 0;

            IntPtr ingamehudAddr = FindPatternInScript("ingamehud",
                new byte[] { (byte)(0x6F + fix), 0x39, 0x08, (byte)(0x5F + fix), 0x00, 0x00, 0x00, (byte)(0x5E + fix), 0x00, 0x00, 0x00, 0x47 },
                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF });

            if (ingamehudAddr == IntPtr.Zero)
            {
                ConsoleLog.Debug("[[IMNOTEPIX]] [ingamehud Memory] Total-height pattern not found.");
                return (-1, -1, -1);
            }

            int prevTotalHeightGlobalId = *(int*)(ingamehudAddr + 4) & 0xFFFFFF;
            int sharedGlobalId = *(int*)(ingamehudAddr + 8) & 0xFFFFFF;
            int sharedTimerbarsTotalHeightOffset = *(short*)(ingamehudAddr + 12);
            return (prevTotalHeightGlobalId, sharedGlobalId, sharedTimerbarsTotalHeightOffset);
        }

        private static int ResolveTimershudInstructionalButtonsNumRowsOffset()
        {
            IntPtr ingamehudAddr = FindPatternInScript("ingamehud",
                new byte[] { 0x47, 0x00, 0x00, 0x22, 0x10, 0x0E, 0x39 },
                new byte[] { 0xFF, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF });

            if (ingamehudAddr == IntPtr.Zero)
            {
                ConsoleLog.Debug("[[IMNOTEPIX]] [ingamehud Memory] InstructionalButtons rows pattern not found.");
                return -1;
            }

            return *(short*)(ingamehudAddr + 1);
        }

        // ------------------------------------------------------------
        // Pattern-scanning inside a loaded script's own bytecode (as opposed to the exe,
        // which Game.FindPattern already covers). Direct port of the original.
        // ------------------------------------------------------------

        private static IntPtr FindPatternInScript(string name, byte[] pattern, byte[] mask)
        {
            if (scrProgramRegistry_sm_Instance == IntPtr.Zero || scrProgramRegistry_Find == IntPtr.Zero)
            {
                ConsoleLog.Debug($"[[IMNOTEPIX]] [FindPatternInScript] ERROR - scrProgramRegistry not available");
                return IntPtr.Zero;
            }

            scrProgram* prog = scrProgramRegistry.Instance.Find(name);

            if (prog == null)
            {
                ConsoleLog.Debug($"[[IMNOTEPIX]] [FindPatternInScript] ERROR - scrProgram '{name}' not loaded");
                return IntPtr.Zero;
            }

            const uint CodePageMaxSize = 0x4000;

            int numCodePages = (int)(prog->CodeLength + (CodePageMaxSize - 1)) >> 14;
            for (int i = 0; i < numCodePages; i++)
            {
                uint size = i == (numCodePages - 1) ? (prog->CodeLength & 0x3FFF) : CodePageMaxSize;

                IntPtr match = FindPatternInRange(prog->CodePages[i], size, pattern, mask);
                if (match != IntPtr.Zero)
                {
                    return match;
                }
            }

            ConsoleLog.Debug($"[[IMNOTEPIX]] [FindPatternInScript] ERROR - pattern not found in scrProgram '{name}'");
            return IntPtr.Zero;
        }

        private static IntPtr FindPatternInRange(byte* address, uint size, byte[] pattern, byte[] mask)
        {
            byte* endAddress = address + (size - pattern.Length);
            for (; address <= endAddress; address += 1)
            {
                bool found = true;
                for (int i = 0; i < pattern.Length; i++)
                {
                    if ((address[i] & mask[i]) != pattern[i])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return (IntPtr)address;
                }
            }

            return IntPtr.Zero;
        }
    }

    /// <summary>
    /// scrProgramRegistry/scrProgram - direct port, unchanged. These describe the game's own
    /// internal script-registry layout, not anything RPH-specific, so they carry over as-is.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 0x658)]
    internal unsafe struct scrProgramRegistry
    {
        public scrProgram* Find(string name) => Find((uint)Game.GenerateHash(name));
        // NOTE: verify Game.GetHashKey(string) is the exact SHVDN member name/signature in your build.
        public scrProgram* Find(uint name) => ((delegate* unmanaged[Thiscall]<ref scrProgramRegistry, uint, scrProgram*>)Memory.scrProgramRegistry_Find)(ref this, name);

        public static ref scrProgramRegistry Instance => ref *(scrProgramRegistry*)(void*)Memory.scrProgramRegistry_sm_Instance;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x80)]
    internal unsafe struct scrProgram
    {
        [FieldOffset(0x10)] public byte** CodePages;
        [FieldOffset(0x18)] public uint Hash;
        [FieldOffset(0x1C)] public uint CodeLength;
        [FieldOffset(0x20)] public uint ArgsCount;
        [FieldOffset(0x24)] public uint StaticsCount;
        [FieldOffset(0x28)] public uint GlobalsCount;
        [FieldOffset(0x2C)] public uint NativesCount;
        [FieldOffset(0x58)] public uint NameHash;
        [FieldOffset(0x5C)] public uint NumRefs;
        [FieldOffset(0x60)] public IntPtr Name;
        [FieldOffset(0x68)] public byte** Strings;
        [FieldOffset(0x70)] public uint StringsCount;
    }
}