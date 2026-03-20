using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Reflection;

namespace IMNOTEPIX.Framework.Funcs
{
    public static class fPlayer
    {
        // ─────────────────────────────────────────────────────────────────────
        // Player ped shortcuts
        // ─────────────────────────────────────────────────────────────────────

        public static GTA.Ped Character => Game.Player.Character;

        /// <summary>
        /// Returns the player ped via the PLAYER_PED_ID native.
        /// Prefer Game.Player.Character for new code — this exists for callsites
        /// that need the native handle explicitly.
        /// </summary>
        public static GTA.Ped PlayerPedID()
            => Function.Call<GTA.Ped>(Hash.PLAYER_PED_ID);

        // ─────────────────────────────────────────────────────────────────────
        // Model identification
        // ─────────────────────────────────────────────────────────────────────

        public static uint GetPlayerModelHash() => (uint)Game.Player.Character.Model.Hash;

        public static bool IsFreemodeMale => GetPlayerModelHash() == 1885233650U;
        public static bool IsFreemodeFemale => GetPlayerModelHash() == 2627665880U;
        public static bool IsFreemodePed => IsFreemodeMale || IsFreemodeFemale;

        public static bool IsMichael => GetPlayerModelHash() == 225514697U;
        public static bool IsFranklin => GetPlayerModelHash() == 2602752943U;
        public static bool IsTrevor => GetPlayerModelHash() == 2608926626U;
        public static bool IsStoryPed => IsMichael || IsFranklin || IsTrevor;

        // ─────────────────────────────────────────────────────────────────────
        // Cutscene model swapping
        //
        // These methods write directly to ped memory to temporarily swap the
        // model hash used by cutscene systems. This is not a native API.
        //
        // WARNING: memory offset +56 (0x38) from ped base address is
        // build-specific. Verify against your target GTA V version before use.
        // An incorrect offset will corrupt ped state silently.
        //
        // Freemode peds: previous model is not stored — their hashes are
        // well-known constants and are written directly in PlayerModelSetBack.
        // Non-freemode peds: previous model is saved in PlayerPreviousModel.
        // ─────────────────────────────────────────────────────────────────────

        public static PedHash PlayerPreviousModel;

        // The cutscene male model hash written during cutscene setup.
        // Value: -1080659212L (build-specific — verify before changing).
        private const long CutsceneModelHash = -1080659212L;

        public static unsafe void PlayerModelSet(GTA.Ped ped)
        {
            // Save the previous model only for non-freemode peds.
            // Freemode hashes are constants and restored directly in PlayerModelSetBack.
            if (!IsFreemodeMale && !IsFreemodeFemale)
                PlayerPreviousModel = ped.Model;

            WriteModelHash(ped, CutsceneModelHash);
        }

        public static unsafe void PlayerModelSetBack(GTA.Ped ped)
        {
            if (IsFreemodeMale)
                WriteModelHash(ped, 1885233650L);
            else if (IsFreemodeFemale)
                WriteModelHash(ped, unchecked((long)(ulong)2627665880U));
            else
                WriteModelHash(ped, (long)(ulong)PlayerPreviousModel);
        }

        private static unsafe void WriteModelHash(GTA.Ped ped, long hash)
        {
            ulong baseAddr = (ulong)(long)ped.MemoryAddress;
            *(long*)(baseAddr + 56UL) = hash; // +32 + 24 = +56 from ped base
        }

        // ─────────────────────────────────────────────────────────────────────
        // Wanted level
        //
        // SHVDN's Wanted class has no public constructor — CreateWanted() uses
        // reflection to call the internal constructor with the player handle.
        // This mirrors how SHVDN instantiates it internally.
        // ─────────────────────────────────────────────────────────────────────

        // Lazily initialized to avoid class-load-time reflection failure
        // if SHVDN isn't ready yet when the static class is first accessed.
        private static Wanted _wanted;
        private static Wanted Wanted => _wanted ??= CreateWanted(Game.Player.Handle);

        public static bool IsWanted => Wanted.WantedLevel > 0;

        public static void SetWantedLevel(int level)
        {
            try
            {
                Wanted.SetWantedLevel(level, false);
                Wanted.ApplyWantedLevelChangeNow(false);
            }
            catch (Exception ex)
            {
                fHud.ShowNotification("Wanted function failed: " + ex.Message, true);
            }
        }

