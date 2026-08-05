using GTA;
using GTA.Math;
using GTA.Native;
using System;

namespace IMNOTEPIX.Framework.fWorld.Map
{
    public class Interior
    {
        public static float GetInteriorHeading(int interior)
            => Function.Call<float>(Hash.GET_​INTERIOR_​HEADING, interior);

        public static void GetInteriorLocationAndNamehash(int interior, out Vector3 position, out uint nameHash)
        {
            Vector3 outPos = default;
            uint outHash = default;
            unsafe { Function.Call(Hash.GET_​INTERIOR_​LOCATION_​AND_​NAMEHASH, interior, &outPos, &outHash); }
            position = outPos;
            nameHash = outHash;
        }

        public static void GetInteriorLocationAndNamehash(int interior, out float x, out float y, out float z, out uint nameHash)
        {
            Vector3 outPos = default;
            uint outHash = default;
            unsafe { Function.Call(Hash.GET_​INTERIOR_​LOCATION_​AND_​NAMEHASH, interior, &outPos, &outHash); }
            x = outPos.X;
            y = outPos.Y;
            z = outPos.Z;
            nameHash = outHash;
        }

        public static int GetInteriorGroupId(int interior)
            => Function.Call<int>(Hash.GET_​INTERIOR_​GROUP_​ID, interior);

        public static Vector3 GetOffsetFromInteriorInWorldCoords(int interior, float x, float y, float z)
            => Function.Call<Vector3>(Hash.GET_​OFFSET_​FROM_​INTERIOR_​IN_​WORLD_​COORDS, interior, x, y, z);

        public static Vector3 GetOffsetFromInteriorInWorldCoords(int interior, Vector3 xyz)
            => Function.Call<Vector3>(Hash.GET_​OFFSET_​FROM_​INTERIOR_​IN_​WORLD_​COORDS, interior, xyz.X, xyz.Y, xyz.Z);

        public static bool IsInteriorScene()
            => Function.Call<bool>(Hash.IS_​INTERIOR_​SCENE);

        public static bool IsValidInterior(int interior)
            => Function.Call<bool>(Hash.IS_​VALID_​INTERIOR, interior);

        public static void ClearRoomForEntity(GTA.Entity entity)
            => Function.Call(Hash.CLEAR_​ROOM_​FOR_​ENTITY, entity);

        public static void ForceRoomForEntity(GTA.Entity entity, int interior, int roomHashKey)
            => Function.Call(Hash.FORCE_ROOM_FOR_ENTITY, entity, interior, roomHashKey);

        public static uint GetRoomKeyFromEntity(GTA.Entity entity)
            => Function.Call<uint>(Hash.GET_ROOM_KEY_FROM_ENTITY, entity);

        public static uint GetKeyForEntityInRoom(GTA.Entity entity)
            => Function.Call<uint>(Hash.GET_​KEY_​FOR_​ENTITY_​IN_​ROOM, entity);

        public static uint GetInteriorFromEntity(GTA.Entity entity)
            => Function.Call<uint>(Hash.GET_INTERIOR_FROM_ENTITY, entity);

        public static void RetainEntityInInterior(GTA.Entity entity, int interior)
            => Function.Call(Hash.RETAIN_​ENTITY_​IN_​INTERIOR, entity, interior);

        public static void ClearInteriorStateOfEntity(GTA.Entity entity)
            => Function.Call(Hash.CLEAR_​INTERIOR_​STATE_​OF_​ENTITY, entity);

        /// <summary>
        /// Undocumented, p0 might be entity, p1 might be a bool toggle on/off.
        /// </summary>
        public static void ForceActivatingTrackingOnEntity(int p0, int p1)
            => Function.Call(Hash.FORCE_​ACTIVATING_​TRACKING_​ON_​ENTITY, p0, p1);

        public static void ForceRoomForGameViewport(int interiorID, uint roomHashKey)
            => Function.Call(Hash.FORCE_​ROOM_​FOR_​GAME_​VIEWPORT, interiorID, roomHashKey);

        public static void SetRoomForGameViewportByName(string roomName)
            => Function.Call(Hash.SET_​ROOM_​FOR_​GAME_​VIEWPORT_​BY_​NAME, roomName);

        public static void SetRoomForGameViewportByKey(uint roomHashKey)
            => Function.Call(Hash.SET_​ROOM_​FOR_​GAME_​VIEWPORT_​BY_​KEY, roomHashKey);

        public static uint GetRoomKeyForGameViewport()
            => Function.Call<uint>(Hash.GET_​ROOM_​KEY_​FOR_​GAME_​VIEWPORT);

        public static void ClearRoomForGameViewport()
            => Function.Call(Hash.CLEAR_​ROOM_​FOR_​GAME_​VIEWPORT);

        public static int GetInteriorFromPrimaryView()
            => Function.Call<int>(Hash.GET_​INTERIOR_​FROM_​PRIMARY_​VIEW);

        public static int GetInteriorAtCoords(float x, float y, float z)
            => Function.Call<int>(Hash.GET_​INTERIOR_​AT_​COORDS, x, y, z);

        public static int GetInteriorAtCoords(Vector3 xyz)
            => Function.Call<int>(Hash.GET_​INTERIOR_​AT_​COORDS, xyz.X, xyz.Y, xyz.Z);

