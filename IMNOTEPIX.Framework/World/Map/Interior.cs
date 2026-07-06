using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fWorld.Map
{
    public class Interior
    {
        public static uint GetInteriorFromEntity(GTA.Entity entity)
            => Function.Call<uint>(Hash.GET_INTERIOR_FROM_ENTITY, entity);

        public static uint GetRoomKeyFromEntity(GTA.Entity entity)
            => Function.Call<uint>(Hash.GET_ROOM_KEY_FROM_ENTITY, entity);

        public static uint GetInteriorFromCoords(Vector3 coords)
            => Function.Call<uint>(Hash.GET_INTERIOR_AT_COORDS, coords.X, coords.Y, coords.Z);

        public static int GetInteriorAtCoordsWithType(Vector3 xyz, string interiorType)
            => Function.Call<int>(Hash.GET_​INTERIOR_​AT_​COORDS_​WITH_​TYPE, xyz.X, xyz.Y, xyz.Z, interiorType);

        public static bool IsInteriorReady(int interior)
            => Function.Call<bool>(Hash.IS_​INTERIOR_​READY, interior);

        public static bool IsIplActive(string iplName)
            => Function.Call<bool>(Hash.IS_​IPL_​ACTIVE, iplName);

        public static void ForceRoomForEntity(GTA.Entity entity, int interior, int roomHashKey)
           => Function.Call(Hash.FORCE_ROOM_FOR_ENTITY, entity, interior, roomHashKey);

        public static void SetRoomForGameViewportByName(string roomName)
           => Function.Call(Hash.SET_​ROOM_​FOR_​GAME_​VIEWPORT_​BY_​NAME, roomName);

        public static void PinInteriorInMemory(int interior)
           => Function.Call(Hash.PIN_​INTERIOR_​IN_​MEMORY, interior);

        public static void DisableInterior(int interior, bool toggle = true)
           => Function.Call(Hash.DISABLE_​INTERIOR, interior, toggle);

        public static void RequestIpl(string iplName)
           => Function.Call(Hash.REQUEST_IPL, iplName);

        public static void RemoveIpl(string iplName)
           => Function.Call(Hash.REMOVE_IPL, iplName);
    }
}
