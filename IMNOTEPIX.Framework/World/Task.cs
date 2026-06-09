using GTA;
using GTA.Math;
using GTA.Native;
using System.Runtime.InteropServices;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Task
    {
        public enum VehicleChaseBehaviorFlag
        {
            None = 0,
            AggressiveRamming,
            RamAttempts,
            BoxingTacticWithPIT = 8,
            RammingLessAggressive = 16,
            StayBackNoContact = 32
        }

        public static bool DoesScriptedCoverPointExistAtCoords(float x, float y, float z)
            => Function.Call<bool>(Hash.DOES_SCRIPTED_COVER_POINT_EXIST_AT_COORDS, x, y, z);

        public static bool DoesScriptedCoverPointExistAtCoords(Vector3 xyz)
            => Function.Call<bool>(Hash.DOES_SCRIPTED_COVER_POINT_EXIST_AT_COORDS, xyz.X, xyz.Y, xyz.Z);

        public static int AddCoverPoint(float x, float y, float z, float direction, int usage, int height, int arc, bool isPriority)
            => Function.Call<int>(Hash.ADD_COVER_POINT, x, y, z, direction, usage, height, arc, isPriority);

        public static int AddCoverPoint(Vector3 xyz, float direction, int usage, int height, int arc, bool isPriority)
            => Function.Call<int>(Hash.ADD_COVER_POINT, xyz.X, xyz.Y, xyz.Z, direction, usage, height, arc, isPriority);

        public static void AddCoverBlockingArea(float startX, float startY, float startZ, float endX, float endY, float endZ, bool blockObjects, bool blockVehicles, bool blockMap, bool blockPlayer)
            => Function.Call(Hash.ADD_COVER_BLOCKING_AREA, startX, startY, startZ, endX, endY, endZ, blockObjects, blockVehicles, blockMap, blockPlayer);

        public static void AddCoverBlockingArea(Vector3 start, Vector3 end, bool blockObjects, bool blockVehicles, bool blockMap, bool blockPlayer)
            => Function.Call(Hash.ADD_COVER_BLOCKING_AREA, start.X, start.Y, start.Z, end.X, end.Y, end.Z, blockObjects, blockVehicles, blockMap, blockPlayer);

        public static void RemoveCoverPoint(int coverpoint)
            => Function.Call(Hash.REMOVE_COVER_POINT, coverpoint);

        public static void TaskCombatHatedTargetsAroundPed(GTA.Ped ped, float radius, int combatFlags)
            => Function.Call(Hash.TASK_COMBAT_HATED_TARGETS_AROUND_PED, ped, radius, combatFlags);

        public static void SetTaskVehicleChaseBehaviorFlag(GTA.Ped ped, int flag, bool set)
            => Function.Call(Hash.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG, ped, flag, set);

        public static void SetTaskVehicleChaseBehaviorFlag(GTA.Ped ped, VehicleChaseBehaviorFlag flag, bool set)
            => Function.Call(Hash.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG, ped, (int)flag, set);
    }
}
