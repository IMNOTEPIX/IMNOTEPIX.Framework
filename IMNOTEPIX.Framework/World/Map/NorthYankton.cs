using GTA.Math;
using IMNOTEPIX.Framework.fCore;
using System;
using Screen = GTA.UI.Screen;
using System.Collections;

namespace IMNOTEPIX.Framework.fWorld.Map
{
    public class NorthYankton : GTA.Script
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

        public NorthYankton()
        {
            AfterAbort.Register("NorthYanktonExitSequence", ExitNorthYanktonSequence);

            this.Aborted += OnAbort;
        }

        private IEnumerator ExitNorthYanktonSequence()
        {
            Screen.FadeOut(500);
            while (GTA.UI.Screen.IsFadingOut)
                yield return fCore.AfterAbort.WaitFor(0);
            fPlayer.Player.PedPos(-1041.75f, -2744.574f, 20.35939f, -30f);
            yield return AfterAbort.WaitFor(750);
            YankTon = true;
            UnloadYankton();
            yield return AfterAbort.WaitFor(750);
            Screen.FadeIn(1500);
            Ped.SetPedUsingActionMode(fPlayer.Player.Character, false, -1, "DEFAULT_ACTION");
            fPlayer.Player.SimulateInputGait(1f, 3000);
        }

