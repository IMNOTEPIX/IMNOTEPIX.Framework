using GTA;
using GTA.Native;
using System.ComponentModel;
using System;

namespace IMNOTEPIX.Framework.Core
{
    [Obsolete("Use ScriptManager instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class InGameScripts
    {
        // ─────────────────────────────────────────────────────────────────────
        // Start
        // ─────────────────────────────────────────────────────────────────────

        public static void StartAllNeededToStartScripts()
        {
            StartScriptMissionTriggerers();
            StartScriptSideJobs();
        }

        private static void StartScriptMissionTriggerers()
        {
            StartScript("mission_triggerer_a", 5050);
            StartScript("mission_triggerer_b", 5050);
            StartScript("mission_triggerer_c", 5050);
            StartScript("mission_triggerer_d", 5050);
        }

        private static void StartScriptSideJobs()
        {
            StartScript("controller_taxi", 2050);
            StartScript("controller_towing", 2050);
            StartScript("controller_trafficking", 1424);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Terminate
        // ─────────────────────────────────────────────────────────────────────

        public static void TerminateScriptsExceptMissions()
        {
            TerminateLaunchers();
            TerminateAllPropertyManagement();
            TerminateAllRandomEvents();
            TerminateRaces();
            TerminateSideJobs();
            TerminateAmbientAreas();
            TerminateBaseJumps();
            TerminateMichaelEvents();
            TerminateRampages();
        }

        public static void TerminateScriptsWithMissions()
        {
            TerminateScriptsExceptMissions();
            TerminateMissionTriggerers();
        }

        private static void TerminateMissionTriggerers()
        {
            Terminate("mission_triggerer_a");
            Terminate("mission_triggerer_b");
            Terminate("mission_triggerer_c");
            Terminate("mission_triggerer_d");
        }

        private static void TerminateSideJobs()
        {
            Terminate("controller_taxi");
            Terminate("controller_towing");
            Terminate("controller_trafficking");
        }

        private static void TerminateAmbientAreas()
        {
            TerminateShops();
            TerminateStripClub();
            TerminateCinemas();
            TerminateGolf();
            TerminateBlimp();
            TerminateAmusementPark();
            TerminateCableCars();
            TerminateBootyCalls();
        }

        private static void TerminateShops() => Terminate("shop_controller");
        private static void TerminateStripClub() => Terminate("stripclub");
        private static void TerminateCinemas() => Terminate("act_cinema");
        private static void TerminateBlimp() => Terminate("ambientblimp");
        private static void TerminateAmusementPark() => Terminate("fairgroundhub");
        private static void TerminateCableCars() => Terminate("cablecar");
        private static void TerminateBootyCalls() => Terminate("bootycallhandler");
        private static void TerminateBaseJumps() => Terminate("bj");

        private static void TerminateGolf()
        {
            Terminate("launcher_golf");
            Terminate("golf");
            Terminate("golf_ai_foursome");
            Terminate("golf_ai_foursome_putting");
        }

        private static void TerminateRaces()
        {
            Terminate("launcher_racing");
            Terminate("launcher_offroadracing");
            Terminate("controller_races");
            Terminate("country_race_controller");
        }

        private static void TerminateRampages()
        {
            Terminate("launcher_rampage");
            Terminate("rampage_controller");
        }

        private static void TerminateMichaelEvents()
        {
            Terminate("me_jimmy1");
            Terminate("me_amanda1");
            Terminate("me_tracey1");
        }

        private static void TerminateLaunchers()
        {
            Terminate("launcher_abigail"); Terminate("launcher_barry");
            Terminate("launcher_basejumpheli"); Terminate("launcher_basejumppack");
            Terminate("launcher_carwash"); Terminate("launcher_darts");
            Terminate("launcher_dreyfuss"); Terminate("launcher_epsilon");
            Terminate("launcher_extreme"); Terminate("launcher_fanatic");
            Terminate("launcher_hao"); Terminate("launcher_hunting");
            Terminate("launcher_hunting_ambient");
            Terminate("launcher_josh"); Terminate("launcher_maude");
            Terminate("launcher_minute"); Terminate("launcher_mrsphilips");
            Terminate("launcher_nigel"); Terminate("launcher_omega");
            Terminate("launcher_paparazzo"); Terminate("launcher_pilotschool");
            Terminate("launcher_range"); Terminate("launcher_stunts");
            Terminate("launcher_tennis"); Terminate("launcher_thelastone");
            Terminate("launcher_tonya"); Terminate("launcher_triathlon");
            Terminate("launcher_yoga"); Terminate("bailbond_launcher");
        }

        private static void TerminateAllPropertyManagement()
        {
            Terminate("pm_defend"); Terminate("pm_delivery");
            Terminate("pm_gang_attack"); Terminate("pm_plane_promotion");
            Terminate("pm_recover_stolen");
        }

        private static void TerminateAllRandomEvents()
        {
            Terminate("re_yetarian"); Terminate("re_stag_do");
            Terminate("re_snatched"); Terminate("re_shoprobbery");
            Terminate("re_securityvan"); Terminate("re_seaplane");
            Terminate("re_rescuehostage"); Terminate("re_prisonvanbreak");
            Terminate("re_prisonerlift"); Terminate("re_prison");
            Terminate("re_paparazzi"); Terminate("re_muggings");
            Terminate("re_mountdance"); Terminate("re_monkey");
            Terminate("re_lured"); Terminate("re_lossantosintl");
            Terminate("re_homeland_security");
            Terminate("re_hitch_lift"); Terminate("re_getaway_driver");
            Terminate("re_gangfight"); Terminate("re_gang_intimidation");
            Terminate("re_duel"); Terminate("re_drunkdriver");
            Terminate("re_domestic"); Terminate("re_dealgonewrong");
            Terminate("re_cultshootout"); Terminate("re_crashrescue");
            Terminate("re_chasethieves"); Terminate("re_cartheft");
            Terminate("re_bus_tours"); Terminate("re_burials");
            Terminate("re_border"); Terminate("re_bikethief");
            Terminate("re_atmrobbery"); Terminate("re_arrests");
            Terminate("re_armybase"); Terminate("re_accident");
            Terminate("re_abandonedcar");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        public static void StartScript(string scriptName, int buffer)
        {
            RequestScript(scriptName);
            Function.Call(Hash.START_NEW_SCRIPT, scriptName, buffer);
            Function.Call(Hash.SET_SCRIPT_AS_NO_LONGER_NEEDED, scriptName);
        }

        // Fixed: was an instance method on a fully static class
        private static void RequestScript(string scriptName)
        {
            while (!Function.Call<bool>(Hash.HAS_SCRIPT_LOADED, scriptName))
            {
                Function.Call(Hash.REQUEST_SCRIPT, scriptName);
                Script.Yield();
            }
        }

        private static void Terminate(string scriptName)
            => Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, scriptName);
    }
}
