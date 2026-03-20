namespace IMNOTEPIX.Framework.Funcs.Interior
{
    using GTA;
    using GTA.Math;
    using IMNOTEPIX.Framework.Funcs.Ped;
    using System;

    using Ped = GTA.Ped;
    public class PrologueMap : Script
    {
        public static bool YankTon = false;
        public static bool YankTonRemove = true;
        public static bool Depot = false;
        public static bool DepotRemove = true;
        private static readonly string[] NorthYanktonIPLs = new string[]
        {
    "prologue01",
    "prologue02",
    "prologue03",
    "prologue04",
    "prologue05",
    "prologue06",
    "prologuerd",
    "Prologue01c",
    "Prologue01d",
    "Prologue01e",
    "Prologue01f",
    "Prologue01g",
    "prologue01h",
    "prologue01i",
    "prologue01j",
    "prologue01k",
    "prologue01z",
    "prologue03b",
    "prologue04b",
    "prologue05b",
    "prologue06b",
    "prologuerdb",
    "DES_ProTree_start",
    "DES_ProTree_start_lod",
    "prologue04_cover",
    "prologue03_grv_fun",
    "prologue03_grv_cov",
    "prologue_LODLights",
    "prologue_DistantLights"
        };

        public PrologueMap()
        {
            Aborted += PrologueMap_Aborted;
        }

        private void PrologueMap_Aborted(object sender, EventArgs e)
        {
            UnloadYankton();
        }


        public static bool isNorthYanktonLoaded = false;
        private static bool areRoadsEnabled = false;



        /// <summary>
        /// Load all North Yankton IPLs and configure the map
        /// Replaces 30+ lines of scattered IPL loading code
        /// </summary>
        public static void LoadNorthYanktonMap()
        {
            if (isNorthYanktonLoaded) return;

            // Load all North Yankton IPLs
            foreach (string ipl in NorthYanktonIPLs)
            {
                fInterior.RequestIpl(ipl);
            }

            // Configure map settings
            YankTon = true;
            LoadYankton();
            EnableNorthYanktonTrainTracks(true);

            // Enable culling boxes
            fStreaming.SetMapDataCullboxEnabled("prologue", true);
            fStreaming.SetMapDataCullboxEnabled("Prologue_Main", true);

            // Enable zone
            fZone.SetZoneEnabled(fZone.GetZoneFromNameID("Prol"), true);

            // Enable North Yankton map on HUD
            fHud.ToggleNorthYanktonMap(true);

            isNorthYanktonLoaded = true;
        }

        /// <summary>
        /// Unload all North Yankton IPLs and restore normal map
        /// Replaces 35+ lines of scattered IPL unloading code
        /// </summary>
        public static void UnloadNorthYanktonMap()
        {
            if (!isNorthYanktonLoaded) return;

            // Remove all North Yankton IPLs
            foreach (string ipl in NorthYanktonIPLs)
            {
                fInterior.RemoveIpl(ipl);
            }

            // Disable map settings
            YankTon = false;
            EnableNorthYanktonTrainTracks(false);

            // Disable culling boxes
            fStreaming.SetMapDataCullboxEnabled("prologue", false);
            fStreaming.SetMapDataCullboxEnabled("Prologue_Main", false);

            fZone.SetZoneEnabled(fZone.GetZoneFromNameID("Prol"), false);

            // Disable zone
            int zone = fZone.GetZoneFromNameID("PrLog");
            fZone.SetZoneEnabled(zone, false);

            // Disable North Yankton map on HUD
            fHud.ToggleNorthYanktonMap(false);

            isNorthYanktonLoaded = false;
        }

        /// <summary>
        /// Enable roads in North Yankton
        /// </summary>
        public static void EnableNorthYanktonRoads()
        {
            if (areRoadsEnabled) return;

            fPathfind.SetAllowStreamPrologueNodes(true);

            // Enable main road areas
            fPathfind.SetRoadsInAngledArea(
                new Vector3(5526.24f, -5137.23f, 61.78925f),
                new Vector3(3679.327f, -4973.879f, 125.0828f),
                192.0f, true, true, true);

            fPathfind.SetRoadsInAngledArea(
                new Vector3(3691.211f, -4941.24f, 94.59368f),
                new Vector3(3511.115f, -4689.191f, 126.7621f),
                16.0f, true, true, true);

            fPathfind.SetRoadsInAngledArea(
                new Vector3(3510.004f, -4865.81f, 94.69557f),
                new Vector3(3204.424f, -4833.8147f, 126.8152f),
                16.0f, true, true, true);

            fPathfind.SetRoadsInAngledArea(
                new Vector3(3186.534f, -4832.798f, 109.8148f),
                new Vector3(3204.187f, -4833.993f, 114.815f),
                16.0f, true, true, true);

            areRoadsEnabled = true;
        }

        /// <summary>
        /// Disable roads in North Yankton
        /// </summary>
        public static void DisableNorthYanktonRoads()
        {
            if (!areRoadsEnabled) return;

            fPathfind.SetAllowStreamPrologueNodes(false);

            // Disable main road areas
            fPathfind.SetRoadsInAngledArea(
                new Vector3(5526.24f, -5137.23f, 61.78925f),
                new Vector3(3679.327f, -4973.879f, 125.0828f),
                192.0f, false, false, true);

            fPathfind.SetRoadsInAngledArea(
                new Vector3(3691.211f, -4941.24f, 94.59368f),
                new Vector3(3511.115f, -4689.191f, 126.7621f),
                16.0f, false, false, true);

            fPathfind.SetRoadsInAngledArea(
                new Vector3(3510.004f, -4865.81f, 94.69557f),
                new Vector3(3204.424f, -4833.8147f, 126.8152f),
                16.0f, false, false, true);

            fPathfind.SetRoadsInAngledArea(
                new Vector3(3186.534f, -4832.798f, 109.8148f),
                new Vector3(3204.187f, -4833.993f, 114.815f),
                16.0f, false, false, true);

            areRoadsEnabled = false;
        }

        /// <summary>
        /// Reset IPL manager state (for mission restart)
        /// </summary>
        public static void Reset()
        {
            if (isNorthYanktonLoaded)
            {
                UnloadNorthYanktonMap();
            }

            if (areRoadsEnabled)
            {
                DisableNorthYanktonRoads();
            }
        }



        public bool IsNorthYanktonLoaded => isNorthYanktonLoaded;
        public bool AreRoadsEnabled => areRoadsEnabled;


        void Yankton()
        {
            if (!YankTon)
            {
                #region EnableYankton + CashDepot
                fInterior.RequestIpl("prologue06_int");
                int l_1196 = fInterior.GetInteriorAtCoordsWithType(new Vector3(5311.236f, -5212.563f, (85.7187f - 3.2f)), "V_CashDepot");
                fInterior.PinInteriorInMemory(l_1196);
                while (true)
                {
                    if (fInterior.IsInteriorReady(l_1196))
                    {
                        break;
                    }
                    Wait(0);
                }
                fInterior.RequestIpl("prologue01");
                fInterior.RequestIpl("prologue02");
                fInterior.RequestIpl("prologue03");
                fInterior.RequestIpl("prologue04");
                fInterior.RequestIpl("prologue05");
                fInterior.RequestIpl("prologue06");
                fInterior.RequestIpl("prologuerd");
                fInterior.RequestIpl("Prologue01c");
                fInterior.RequestIpl("Prologue01d");
                fInterior.RequestIpl("Prologue01e");
                fInterior.RequestIpl("Prologue01f");
                fInterior.RequestIpl("Prologue01g");
                fInterior.RequestIpl("prologue01h");
                fInterior.RequestIpl("prologue01i");
                fInterior.RequestIpl("prologue01j");
                fInterior.RequestIpl("prologue01k");
                fInterior.RequestIpl("prologue01z");
                fInterior.RequestIpl("prologue03b");
                fInterior.RequestIpl("prologue04b");
                fInterior.RequestIpl("prologue05b");
                fInterior.RequestIpl("prologue06b");
                fInterior.RequestIpl("prologuerdb");
                fInterior.RequestIpl("DES_ProTree_start");
                fInterior.RequestIpl("DES_ProTree_start_lod");
                fInterior.RequestIpl("prologue04_cover");
                fInterior.RequestIpl("prologue03_grv_fun");
                fInterior.RequestIpl("prologue03_grv_cov");
                fInterior.RequestIpl("prologue_LODLights");
                fInterior.RequestIpl("prologue_DistantLights");
                int zone = fZone.GetZoneFromNameID("PrLog");
                fZone.SetZoneEnabled(zone, true);
                fHud.ToggleNorthYanktonMap(true);
                fPathfind.SetAllowStreamPrologueNodes(true);
                fPathfind.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, true, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, true, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, true, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, true, true);
                Entity e = fPlayer.PlayerPedID();
                if (fPed.IsPedInAnyVehicle((Ped)e, false))
                    e = fPed.GetVehiclePedIsUsing((Ped)e);
                fEntity.SetEntityCoordsNoOffset(e, new Vector3(5342.45f, -5189.75f, 82.77f), false, false, true);
                YankTon = true;
                #endregion
            }
            else if (YankTonRemove)
            {
                #region YanktonRemove
                fInterior.RemoveIpl("prologue06_int");
                fInterior.RemoveIpl("prologue01");
                fInterior.RemoveIpl("prologue02");
                fInterior.RemoveIpl("prologue03");
                fInterior.RemoveIpl("prologue04");
                fInterior.RemoveIpl("prologue05");
                fInterior.RemoveIpl("prologue06");
                fInterior.RemoveIpl("prologuerd");
                fInterior.RemoveIpl("Prologue01c");
                fInterior.RemoveIpl("Prologue01d");
                fInterior.RemoveIpl("Prologue01e");
                fInterior.RemoveIpl("Prologue01f");
                fInterior.RemoveIpl("Prologue01g");
                fInterior.RemoveIpl("prologue01h");
                fInterior.RemoveIpl("prologue01i");
                fInterior.RemoveIpl("prologue01j");
                fInterior.RemoveIpl("prologue01k");
                fInterior.RemoveIpl("prologue01z");
                fInterior.RemoveIpl("prologue03b");
                fInterior.RemoveIpl("prologue04b");
                fInterior.RemoveIpl("prologue05b");
                fInterior.RemoveIpl("prologue06b");
                fInterior.RemoveIpl("prologuerdb");
                fInterior.RemoveIpl("DES_ProTree_start");
                fInterior.RemoveIpl("DES_ProTree_start_lod");
                fInterior.RemoveIpl("prologue04_cover");
                fInterior.RemoveIpl("prologue03_grv_fun");
                fInterior.RemoveIpl("prologue03_grv_cov");
                fInterior.RemoveIpl("prologue_LODLights");
                fInterior.RemoveIpl("prologue_DistantLights");
                int zone = fZone.GetZoneFromNameID("PrLog");
                fZone.SetZoneEnabled(zone, false);
                fHud.ToggleNorthYanktonMap(false);
                fPathfind.SetAllowStreamPrologueNodes(false);
                fPathfind.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, false, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, false, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, false, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, false, true);
                YankTon = false;
                #endregion
            }
        }
        public static void LoadDepot()
        {
            if (!Depot)
            {
                fInterior.RequestIpl("prologue06_int");
                int l_1196 = fInterior.GetInteriorAtCoordsWithType(new Vector3(5311.236f, -5212.563f, (85.7187f - 3.2f)), "V_CashDepot");
                fInterior.PinInteriorInMemory(l_1196);
                while (true)
                {
                    if (fInterior.IsInteriorReady(l_1196))
                        break;
                    Wait(0);
                }
                Depot = true;
            }
        }
        public static void UnloadDepot()
        {
            if (Depot && DepotRemove)
            {
                fInterior.RemoveIpl("prologue06_int");
                int l_1196 = fInterior.GetInteriorAtCoordsWithType(new Vector3(5311.236f, -5212.563f, (85.7187f - 3.2f)), "V_CashDepot");
                fInterior.DisableInterior(l_1196);
                while (true)
                {
                    if (!fInterior.IsInteriorReady(l_1196))
                        break;
                    Wait(0);
                }
                Depot = false;
            }
        }
        public static void LoadYankton(bool teleportToDepot = false)
        {
            if (!YankTon)
            {
                LoadDepot();
                foreach (string ipl in NorthYanktonIPLs)
                {
                    fInterior.RequestIpl(ipl);
                }
                fStreaming.SetMapDataCullboxEnabled("prologue", true);
                fStreaming.SetMapDataCullboxEnabled("Prologue_Main", true);
                fZone.SetZoneEnabled(fZone.GetZoneFromNameID("Prol"), true);
                int zone = fZone.GetZoneFromNameID("PrLog");
                fZone.SetZoneEnabled(zone, true);
                fHud.ToggleNorthYanktonMap(true);
                fPathfind.SetAllowStreamPrologueNodes(true);
                fPathfind.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, true, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, true, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, true, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, true, true);
                if (teleportToDepot)
                {
                    Entity e = fPlayer.PlayerPedID();
                    if (fPed.IsPedInAnyVehicle((Ped)e, false))
                        e = fPed.GetVehiclePedIsUsing((Ped)e);
                    fEntity.SetEntityCoordsNoOffset(e, new Vector3(5342.45f, -5189.75f, 82.77f), false, false, true);
                }
                YankTon = true;
            }
        }
        public static void UnloadYankton()
        {
            if (YankTon && YankTonRemove)
            {
                UnloadDepot();
                fInterior.RemoveIpl("prologue06_int");
                fInterior.RemoveIpl("prologue01");
                fInterior.RemoveIpl("prologue02");
                fInterior.RemoveIpl("prologue03");
                fInterior.RemoveIpl("prologue04");
                fInterior.RemoveIpl("prologue05");
                fInterior.RemoveIpl("prologue06");
                fInterior.RemoveIpl("prologuerd");
                fInterior.RemoveIpl("Prologue01c");
                fInterior.RemoveIpl("Prologue01d");
                fInterior.RemoveIpl("Prologue01e");
                fInterior.RemoveIpl("Prologue01f");
                fInterior.RemoveIpl("Prologue01g");
                fInterior.RemoveIpl("prologue01h");
                fInterior.RemoveIpl("prologue01i");
                fInterior.RemoveIpl("prologue01j");
                fInterior.RemoveIpl("prologue01k");
                fInterior.RemoveIpl("prologue01z");
                fInterior.RemoveIpl("prologue03b");
                fInterior.RemoveIpl("prologue04b");
                fInterior.RemoveIpl("prologue05b");
                fInterior.RemoveIpl("prologue06b");
                fInterior.RemoveIpl("prologuerdb");
                fInterior.RemoveIpl("DES_ProTree_start");
                fInterior.RemoveIpl("prologue_grv_torch");
                fInterior.RemoveIpl("prologue03_grv_dug");
                fInterior.RemoveIpl("DES_ProTree_start_lod");
                fInterior.RemoveIpl("prologue04_cover");
                fInterior.RemoveIpl("prologue03_grv_fun");
                fInterior.RemoveIpl("prologue03_grv_cov");
                fInterior.RemoveIpl("prologue_LODLights");
                fInterior.RemoveIpl("prologue_DistantLights");
                int zone = fZone.GetZoneFromNameID("PrLog");
                fZone.SetZoneEnabled(zone, false);
                fHud.ToggleNorthYanktonMap(false);
                fPathfind.SetAllowStreamPrologueNodes(false);
                fPathfind.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, false, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, false, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, false, true);
                fPathfind.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, false, true);
                YankTon = false;
            }
        }

        public static void EnableNorthYanktonTrainTracks(bool enabled)
        {
            if (enabled)
            {
                fVehicle.SwitchTrainTrack(fVehicle.TrainTrack.YanktonPrologueMission, true);
                fVehicle.SwitchTrainTrack(fVehicle.TrainTrack.YanktonFreight, true);
                return;
            }
            fVehicle.SwitchTrainTrack(fVehicle.TrainTrack.YanktonPrologueMission, false);
            fVehicle.SwitchTrainTrack(fVehicle.TrainTrack.YanktonFreight, false);
        }
    }
}
