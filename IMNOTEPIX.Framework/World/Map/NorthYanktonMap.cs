using GTA;
using GTA.Math;
using System;
using System.Windows.Forms;

namespace IMNOTEPIX.Framework.fWorld.Map
{
    public class NorthYanktonMap : Script
    {
        public static bool YankTon = false;
        public static bool YankTonRemove = true;
        public static bool Depot = false;
        public static bool DepotRemove = true;
        private static readonly string[] NorthYanktonIPLs = 
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

        public NorthYanktonMap()
        {
            Aborted += PrologueMap_Aborted;
        }

        private void PrologueMap_Aborted(object sender, EventArgs e)
        {
            if (YankTon)
            {
                fPlayer.Player.PedPos(-1038.744f, -2739.543f, 20.16927f, -30f);
                Ped.SetPedUsingActionMode(fPlayer.Player.Character, false, -1, "DEFAULT_ACTION");
            }
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
            if (isNorthYanktonLoaded)
                return;

            // Load all North Yankton IPLs
            foreach (string ipl in NorthYanktonIPLs)
            {
                Interior.RequestIpl(ipl);
            }

            // Configure map settings
            YankTon = true;
            LoadYankton();
            EnableNorthYanktonTrainTracks(true);

            // Enable culling boxes
            fCore.Streaming.SetMapDataCullboxEnabled("prologue", true);
            fCore.Streaming.SetMapDataCullboxEnabled("Prologue_Main", true);

            // Enable zone
            fWorld.Zone.SetZoneEnabled(fWorld.Zone.GetZoneFromNameID("Prol"), true);

            // Enable North Yankton map on HUD
            fUI.Hud.ToggleNorthYanktonMap(true);

            isNorthYanktonLoaded = true;
        }

        /// <summary>
        /// Unload all North Yankton IPLs and restore normal map
        /// Replaces 35+ lines of scattered IPL unloading code
        /// </summary>
        public static void UnloadNorthYanktonMap()
        {
            if (!isNorthYanktonLoaded)
                return;

            // Remove all North Yankton IPLs
            foreach (string ipl in NorthYanktonIPLs)
            {
                Interior.RemoveIpl(ipl);
            }

            // Disable map settings
            YankTon = false;
            EnableNorthYanktonTrainTracks(false);

            // Disable culling boxes
            fCore.Streaming.SetMapDataCullboxEnabled("prologue", false);
            fCore.Streaming.SetMapDataCullboxEnabled("Prologue_Main", false);

            fWorld.Zone.SetZoneEnabled(fWorld.Zone.GetZoneFromNameID("Prol"), false);

            // Disable zone
            int zone = fWorld.Zone.GetZoneFromNameID("PrLog");
            fWorld.Zone.SetZoneEnabled(zone, false);

            // Disable North Yankton map on HUD
            fUI.Hud.ToggleNorthYanktonMap(false);

            isNorthYanktonLoaded = false;
        }