        private void OnAbort(object sender, EventArgs e)
        {
            if (YankTon)
                AfterAbort.Request("NorthYanktonExitSequence");
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
                Streaming.RequestIpl(ipl);
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
                Streaming.RemoveIpl(ipl);
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
                Streaming.RequestIpl("prologue06_int");
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
                Streaming.RequestIpl("prologue01");
                Streaming.RequestIpl("prologue02");
                Streaming.RequestIpl("prologue03");
                Streaming.RequestIpl("prologue04");
                Streaming.RequestIpl("prologue05");
                Streaming.RequestIpl("prologue06");
                Streaming.RequestIpl("prologuerd");
                Streaming.RequestIpl("Prologue01c");
                Streaming.RequestIpl("Prologue01d");
                Streaming.RequestIpl("Prologue01e");
                Streaming.RequestIpl("Prologue01f");
                Streaming.RequestIpl("Prologue01g");
                Streaming.RequestIpl("prologue01h");
                Streaming.RequestIpl("prologue01i");
                Streaming.RequestIpl("prologue01j");
                Streaming.RequestIpl("prologue01k");
                Streaming.RequestIpl("prologue01z");
                Streaming.RequestIpl("prologue03b");
                Streaming.RequestIpl("prologue04b");
                Streaming.RequestIpl("prologue05b");
                Streaming.RequestIpl("prologue06b");
                Streaming.RequestIpl("prologuerdb");
                Streaming.RequestIpl("DES_ProTree_start");
                Streaming.RequestIpl("DES_ProTree_start_lod");
                Streaming.RequestIpl("prologue04_cover");
                Streaming.RequestIpl("prologue03_grv_fun");
                Streaming.RequestIpl("prologue03_grv_cov");
                Streaming.RequestIpl("prologue_LODLights");
                Streaming.RequestIpl("prologue_DistantLights");
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
                Streaming.RemoveIpl("prologue06_int");
                Streaming.RemoveIpl("prologue01");
                Streaming.RemoveIpl("prologue02");
                Streaming.RemoveIpl("prologue03");
                Streaming.RemoveIpl("prologue04");
                Streaming.RemoveIpl("prologue05");
                Streaming.RemoveIpl("prologue06");
                Streaming.RemoveIpl("prologuerd");
                Streaming.RemoveIpl("Prologue01c");
                Streaming.RemoveIpl("Prologue01d");
                Streaming.RemoveIpl("Prologue01e");
                Streaming.RemoveIpl("Prologue01f");
                Streaming.RemoveIpl("Prologue01g");
                Streaming.RemoveIpl("prologue01h");
                Streaming.RemoveIpl("prologue01i");
                Streaming.RemoveIpl("prologue01j");
                Streaming.RemoveIpl("prologue01k");
                Streaming.RemoveIpl("prologue01z");
                Streaming.RemoveIpl("prologue03b");
                Streaming.RemoveIpl("prologue04b");
                Streaming.RemoveIpl("prologue05b");
                Streaming.RemoveIpl("prologue06b");
                Streaming.RemoveIpl("prologuerdb");
                Streaming.RemoveIpl("DES_ProTree_start");
                Streaming.RemoveIpl("DES_ProTree_start_lod");
                Streaming.RemoveIpl("prologue04_cover");
                Streaming.RemoveIpl("prologue03_grv_fun");
                Streaming.RemoveIpl("prologue03_grv_cov");
                Streaming.RemoveIpl("prologue_LODLights");
                Streaming.RemoveIpl("prologue_DistantLights");
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
                Streaming.RequestIpl("prologue06_int");
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
                Streaming.RemoveIpl("prologue06_int");
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
                return;

            YankTon = true;

            fCore.Logger.Info("NorthYankton", "Loading North Yankton...");

            LoadDepot();
            foreach (string ipl in NorthYanktonIPLs)
            {
                Streaming.RequestIpl(ipl);
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

            fCore.Logger.Info("NorthYankton", "Successfully loaded North Yankton.");
        }
        public static void UnloadYankton()
        {
            if (YankTon && YankTonRemove)
                return;

            YankTon = false;


            fCore.Logger.Info("NorthYankton", "Unloading North Yankton...");

            UnloadDepot();
            Streaming.RemoveIpl("prologue06_int");
            Streaming.RemoveIpl("prologue01");
            Streaming.RemoveIpl("prologue02");
            Streaming.RemoveIpl("prologue03");
            Streaming.RemoveIpl("prologue04");
            Streaming.RemoveIpl("prologue05");
            Streaming.RemoveIpl("prologue06");
            Streaming.RemoveIpl("prologuerd");
            Streaming.RemoveIpl("Prologue01c");
            Streaming.RemoveIpl("Prologue01d");
            Streaming.RemoveIpl("Prologue01e");
            Streaming.RemoveIpl("Prologue01f");
            Streaming.RemoveIpl("Prologue01g");
            Streaming.RemoveIpl("prologue01h");
            Streaming.RemoveIpl("prologue01i");
            Streaming.RemoveIpl("prologue01j");
            Streaming.RemoveIpl("prologue01k");
            Streaming.RemoveIpl("prologue01z");
            Streaming.RemoveIpl("prologue03b");
            Streaming.RemoveIpl("prologue04b");
            Streaming.RemoveIpl("prologue05b");
            Streaming.RemoveIpl("prologue06b");
            Streaming.RemoveIpl("prologuerdb");
            Streaming.RemoveIpl("DES_ProTree_start");
            Streaming.RemoveIpl("prologue_grv_torch");
            Streaming.RemoveIpl("prologue03_grv_dug");
            Streaming.RemoveIpl("DES_ProTree_start_lod");
            Streaming.RemoveIpl("prologue04_cover");
            Streaming.RemoveIpl("prologue03_grv_fun");
            Streaming.RemoveIpl("prologue03_grv_cov");
            Streaming.RemoveIpl("prologue_LODLights");
            Streaming.RemoveIpl("prologue_DistantLights");
            fWorld.Zone.SetZoneEnabled(fWorld.Zone.GetZoneFromNameID("Prol"), false);
            fCore.Streaming.SetMapDataCullboxEnabled("prologue", false);
            fCore.Streaming.SetMapDataCullboxEnabled("Prologue_Main", false);
            int zone = fWorld.Zone.GetZoneFromNameID("PrLog");
            fWorld.Zone.SetZoneEnabled(zone, false);
            fUI.Hud.ToggleNorthYanktonMap(false);
            fWorld.Pathfind.Nodes.SetAllowStreamPrologueNodes(false);
            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(5526.24f, -5137.23f, 61.78925f), new Vector3(3679.327f, -4973.879f, 125.0828f), 192.0f, false, false, true);
            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3691.211f, -4941.24f, 94.59368f), new Vector3(3511.115f, -4689.191f, 126.7621f), 16.0f, false, false, true);
            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3510.004f, -4865.81f, 94.69557f), new Vector3(3204.424f, -4833.8147f, 126.8152f), 16.0f, false, false, true);
            fWorld.Pathfind.Roads.SetRoadsInAngledArea(new Vector3(3186.534f, -4832.798f, 109.8148f), new Vector3(3204.187f, -4833.993f, 114.815f), 16.0f, false, false, true);

            fCore.Logger.Info("NorthYankton", "Successfully unloaded North Yankton.");
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
