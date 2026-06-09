using GTA;
using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Pathfind
    {
        public  class Nodes
        {
            /// <summary>
            /// Returns the closest vehicle nav node to <paramref name="xyz"/>.
            /// <paramref name="outPosition"/> is the full world-space position of the node.
            /// nodeFlags: 0 = any | 1 = main road | bit flags for other constraints.
            /// </summary>
            public static unsafe bool GetClosestVehicleNode(Vector3 xyz, out Vector3 outPosition, int nodeFlags = 0, float p5 = 0f, float p6 = 0f)
            {
                Vector3 vector = default;
                bool result = Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE, xyz.X, xyz.Y, xyz.Z, &vector, nodeFlags, p5, p6);
                outPosition = vector;
                return result;
            }

            /// <summary>
            /// Returns the closest vehicle nav node as a 2D map position.
            /// outPosition.X = world X, outPosition.Y = world Z (height is discarded).
            /// Use the Vector3 overload if you need the full position including height.
            /// nodeFlags: 0 = any | 1 = main road | bit flags for other constraints.
            /// </summary>
            public static unsafe bool GetClosestVehicleNode(Vector3 xyz, out Vector2 outPosition, int nodeFlags = 0, float p5 = 0f, float p6 = 0f)
            {
                Vector3 vector = default;
                bool result = Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE, xyz.X, xyz.Y, xyz.Z, &vector, nodeFlags, p5, p6);

                // World X/Z → 2D map X/Y. World Y (north/south) is dropped intentionally.
                outPosition = new Vector2(vector.X, vector.Z);
                return result;
            }

            /// <summary>
            /// Returns the closest vehicle nav node along with the road heading at that node.
            /// nodeType: 0 = main roads | 1 = any dry path | 3 = water
            /// </summary>
            public static unsafe bool GetClosestVehicleNodeWithHeading(Vector3 pos, out Vector3 outPosition, out float outHeading, int nodeType, float p6 = 3.0f, float p7 = 0f)
            {
                Vector3 vector = default;
                float heading = default;
                bool result = Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING, pos.X, pos.Y, pos.Z, &vector, &heading, nodeType, p6, p7);
                outPosition = vector;
                outHeading = heading;
                return result;
            }

            /// <summary>
            /// Allows or blocks the prologue (North Yankton) AI path nodes from streaming.
            /// Must be true before North Yankton roads are usable by AI.
            /// </summary>
            public static void SetAllowStreamPrologueNodes(bool toggle)
               => Function.Call(Hash.SET_ALLOW_STREAM_PROLOGUE_NODES, toggle);
        }

        public class Roads
        {
            public static bool IsPointOnRoad(Vector3 xyz)
                => Function.Call<bool>(Hash.IS_POINT_ON_ROAD, xyz.X, xyz.Y, xyz.Z);

            /// <summary>
            /// Returns a position on the side of the road nearest to <paramref name="pos"/>.
            /// p3 is always -1 for this native — it selects the nearest side automatically.
            /// </summary>
            public static unsafe bool GetPositionBySideOfRoad(Vector3 pos, out Vector3 outPosition)
            {
                Vector3 vector = default;
                bool result = Function.Call<bool>(Hash.GET_POSITION_BY_SIDE_OF_ROAD, pos.X, pos.Y, pos.Z,
                    -1,   // side selector: -1 = nearest side, 0 = left, 1 = right
                    &vector);
                outPosition = vector;
                return result;
            }

            /// <summary>
            /// Enables or disables AI roads in a rectangle defined by two points and a width.
            /// Used to activate North Yankton prologue road nodes.
            /// unknown1/2/3 — keep as false/true/true for prologue road activation.
            /// </summary>
            public static void SetRoadsInAngledArea(Vector3 xyz1, Vector3 xyz2, float width, bool unknown1, bool unknown2, bool unknown3)
               => Function.Call(Hash.SET_ROADS_IN_ANGLED_AREA, xyz1.X, xyz1.Y, xyz1.Z, xyz2.X, xyz2.Y, xyz2.Z, width, unknown1, unknown2, unknown3);

            /// <summary>
            /// Enables or disables AI roads in an axis-aligned box.
            /// nodeEnabled = false disables pathfinding through the area.
            /// </summary>
            public static void SetRoadsInArea(Vector3 xyz1, Vector3 xyz2, bool nodeEnabled, bool unknown2)
               => Function.Call(Hash.SET_ROADS_IN_AREA, xyz1.X, xyz1.Y, xyz1.Z, xyz2.X, xyz2.Y, xyz2.Z, nodeEnabled, unknown2);
        }

        public class Navmesh
        {

            public static void AddNavmeshRequiredRegion(float x, float y, float radius)
                => Function.Call(Hash.ADD_NAVMESH_REQUIRED_REGION, x, y, radius);

            public static void AddNavmeshRequiredRegion(Vector2 xy, float radius)
                => Function.Call(Hash.ADD_NAVMESH_REQUIRED_REGION, xy.X, xy.Y, radius);

            public static void RemoveNavmeshRequiredRegions()
                => Function.Call(Hash.REMOVE_NAVMESH_REQUIRED_REGIONS);

            public static bool IsNavmeshRequiredRegionInUse()
                => Function.Call<bool>(Hash.IS_NAVMESH_REQUIRED_REGION_IN_USE);

            public static void DisableNavmeshInArea(float x1, float y1, float z1, float x2, float y2, float z2, bool toggle)
                => Function.Call(Hash.DISABLE_NAVMESH_IN_AREA, x1, y1, z1, x2, y2, z2, toggle);

            public static void DisableNavmeshInArea(Vector3 xyz1, Vector3 xyz2, bool toggle)
                => Function.Call(Hash.DISABLE_NAVMESH_IN_AREA, xyz1.X, xyz1.Y, xyz1.Z, xyz2.X, xyz2.Y, xyz2.Z, toggle);

            public static bool AreAllNavmeshRegionsLoaded()
                => Function.Call<bool>(Hash.ARE_ALL_NAVMESH_REGIONS_LOADED);

            public static bool IsNavmeshLoadedInArea(float x1, float y1, float z1, float x2, float y2, float z2)
                => Function.Call<bool>(Hash.IS_NAVMESH_LOADED_IN_AREA, x1, y1, z1, x2, y2, z2);

            public static bool IsNavmeshLoadedInArea(Vector3 xyz1, Vector3 xyz2)
                => Function.Call<bool>(Hash.IS_NAVMESH_LOADED_IN_AREA, xyz1.X, xyz1.Y, xyz1.Z, xyz2.X, xyz2.Y, xyz2.Z);

            public static int GetNumNavmeshesExistingInArea(float x1, float y1, float z1, float x2, float y2, float z2)
                => Function.Call<int>(Hash.GET_NUM_NAVMESHES_EXISTING_IN_AREA, x1, y1, z1, x2, y2, z2);

            public static int GetNumNavmeshesExistingInArea(Vector3 xyz1, Vector3 xyz2)
                => Function.Call<int>(Hash.GET_NUM_NAVMESHES_EXISTING_IN_AREA, xyz1.X, xyz1.Y, xyz1.Z, xyz2.X, xyz2.Y, xyz2.Z);

            public static int AddNavmeshBlockingObject(float x, float y, float z, float width, float length, float height, float heading, bool bPermanent, int flags)
                => Function.Call<int>(Hash.ADD_NAVMESH_BLOCKING_OBJECT, x, y, z, width, length, height, heading, bPermanent, flags);

            public static int AddNavmeshBlockingObject(Vector3 xyz, Vector3 scale, float heading, bool bPermanent, int flags)
                => Function.Call<int>(Hash.ADD_NAVMESH_BLOCKING_OBJECT, xyz.X, xyz.Y, xyz.Z, scale.X, scale.Y, scale.Z, heading, bPermanent, flags);

            public static void UpdateNavmeshBlockingObject(int handle, float posX, float posY, float posZ, float scaleX, float scaleY, float scaleZ, float heading, int flags)
                => Function.Call(Hash.UPDATE_NAVMESH_BLOCKING_OBJECT, handle, posX, posY, posZ, scaleX, scaleY, scaleZ, heading, flags);

            public static void UpdateNavmeshBlockingObject(int handle, Vector3 position, Vector3 scale, float heading, int flags)
                => Function.Call(Hash.UPDATE_NAVMESH_BLOCKING_OBJECT, handle, position.X, position.Y, position.Z, scale.X, scale.Y, scale.Z, heading, flags);

            public static void RemoveNavmeshBlockingObject(int handle)
                => Function.Call(Hash.REMOVE_NAVMESH_BLOCKING_OBJECT, handle);

            public static bool DoesNavmeshBlockingObjectExist(int handle)
                => Function.Call<bool>(Hash.DOES_NAVMESH_BLOCKING_OBJECT_EXIST, handle);
        }
    }
}
