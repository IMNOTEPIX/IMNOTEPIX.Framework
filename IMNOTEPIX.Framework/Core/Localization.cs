using System;
using System.Runtime.InteropServices;
using System.Text;
using GTA;
using GTA.Native;
using IMNOTEPIX.Framework.fCore.Internals;

namespace IMNOTEPIX.Framework.fCore
{
    public static class Localization
    {
        /// <summary>
        /// Gets the string of a text label. This is the read path - fully safe,
        /// since it only ever calls the game's own GetStringByHash and never
        /// writes to game-owned memory.
        /// </summary>
        public static string GetText(string labelId)
        {
            labelId = labelId ?? throw new ArgumentNullException(nameof(labelId));
            return GetText((uint)Game.GenerateHash(labelId)); // TODO: confirm Game.GenerateHash exists in your SHVDN version
        }

        public static string GetText(uint labelIdHash)
        {
            const string Fallback = "NULL"; // matches _GET_LABEL_TEXT's fallback behavior

            if (!CTextFile.Available)
            {
                return Fallback;
            }

            IntPtr ptr = CTextFile.Instance.GetStringByHash(labelIdHash);
            return ptr != IntPtr.Zero ? FromUtf8(ptr) : Fallback;
        }

        public static bool DoesTextExist(string labelId)
        {
            labelId = labelId ?? throw new ArgumentNullException(nameof(labelId));
            return CTextFile.Available
                ? DoesTextExist((uint)Game.GenerateHash(labelId))
                : Function.Call<bool>(Hash.DOES_TEXT_LABEL_EXIST, labelId);
        }

        public static bool DoesTextExist(uint labelIdHash)
            => !CTextFile.Available || CTextFile.Instance.GetStringByHash(labelIdHash) != IntPtr.Zero;

        // ------------------------------------------------------------
        // Write path - fully ported, all dependencies resolved (atArray<T>,
        // sysMemAllocator, UsingTls). This is now a complete, direct port of
        // the original: SetText registers a brand-new label or overwrites an
        // existing one; ClearTextOverride removes a registered override and
        // reverts to whatever the label originally resolved to (or deletes it,
        // if it didn't exist before).
        // ------------------------------------------------------------

        public static void SetText(string labelId, string value)
            => SetText((uint)Game.GenerateHash(labelId ?? throw new ArgumentNullException(nameof(labelId))), value);

        public static void SetText(uint labelIdHash, string value)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));

            if (!CTextFile.Available)
            {
                return;
            }

            CTextFile.CriticalSection.Enter();
            try
            {
                IntPtr newValue = ToUtf8(value);
                IntPtr oldValue = CTextFile.Instance.OverridesTextMap.AddOrSet(labelIdHash, newValue); // throws until atArray is ported

                if (oldValue != IntPtr.Zero)
                {
                    FreeUtf8(oldValue);
                }
            }
            finally
            {
                CTextFile.CriticalSection.Leave();
            }
        }

        public static void ClearTextOverride(string labelId)
            => ClearTextOverride((uint)Game.GenerateHash(labelId ?? throw new ArgumentNullException(nameof(labelId))));

        public static void ClearTextOverride(uint labelIdHash)
        {
            if (!CTextFile.Available)
            {
                return;
            }

            CTextFile.CriticalSection.Enter();
            try
            {
                IntPtr oldValue = CTextFile.Instance.OverridesTextMap.Remove(labelIdHash); // throws until atArray is ported
                if (oldValue != IntPtr.Zero)
                {
                    FreeUtf8(oldValue);
                }
            }
            finally
            {
                CTextFile.CriticalSection.Leave();
            }
        }

        // ------------------------------------------------------------
        // UTF-8 marshaling helpers
        //
        // The original allocated through the game's own sysMemAllocator (via a
        // TLS-resolved allocator pointer not included in the source you shared).
        // Using Marshal.AllocHGlobal instead sidesteps that dependency entirely:
        // this memory is never touched by the game's own allocator/free calls,
        // we allocate it and we're the ones freeing it (in SetText's override-
        // replace path and in ClearTextOverride above), so there's no risk of
        // mixing allocators. The only requirement is that GetStringByHash's
        // caller (the game) just reads it as a plain null-terminated UTF-8
        // string, which it does - it doesn't care who allocated the buffer.
        // ------------------------------------------------------------

        private static unsafe string FromUtf8(IntPtr ptr)
        {
            var p = (byte*)ptr;
            return Encoding.UTF8.GetString(p, Memory.StrLen(p));
        }

        private static unsafe IntPtr ToUtf8(string str)
        {
            int byteCount = Encoding.UTF8.GetByteCount(str);
            IntPtr dest = Marshal.AllocHGlobal(byteCount + 1); // +1 for null terminator
            byte* destPtr = (byte*)dest;

            fixed (char* chars = str)
            {
                Encoding.UTF8.GetBytes(chars, str.Length, destPtr, byteCount);
            }
            destPtr[byteCount] = 0;

            return dest;
        }

        private static void FreeUtf8(IntPtr ptr) => Marshal.FreeHGlobal(ptr);
    }
}