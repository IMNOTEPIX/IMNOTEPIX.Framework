using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Zone
    {
        /// <summary>Returns the zone ID at the given world coordinates.</summary>
        public static int GetZoneAtCoords(Vector3 position)
            => Function.Call<int>(Hash.GET_ZONE_AT_COORDS, position.X, position.Y, position.Z);

        /// <summary>Returns the zone ID for a zone name string (e.g. "AIRP").</summary>
        public static int GetZoneFromNameID(string zoneName)
            => Function.Call<int>(Hash.GET_ZONE_FROM_NAME_ID, zoneName);

        /// <summary>Returns the short name label of the zone at the given world coordinates.</summary>
        public static string GetNameOfZone(Vector3 position)
            => Function.Call<string>(Hash.GET_NAME_OF_ZONE, position.X, position.Y, position.Z);

        /// <summary>
        /// Returns the zone's "scumminess" rating (1–5), used for the signal-bar
        /// display on the iFruit phone.
        /// </summary>
        public static int GetZoneScumminess(int zoneId)
            => Function.Call<int>(Hash.GET_ZONE_SCUMMINESS, zoneId);

        /// <summary>Returns the hash of the map area (interior/exterior region) at the given coordinates.</summary>
        public static Hash GetHashOfMapAreaAtCoords(Vector3 position)
            => Function.Call<Hash>(Hash.GET_HASH_OF_MAP_AREA_AT_COORDS, position.X, position.Y, position.Z);

        /// <summary>Enables or disables a zone by ID.</summary>
        public static void SetZoneEnabled(int zoneId, bool enabled)
            => Function.Call(Hash.SET_ZONE_ENABLED, zoneId, enabled);

        /// <summary>Returns the population schedule ID assigned to a zone.</summary>
        public static int GetZonePopschedule(int zoneId)
            => Function.Call<int>(Hash.GET_ZONE_POPSCHEDULE, zoneId);

        /// <summary>Overrides the vehicle model spawned by a population schedule slot.</summary>
        public static void OverridePopscheduleVehicleModel(int scheduleId, Hash vehicleHash)
            => Function.Call(Hash.OVERRIDE_POPSCHEDULE_VEHICLE_MODEL, scheduleId, vehicleHash);

        /// <summary>Clears any vehicle model override on a population schedule slot.</summary>
        public static void ClearPopscheduleOverrideVehicleModel(int scheduleId)
            => Function.Call(Hash.CLEAR_POPSCHEDULE_OVERRIDE_VEHICLE_MODEL, scheduleId);
    }
}
