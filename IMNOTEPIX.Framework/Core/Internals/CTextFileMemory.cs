using GTA;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IMNOTEPIX.Framework.fCore.Internals
{

    /// <summary>
    /// Port of RAGENativeUI's memory-address resolution, scaled to CTextFile only.
    /// </summary>
    // The unrelated pieces (Screen, CScaleformMgr, CBusySpinner, scrProgramRegistry,
    // CTextFormat, DirectNatives, CTextStyle, timershud globals) were dropped since
    // they're not needed for text-entry work and each one is its own separate
    // verification surface.
    internal static unsafe partial class Memory
    {
        public static readonly int TLS_AllocatorOffset = -1;
        public static readonly int sysMemAllocator_Allocate_VTableOffset = -1;

        public static readonly IntPtr CTextFile_sm_Instance;
        public static readonly IntPtr CTextFile_sm_CriticalSection;
        public static readonly IntPtr CTextFile_GetStringByHash;
        public static readonly int CTextFile_OverridesTextMapOffset = -1;

        static Memory()
        {
            // NOTE: original RPH source cached these in a Shared.MemoryAddresses[]
            // array to survive plugin-domain reloads without re-scanning. SHVDN
            // re-runs static constructors per script (re)load anyway, and a handful
            // of pattern scans costs a few ms at most, so that indirection was
            // dropped in favor of plain static fields.

            // Direct port from the original Memory.cs - needed by sysMemAllocator
            // below, which atArray<T>.Add() calls to grow OverridesTextMap.Pairs.
            IntPtr tlsAllocatorOffsetAddr = FindAddress(() => Game.FindPattern("B9 ?? ?? ?? ?? 48 8B 0C 01 45 33 C9 49 8B D2"));
            if (tlsAllocatorOffsetAddr != IntPtr.Zero)
            {
                TLS_AllocatorOffset = *(int*)(tlsAllocatorOffsetAddr + 1);
                sysMemAllocator_Allocate_VTableOffset = *(byte*)(tlsAllocatorOffsetAddr + 0x15);
            }

            CTextFile_sm_Instance = FindAddress(() =>
            {
                IntPtr addr = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 48 8B D3 E8 ?? ?? ?? ?? 48 8D 54 24 ?? 48 8D 0D");
                if (addr != IntPtr.Zero)
                {
                    addr += *(int*)(addr + 3) + 7;
                }
                return addr;
            });

            {
                IntPtr addr = FindAddress(() => Game.FindPattern("48 8D 0D ?? ?? ?? ?? 44 8B F2 E8 ?? ?? ?? ?? 83 8B"));
                if (addr != IntPtr.Zero)
                {
                    CTextFile_GetStringByHash = addr - 0x19;
                    CTextFile_sm_CriticalSection = addr + *(int*)(addr + 3) + 7;

                    // Original selected between two known offsets (0x258 pre-b3407,
                    // 0x288 from b3407 onward) using RPH's Game.ProductVersion.Build.
                    // SHVDN doesn't expose a raw build number the same way, so this
                    // reads it directly from the exe's file version instead - avoids
                    // depending on whichever GameVersion enum members happen to be
                    // defined in your SHVDN build.
                    int build = GetExeBuildNumber();
                    CTextFile_OverridesTextMapOffset = *(int*)(addr + (build >= 3407 ? 0x2D : 0x19));
                }
            }
        }

        private static int GetExeBuildNumber()
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            FileVersionInfo vi = FileVersionInfo.GetVersionInfo(exePath);
            return vi.FileBuildPart;
        }

        private static IntPtr FindAddress(Func<IntPtr> find, [CallerLineNumber] int lineNumber = 0)
        {
            IntPtr addr = find();
            if (addr == IntPtr.Zero)
            {
                ConsoleLog.Debug($"[[IMNOTEPIX]] [CTextFile Memory] Pattern at line #{lineNumber} not found.");
            }
            return addr;
        }

        public static int StrLen(byte* str)
        {
            int len = 0;
            while (str[len] != 0) { len++; }
            return len;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    internal unsafe struct atArray<T> where T : unmanaged
    {
        public T* Items;
        public ushort Count;
        public ushort Size;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= 0 && index < Count)
                {
                    return ref Items[index];
                }

                throw new ArgumentOutOfRangeException(nameof(index), index, $"Out of Range (Count:{Count}, Size:{Size})");
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(ref this);

        public ref T Add()
        {
            if (Count == Size)
            {
                var newSize = Size + 16;
                var newByteSize = (ulong)(sizeof(T) * newSize);
                var newItems = sysMemAllocator.TheAllocator.Allocate(newByteSize, 16, 0);
                Buffer.MemoryCopy(Items, newItems, newByteSize, (ulong)(sizeof(T) * Size));
                sysMemAllocator.TheAllocator.Free(Items);
                Items = (T*)newItems;
                Size = (ushort)newSize;
            }

            return ref Items[Count++];
        }

        public T RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Out of Range (Count:{Count}, Size:{Size})");
            }

            var removed = Items[index];
            for (int i = index; i < (Count - 1); i++)
            {
                Items[i] = Items[i + 1];
            }
            Count--;
            return removed;
        }

        public ref struct Enumerator
        {
            private readonly atArray<T>* array;
            private int index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(ref atArray<T> arr)
            {
                fixed (atArray<T>* p = &arr)
                    array = p;

                index = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int newIndex = index + 1;
                if (newIndex < array->Count)
                {
                    index = newIndex;
                    return true;
                }

                return false;
            }

            public ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref (*array)[index];
            }
        }
    }

    internal unsafe struct sysMemAllocator
    {
        private readonly IntPtr* vtable;

        public void* Allocate(ulong size, ulong align, int subAllocator) =>
            ((delegate* unmanaged[Thiscall]<ref sysMemAllocator, ulong, ulong, int, void*>)vtable[VTableIndexAllocate])(ref this, size, align, subAllocator);

        public void Free(void* ptr) =>
            ((delegate* unmanaged[Thiscall]<ref sysMemAllocator, void*, void>)vtable[VTableIndexFree])(ref this, ptr);

        private static readonly int VTableIndexAllocate = Memory.sysMemAllocator_Allocate_VTableOffset / 8;
        private static readonly int VTableIndexFree = VTableIndexAllocate + 2;

        public static unsafe ref sysMemAllocator TheAllocator =>
            ref *(sysMemAllocator*)(void*)(IntPtr)UsingTls.GetFromMain(Memory.TLS_AllocatorOffset);
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x28)]
    internal struct CRITICAL_SECTION
    {
        public IntPtr DebugInfo;

        public void Enter()
        {
            if (DebugInfo != IntPtr.Zero)
            {
                EnterCriticalSection(ref this);
            }
        }

        public void Leave()
        {
            if (DebugInfo != IntPtr.Zero)
            {
                LeaveCriticalSection(ref this);
            }
        }

        [DllImport("kernel32.dll")] private static extern void EnterCriticalSection(ref CRITICAL_SECTION lpCriticalSection);
        [DllImport("kernel32.dll")] private static extern void LeaveCriticalSection(ref CRITICAL_SECTION lpCriticalSection);
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CTextFile
    {
        [StructLayout(LayoutKind.Explicit, Size = 0x18)]
        public struct atBinaryMap
        {
            [StructLayout(LayoutKind.Explicit, Size = 0x10)]
            public struct DataPair
            {
                [FieldOffset(0x0)] public uint Key;
                [FieldOffset(0x8)] public IntPtr Value;
            }

            [FieldOffset(0x00), MarshalAs(UnmanagedType.I1)] public bool IsSorted;
            [FieldOffset(0x08)] public atArray<DataPair> Pairs;

            // Direct port - matches the original 1:1, now that atArray<T>.Add() is known.
            // Still transitively blocked on UsingTls.GetFromMain (see sysMemAllocator).
            public IntPtr AddOrSet(uint key, IntPtr value)
            {
                DataPair pairToInsert = new DataPair { Key = key, Value = value };
                foreach (ref var pair in Pairs)
                {
                    if (pair.Key == pairToInsert.Key)
                    {
                        var tmp = pair.Value;
                        pair.Value = pairToInsert.Value;
                        return tmp;
                    }

                    if (pair.Key > pairToInsert.Key)
                    {
                        var tmp = pair;
                        pair = pairToInsert;
                        pairToInsert = tmp;
                    }
                }

                Pairs.Add() = pairToInsert;
                return IntPtr.Zero;
            }

            public IntPtr Remove(uint key)
            {
                int index = -1;
                for (int i = 0; i < Pairs.Count; i++)
                {
                    if (Pairs[i].Key == key)
                    {
                        index = i;
                        break;
                    }
                }

                return index != -1 ? Pairs.RemoveAt(index).Value : IntPtr.Zero;
            }
        }

        public ref atBinaryMap OverridesTextMap
        {
            get
            {
                fixed (CTextFile* pThis = &this)
                {
                    return ref *(atBinaryMap*)((byte*)pThis + Memory.CTextFile_OverridesTextMapOffset);
                }
            }
        }

        public IntPtr GetStringByHash(uint hash) =>
            ((delegate* unmanaged[Thiscall]<ref CTextFile, uint, nint>)Memory.CTextFile_GetStringByHash)(ref this, hash);

        public static unsafe ref CTextFile Instance => ref *(CTextFile*)Memory.CTextFile_sm_Instance;
        public static unsafe ref CRITICAL_SECTION CriticalSection => ref *(CRITICAL_SECTION*)Memory.CTextFile_sm_CriticalSection;

        public static readonly bool Available = Memory.CTextFile_sm_Instance != IntPtr.Zero &&
                                                 Memory.CTextFile_OverridesTextMapOffset > 0 &&
                                                 Memory.CTextFile_OverridesTextMapOffset <= 0x800 &&
                                                 Memory.CTextFile_GetStringByHash != IntPtr.Zero &&
                                                 Memory.CTextFile_sm_CriticalSection != IntPtr.Zero;
    }
}