        /// <summary>
        /// Enable roads in North Yankton
        /// </summary>
        public static void EnableNorthYanktonRoads()
        {
            if (areRoadsEnabled)
                return;

            fWorld.Pathfind.Nodes.SetAllowStreamPrologueNodes(true);

            // Enable main road areas
            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, true, true, true);

            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, true, true, true);

            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, true, true, true);

            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, true, true, true);

            areRoadsEnabled = true;
        }

        /// <summary>
        /// Disable roads in North Yankton
        /// </summary>
        public static void DisableNorthYanktonRoads(bool useRoadEnabledCheck)
        {
            if (!areRoadsEnabled && useRoadEnabledCheck)
                return;

            fWorld.Pathfind.Nodes.SetAllowStreamPrologueNodes(false);

            // Disable main road areas
            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, false, true);

            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, false, true);

            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, false, true);

            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, false, true);

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
                DisableNorthYanktonRoads(true);
            }
        }



        public bool IsNorthYanktonLoaded => isNorthYanktonLoaded;
        public bool AreRoadsEnabled => areRoadsEnabled;


        void Yankton()
        {
            if (!YankTon)
            {
                #region EnableYankton + CashDepot
                Interior.RequestIpl("prologue06_int");
                int l_1196 = Interior.GetInteriorAtCoordsWithType(new Vector3(5311.236f, -5212.563f, (85.7187f - 3.2f)), "V_CashDepot");
                Interior.PinInteriorInMemory(l_1196);
                while (true)
                {
                    if (Interior.IsInteriorReady(l_1196))
                    {
                        break;
                    }
                    Wait(0);
                }
                Interior.RequestIpl("prologue01");
                Interior.RequestIpl("prologue02");
                Interior.RequestIpl("prologue03");
                Interior.RequestIpl("prologue04");
                Interior.RequestIpl("prologue05");
                Interior.RequestIpl("prologue06");
                Interior.RequestIpl("prologuerd");
                Interior.RequestIpl("Prologue01c");
                Interior.RequestIpl("Prologue01d");
                Interior.RequestIpl("Prologue01e");
                Interior.RequestIpl("Prologue01f");
                Interior.RequestIpl("Prologue01g");
                Interior.RequestIpl("prologue01h");
                Interior.RequestIpl("prologue01i");
                Interior.RequestIpl("prologue01j");
                Interior.RequestIpl("prologue01k");
                Interior.RequestIpl("prologue01z");
                Interior.RequestIpl("prologue03b");
                Interior.RequestIpl("prologue04b");
                Interior.RequestIpl("prologue05b");
                Interior.RequestIpl("prologue06b");
                Interior.RequestIpl("prologuerdb");
                Interior.RequestIpl("DES_ProTree_start");
                Interior.RequestIpl("DES_ProTree_start_lod");
                Interior.RequestIpl("prologue04_cover");
                Interior.RequestIpl("prologue03_grv_fun");
                Interior.RequestIpl("prologue03_grv_cov");
                Interior.RequestIpl("prologue_LODLights");
                Interior.RequestIpl("prologue_DistantLights");
                int zone = Zone.GetZoneFromNameID("PrLog");
                Zone.SetZoneEnabled(zone, true);
                fUI.Hud.ToggleNorthYanktonMap(true);
                Pathfind.Nodes.SetAllowStreamPrologueNodes(true);
                Pathfind.Roads.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, true, true);
                Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, true, true);
                Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, true, true);
                Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, true, true);
                GTA.Entity e = fPlayer.Player.Character;
                if (Ped.IsPedInAnyVehicle((GTA.Ped)e, false))
                    e = Ped.GetVehiclePedIsUsing((GTA.Ped)e);
                Entity.SetEntityCoordsNoOffset(e, new Vector3(5342.45f, -5189.75f, 82.77f), false, false, true);
                YankTon = true;
                #endregion
            }
            else if (YankTonRemove)
            {
                #region YanktonRemove
                Interior.RemoveIpl("prologue06_int");
                Interior.RemoveIpl("prologue01");
                Interior.RemoveIpl("prologue02");
                Interior.RemoveIpl("prologue03");
                Interior.RemoveIpl("prologue04");
                Interior.RemoveIpl("prologue05");
                Interior.RemoveIpl("prologue06");
                Interior.RemoveIpl("prologuerd");
                Interior.RemoveIpl("Prologue01c");
                Interior.RemoveIpl("Prologue01d");
                Interior.RemoveIpl("Prologue01e");
                Interior.RemoveIpl("Prologue01f");
                Interior.RemoveIpl("Prologue01g");
                Interior.RemoveIpl("prologue01h");
                Interior.RemoveIpl("prologue01i");
                Interior.RemoveIpl("prologue01j");
                Interior.RemoveIpl("prologue01k");
                Interior.RemoveIpl("prologue01z");
                Interior.RemoveIpl("prologue03b");
                Interior.RemoveIpl("prologue04b");
                Interior.RemoveIpl("prologue05b");
                Interior.RemoveIpl("prologue06b");
                Interior.RemoveIpl("prologuerdb");
                Interior.RemoveIpl("DES_ProTree_start");
                Interior.RemoveIpl("DES_ProTree_start_lod");
                Interior.RemoveIpl("prologue04_cover");
                Interior.RemoveIpl("prologue03_grv_fun");
                Interior.RemoveIpl("prologue03_grv_cov");
                Interior.RemoveIpl("prologue_LODLights");
                Interior.RemoveIpl("prologue_DistantLights");
                int zone = fWorld.Zone.GetZoneFromNameID("PrLog");
                fWorld.Zone.SetZoneEnabled(zone, false);
                fUI.Hud.ToggleNorthYanktonMap(false);
                fWorld.Pathfind.Nodes.SetAllowStreamPrologueNodes(false);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, false, true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, false, true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, false, true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, false, true);
                YankTon = false;
                #endregion
            }
        }
        public static void LoadDepot()
        {
            if (!Depot)
            {
                Interior.RequestIpl("prologue06_int");
                int l_1196 = Interior.GetInteriorAtCoordsWithType(new Vector3(5311.236f, -5212.563f, (85.7187f - 3.2f)), "V_CashDepot");
                Interior.PinInteriorInMemory(l_1196);
                while (true)
                {
                    if (Interior.IsInteriorReady(l_1196))
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
                Interior.RemoveIpl("prologue06_int");
                int l_1196 = Interior.GetInteriorAtCoordsWithType(new Vector3(5311.236f, -5212.563f, (85.7187f - 3.2f)), "V_CashDepot");
                Interior.DisableInterior(l_1196);
                while (true)
                {
                    if (!Interior.IsInteriorReady(l_1196))
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
                    Interior.RequestIpl(ipl);
                }
                fCore.Streaming.SetMapDataCullboxEnabled("prologue", true);
                fCore.Streaming.SetMapDataCullboxEnabled("Prologue_Main", true);
                fWorld.Zone.SetZoneEnabled(fWorld.Zone.GetZoneFromNameID("Prol"), true);
                int zone = fWorld.Zone.GetZoneFromNameID("PrLog");
                fWorld.Zone.SetZoneEnabled(zone, true);
                fUI.Hud.ToggleNorthYanktonMap(true);
                fWorld.Pathfind.Nodes.SetAllowStreamPrologueNodes(true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, true, true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, true, true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, true, true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, true, true);
                if (teleportToDepot)
                {
                    GTA.Entity e = fPlayer.Player.Character;
                    if (fWorld.Ped.IsPedInAnyVehicle((GTA.Ped)e, false))
                        e = fWorld.Ped.GetVehiclePedIsUsing((GTA.Ped)e);
                    fWorld.Entity.SetEntityCoordsNoOffset(e, new Vector3(5342.45f, -5189.75f, 82.77f), false, false, true);
                }
                YankTon = true;
            }
        }
        public static void UnloadYankton()
        {
            if (YankTon && YankTonRemove)
            {
                UnloadDepot();
                Interior.RemoveIpl("prologue06_int");
                Interior.RemoveIpl("prologue01");
                Interior.RemoveIpl("prologue02");
                Interior.RemoveIpl("prologue03");
                Interior.RemoveIpl("prologue04");
                Interior.RemoveIpl("prologue05");
                Interior.RemoveIpl("prologue06");
                Interior.RemoveIpl("prologuerd");
                Interior.RemoveIpl("Prologue01c");
                Interior.RemoveIpl("Prologue01d");
                Interior.RemoveIpl("Prologue01e");
                Interior.RemoveIpl("Prologue01f");
                Interior.RemoveIpl("Prologue01g");
                Interior.RemoveIpl("prologue01h");
                Interior.RemoveIpl("prologue01i");
                Interior.RemoveIpl("prologue01j");
                Interior.RemoveIpl("prologue01k");
                Interior.RemoveIpl("prologue01z");
                Interior.RemoveIpl("prologue03b");
                Interior.RemoveIpl("prologue04b");
                Interior.RemoveIpl("prologue05b");
                Interior.RemoveIpl("prologue06b");
                Interior.RemoveIpl("prologuerdb");
                Interior.RemoveIpl("DES_ProTree_start");
                Interior.RemoveIpl("prologue_grv_torch");
                Interior.RemoveIpl("prologue03_grv_dug");
                Interior.RemoveIpl("DES_ProTree_start_lod");
                Interior.RemoveIpl("prologue04_cover");
                Interior.RemoveIpl("prologue03_grv_fun");
                Interior.RemoveIpl("prologue03_grv_cov");
                Interior.RemoveIpl("prologue_LODLights");
                Interior.RemoveIpl("prologue_DistantLights");
                int zone = fWorld.Zone.GetZoneFromNameID("PrLog");
                fWorld.Zone.SetZoneEnabled(zone, false);
                fUI.Hud.ToggleNorthYanktonMap(false);
                fWorld.Pathfind.Nodes.SetAllowStreamPrologueNodes(false);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, false, true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, false, true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, false, true);
                fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, false, true);
                YankTon = false;
            }
        }

        public static void EnableNorthYanktonTrainTracks(bool enabled)
        {
            if (enabled)
            {
                fWorld.Vehicle.SwitchTrainTrack(fWorld.Vehicle.TrainTrack.YanktonPrologueMission, true);
                fWorld.Vehicle.SwitchTrainTrack(fWorld.Vehicle.TrainTrack.YanktonFreight, true);
                return;
            }
            fWorld.Vehicle.SwitchTrainTrack(fWorld.Vehicle.TrainTrack.YanktonPrologueMission, false);
            fWorld.Vehicle.SwitchTrainTrack(fWorld.Vehicle.TrainTrack.YanktonFreight, false);
        }
    }
}