        public static void AddPickupToInteriorRoomByName(Pickup pickup, string roomName)
            => Function.Call(Hash.ADD_​PICKUP_​TO_​INTERIOR_​ROOM_​BY_​NAME, pickup, roomName);

        public static void PinInteriorInMemory(int interior)
           => Function.Call(Hash.PIN_​INTERIOR_​IN_​MEMORY, interior);

        public static void UnpinInterior(int interior)
            => Function.Call(Hash.UNPIN_​INTERIOR, interior);

        public static bool IsInteriorReady(int interior)
            => Function.Call<bool>(Hash.IS_​INTERIOR_​READY, interior);

        public static bool SetInteriorInUse(int interior)
            => Function.Call<bool>(Hash.SET_​INTERIOR_​IN_​USE, interior);

        public static int GetInteriorAtCoordsWithType(Vector3 xyz, string interiorType)
            => Function.Call<int>(Hash.GET_​INTERIOR_​AT_​COORDS_​WITH_​TYPE, xyz.X, xyz.Y, xyz.Z, interiorType);

        public static int GetInteriorAtCoordsWithType(float x, float y, float z, string interiorType)
            => Function.Call<int>(Hash.GET_​INTERIOR_​AT_​COORDS_​WITH_​TYPE, x, y, z, interiorType);

        public static int GetInteriorAtCoordsWithTypehash(float x, float y, float z, uint typeHash)
            => Function.Call<int>(Hash.GET_​INTERIOR_​AT_​COORDS_​WITH_​TYPEHASH, x, y, z, typeHash);

        public static int GetInteriorAtCoordsWithTypehash(Vector3 xyz, uint typeHash)
            => Function.Call<int>(Hash.GET_​INTERIOR_​AT_​COORDS_​WITH_​TYPEHASH, xyz.X, xyz.Y, xyz.Z, typeHash);

        public static void ActivateInteriorGroupsUsingCamera()
            => Function.Call(Hash.ACTIVATE_​INTERIOR_​GROUPS_​USING_​CAMERA);

        public static bool IsCollisionMarkedOutside(float x, float y, float z)
            => Function.Call<bool>(Hash.IS_​COLLISION_​MARKED_​OUTSIDE, x, y, z);

        public static bool IsCollisionMarkedOutside(Vector3 xyz)
            => Function.Call<bool>(Hash.IS_​COLLISION_​MARKED_​OUTSIDE, xyz.X, xyz.Y, xyz.Z);

        public static int GetInteriorFromCollision(float x, float y, float z)
            => Function.Call<int>(Hash.GET_​INTERIOR_​FROM_​COLLISION, x, y, z);

        public static int GetInteriorFromCollision(Vector3 xyz)
            => Function.Call<int>(Hash.GET_​INTERIOR_​FROM_​COLLISION, xyz.X, xyz.Y, xyz.Z);

        public static void EnableStadiumProbesThisFrame(bool toggle)
            => Function.Call(Hash.ENABLE_​STADIUM_​PROBES_​THIS_​FRAME, toggle);

        public static void ActivateInteriorEntitySet(int interior, string entitySetName)
            => Function.Call(Hash.ACTIVATE_INTERIOR_ENTITY_SET, interior, entitySetName);

        public static void DeactivateInteriorEntitySet(int interior, string entitySetName)
            => Function.Call(Hash.DEACTIVATE_INTERIOR_ENTITY_SET, interior, entitySetName);

        public static bool IsInteriorEntitySetActive(int interior, string entitySetName)
            => Function.Call<bool>(Hash.IS_INTERIOR_ENTITY_SET_ACTIVE, interior, entitySetName);

        public static void SetInteriorEntitySetTintIndex(int interior, string entitySetName, int color)
            => Function.Call(Hash.SET_​INTERIOR_​ENTITY_​SET_​TINT_​INDEX, interior, entitySetName, color);

        public static void RefreshInterior(int interior)
            => Function.Call(Hash.REFRESH_​INTERIOR, interior);

        public static void EnableExteriorCullModelThisFrame(uint mapObjectHash)
            => Function.Call(Hash.ENABLE_​EXTERIOR_​CULL_​MODEL_​THIS_​FRAME, mapObjectHash);

        public static void EnableShadowCullModelThisFrame(uint mapObjectHash)
            => Function.Call(Hash.ENABLE_​SHADOW_​CULL_​MODEL_​THIS_​FRAME, mapObjectHash);

        public static void DisableInterior(int interior, bool toggle = true)
            => Function.Call(Hash.DISABLE_​INTERIOR, interior, toggle);

        public static bool IsInteriorDisabled(int interior)
            => Function.Call<bool>(Hash.IS_​INTERIOR_​DISABLED, interior);

        public static void CapInterior(int interior, bool toggle)
            => Function.Call(Hash.CAP_​INTERIOR, interior, toggle);

        public static bool IsInteriorCapped(int interior)
            => Function.Call<bool>(Hash.IS_​INTERIOR_​CAPPED, interior);

        public static void DisableMetroSystem(bool toggle)
            => Function.Call(Hash.DISABLE_​METRO_​SYSTEM, toggle);

        public static void SetIsExteriorOnly(GTA.Entity entity, bool toggle)
            => Function.Call(Hash.SET_​IS_​EXTERIOR_​ONLY, entity, toggle);
    }
}
