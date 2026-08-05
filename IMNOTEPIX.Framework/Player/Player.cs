using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Reflection;

namespace IMNOTEPIX.Framework.fPlayer
{
    public class Player
    {
        public static GTA.Ped Character => Game.Player.Character;

        [Obsolete("Preferably use Game.Player.Character or fWorld.Player.Character instead.")]
        public static GTA.Ped PlayerPedID()
            => Function.Call<GTA.Ped>(Hash.PLAYER_PED_ID);

        public static GTA.Player PlayerId()
            => Function.Call<GTA.Player>(Hash.PLAYER_ID);

        public static uint GetPlayerModelHash() => (uint)Game.Player.Character.Model.Hash;

        public static bool IsFreemodeMale => GetPlayerModelHash() == 1885233650U;
        public static bool IsFreemodeFemale => GetPlayerModelHash() == 2627665880U;
        public static bool IsFreemodePed => IsFreemodeMale || IsFreemodeFemale;

        public static bool IsMichael => GetPlayerModelHash() == 225514697U;
        public static bool IsFranklin => GetPlayerModelHash() == 2602752943U;
        public static bool IsTrevor => GetPlayerModelHash() == 2608926626U;
        public static bool IsStoryPed => IsMichael || IsFranklin || IsTrevor;

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

        public static void PlayerModelSetBack(GTA.Ped ped)
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

        public static bool IsWanted => Game.Player.Wanted.WantedLevel > 0;

        public static void SetWantedLevel(int level)
        {
            Game.Player.Wanted.SetWantedLevel(level, false);
            Game.Player.Wanted.ApplyWantedLevelChangeNow(false);
        }

        public static void SetWantedLevelNoDrop(int level)
        {
            Game.Player.Wanted.SetWantedLevelNoDrop(level, false);
            Game.Player.Wanted.ApplyWantedLevelChangeNow(false);
        }

        public static void SetDispatchCops(bool enable)
            => Game.Player.Wanted.DispatchesCopsForPlayer = enable;

        /// <summary>Clears all wanted stars immediately.</summary>
        [Obsolete("Use ClearWantedLevel instead.")]
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

        public static float GetDistanceTo(Vector3 position)
            => Game.Player.Character.Position.DistanceTo(position);

        public static float GetDistanceTo(Vector2 position)
        {
            var pos = Game.Player.Character.Position;
            return new Vector2(pos.X, pos.Y).DistanceTo(position);
        }

        public static float GetVehicleDistanceTo(Vector3 position)
        {
            var vehicle = Game.Player.Character.CurrentVehicle;
            return vehicle != null ? vehicle.Position.DistanceTo(position) : float.NaN;
        }

        public static float GetVehicleDistanceTo(Vector2 position)
        {
            var vehicle = Game.Player.Character.CurrentVehicle;
            if (vehicle == null)
                return float.NaN;

            var pos = vehicle.Position;
            return new Vector2(pos.X, pos.Y).DistanceTo(position);
        }

        public static void ResetInputGait()
            => Function.Call(Hash.RESET_PLAYER_INPUT_GAIT, Game.Player);

        public static void SimulateInputGait(float moveBlendRatio, int timer = 2000, float heading = 0f, bool useRelativeHeading = true, bool noInputInterruption = false)
           => Function.Call(Hash.SIMULATE_PLAYER_INPUT_GAIT, Game.Player, moveBlendRatio, timer, heading, useRelativeHeading, noInputInterruption);

        public static bool IsBeingArrested(bool atArresting)
            => Function.Call<bool>(Hash.IS_​PLAYER_​BEING_​ARRESTED, Game.Player, atArresting);

        public static void ClearWantedLevel()
            => Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, Game.Player);

        public static void PoliceIgnorePlayer(bool ignore)
            => Game.Player.Wanted.SetPoliceIgnorePlayer(ignore);

        public static void EveryoneIgnorePlayer(bool ignore)
            => Game.Player.Wanted.SetEveryoneIgnorePlayer(ignore);

        /// <summary>
        /// Sets whether all random peds will run away from the player if they are agitated (threatened) (bool=true), or if they will stand their ground (bool=false).
        /// </summary>
        /// <param name="toggle">true/false.</param>
        public static void SetAllRandomPedsFlee(bool toggle)
            => Function.Call(Hash.SET_​ALL_​RANDOM_​PEDS_​FLEE, Game.Player, toggle);

        public static void SetAllRandomPedsFleeThisFrame()
            => Function.Call(Hash.SET_​ALL_​RANDOM_​PEDS_​FLEE_​THIS_​FRAME, Game.Player);

        public static void SetWantedLevelMultiplier(float multiplier)
            => Function.Call(Hash.SET_​WANTED_​LEVEL_​MULTIPLIER, multiplier);

        /// <summary>
        /// Max value is 1.0.
        /// </summary>
        /// <param name="difficulty"></param>
        public static void SetWantedLevelDifficulty(float difficulty)
            => Function.Call(Hash.SET_​WANTED_​LEVEL_​DIFFICULTY, Game.Player, difficulty);

