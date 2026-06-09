using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Vehicle
    {
        public enum TrainTrack
        {
            /// <summary>Main loop around all of San Andreas. Sample pos: (1084.48, 3231.45, 39.26)</summary>
            MainLoop = 0,
            /// <summary>Branch serving Davis Quartz quarry. Sample pos: (2787.94, 2837.48, 35.39)</summary>
            DavisQuartzBranch,
            /// <summary>Second track alongside the live track along Roy Lewenstein Blvd. Sample pos: (290.53, -1845.34, 25.78)</summary>
            RoyLewensteinParallel,
            /// <summary>Underground metro circuit. Sample pos: (193.20, -603.84, 16.76)</summary>
            MetroCircuit,
            /// <summary>First branch spur into Mirror Park railyard. Sample pos: (536.95, -452.13, 23.80)</summary>
            MirrorParkBranch1,
            /// <summary>Second branch spur into Mirror Park railyard. Sample pos: (561.52, -432.01, 23.84)</summary>
            MirrorParkBranch2,
            /// <summary>Los Santos main branch leading to Mirror Park railyard. Sample pos: (499.60, -1571.59, 28.34)</summary>
            LosSantosToMirrorPark,
            /// <summary>Overground section of metro track along Forum Dr. Sample pos: (-213.54, -1019.98, 28.14)</summary>
            MetroForumDrOverground,
            /// <summary>Branch connecting to Mirror Park railyard. Sample pos: (532.34, -952.22, 26.12)</summary>
            MirrorParkConnector,
            /// <summary>North Yankton freight train. Sample pos: (4318.79, -4714.68, 112.31)</summary>
            YanktonFreight,
            /// <summary>Partial metro track near Mission Row. Sample pos: (519.82, -1187.75, 28.32)</summary>
            MetroMissionRow,
            /// <summary>North Yankton prologue mission train. Sample pos: (4243.14, -6506.32, 109.38)</summary>
            YanktonPrologueMission,
        }

        /// <summary>Enables or disables train spawning on <paramref name="track"/>.</summary>
        public static void SwitchTrainTrack(TrainTrack track, bool enabled)
        {
            Function.Call(Hash.SWITCH_TRAIN_TRACK, (int)track, enabled);
        }

        public static GTA.Vehicle CreateVehicle(Model model, Vector3 pos, float heading = 0f)
        {
            GTA.Vehicle veh = GTA.World.CreateVehicle(model, pos, heading);

            if (veh != null && !veh.Exists())
                GTA.Script.Yield();

            return (veh != null && veh.Exists()) ? veh : null;
        }

        public static GTA.Vehicle CreateVehicleForList(List<GTA.Vehicle> list, Model model, Vector3 pos, float heading = 0f)
        {
            GTA.Vehicle veh = CreateVehicle(model, pos, heading);

            if (veh != null && !list.Contains(veh))
                list.Add(veh);

            return veh;
        }

        public static void DeleteVehicleList(List<GTA.Vehicle> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && list[i].Exists())
                    list[i].Delete();
            }
            list.Clear();
        }

        // Does not resize the array; callers should null out slots if needed.
        public static void DeleteVehicleArray(GTA.Vehicle[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null && array[i].Exists())
                    array[i].Delete();
            }
        }

        /// <summary>
        /// Sets the primary and secondary paint colours on <paramref name="vehicle"/>.
        /// <see href="https://pastebin.com/pwHci0xK">Colour index reference.</see>
        /// </summary>
        public static void SetVehicleColours(GTA.Vehicle vehicle, int primaryColour, int secondaryColour)
        {
            Function.Call(Hash.SET_VEHICLE_COLOURS, vehicle, primaryColour, secondaryColour);
        }

        /// <summary>
        /// Marks whether <paramref name="vehicle"/> is considered by the player's targeting system.
        /// </summary>
        public static void SetVehicleConsideredByPlayer(GTA.Vehicle vehicle, bool considered)
        {
            Function.Call(Hash.SET_VEHICLE_IS_CONSIDERED_BY_PLAYER, vehicle, considered);
        }

        /// <summary>Disables or re-enables a specific mounted weapon on <paramref name="vehicle"/>.</summary>
        public static void DisableVehicleWeapon(GTA.Vehicle vehicle, GTA.Ped owner, Hash weaponHash, bool disabled)
        {
            Function.Call(Hash.DISABLE_VEHICLE_WEAPON, disabled, weaponHash, vehicle, owner);
        }

        /// <summary>Returns <c>true</c> if the specified mounted weapon on <paramref name="vehicle"/> is disabled.</summary>
        public static bool IsVehicleWeaponDisabled(GTA.Vehicle vehicle, GTA.Ped owner, Hash weaponHash)
        {
            return Function.Call<bool>(Hash.IS_VEHICLE_WEAPON_DISABLED, weaponHash, vehicle, owner);
        }

        /// <summary>Returns the current engine health of <paramref name="vehicle"/> (0–1000).</summary>
        public static float GetVehicleEngineHealth(GTA.Vehicle vehicle)
        {
            return Function.Call<float>(Hash.GET_VEHICLE_ENGINE_HEALTH, vehicle);
        }

        /// <summary>
        /// Attempts to place <paramref name="vehicle"/> flat on the ground beneath it.
        /// </summary>
        /// <remarks>
        /// Returns <c>true</c> on success.
        /// </remarks>
        public static bool SetVehicleOnProperly(GTA.Vehicle vehicle, float p1 = 5.0f)
        {
            return Function.Call<bool>(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, vehicle, p1);
        }

        /// <summary>
        /// Returns <c>true</c> once the vehicle's stuck timer exceeds <paramref name="thresholdMs"/> milliseconds.
        /// </summary>
        public static bool IsVehicleStuckTimerUp(GTA.Vehicle vehicle, int timerIndex, int thresholdMs)
        {
            return Function.Call<bool>(Hash.IS_VEHICLE_STUCK_TIMER_UP, vehicle, timerIndex, thresholdMs);
        }

        public static void SetVehicleStrong(GTA.Vehicle vehicle, bool toggle)
            => Function.Call(Hash.SET_VEHICLE_STRONG, vehicle, toggle);

        public static GTA.Vehicle CreateVehicle(int modelHash, float x, float y, float z, float heading, bool isNetwork, bool bScriptHostVeh, bool p7)
            => Function.Call<GTA.Vehicle>(Hash.CREATE_VEHICLE, modelHash, x, y, z, heading, isNetwork, bScriptHostVeh, p7);
        public static GTA.Vehicle CreateVehicle(int modelHash, Vector3 xyz, float heading, bool isNetwork, bool bScriptHostVeh, bool p7)
            => Function.Call<GTA.Vehicle>(Hash.CREATE_VEHICLE, modelHash, xyz.X, xyz.Y, xyz.Z, heading, isNetwork, bScriptHostVeh, p7);

        public static void SetVehicleDirtLevel(GTA.Vehicle vehicle, float dirtLevel)
            => Function.Call(Hash.SET_VEHICLE_DIRT_LEVEL, vehicle, dirtLevel);

        public static GTA.Ped GetPedInVehicleSeat(GTA.Vehicle vehicle, int seatIndex, bool p2)
            => Function.Call<GTA.Ped>(Hash.GET_PED_IN_VEHICLE_SEAT, vehicle, seatIndex, p2);

        public static void SetVehicleSiren(GTA.Vehicle vehicle, bool toggle)
            => Function.Call(Hash.SET_VEHICLE_SIREN, vehicle, toggle);

        public static void SetVehicleHasMutedSirens(GTA.Vehicle vehicle, bool toggle)
            => Function.Call(Hash.SET_VEHICLE_HAS_MUTED_SIRENS, vehicle, toggle);

        public static void SetVehicleEngineHealth(GTA.Vehicle vehicle, float health)
            => Function.Call(Hash.SET_VEHICLE_ENGINE_HEALTH, vehicle, health);

        public static float GetVehiclePetrolTankHealth(GTA.Vehicle vehicle)
            => Function.Call<float>(Hash.GET_VEHICLE_PETROL_TANK_HEALTH, vehicle);

        public static void SetVehiclePetrolTankHealth(GTA.Vehicle vehicle, float health)
            => Function.Call(Hash.SET_VEHICLE_PETROL_TANK_HEALTH, vehicle, health);
    }
}
