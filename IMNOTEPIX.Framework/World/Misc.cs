using GTA;
using GTA.Math;
using GTA.Native;
using System;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Misc
    {
        public enum DispatchType
        {
            DT_Invalid,
            DT_PoliceAutomobile,
            DT_PoliceHelicopter,
            DT_FireDepartment,
            DT_SwatAutomobile,
            DT_AmbulanceDepartment,
            DT_PoliceRiders,
            DT_PoliceVehicleRequest,
            DT_PoliceRoadBlock,
            DT_PoliceAutomobileWaitPulledOver,
            DT_PoliceAutomobileWaitCruising,
            DT_Gangs,
            DT_SwatHelicopter,
            DT_PoliceBoat,
            DT_ArmyVehicle,
            DT_BikerBackup
        }

        public static void EnableDispatchService(DispatchType type, bool toggle)
        {
            Function.Call(Hash.BLOCK_DISPATCH_SERVICE_RESOURCE_CREATION, (int)type, !toggle);
            Function.Call(Hash.ENABLE_DISPATCH_SERVICE, (int)type, toggle);
        }

        public static bool HasBulletImpactedInArea(float x, float y, float z, float radius)
            => Function.Call<bool>(Hash.HAS_BULLET_IMPACTED_IN_AREA, x, y, z, radius);

        /// <summary>
        /// Calls the CLEAR_AREA native directly.
        /// Use ClearAreaEntities() if you need selective entity deletion by type.
        /// </summary>
        public static void ClearArea(float x, float y, float z, float radius, bool p4, bool ignoreCopCars, bool ignoreObjects, bool p7)
           => Function.Call(Hash.CLEAR_AREA, x, y, z, radius, p4, ignoreCopCars, ignoreObjects, p7);

        /// <summary>
        /// Deletes all entities within <paramref name="radius"/> of <paramref name="center"/>,
        /// with optional per-type exclusions.
        ///
        /// NOTE: renamed from ClearArea() and flag semantics corrected —
        /// the original had inverted logic where skipVehicles=true deleted everything
        /// except vehicles rather than preserving them.
        /// </summary>
        public static void ClearAreaEntities(Vector3 center, float radius, bool skipVehicles = false, bool skipPeds = false, bool skipProps = false)
        {
            foreach (var entity in GTA.World.GetAllEntities())
            {
                if (entity.Position.DistanceTo(center) > radius) 
                    continue;

                switch (entity.EntityType)
                {
                    case EntityType.Vehicle 
                    when skipVehicles: continue;
                    case EntityType.Ped 
                    when skipPeds: continue;
                    case EntityType.Prop 
                    when skipProps: continue;
                }

                entity.Delete();
            }
        }

        public static void ClearAreaOfVehicles(Vector3 xyz, float radius, bool p4 = false, bool p5 = false, bool p6 = false, bool p7 = false, bool p8 = false, bool p9 = false, int p10 = 0)
           => Function.Call(Hash.CLEAR_AREA_OF_VEHICLES, xyz.X, xyz.Y, xyz.Z, radius, p4, p5, p6, p7, p8, p9, p10);

        public static void SetTimeScale(float timeScale)
           => Function.Call(Hash.SET_TIME_SCALE, timeScale);

        public static void SetGamePaused(bool toggle)
           => Function.Call(Hash.SET_GAME_PAUSED, toggle);

        public static void DisableHospitalRestart(int hospitalID, bool disable)
           => Function.Call(Hash.DISABLE_HOSPITAL_RESTART, hospitalID, disable);

        public static void DisablePoliceRestart(int policeID, bool disable)
           => Function.Call(Hash.DISABLE_POLICE_RESTART, policeID, disable);

        [Obsolete("Use joaat instead.")]
        public static Hash GetHashKey(string str)
            => Function.Call<Hash>(Hash.GET_HASH_KEY, str);

        public static uint joaatUnregistered(string str)
            => Function.Call<uint>(Hash.GET_HASH_KEY, str);

        public static int joaat(string str)
            => Function.Call<int>(Hash.GET_HASH_KEY, str);

        public static int GetRandomIntInRange(int startRange, int endRange)
            => Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, startRange, endRange);
    }
}
