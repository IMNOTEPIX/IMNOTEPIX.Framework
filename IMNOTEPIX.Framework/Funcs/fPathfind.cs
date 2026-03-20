namespace IMNOTEPIX.Framework.Funcs
{
    using GTA.Math;
    using GTA.Native;

    public static class fPathfind
    {
        // ─────────────────────────────────────────────────────────────────────
        // Vehicle nodes
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the closest vehicle nav node to <paramref name="xyz"/>.
        /// <paramref name="outPosition"/> is the full world-space position of the node.
        /// nodeFlags: 0 = any | 1 = main road | bit flags for other constraints.
        /// </summary>
        public static unsafe bool GetClosestVehicleNode(
            Vector3 xyz,
            out Vector3 outPosition,
            int nodeFlags = 0,
            float p5 = 0f,
            float p6 = 0f)
        {
            Vector3 vector = default;
            bool result = Function.Call<bool>(
                Hash.GET_CLOSEST_VEHICLE_NODE,
                xyz.X, xyz.Y, xyz.Z, &vector, nodeFlags, p5, p6);
            outPosition = vector;
            return result;
        }

        /// <summary>
        /// Returns the closest vehicle nav node as a 2D map position.
        /// outPosition.X = world X, outPosition.Y = world Z (height is discarded).
        /// Use the Vector3 overload if you need the full position including height.
        /// nodeFlags: 0 = any | 1 = main road | bit flags for other constraints.
        /// </summary>
        public static unsafe bool GetClosestVehicleNode(
            Vector3 xyz,
            out Vector2 outPosition,
            int nodeFlags = 0,
            float p5 = 0f,
            float p6 = 0f)
        {
            Vector3 vector = default;
            bool result = Function.Call<bool>(
                Hash.GET_CLOSEST_VEHICLE_NODE,
                xyz.X, xyz.Y, xyz.Z, &vector, nodeFlags, p5, p6);

            // World X/Z → 2D map X/Y. World Y (north/south) is dropped intentionally.
            outPosition = new Vector2(vector.X, vector.Z);
            return result;
        }

        /// <summary>
        /// Returns the closest vehicle nav node along with the road heading at that node.
        /// nodeType: 0 = main roads | 1 = any dry path | 3 = water
        /// </summary>
        public static unsafe bool GetClosestVehicleNodeWithHeading(
            Vector3 pos,
            out Vector3 outPosition,
            out float outHeading,
            int nodeType,
            float p6 = 3.0f,
            float p7 = 0f)
        {
            Vector3 vector = default;
            float heading = default;
            bool result = Function.Call<bool>(
                Hash.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING,
                pos.X, pos.Y, pos.Z, &vector, &heading, nodeType, p6, p7);
            outPosition = vector;
            outHeading = heading;
            return result;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Road queries
        // ─────────────────────────────────────────────────────────────────────

        public static bool IsPointOnRoad(Vector3 xyz)
        {
            return Function.Call<bool>(Hash.IS_POINT_ON_ROAD, xyz.X, xyz.Y, xyz.Z);
        }

        /// <summary>
        /// Returns a position on the side of the road nearest to <paramref name="pos"/>.
        /// p3 is always -1 for this native — it selects the nearest side automatically.
        /// </summary>
        public static unsafe bool GetPositionBySideOfRoad(Vector3 pos, out Vector3 outPosition)
        {
            Vector3 vector = default;
            bool result = Function.Call<bool>(
                Hash.GET_POSITION_BY_SIDE_OF_ROAD,
                pos.X, pos.Y, pos.Z,
                -1,   // side selector: -1 = nearest side, 0 = left, 1 = right
                &vector);
            outPosition = vector;
            return result;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Road enable / disable
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Enables or disables AI roads in a rectangle defined by two points and a width.
        /// Used to activate North Yankton prologue road nodes.
        /// unknown1/2/3 — keep as false/true/true for prologue road activation.
        /// </summary>
        public static void SetRoadsInAngledArea(
            Vector3 xyz1,
            Vector3 xyz2,
            float width,
            bool unknown1,
            bool unknown2,
            bool unknown3)
        {
            Function.Call(Hash.SET_ROADS_IN_ANGLED_AREA,
                xyz1.X, xyz1.Y, xyz1.Z,
                xyz2.X, xyz2.Y, xyz2.Z,
                width, unknown1, unknown2, unknown3);
        }

        /// <summary>
        /// Enables or disables AI roads in an axis-aligned box.
        /// nodeEnabled = false disables pathfinding through the area.
        /// </summary>
        public static void SetRoadsInArea(
            Vector3 xyz1,
            Vector3 xyz2,
            bool nodeEnabled,
            bool unknown2)
        {
            Function.Call(Hash.SET_ROADS_IN_AREA,
                xyz1.X, xyz1.Y, xyz1.Z,
                xyz2.X, xyz2.Y, xyz2.Z,
                nodeEnabled, unknown2);
        }

        /// <summary>
        /// Allows or blocks the prologue (North Yankton) AI path nodes from streaming.
        /// Must be true before North Yankton roads are usable by AI.
        /// </summary>
        public static void SetAllowStreamPrologueNodes(bool toggle)
        {
            Function.Call(Hash.SET_ALLOW_STREAM_PROLOGUE_NODES, toggle);
        }
    }
}