        /// <summary>Clears all wanted stars immediately.</summary>
        public static void SetWantedLevelTo0() => SetWantedLevel(0);

        public static int FakeWantedLevel
        {
            get => Function.Call<int>(Hash.GET_FAKE_WANTED_LEVEL);
            set => Function.Call(Hash.SET_FAKE_WANTED_LEVEL, value);
        }

        public static int MaxWantedLevel
        {
            get => Function.Call<int>(Hash.GET_MAX_WANTED_LEVEL);
            set => Function.Call(Hash.SET_MAX_WANTED_LEVEL, value);
        }

        /// <summary>Prevents any wanted level from being applied.</summary>
        public static void SetMaxWantedLevelTo0() => MaxWantedLevel = 0;

        /// <summary>Restores the standard 5-star wanted system.</summary>
        public static void SetMaxWantedLevelToNormal() => MaxWantedLevel = 5;

        // ─────────────────────────────────────────────────────────────────────
        // Position / teleport
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Teleports the player ped and resets the gameplay camera.
        /// Camera RelativeHeading is set to 0 — the subtraction in the original
        /// was always zero and is removed here.
        /// </summary>
        public static void PedPos(float x, float y, float z, float heading)
        {
            Game.Player.Character.Position = new Vector3(x, y, z);
            Game.Player.Character.Heading = heading;
            GameplayCamera.RelativeHeading = 0f;
            GameplayCamera.RelativePitch = 0f;
        }

        public static void PedPos(float x, float y, float z)
        {
            Game.Player.Character.Position = new Vector3(x, y, z);
            GameplayCamera.RelativeHeading = 0f;
            GameplayCamera.RelativePitch = 0f;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Distance helpers
        // ─────────────────────────────────────────────────────────────────────

        public static float GetDistanceTo(Vector3 position)
            => Game.Player.Character.Position.DistanceTo(position);

        public static float GetDistanceTo(Vector2 position)
        {
            var pos = Game.Player.Character.Position;
            return new Vector2(pos.X, pos.Y).DistanceTo(position);
        }

        /// <summary>
        /// Returns the distance from the player's current vehicle to a position.
        /// Returns float.NaN if the player is not in a vehicle.
        /// </summary>
        public static float GetCarDistanceTo(Vector3 position)
        {
            var vehicle = Game.Player.Character.CurrentVehicle;
            return vehicle != null
                ? vehicle.Position.DistanceTo(position)
                : float.NaN;
        }

        public static float GetCarDistanceTo(Vector2 position)
        {
            var vehicle = Game.Player.Character.CurrentVehicle;
            if (vehicle == null) return float.NaN;

            var pos = vehicle.Position;
            return new Vector2(pos.X, pos.Y).DistanceTo(position);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Gait / movement
        // ─────────────────────────────────────────────────────────────────────

        public static void ResetInputGait()
            => Function.Call(Hash.RESET_PLAYER_INPUT_GAIT, Game.Player);

        public static void SimulateInputGait(
            float moveBlendRatio,
            int timer = 2000,
            float heading = 0f,
            bool useRelativeHeading = true,
            bool noInputInterruption = false)
        {
            Function.Call(Hash.SIMULATE_PLAYER_INPUT_GAIT,
                Game.Player, moveBlendRatio, timer,
                heading, useRelativeHeading, noInputInterruption);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a Wanted instance via reflection. SHVDN's Wanted class has no
        /// public constructor — it is only instantiated internally by the library.
        /// This mirrors that internal instantiation using the player handle.
        /// If SHVDN changes the constructor signature this will throw at runtime.
        /// </summary>
        private static Wanted CreateWanted(int playerHandle)
        {
            var type = typeof(Wanted);
            var instance = type.Assembly.CreateInstance(
                type.FullName,
                ignoreCase: false,
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                args: new object[] { playerHandle },
                culture: null,
                activationAttributes: null);

            return (Wanted)instance
                ?? throw new InvalidOperationException(
                    "Failed to create Wanted instance via reflection. " +
                    "SHVDN constructor signature may have changed.");
        }
    }
}
