using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace IMNOTEPIX.Framework.fWorld.Map
{
    /// <summary>
    /// Handles streaming/unstreaming of the Cayo Perico ("HeistIsland") map based on player
    /// proximity, plus manual force-load / force-unload helpers.
    /// Ported and cleaned up from decompiled reference scripts (ManageCayoPerico / islandhopper)
    /// and cross-checked against Vincentsgm's open-source CayoPericoRPH (MIT licensed),
    /// rewritten for IMNOTEPIX.Framework / ScriptHookVDotNet.
    /// </summary>
    public class CayoPerico : GTA.Script
    {
        #region Config

        /// <summary>If true, the island streams in/out automatically based on distance to IslandCenter.</summary>
        public static bool AutoLoad = false;

        /// <summary>Distance (in game units) at which the island loads/unloads.</summary>
        public static float LoadRadius = 2500f;

        /// <summary>Also stream in the Heist Island Aircraft Carrier IPLs.</summary>
        public static bool EnableAircraftCarrier = false;

        /// <summary>Keep the mansion's main gate closed when the island loads.</summary>
        public static bool KeepMansionGateClosed = false;

        /// <summary>Keep the underwater tunnel grill closed when the island loads.</summary>
        public static bool KeepUnderwaterGrillClosed = false;

        /// <summary>Keep the airstrip hangar doors closed when the island loads.</summary>
        public static bool KeepHangarDoorsClosed = true;

        /// <summary>Spawn the beached humpback whale prop near the airstrip.</summary>
        public static bool SpawnBeachedWhale = false;

        /// <summary>Enable the beach party ambient music emitters while the island is loaded.</summary>
        public static bool EnableBeachPartyMusic = false;

        /// <summary>
        /// Also flip the mansion's main-gate door lock state via the door system (in addition to
        /// skipping the closed-gate IPL). Belt-and-suspenders: some players report the gate can
        /// still act "locked" for collision purposes even once its closed-prop IPL is skipped.
        /// Only applies to the mansion gate - we don't have verified door hashes for the hangar
        /// or underwater grill, so those rely on the IPL swap alone.
        /// </summary>
        public static bool AlsoUnlockMainGateDoors = true;

        /// <summary>
        /// How often (ms) the auto-load proximity check runs. Note this can no longer use
        /// Script.Interval (see constructor comment) - it's enforced manually via game time.
        /// </summary>
        public static int ProximityCheckIntervalMs = 2000;

        /// <summary>
        /// Fakes a correct minimap while standing on the island. Cayo Perico's real-world
        /// coordinates alias over an ocean tile on the mainland minimap, so without this the
        /// radar shows empty water instead of the island. Runs every frame while active - see
        /// UpdateMinimap().
        /// </summary>
        public static bool EnableMinimapOverride = true;

        /// <summary>
        /// Registers the island's map zone so the on-screen area name reads "Cayo Perico"
        /// instead of whatever mainland zone the coordinates would otherwise resolve to.
        /// Looked up dynamically via GET_ZONE_FROM_NAME_ID rather than a hardcoded zone index,
        /// so it survives game updates that shuffle zone indices.
        /// </summary>
        public static bool EnableZoneRegistration = true;

        /// <summary>
        /// Turns on the island's ambient soundscape (jungle/ocean/wildlife) via its ambient
        /// zone list. Separate from EnableBeachPartyMusic, which only covers the beach party
        /// emitters specifically.
        /// </summary>
        public static bool EnableAmbientIslandSounds = true;

        /// <summary>
        /// Enables the scenario groups that let ambient peds (workers, guards, tourists) spawn
        /// on the island. Without this the island is otherwise empty of ambient NPCs.
        /// </summary>
        public static bool EnableAmbientPeds = true;

        /// <summary>
        /// Works around a known rendering bug where ocean waves can clip through the island's
        /// terrain near the coastline, by forcing the weather to Extra Sunny (which minimizes
        /// wave height) while the island is loaded, and restoring whatever weather was active
        /// beforehand on unload. This is the documented community workaround for the issue, not
        /// a proper native-level fix - if you have a cleaner fix, this flag makes it easy to
        /// swap out.
        /// </summary>
        public static bool FixWaterClipping = true;

        /// <summary>
        /// Disables mainland "prologue" traffic-path node streaming while the island is loaded
        /// (and re-enables it on unload), to reduce conflicts between the island's own traffic
        /// paths and the mainland's. Ported from the same lever used in the original decompiled
        /// ManageCayoPerico script for a similar area-transition case - not guaranteed to be
        /// identical to what CayoPericoRPH's own TrafficPaths.cs does internally, since I
        /// couldn't verify that file's exact native calls.
        /// </summary>
        public static bool ToggleTrafficPathStreaming = true;

        #endregion

        #region State

        /// <summary>True while the Cayo Perico island is currently streamed in.</summary>
        public static bool IsActive { get; private set; }

        private static readonly Vector3 IslandCenter = new Vector3(5103.374f, -5172.456f, 18f);

        private const string HangarDoorsClosedIpl = "h4_islandairstrip_doorsclosed";
        private const string MansionGateClosedIpl = "h4_mansion_gate_closed";
        private const string UnderwaterGateClosedIpl = "h4_Underwater_Gate_Closed";
        private const string BeachedWhaleIpl = "h4_mph4_islp_stranded_humpwhale";

        private const string BeachMusicEmitterLeft = "se_dlc_hei4_island_beach_party_music_new_01_left";
        private const string BeachMusicEmitterRight = "se_dlc_hei4_island_beach_party_music_new_02_right";

        // Heist-island map zone, looked up dynamically rather than hardcoded.
        private const string HeistZoneNameId = "IsHeist";

        // The real-world mainland zone underlying the island's coordinates, which needs to be
        // re-enabled on unload. Kept as a hardcoded index (unlike the heist zone above) because
        // it's ported directly from the original decompiled islandhopper script, which we know
        // works, and GET_ZONE_FROM_NAME_ID needs a name string we don't have for this one.
        private const int MainlandFallbackZoneIndex = 1294;

        private const string AmbientIslandZoneList = "AZL_DLC_Hei4_Island_Zones";

        private static readonly string[] AmbientPedScenarioGroups = { "Heist_Island_Peds", "Heist_Island_Peds_2" };

        // Weather forced while FixWaterClipping is on, and the weather to restore on unload.
        private const GTA.Weather WaveFixWeather = GTA.Weather.ExtraSunny;
        private static GTA.Weather? _weatherBeforeLoad;

        // Native 0xDD3D5F9CA0C715D0 = NETWORK_DISABLE_LEAVE_REMOTE_PED_BEHIND(BOOL toggle).
        // Identified via a community report (CayoPericoRPH author, citing citizenfx/fivem's
        // CrossMapping_Universal.h) and cross-checked independently against docs.fivem.net,
        // which lists the same native under its canonical hash 0x977DB4641F6FC3DB with a
        // matching single-bool signature - the two hashes differing is expected, since R*
        // rescrambles native hashes per game build and the crossmap table is exactly what
        // translates between them. Its purpose in a networking context: prevents remotely-
        // owned peds from being "left behind"/migrated away when the world switches to MP-only
        // map content (which is what streaming in the heist island via SHV actually is under
        // the hood). That explanation is our inference from the native's name and the context
        // it's called in, not something Rockstar documents - but the identification itself is
        // solid.
        private const Hash NetworkDisableLeaveRemotePedBehind = Hash.NETWORK_DISABLE_LEAVE_REMOTE_PED_BEHIND;

        /// <summary>
        /// The full set of Cayo Perico interior/prop IPLs, sourced from the CayoPericoIPLs.txt
        /// data file (as bundled with Vincentsgm's CayoPericoRPH release). The three "*_closed"
        /// entries are the closed-gate/door placeholder props and are handled separately via the
        /// KeepXClosed flags rather than being unconditionally loaded.
        /// </summary>
        private static readonly string[] IslandIpls =
        {
            "h4_islandairstrip",
            "h4_islandairstrip_props",
            "h4_islandx_mansion",
            "h4_islandx_mansion_props",
            "h4_islandx_props",
            "h4_islandxdock",
            "h4_islandxdock_props",
            "h4_islandxdock_props_2",
            "h4_islandxtower",
            "h4_islandx_maindock",
            "h4_islandx_maindock_props",
            "h4_islandx_maindock_props_2",
            "h4_IslandX_Mansion_Vault",
            "h4_islandairstrip_propsb",
            "h4_beach",
            "h4_beach_props",
            "h4_beach_bar_props",
            "h4_islandx_barrack_props",
            "h4_islandx_checkpoint",
            "h4_islandx_checkpoint_props",
            "h4_islandx_Mansion_Office",
            "h4_islandx_Mansion_LockUp_01",
            "h4_islandx_Mansion_LockUp_02",
            "h4_islandx_Mansion_LockUp_03",
            "h4_islandairstrip_hangar_props",
            "h4_IslandX_Mansion_B",
            HangarDoorsClosedIpl,
            UnderwaterGateClosedIpl,
            MansionGateClosedIpl,
            "h4_aa_guns",
            "h4_IslandX_Mansion_GuardFence",
            "h4_IslandX_Mansion_Entrance_Fence",
            "h4_IslandX_Mansion_B_Side_Fence",
            "h4_IslandX_Mansion_Lights",
            "h4_islandxcanal_props",
            "h4_beach_props_party",
            "h4_islandX_Terrain_props_06_a",
            "h4_islandX_Terrain_props_06_b",
            "h4_islandX_Terrain_props_06_c",
            "h4_islandX_Terrain_props_05_a",
            "h4_islandX_Terrain_props_05_b",
            "h4_islandX_Terrain_props_05_c",
            "h4_islandX_Terrain_props_05_d",
            "h4_islandX_Terrain_props_05_e",
            "h4_islandX_Terrain_props_05_f",
            "H4_islandx_terrain_01",
            "H4_islandx_terrain_02",
            "H4_islandx_terrain_03",
            "H4_islandx_terrain_04",
            "H4_islandx_terrain_05",
            "H4_islandx_terrain_06",
            "h4_ne_ipl_00", "h4_ne_ipl_01", "h4_ne_ipl_02", "h4_ne_ipl_03", "h4_ne_ipl_04",
            "h4_ne_ipl_05", "h4_ne_ipl_06", "h4_ne_ipl_07", "h4_ne_ipl_08", "h4_ne_ipl_09",
            "h4_nw_ipl_00", "h4_nw_ipl_01", "h4_nw_ipl_02", "h4_nw_ipl_03", "h4_nw_ipl_04",
            "h4_nw_ipl_05", "h4_nw_ipl_06", "h4_nw_ipl_07", "h4_nw_ipl_08", "h4_nw_ipl_09",
            "h4_se_ipl_00", "h4_se_ipl_01", "h4_se_ipl_02", "h4_se_ipl_03", "h4_se_ipl_04",
            "h4_se_ipl_05", "h4_se_ipl_06", "h4_se_ipl_07", "h4_se_ipl_08", "h4_se_ipl_09",
            "h4_sw_ipl_00", "h4_sw_ipl_01", "h4_sw_ipl_02", "h4_sw_ipl_03", "h4_sw_ipl_04",
            "h4_sw_ipl_05", "h4_sw_ipl_06", "h4_sw_ipl_07", "h4_sw_ipl_08", "h4_sw_ipl_09",
            "h4_islandxtower_veg",
            "h4_islandx_sea_mines",
            "h4_islandx",
            "h4_islandx_barrack_hatch",
            "h4_islandxdock_water_hatch",
            "h4_beach_party",
            "h4_islandx"
        };

        // Fake radar "interior" used to force the correct minimap tile while on the island.
        // Name and coordinates confirmed against CayoPericoRPH's Minimap.cs.
        private const string FakeIslandRadarName = "h4_fake_islandx";
        private const float FakeIslandRadarX = 4700f;
        private const float FakeIslandRadarY = -5145f;

        // Mansion main-gate doors, hash + position pairs (from CayoPericoRPH's Settings.cs).
        // Two hashes shared across four leaves (north/south each have a matching pair).
        private static readonly (uint Hash, Vector3 Position)[] MansionGateDoors =
        {
            (0xa22c5a6a, new Vector3(4981.0122f, -5712.747f, 20.7810f)), // North-Left
            (0x4872b7e6, new Vector3(4984.1388f, -5709.248f, 20.7810f)), // North-Right
            (0xa22c5a6a, new Vector3(4990.6812f, -5715.106f, 20.7810f)), // South-Left
            (0x4872b7e6, new Vector3(4987.5874f, -5718.634f, 20.7810f)), // South-Right
        };

        private static List<Prop> HangerFixObj = new List<Prop>();

        private static readonly Vector3 rot20 = new Vector3(10f, 0f, 20f);

        private static readonly Vector3[] HangerFixCoords =
        {
            (new Vector3(4443.07f, -4479.457f, 4.681903f)),
            (new Vector3(4438.089f, -4481.269f, 4.681903f)),
            (new Vector3(4449.187f, -4477.23f, 4.681903f)),
            (new Vector3(4454.287f, -4475.746f, 4.681903f)),
            (new Vector3(4449.307f, -4477.559f, 4.681903f)),
            (new Vector3(4449.293f, -4477.521f, 4.681903f)),
            (new Vector3(4443.213f, -4479.734f, 4.681903f)),
            (new Vector3(4438.233f, -4481.547f, 4.681903f)),
            /*(new Vector3(), rot20),
            (new Vector3(), rot20),
            (new Vector3(), rot20),
            (new Vector3(), rot20),*/
        };

        private bool _wasNearby;
        private int _nextProximityCheckGameTime;

        public CayoPerico()
        {
            fCore.AfterAbort.Register("CayoPericoAfterAbort", CayoPericoAfterAbort);

            Tick += OnTick;
            Aborted += OnAbort;
        }

        private void OnAbort(object sender, EventArgs e)
        {
            if (IsActive)
                fCore.AfterAbort.Request("CayoPericoAfterAbort");
            HangerFixDelete();
        }

        private void OnTick(object sender, EventArgs e)
        {
            UpdateMinimap();

            if (!AutoLoad)
                return;

            if (Game.GameTime < _nextProximityCheckGameTime)
                return;

            _nextProximityCheckGameTime = Game.GameTime + ProximityCheckIntervalMs;

            bool playerNearby = IsWithinDistance(Game.Player.Character.Position, IslandCenter, LoadRadius);

            // Edge-triggered: only act on state transitions, not every poll.
            if (playerNearby == _wasNearby)
                return;

            _wasNearby = playerNearby;

            if (playerNearby && !IsActive)
                LoadIsland();
            else if (!playerNearby && IsActive)
                UnloadIsland();
        }

        /// <summary>
        /// Forces the minimap to render the island's radar tile ("h4_fake_islandx") instead of
        /// whatever the player's real GTA-map coordinates would normally show, since the
        /// island's world-space coordinates sit on top of ocean on the mainland map. Only
        /// applies while the island is loaded and the player isn't already inside a real
        /// interior (which has its own radar override and would conflict with this one).
        /// Must run every Tick - see constructor comment.
        /// </summary>
        private void UpdateMinimap()
        {
            if (!EnableMinimapOverride || !IsActive)
                return;

            bool inRealInterior = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character.Handle) != 0;

            if (inRealInterior)
            {
                // Let the game's normal interior radar handling take over.
                Function.Call(Hash.SET_RADAR_AS_EXTERIOR_THIS_FRAME);
                return;
            }

            Function.Call(Hash.SET_RADAR_AS_INTERIOR_THIS_FRAME, FakeIslandRadarName, FakeIslandRadarX, FakeIslandRadarY, 0, 0);
        }

        #endregion

        #region Public API

        /// <summary>Streams in the Cayo Perico island and applies the configured options.</summary>
        public static void LoadIsland()
        {
            if (IsActive)
                return;

            IsActive = true;

            fCore.Logger.Info("CayoPerico", "Loading Cayo Perico Island...");

            UnloadYankton();

            Function.Call(Hash.SET_ISLAND_ENABLED, "HeistIsland", true);
            Function.Call(Hash.SET_ALLOW_STREAM_HEIST_ISLAND_NODES, true);
            Function.Call(NetworkDisableLeaveRemotePedBehind, true);
            Function.Call(Hash.SET_MAPDATACULLBOX_ENABLED, "HeistIsland", 0);
            Function.Call(Hash.SET_USE_ISLAND_MAP, true);
            Function.Call(Hash.LOAD_GLOBAL_WATER_FILE, 1);
            Function.Call(Hash.SET_AUDIO_FLAG, "PlayerOnDLCHeist4Island", true);
            HangerFix();
            int CayoAirstripHanger = 0;
            if (!Interior.IsValidInterior(CayoAirstripHanger))
            {
                CayoAirstripHanger = Interior.GetInteriorAtCoordsWithType(4439.823f, -4461.717f, 4.7f, "h4_airstrip_hanger");
                Script.Wait(500);
            }
            else
            {
                Interior.PinInteriorInMemory(CayoAirstripHanger);
                fCore.Streaming.SetInteriorActive(CayoAirstripHanger, true);
                Interior.DisableInterior(CayoAirstripHanger, false);

                if (Interior.IsInteriorCapped(CayoAirstripHanger))
                    Interior.CapInterior(CayoAirstripHanger, false);

                Interior.RefreshInterior(CayoAirstripHanger);
            }


            foreach (string ipl in IslandIpls)
            {
                if (ShouldSkipClosedGateIpl(ipl))
                    continue;

                if (ipl.Equals(HangarDoorsClosedIpl, StringComparison.OrdinalIgnoreCase) && KeepHangarDoorsClosed)
                {
                    Function.Call(Hash.REMOVE_IPL, "h4_islandairstrip_doorsopen");
                    Function.Call(Hash.REQUEST_IPL, HangarDoorsClosedIpl);
                    continue;
                }
                else if (!KeepHangarDoorsClosed)
                {
                    Function.Call(Hash.REMOVE_IPL, HangarDoorsClosedIpl);
                    Function.Call(Hash.REQUEST_IPL, "h4_islandairstrip_doorsopen");
                    continue;
                }

                Function.Call(Hash.REQUEST_IPL, ipl);
            }

            if (EnableAircraftCarrier)
            {
                Function.Call(Hash.REQUEST_IPL, "hei_carrier");
                Function.Call(Hash.REQUEST_IPL, "hei_carrier_DistantLights");
                Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int1");
                Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int2");
                Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int3");
                Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int4");
                Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int5");
                Function.Call(Hash.REQUEST_IPL, "hei_Carrier_int6");
                Function.Call(Hash.REQUEST_IPL, "hei_carrier_LODLights");
                Function.Call(Hash.SET_AMBIENT_ZONE_LIST_STATE_PERSISTENT, "AZL_HEI_AIRCRAFT_CARRIER", true, true);
            }

            if (!KeepMansionGateClosed && AlsoUnlockMainGateDoors)
                SetMansionGateLocked(false);

            if (SpawnBeachedWhale)
                Function.Call(Hash.REQUEST_IPL, BeachedWhaleIpl);
            else
                Function.Call(Hash.REMOVE_IPL, BeachedWhaleIpl);

            if (EnableBeachPartyMusic)
            {
                Function.Call(Hash.SET_STATIC_EMITTER_ENABLED, BeachMusicEmitterLeft, true);
                Function.Call(Hash.SET_STATIC_EMITTER_ENABLED, BeachMusicEmitterRight, true);
            }

            if (EnableZoneRegistration)
            {
                int zone = Function.Call<int>(Hash.GET_ZONE_FROM_NAME_ID, HeistZoneNameId);
                if (zone != 0)
                    Function.Call(Hash.SET_ZONE_ENABLED, zone, true);
            }

            if (EnableAmbientIslandSounds)
                Function.Call(Hash.SET_AMBIENT_ZONE_LIST_STATE, AmbientIslandZoneList, true, true);

            if (EnableAmbientPeds)
            {
                foreach (string group in AmbientPedScenarioGroups)
                    Function.Call(Hash.SET_SCENARIO_GROUP_ENABLED, group, true);
            }

            if (FixWaterClipping)
            {
                _weatherBeforeLoad = GTA.World.Weather;
                GTA.World.Weather = WaveFixWeather;
            }

            if (ToggleTrafficPathStreaming)
                Function.Call(Hash.SET_ALLOW_STREAM_PROLOGUE_NODES, false);

            fCore.Logger.Info("CayoPerico", "Successfully loaded Cayo Perico Island.");
        }

        /// <summary>Streams out the Cayo Perico island and reverts all associated state.</summary>
        public static void UnloadIsland()
        {
            if (!IsActive)
                return;

            IsActive = false;

            fCore.Logger.Info("CayoPerico", "Unloading Cayo Perico Island...");

            Function.Call(Hash.SET_STATIC_EMITTER_ENABLED, BeachMusicEmitterLeft, false);
            Function.Call(Hash.SET_STATIC_EMITTER_ENABLED, BeachMusicEmitterRight, false);

            if (AlsoUnlockMainGateDoors)
                SetMansionGateLocked(true);

            foreach (string ipl in IslandIpls.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                Function.Call(Hash.REMOVE_IPL, ipl);
            }

            Function.Call(Hash.REMOVE_IPL, BeachedWhaleIpl);

            if (EnableZoneRegistration)
            {
                int zone = Function.Call<int>(Hash.GET_ZONE_FROM_NAME_ID, HeistZoneNameId);
                if (zone != 0)
                    Function.Call(Hash.SET_ZONE_ENABLED, zone, false);

                Function.Call(Hash.SET_ZONE_ENABLED, MainlandFallbackZoneIndex, true);
            }

            if (EnableAmbientIslandSounds)
                Function.Call(Hash.SET_AMBIENT_ZONE_LIST_STATE, AmbientIslandZoneList, false, false);

            if (EnableAmbientPeds)
            {
                foreach (string group in AmbientPedScenarioGroups)
                    Function.Call(Hash.SET_SCENARIO_GROUP_ENABLED, group, false);
            }

            if (FixWaterClipping && _weatherBeforeLoad.HasValue)
            {
                GTA.World.Weather = _weatherBeforeLoad.Value;
                _weatherBeforeLoad = null;
            }

            if (ToggleTrafficPathStreaming)
                Function.Call(Hash.SET_ALLOW_STREAM_PROLOGUE_NODES, true);

            int CayoAirstripHanger = 0;
            if (!Interior.IsValidInterior(CayoAirstripHanger))
            {
                CayoAirstripHanger = Interior.GetInteriorAtCoordsWithType(4439.823f, -4461.717f, 4.7f, "h4_airstrip_hanger");
                Script.Wait(500);
            }
            else
            {
                Interior.UnpinInterior(CayoAirstripHanger);
                fCore.Streaming.SetInteriorActive(CayoAirstripHanger, false);
                Interior.DisableInterior(CayoAirstripHanger, true);

                if (!Interior.IsInteriorCapped(CayoAirstripHanger))
                    Interior.CapInterior(CayoAirstripHanger, true);

                Interior.RefreshInterior(CayoAirstripHanger);
            }

            Function.Call(Hash.SET_ISLAND_ENABLED, "HeistIsland", false);
            Function.Call(Hash.SET_ALLOW_STREAM_HEIST_ISLAND_NODES, false);
            Function.Call(NetworkDisableLeaveRemotePedBehind, false);
            Function.Call(Hash.SET_USE_ISLAND_MAP, false);
            Function.Call(Hash.LOAD_GLOBAL_WATER_FILE, 0);
            Function.Call(Hash.SET_AUDIO_FLAG, "PlayerOnDLCHeist4Island", false);

            HangerFixDelete();

            fCore.Logger.Info("CayoPerico", "Successfully unloaded Cayo Perico Island.");
        }

        /// <summary>Force-loads the island and teleports the player to the standard arrival point.</summary>
        public static void LoadIslandAndTeleport(Vector3? arrivalPoint = null)
        {
            LoadIsland();
            Game.Player.Character.Position = arrivalPoint ?? new Vector3(4461.3154f, -4468.136f, 4.245f);
        }

        /// <summary>Force-unloads the island and teleports the player back to the mainland.</summary>
        public static void UnloadIslandAndTeleport(Vector3? returnPoint = null)
        {
            UnloadIsland();
            Game.Player.Character.Position = returnPoint ?? new Vector3(-1042.299f, -2746.373f, 21.3594f);
        }

        private static IEnumerator CayoPericoAfterAbort()
        {
            GTA.UI.Screen.FadeOut(500);
            while (GTA.UI.Screen.IsFadingOut)
                yield return fCore.AfterAbort.WaitFor(0);
            fPlayer.Player.PedPos(-1041.75f, -2744.574f, 20.35939f, -30f);
            yield return fCore.AfterAbort.WaitFor(750);
            IsActive = true;
            UnloadIsland();
            yield return fCore.AfterAbort.WaitFor(750);
            GTA.UI.Screen.FadeIn(1500);
            Ped.SetPedUsingActionMode(fPlayer.Player.Character, false, -1, "DEFAULT_ACTION");
            fPlayer.Player.SimulateInputGait(1f, 3000);
        }

        #endregion

        #region Helpers

        private static void UnloadYankton()
        {
            fCore.Streaming.RemoveIpl("prologue06_int");
            int l_1196 = Interior.GetInteriorAtCoordsWithType(new Vector3(5311.236f, -5212.563f, (85.7187f - 3.2f)), "V_CashDepot");
            Interior.DisableInterior(l_1196);
            while (true)
            {
                if (!Interior.IsInteriorReady(l_1196))
                    break;
                Wait(0);
            }
            fCore.Streaming.RemoveIpl("prologue06_int");
            fCore.Streaming.RemoveIpl("prologue01");
            fCore.Streaming.RemoveIpl("prologue02");
            fCore.Streaming.RemoveIpl("prologue03");
            fCore.Streaming.RemoveIpl("prologue04");
            fCore.Streaming.RemoveIpl("prologue05");
            fCore.Streaming.RemoveIpl("prologue06");
            fCore.Streaming.RemoveIpl("prologuerd");
            fCore.Streaming.RemoveIpl("Prologue01c");
            fCore.Streaming.RemoveIpl("Prologue01d");
            fCore.Streaming.RemoveIpl("Prologue01e");
            fCore.Streaming.RemoveIpl("Prologue01f");
            fCore.Streaming.RemoveIpl("Prologue01g");
            fCore.Streaming.RemoveIpl("prologue01h");
            fCore.Streaming.RemoveIpl("prologue01i");
            fCore.Streaming.RemoveIpl("prologue01j");
            fCore.Streaming.RemoveIpl("prologue01k");
            fCore.Streaming.RemoveIpl("prologue01z");
            fCore.Streaming.RemoveIpl("prologue03b");
            fCore.Streaming.RemoveIpl("prologue04b");
            fCore.Streaming.RemoveIpl("prologue05b");
            fCore.Streaming.RemoveIpl("prologue06b");
            fCore.Streaming.RemoveIpl("prologuerdb");
            fCore.Streaming.RemoveIpl("DES_ProTree_start");
            fCore.Streaming.RemoveIpl("prologue_grv_torch");
            fCore.Streaming.RemoveIpl("prologue03_grv_dug");
            fCore.Streaming.RemoveIpl("DES_ProTree_start_lod");
            fCore.Streaming.RemoveIpl("prologue04_cover");
            fCore.Streaming.RemoveIpl("prologue03_grv_fun");
            fCore.Streaming.RemoveIpl("prologue03_grv_cov");
            fCore.Streaming.RemoveIpl("prologue_LODLights");
            fCore.Streaming.RemoveIpl("prologue_DistantLights");
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
        }

        private static bool ShouldSkipClosedGateIpl(string ipl)
        {
            if (ipl.Equals(UnderwaterGateClosedIpl, StringComparison.OrdinalIgnoreCase))
                return !KeepUnderwaterGrillClosed;

            if (ipl.Equals(MansionGateClosedIpl, StringComparison.OrdinalIgnoreCase))
                return !KeepMansionGateClosed;

            return false;
        }

        /// <summary>
        /// Flips the mansion main-gate doors' lock state via the door system, on top of the
        /// closed-gate IPL swap. Uses SET_STATE_OF_CLOSEST_DOOR_OF_TYPE, which should be present
        /// in GTA.Native.Hash under recent ScriptHookVDotNet versions; if your version doesn't
        /// have it, call the raw native hash instead (check alloc8or's nativedb for the current
        /// value) or set AlsoUnlockMainGateDoors = false to skip this step entirely.
        /// </summary>
        private static void SetMansionGateLocked(bool locked)
        {
            foreach (var door in MansionGateDoors)
            {
                Function.Call(Hash.SET_STATE_OF_CLOSEST_DOOR_OF_TYPE,
                    door.Hash, door.Position.X, door.Position.Y, door.Position.Z,
                    locked, 0f, false);
            }
        }

        private static bool IsWithinDistance(Vector3 a, Vector3 b, float radius)
        {
            // VDIST2 returns squared distance - avoids a sqrt for a simple radius check.
            float distSquared = Function.Call<float>(Hash.VDIST2, a.X, a.Y, a.Z, b.X, b.Y, b.Z);
            return distSquared < radius * radius;
        }

        private static bool FixModelLoad()
            => new Model(1693207013).Request(900);

        private static void HangerFix()
        {
            if (FixModelLoad())
            {
                if (HangerFixObj.Count != HangerFixCoords.Length)
                {
                    foreach (var xyz in HangerFixCoords)
                    {
                        Object.CreateObjectNoOffsetForList(HangerFixObj, 1693207013, xyz, rot20, false);
                    }
                }
            }
            if (HangerFixObj.Count == HangerFixCoords.Length)
            {
                foreach (var obj in HangerFixObj)
                {
                    obj.IsPositionFrozen = true;
                    obj.DontLoadCollision = true;
                    obj.CanBeClimbed = false;
                    obj.CanBeAutoVaulted = false;
                    obj.IsInvincible = true;
                    obj.IsPersistent = true;
                    obj.IsVisible = false;
                    Entity.SetEntityCanBeDamaged(obj, false);
                    Entity.SetEntityProofs(obj, true, true, true, true, true, true, true, true);
                    Entity.SetEntityNoCollisionEntity(obj, fPlayer.Player.Character, false);
                    Entity.SetEntityNoCollisionEntity(fPlayer.Player.Character, obj, false);
                    Entity.EnableEntityBulletCollision(obj);
                }
            }
        }

        private static void HangerFixDelete()
        {
            foreach (var obj in HangerFixObj)
            {
                if (obj != null && obj.Exists())
                    obj.Delete();
            }
            HangerFixObj.Clear();
        }

        #endregion
    }
}