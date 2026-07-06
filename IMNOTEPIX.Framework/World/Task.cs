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

        public static void TaskVehicleDriveWander(GTA.Ped ped, GTA.Vehicle vehicle, float speed, int drivingStyle)
            => Function.Call(Hash.TASK_​VEHICLE_​DRIVE_​WANDER, ped, vehicle, speed, drivingStyle);

        public static void TaskVehicleDriveToCoord(GTA.Ped ped, GTA.Vehicle vehicle, Vector3 xyz, float speed, int p6, int vehicleModel, int drivingMode, float stopRange, float straightLineDistance)
            => Function.Call(Hash.TASK_​VEHICLE_​DRIVE_​TO_​COORD, ped, vehicle, xyz.X, xyz.Y, xyz.Z, speed, p6, vehicleModel, drivingMode, stopRange, straightLineDistance);

        public static void TaskVehicleDriveToCoord(GTA.Ped ped, GTA.Vehicle vehicle, float x, float y, float z, float speed, int p6, int vehicleModel, int drivingMode, float stopRange, float straightLineDistance)
            => Function.Call(Hash.TASK_​VEHICLE_​DRIVE_​TO_​COORD, ped, vehicle, x, y, z, speed, p6, vehicleModel, drivingMode, stopRange, straightLineDistance);

        public static void TaskVehicleMissionCoordsTarget(GTA.Ped ped, GTA.Vehicle vehicle, Vector3 xyz, int mission, float cruiseSpeed, int drivingStyle, float targetReached, float straightLineDistance, bool DriveAgainstTraffic)
            => Function.Call(Hash.TASK_​VEHICLE_​MISSION_​COORS_​TARGET, ped, vehicle, xyz.X, xyz.Y, xyz.Z, mission, cruiseSpeed, drivingStyle, targetReached, straightLineDistance, DriveAgainstTraffic);

        public static void TaskVehicleMissionCoordsTarget(GTA.Ped ped, GTA.Vehicle vehicle, float x, float y, float z, int mission, float cruiseSpeed, int drivingStyle, float targetReached, float straightLineDistance, bool DriveAgainstTraffic)
            => Function.Call(Hash.TASK_​VEHICLE_​MISSION_​COORS_​TARGET, ped, vehicle, x, y, z, mission, cruiseSpeed, drivingStyle, targetReached, straightLineDistance, DriveAgainstTraffic);

        public static void TaskVehicleEscort(GTA.Ped ped, GTA.Vehicle vehicle, GTA.Vehicle targetVehicle, int mode, float speed, int drivingStyle, float minDistance, int minHeightAboveTerrain, float noRoadsDistance)
            => Function.Call(Hash.TASK_​VEHICLE_​ESCORT, ped, vehicle, targetVehicle, mode, speed, drivingStyle, minDistance, minHeightAboveTerrain, noRoadsDistance);

        public static void TaskCombatPed(GTA.Ped ped, GTA.Ped targetPed, int combatFlags, int threatResponseFlags)
            => Function.Call(Hash.TASK_​COMBAT_​PED, ped, targetPed, combatFlags, threatResponseFlags);

        public static void ClearPedTasks(GTA.Ped ped)
            => Function.Call(Hash.CLEAR_​PED_​TASKS, ped);

        public static int GetScriptTaskStatus(GTA.Ped ped, int taskHash)
            => Function.Call<int>(Hash.GET_​SCRIPT_​TASK_​STATUS, ped, taskHash);

        public static void TaskLookAtEntity(GTA.Ped ped, GTA.Entity lookAt, int duration, int flags, int priority)
            => Function.Call(Hash.TASK_​LOOK_​AT_​ENTITY, ped, lookAt, duration, flags, priority);
    }
}