        public static void ResetWantedLevelDifficulty()
            => Function.Call(Hash.RESET_​WANTED_​LEVEL_​DIFFICULTY, Game.Player);

        public static void ReportCrime(CrimeType crimeType, int wantedLvlThresh)
            => Function.Call(Hash.REPORT_​CRIME, Game.Player, (int)crimeType, wantedLvlThresh);

        public static void SuppressCrimeThisFrame(int crimeType)
            => Function.Call(Hash.SUPPRESS_​CRIME_​THIS_​FRAME, Game.Player, crimeType);

        public static void SuppressWitnessesCallingPoliceThisFrame()
            => Function.Call(Hash.SUPPRESS_​WITNESSES_​CALLING_​POLICE_​THIS_​FRAME, Game.Player);

        public static void SetPlayerCanBeHassledByGangs(bool toggle)
            => Function.Call(Hash.SET_​PLAYER_​CAN_​BE_​HASSLED_​BY_​GANGS, Game.Player, toggle);

        public static void GangHassle(bool toggle)
        {
            RelationshipGroup player = new RelationshipGroup(RelationshipGroupHash.Player);
            if (!toggle)
            {
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangBallas, Relationship.Like);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangCult, Relationship.Like);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangFamily, Relationship.Like);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangHillbilly, Relationship.Like);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangLost, Relationship.Like);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangMarabunte, Relationship.Like);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangMexican, Relationship.Like);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangSalva, Relationship.Like);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangWeiCheng, Relationship.Like);
                SetPlayerCanBeHassledByGangs(false);
            }
            else
            {
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangBallas, Relationship.Pedestrians);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangCult, Relationship.Pedestrians);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangFamily, Relationship.Pedestrians);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangHillbilly, Relationship.Pedestrians);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangLost, Relationship.Pedestrians);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangMarabunte, Relationship.Pedestrians);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangMexican, Relationship.Pedestrians);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangSalva, Relationship.Pedestrians);
                player.SetRelationshipBetweenGroups(RelationshipGroupHash.AmbientGangWeiCheng, Relationship.Pedestrians);
                SetPlayerCanBeHassledByGangs(true);
            }
        }

        public static void SetPlayerCanUseCover(bool toggle)
            => Function.Call(Hash.SET_​PLAYER_​CAN_​USE_​COVER, Game.Player, toggle);

        // used with 1,2,8,64,128 in the scripts
        public static void ForceCleanup(int cleanupFlags)
            => Function.Call(Hash.FORCE_​CLEANUP, cleanupFlags);

        public static void EnableSpecialAbility(bool toggle)
            => Function.Call(Hash.ENABLE_SPECIAL_ABILITY, Game.Player, toggle);

        public static int GetSpecialAbility()
        {
            int result = -1;
            unsafe
            {
                if (!IsMichael)
                {
                    if (!IsFranklin)
                    {
                        if (IsTrevor)
                            Function.Call<bool>(Hash.STAT_GET_INT, fWorld.Misc.joaat("sp2_special_ability"), &result, -1);
                    }
                    else
                        Function.Call<bool>(Hash.STAT_GET_INT, fWorld.Misc.joaat("sp1_special_ability"), &result, -1);
                }
                else
                    Function.Call<bool>(Hash.STAT_GET_INT, fWorld.Misc.joaat("sp0_special_ability"), &result, -1);

            }
            return result;
        }

        public static void SetSpecialAbility (int specialAbility)
        {
            if (!IsMichael)
            {
                if (!IsFranklin)
                {
                    if (IsTrevor)
                        Function.Call<bool>(Hash.STAT_SET_INT, fWorld.Misc.joaat("sp2_special_ability"), specialAbility, true);
                }
                else
                    Function.Call<bool>(Hash.STAT_SET_INT, fWorld.Misc.joaat("sp1_special_ability"), specialAbility, true);
            }
            else
                Function.Call<bool>(Hash.STAT_SET_INT, fWorld.Misc.joaat("sp0_special_ability"), specialAbility, true);
            Function.Call(Hash.UPDATE_SPECIAL_ABILITY_FROM_STAT, Game.Player, 0);
        }

        public static void UnlockSpecialAbility(bool toggle)
        {
            if (IsMichael)
                Function.Call(Hash.SPECIAL_ABILITY_UNLOCK, fWorld.Misc.joaat("player_zero"), 0);
            if (IsFranklin)
                Function.Call(Hash.SPECIAL_ABILITY_UNLOCK, fWorld.Misc.joaat("player_one"), 0);
            if (IsTrevor)
                Function.Call(Hash.SPECIAL_ABILITY_UNLOCK, fWorld.Misc.joaat("player_two"), 0);
        }
        public static void LockSpecialAbility(bool toggle)
        {
            if (IsMichael)
                Function.Call(Hash.SPECIAL_ABILITY_LOCK, fWorld.Misc.joaat("player_zero"), 0);
            if (IsFranklin)
                Function.Call(Hash.SPECIAL_ABILITY_LOCK, fWorld.Misc.joaat("player_one"), 0);
            if (IsTrevor)
                Function.Call(Hash.SPECIAL_ABILITY_LOCK, fWorld.Misc.joaat("player_two"), 0);
        }
    }
}
