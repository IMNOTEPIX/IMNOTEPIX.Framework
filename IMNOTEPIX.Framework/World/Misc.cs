using GTA;
using GTA.Math;
using GTA.Native;
using System;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Misc
    {
        public static float GetRandomFloatInRange(float startRange, float endRange)
            => Function.Call<float>(Hash.GET_​RANDOM_​FLOAT_​IN_​RANGE, startRange, endRange);

        public static void ShootSingleBulletBetweenCoords(float x1, float y1, float z1, float x2, float y2, float z2, int damage, bool p7, int weaponHash, GTA.Ped ownerPed, bool isAudible, bool isInvisible, float speed)
            => Function.Call(Hash.SHOOT_​SINGLE_​BULLET_​BETWEEN_​COORDS, x1, y1, z1, x2, y2, z2, damage, p7, weaponHash, ownerPed, isAudible, isInvisible, speed);

        public static void ShootSingleBulletBetweenCoords(Vector3 xyz1, Vector3 xyz2, int damage, bool p7, int weaponHash, GTA.Ped ownerPed, bool isAudible, bool isInvisible, float speed)
            => Function.Call(Hash.SHOOT_​SINGLE_​BULLET_​BETWEEN_​COORDS, xyz1.X, xyz1.Y, xyz1.Z, xyz2.X, xyz2.Y, xyz2.Z, damage, p7, weaponHash, ownerPed, isAudible, isInvisible, speed);

        public static unsafe bool GetGroundZFor3DCoord(float x, float y, float z, float* groundZ, bool ignoreWater, bool p5)
            => Function.Call<bool>(Hash.GET_​GROUND_​Z_​FOR_​3D_​COORD, x, y, z, groundZ, ignoreWater, p5);

        public static unsafe bool GetGroundZFor3DCoord(Vector3 xyz, float* groundZ, bool ignoreWater, bool p5)
            => Function.Call<bool>(Hash.GET_​GROUND_​Z_​FOR_​3D_​COORD, xyz.X, xyz.Y, xyz.Z, groundZ, ignoreWater, p5);

        public static bool IsBulletInArea(float x, float y, float z, float radius, bool ownedByPlayer)
            => Function.Call<bool>(Hash.IS_​BULLET_​IN_​AREA, x, y, z, radius, ownedByPlayer);

        public static bool IsBulletInArea(Vector3 xyz, float radius, bool ownedByPlayer)
            => Function.Call<bool>(Hash.IS_​BULLET_​IN_​AREA, xyz.X, xyz.Y, xyz.Z, radius, ownedByPlayer);

        public static float GetDistanceBetweenCoords(float x1, float y1, float z1, float x2, float y2, float z2, bool useZ)
            => Function.Call<float>(Hash.GET_DISTANCE_BETWEEN_COORDS, x1, y1, z1, x2, y2, z2, useZ);

        public static float GetDistanceBetweenCoords(Vector3 xyz1, Vector3 xyz2, bool useZ)
            => Function.Call<float>(Hash.GET_DISTANCE_BETWEEN_COORDS, xyz1.X, xyz1.Y, xyz1.Z, xyz2.X, xyz2.Y, xyz2.Z, useZ);

        public static int GetGameTimer()
            => Function.Call<int>(Hash.GET_GAME_TIMER);

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
                    when skipVehicles || entity == fPlayer.Player.Character.CurrentVehicle: continue;
                    case EntityType.Ped 
                    when skipPeds || entity == fPlayer.Player.Character: continue;
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
        public static int GetHashKey(string str)
            => Function.Call<int>(Hash.GET_HASH_KEY, str);

        public static uint joaatUnregistered(string str)
            => Function.Call<uint>(Hash.GET_HASH_KEY, str);

        public static int joaat(string str)
            => Function.Call<int>(Hash.GET_HASH_KEY, str);

        public static int GetRandomIntInRange(int startRange, int endRange)
            => Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, startRange, endRange);

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
    }
}
