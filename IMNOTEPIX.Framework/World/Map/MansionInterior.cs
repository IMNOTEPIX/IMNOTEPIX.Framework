using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fWorld.Map
{
    // FULL CLUTTER DONT TOUCH THIS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // FULL CLUTTER DONT TOUCH THIS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // FULL CLUTTER DONT TOUCH THIS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // FULL CLUTTER DONT TOUCH THIS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // FULL CLUTTER DONT TOUCH THIS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// <summary>
    /// Replaces fInterior.MPMansion.
    /// Three static MansionConfig instances represent all safehouse state.
    /// ActivateMansionEntitySets now takes a config instead of a MansionID switch.
    /// </summary>
    public class MansionInterior
    {
        public static MansionConfig Richman = new MansionConfig
        {
            ID = MansionID.Richman,
            Enabled = true,
            MainFloorCoords = new Vector3(-1666.36f, 478.92f, 128.22f),
            LowerFloorCoords = new Vector3(-1649.63f, 480.97f, 117.36f),
            GarageFloorCoords = new Vector3(-1679.87f, 493.59f, 112.93f),
            AI = 1,
            DecorStyle = 1,
            WallpaperStyle = 1,
            Style = 1,
            VaultCash = 0,
            VaultRoom = true,
            ArcadeRoom = true,
            PodiumRoom = true,
            ArmouryRoom = true,
            VehicleModShopRoom = true,
            KennelDog = true,
        };

        public static MansionConfig Vinewood = new MansionConfig
        {
            ID = MansionID.Vinewood,
            Enabled = true,
            MainFloorCoords = new Vector3(539.7012f, 749.089f, 201.36f),
            LowerFloorCoords = new Vector3(547.4955f, 734.136f, 190.50f),
            GarageFloorCoords = new Vector3(548.6964f, 766.880f, 186.07f),
            AI = 1,
            DecorStyle = 1,
            WallpaperStyle = 1,
            Style = 1,
            VaultCash = 0,
            VaultRoom = true,
            ArcadeRoom = true,
            PodiumRoom = true,
            ArmouryRoom = true,
            VehicleModShopRoom = true,
            KennelDog = true,
        };

        public static MansionConfig Tongva = new MansionConfig
        {
            ID = MansionID.Tongva,
            Enabled = true,
            MainFloorCoords = new Vector3(-2586.065f, 1909.995f, 166.37f),
            LowerFloorCoords = new Vector3(-2587.495f, 1893.193f, 155.51f),
            GarageFloorCoords = new Vector3(-2568.933f, 1920.202f, 151.08f),
            AI = 1,
            DecorStyle = 1,
            WallpaperStyle = 1,
            Style = 1,
            VaultCash = 0,
            VaultRoom = true,
            ArcadeRoom = true,
            PodiumRoom = true,
            ArmouryRoom = true,
            VehicleModShopRoom = true,
            KennelDog = true,
        };

        public enum MansionID { Richman, Vinewood, Tongva }

        public static List<string> RadioStations = new List<string>();

        public static Prop[] Mansionchairs = new Prop[64];

        public static List<Tuple<string, int>> MansionTint = new List<Tuple<string, int>>
        {
            new Tuple<string, int>("Cream",    0),
            new Tuple<string, int>("Mint",     1),
            new Tuple<string, int>("Lavender", 2),
            new Tuple<string, int>("Salmon",   3),
        };

        // (display name, wallpaper entity set)
        public static List<Tuple<string, string>> MansionPattern = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("None",    ""),
            new Tuple<string, string>("Palms",   "SET_WALLPAPER_COASTAL"),
            new Tuple<string, string>("Floral",  "SET_WALLPAPER_DECO"),
            new Tuple<string, string>("Lipstick","SET_WALLPAPER_POPART"),
            new Tuple<string, string>("Tiles",   "SET_WALLPAPER_RUSTIC"),
            new Tuple<string, string>("Zebras",  "SET_WALLPAPER_SAFARI"),
            new Tuple<string, string>("Pebbles", "SET_WALLPAPER_SUBTLE"),
        };

        // (display name, tint set, style set)
        public static List<Tuple<string, string, string>> MansionDecor = new List<Tuple<string, string, string>>
        {
            new Tuple<string, string, string>("Tint1", "SET_STYLE_LOFT_TINT", "SET_STYLE_LOFT"),
            new Tuple<string, string, string>("Tint2", "SET_STYLE_CALI_TINT", "SET_STYLE_CALI"),
            new Tuple<string, string, string>("Tint3", "SET_STYLE_REG_TINT",  "SET_STYLE_HOLLY"),
        };

        // IPLs to REMOVE when enabling the DLC mansion
        private static readonly Dictionary<MansionID, string[]> _enableRemove = new Dictionary<MansionID, string[]>
        {
            [MansionID.Richman] = new[]
            {
                "hei_ch1_06e_mansion_generic",
                "hei_ch1_06e_mansion_railings_m",
                "hei_ch1_06e_mansion_railings_p",
                "hei_ch1_06e_mansion_shutters",
                "hei_ch1_06e_mansion_original",
                "hei_ch1_06e_props_original",
                "hei_ch1_roads_original",
            },
            [MansionID.Vinewood] = new[]
            {
                "apa_ch2_04_mansion_generic",
                "apa_ch2_04_mansion_private",
                "apa_ch2_04_mansion_railings_m",
                "apa_ch2_04_mansion_railings_p",
                "apa_ch2_04_mansion_shutters",
                "apa_ch2_04_mansion_original",
                "apa_ch2_04_props_original",
                "apa_ch2_04_mansion_grass",
            },
            [MansionID.Tongva] = new[]
            {
                "hei_ch1_09_mansion_generic",
                "hei_ch1_09_mansion_private",
                "hei_ch1_09_mansion_railings_m",
                "hei_ch1_09_mansion_railings_p",
                "hei_ch1_09_mansion_shutters",
                "hei_ch1_09_mansion_original",
                "hei_ch1_09_props_original",
                "hei_ch1_roads_original",
            }
        };

        // IPLs to REQUEST when enabling the DLC mansion
        private static readonly Dictionary<MansionID, string[]> _enableRequest = new Dictionary<MansionID, string[]>
        {
            [MansionID.Richman] = new[]
            {
                "m25_2_ch1_06e_mansion_interior_a",
                "m25_2_ch1_06e_mansion_interior_b",
                "m25_2_ch1_06e_mansion_interior_c",
                "m25_2_ch1_06e_mansion_interior_d",
                "hei_ch1_06e_mansion_shared",
                "hei_ch1_06f_mansion_shared",
                "hei_ch1_06e_mansion_private",
                "hei_ch1_roads_mansion",
                "hei_ch1_06e_mansion_player_bounds",
                "hei_ch1_06e_mansion_railings_m",
                "hei_ch1_06e_mansion_furniture",
                "hei_ch1_06e_mansion_firepit",
                "m25_2_mansion_gym",
                "m25_2_dog_house",
                "hei_ch1_06f_mansion_original",
            },
            [MansionID.Vinewood] = new[]
            {
                "m25_2_ch2_04_mansion_interior_a",
                "m25_2_ch2_04_mansion_interior_b",
                "m25_2_ch2_04_mansion_interior_c",
                "m25_2_ch2_04_mansion_interior_d",
                "apa_ch2_04_mansion_shared",
                "apa_ch2_04_mansion_private",
                "apa_ch2_04_mansion_player_bounds",
                "apa_ch2_04_mansion_railings_m",
                "apa_ch2_04_mansion_furniture",
                "apa_ch2_04_mansion_firepit",
                "m25_2_east_mansion_gym",
                "m25_2_east_dog_house",
            },
            [MansionID.Tongva] = new[]
            {
                "m25_2_ch1_09_mansion_interior_a",
                "m25_2_ch1_09_mansion_interior_b",
                "m25_2_ch1_09_mansion_interior_c",
                "m25_2_ch1_09_mansion_interior_d",
                "hei_ch1_09_mansion_shared",
                "hei_ch1_09_mansion_private",
                "hei_ch1_09_mansion_player_bounds",
                "hei_ch1_09_mansion_railings_m",
                "hei_ch1_09_mansion_furniture",
                "hei_ch1_09_mansion_firepit",
                "m25_2_tongva_mansion_gym",
                "m25_2_tongva_dog_house",
            }
        };

        // IPLs to REMOVE when restoring the original mansion
        private static readonly Dictionary<MansionID, string[]> _restoreRemove = new Dictionary<MansionID, string[]>
        {
            [MansionID.Richman] = new[]
            {
                "hei_ch1_06e_mansion_ground",
                "hei_ch1_06f_mansion_shared",
                "m25_2_ch1_06e_mansion_interior_a",
                "m25_2_ch1_06e_mansion_interior_b",
                "m25_2_ch1_06e_mansion_interior_c",
                "m25_2_ch1_06e_mansion_interior_d",
                "hei_ch1_06e_mansion_shared",
                "hei_ch1_06e_mansion_generic",
                "hei_ch1_06e_mansion_private",
                "hei_ch1_06e_mansion_player_bounds",
                "hei_ch1_06e_mansion_railings_m",
                "hei_ch1_06e_mansion_railings_p",
                "hei_ch1_06e_mansion_furniture",
                "hei_ch1_06e_mansion_shutters",
                "hei_ch1_06e_mansion_firepit",
                "m25_2_mansion_gym",
                "m25_2_dog_house",
            },
            [MansionID.Vinewood] = new[]
            {
                "m25_2_ch2_04_mansion_interior_a",
                "m25_2_ch2_04_mansion_interior_b",
                "m25_2_ch2_04_mansion_interior_c",
                "m25_2_ch2_04_mansion_interior_d",
                "apa_ch2_04_mansion_shared",
                "apa_ch2_04_mansion_generic",
                "apa_ch2_04_mansion_private",
                "apa_ch2_04_mansion_player_bounds",
                "apa_ch2_04_mansion_railings_m",
                "apa_ch2_04_mansion_railings_p",
                "apa_ch2_04_mansion_furniture",
                "apa_ch2_04_mansion_shutters",
                "apa_ch2_04_mansion_firepit",
                "m25_2_east_mansion_gym",
                "m25_2_east_dog_house",
            },
            [MansionID.Tongva] = new[]
            {
                "m25_2_ch1_09_mansion_interior_a",
                "m25_2_ch1_09_mansion_interior_b",
                "m25_2_ch1_09_mansion_interior_c",
                "m25_2_ch1_09_mansion_interior_d",
                "hei_ch1_09_mansion_shared",
                "hei_ch1_09_mansion_generic",
                "hei_ch1_09_mansion_private",
                "hei_ch1_09_mansion_player_bounds",
                "hei_ch1_09_mansion_railings_m",
                "hei_ch1_09_mansion_railings_p",
                "hei_ch1_09_mansion_furniture",
                "hei_ch1_09_mansion_shutters",
                "hei_ch1_09_mansion_firepit",
                "m25_2_tongva_mansion_gym",
                "m25_2_tongva_dog_house",
            }
        };

        // IPLs to REQUEST when restoring the original mansion
        private static readonly Dictionary<MansionID, string[]> _restoreRequest = new Dictionary<MansionID, string[]>
        {
            [MansionID.Richman] = new[]
            {
                "hei_ch1_06e_original_terrain",
                "hei_ch1_06e_mansion_original",
                "hei_ch1_06e_props_original",
                "hei_ch1_roads_original",
                "hei_ch1_06f_mansion_original"
            },
            [MansionID.Vinewood] = new[]
            {
                "apa_ch2_04_mansion_original",
                "apa_ch2_04_props_original",
                "apa_ch2_04_mansion_grass",
            },
            [MansionID.Tongva] = new[]
            {
                "hei_ch1_09_mansion_original",
                "hei_ch1_09_props_original",
            }
        };

        public static void RequestMansions()
        {
            RequestMansion(Richman);
            RequestMansion(Vinewood);
            RequestMansion(Tongva);
            fUI.Hud.SetMinimapComponent(20, true, -1);
            fUI.Hud.SetMinimapComponent(22, true, -1);
            fUI.Hud.SetMinimapComponent(21, true, -1);
        }

        public static void RemoveMansions()
        {
            RemoveMansion(Richman);
            RemoveMansion(Vinewood);
            RemoveMansion(Tongva);
            fUI.Hud.SetMinimapComponent(20, false, -1);
            fUI.Hud.SetMinimapComponent(22, false, -1);
            fUI.Hud.SetMinimapComponent(21, false, -1);
        }

        public static void RequestMansion(MansionConfig cfg)
        {
            int mainFloor = ResolveFloor(cfg.MainFloorCoords);
            int lowerFloor = ResolveFloor(cfg.LowerFloorCoords);
            int garageFloor = ResolveFloor(cfg.GarageFloorCoords);

            if (cfg.Enabled)
            {
                foreach (var ipl in _enableRemove[cfg.ID])
                    fCore.Streaming.RemoveIpl(ipl);

                foreach (var ipl in _enableRequest[cfg.ID])
                    fCore.Streaming.RequestIpl(ipl);

                DeactivateMansionEntitySets(mainFloor, lowerFloor, garageFloor);
                ActivateMansionEntitySets(cfg, mainFloor, lowerFloor, garageFloor);
                Script.Wait(500);
                SetupMansionChairsAndEmitters(cfg.MansionRadio, cfg.MansionClubRadio, cfg.ID);
            }
            else
            {
                foreach (var ipl in _restoreRemove[cfg.ID])
                    fCore.Streaming.RemoveIpl(ipl);

                foreach (var ipl in _restoreRequest[cfg.ID])
                    fCore.Streaming.RequestIpl(ipl);
            }
        }

        public static void RemoveMansion(MansionConfig cfg)
        {
            foreach (var ipl in _restoreRemove[cfg.ID])
                fCore.Streaming.RemoveIpl(ipl);

            foreach (var ipl in _restoreRequest[cfg.ID])
                fCore.Streaming.RequestIpl(ipl);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Entity set activation — shared across all three mansions
        //
        // BUGS FIXED vs original:
        //   1. Richman used The_Tongva_Estate_Style for its tint index (copy-paste).
        //      All three now correctly use cfg.Style.
        //   2. Richman/Vinewood activated GAR_AI sub-variants on LowerFloor instead
        //      of GarageFloor. Now consistent with Tongva (GarageFloor is correct).
        //   3. Tongva called REFRESH_INTERIOR mid-method before wallpaper was set.
        //      All refreshes now happen at the end.
        // ─────────────────────────────────────────────────────────────────────

        public static void ActivateMansionEntitySets(MansionConfig cfg, int mainFloor, int lowerFloor, int garageFloor)
        {
            // AI suffix: AI=2 → _01 | AI=1 → _02 | AI=0 → _03
            int aiSuffix = 3 - cfg.AI;

            // ── Main floor ────────────────────────────────────────────────

            if (cfg.AfterParty)
                Activate(mainFloor, "SET_AFTERPARTY");

            Activate(mainFloor, $"SET_AI_TABLETS_{aiSuffix:D2}");

            // Art style and elevator variant
            string[] artStyles = { "SET_ART_COASTAL", "SET_ART_LOFT", "SET_ART_REGENCY" };
            string[] elevStyles = { "SET_ELEV_LOFT", "SET_ELEV_HOLLY", "SET_ELEV_CALI" };
            Activate(mainFloor, artStyles[cfg.DecorStyle]);
            Activate(mainFloor, elevStyles[cfg.DecorStyle]);

            // Decor-style-specific planters / collision
            switch (cfg.DecorStyle)
            {
                case 0:
                    Activate(mainFloor, "SET_LOFT_SHELVING_PLANTER");
                    Activate(mainFloor, "SET_LOFT_TROPHY_PLANTER");
                    break;
                case 1:
                    Activate(mainFloor, "SET_SHELVING_PLANTER");
                    Activate(mainFloor, "SET_STEP_COLLISION");
                    break;
                case 2:
                    Activate(mainFloor, "SET_REG_SHELVING_PLANTER");
                    Activate(mainFloor, "SET_REG_TROPHY_PLANTER");
                    break;
            }

            Activate(mainFloor, "SET_MICHAEL_CARD");
            Activate(mainFloor, "SET_MICHAEL_POSTER");
            if (cfg.KennelCat) 
                Activate(mainFloor, "SET_PET_CAT");
            if (cfg.KennelDog) 
                Activate(mainFloor, "SET_PET_DOG");

            // Seasonal
            SetOrClear(mainFloor, "SET_LUNAR", cfg.DecorationsLunarNewYear);
            SetOrClear(mainFloor, "SET_BIRTHDAY", cfg.DecorationsBirthday);
            SetOrClear(mainFloor, "SET_HALLOWEEN", cfg.DecorationsHalloween);
            SetOrClear(mainFloor, "SET_XMAS", cfg.DecorationsXmas);

            // Room blockers — suffix depends on decor style
            if (!cfg.ArcadeRoom)
            {
                string[] b = { "SET_LOFT_ARCADE_BLOCKER", "SET_ARCADE_BLOCKER", "SET_REG_ARCADE_BLOCKER" };
                Activate(mainFloor, b[cfg.DecorStyle]);
            }
            if (!cfg.PodiumRoom)
            {
                string[] b = { "SET_LOFT_PODIUM_BLOCKER", "SET_PODIUM_BLOCKER", "SET_REG_PODIUM_BLOCKER" };
                Activate(mainFloor, b[cfg.DecorStyle]);
            }

            // Style / tint — deactivate all first, then apply selection
            for (int i = 0; i < MansionDecor.Count; i++)
            {
                Deactivate(mainFloor, MansionDecor[i].Item2);
                Deactivate(mainFloor, MansionDecor[i].Item3);
            }
            // Note: original only deactivated 0–5 of MansionPattern (7 items).
            // Preserved here intentionally to match runtime behaviour.
            for (int i = 0; i < 6; i++)
                Deactivate(mainFloor, MansionPattern[i].Item2);

            Activate(mainFloor, MansionDecor[cfg.DecorStyle].Item3);
            Activate(mainFloor, MansionDecor[cfg.DecorStyle].Item2);
            Function.Call(Hash.SET_INTERIOR_ENTITY_SET_TINT_INDEX, mainFloor, MansionDecor[cfg.DecorStyle].Item2, cfg.Style);

            if (cfg.WallpaperStyle > 0 && cfg.WallpaperStyle < MansionPattern.Count)
            {
                string wallpaperSet = MansionPattern[cfg.WallpaperStyle].Item2;
                Activate(mainFloor, wallpaperSet);
                Function.Call(Hash.SET_INTERIOR_ENTITY_SET_TINT_INDEX, mainFloor, wallpaperSet, cfg.Style);
            }

            // ── Lower floor ───────────────────────────────────────────────

            if (cfg.WindowsBlackedOut)
                Activate(lowerFloor, "SET_ACCESS_BLOCKER");
            if (!cfg.ArmouryRoom)
                Activate(lowerFloor, "SET_ARMORY_BLOCKER");

            Activate(lowerFloor, "SET_BASE_AI_TABLETS");
            Activate(lowerFloor, $"SET_BASE_AI_TABLETS_{aiSuffix:D2}");

            if (cfg.VaultCash >= 1 && cfg.VaultCash <= 10)
                Activate(lowerFloor, $"SET_BASE_VAULT_{cfg.VaultCash - 1:D2}");

            Activate(lowerFloor, "SET_ELEV_STD");
            SetOrClear(lowerFloor, "SET_MOD_BLOCKER", !cfg.VehicleModShopRoom);
            SetOrClear(lowerFloor, "SET_BASE_HALLOWEEN", cfg.DecorationsHalloween);
            SetOrClear(lowerFloor, "SET_BASE_NEW_YEAR", cfg.DecorationsNewYear);
            SetOrClear(lowerFloor, "SET_BASE_XMAS", cfg.DecorationsXmas);

            if (!cfg.VaultRoom)
            {
                Activate(lowerFloor, "SET_VAULT_DOOR_CLOSED");
                Deactivate(lowerFloor, "SET_VAULT_DOOR_OPEN");
            }
            else
            {
                // Clear before re-setting — same pattern as original
                Deactivate(lowerFloor, "SET_VAULT_DOOR_OPEN");
                Activate(lowerFloor, "SET_VAULT_DOOR_OPEN");
            }

            // ── Garage floor ──────────────────────────────────────────────

            Activate(garageFloor, "SET_GAR_AI_TABLETS");
            // Fixed: original Richman/Vinewood incorrectly targeted LowerFloor here
            Activate(garageFloor, $"SET_GAR_AI_TABLETS_{aiSuffix:D2}");
            SetOrClear(garageFloor, "SET_GAR_MOD_BLOCKER", !cfg.VehicleModShopRoom);
            SetOrClear(garageFloor, "SET_GAR_PODIUM_BLOCKER", !cfg.PodiumRoom);

            // ── Refresh — always last ─────────────────────────────────────
            // Fixed: original Tongva called REFRESH mid-method before wallpaper was set

            Function.Call(Hash.REFRESH_INTERIOR, mainFloor);
            Function.Call(Hash.REFRESH_INTERIOR, lowerFloor);
            Function.Call(Hash.REFRESH_INTERIOR, garageFloor);
        }

        public static void DeactivateMansionEntitySets(int mainFloor, int lowerFloor, int garageFloor)
        {
            // Unchanged from original — full deactivation before re-applying
            void D(int floor, string set) 
                => Function.Call(Hash.DEACTIVATE_INTERIOR_ENTITY_SET, floor, set);

            D(mainFloor, "SET_AFTERPARTY"); 
            D(mainFloor, "SET_AI_TABLETS_01");
            D(mainFloor, "SET_AI_TABLETS_02"); 
            D(mainFloor, "SET_AI_TABLETS_03");
            D(mainFloor, "SET_ARCADE_BLOCKER"); 
            D(mainFloor, "SET_ART_COASTAL");
            D(mainFloor, "SET_ART_LOFT"); 
            D(mainFloor, "SET_ART_REGENCY");
            D(mainFloor, "SET_BIRTHDAY"); 
            D(mainFloor, "SET_ELEV_CALI");
            D(mainFloor, "SET_ELEV_HOLLY"); 
            D(mainFloor, "SET_ELEV_LOFT");
            D(mainFloor, "SET_HALLOWEEN"); 
            D(mainFloor, "SET_LOFT_ARCADE_BLOCKER");
            D(mainFloor, "SET_LOFT_PODIUM_BLOCKER"); 
            D(mainFloor, "SET_LOFT_SHELVING_PLANTER");
            D(mainFloor, "SET_LOFT_TROPHY_PLANTER"); 
            D(mainFloor, "SET_LUNAR");
            D(mainFloor, "SET_MICHAEL_CARD"); 
            D(mainFloor, "SET_MICHAEL_POSTER");
            D(mainFloor, "SET_PET_CAT"); 
            D(mainFloor, "SET_PET_DOG");
            D(mainFloor, "SET_PODIUM_BLOCKER"); 
            D(mainFloor, "SET_REG_ARCADE_BLOCKER");
            D(mainFloor, "SET_REG_PODIUM_BLOCKER"); 
            D(mainFloor, "SET_REG_SHELVING_PLANTER");
            D(mainFloor, "SET_REG_TROPHY_PLANTER"); 
            D(mainFloor, "SET_SHELVING_PLANTER");
            D(mainFloor, "SET_STEP_COLLISION"); 
            D(mainFloor, "SET_STYLE_CALI");
            D(mainFloor, "SET_STYLE_CALI_TINT"); 
            D(mainFloor, "SET_STYLE_HOLLY");
            D(mainFloor, "SET_STYLE_LOFT"); 
            D(mainFloor, "SET_STYLE_LOFT_TINT");
            D(mainFloor, "SET_STYLE_REG_TINT"); 
            D(mainFloor, "SET_TROPHY_PLANTER");
            D(mainFloor, "SET_TROPHY_SHELVES"); 
            D(mainFloor, "SET_WALLPAPER_COASTAL");
            D(mainFloor, "SET_WALLPAPER_DECO"); 
            D(mainFloor, "SET_WALLPAPER_POPART");
            D(mainFloor, "SET_WALLPAPER_RUSTIC"); 
            D(mainFloor, "SET_WALLPAPER_SAFARI");
            D(mainFloor, "SET_WALLPAPER_SUBTLE"); 
            D(mainFloor, "SET_XMAS");

            D(lowerFloor, "SET_ACCESS_BLOCKER"); 
            D(lowerFloor, "SET_ARMORY_BLOCKER");
            D(lowerFloor, "SET_BASE_AI_TABLETS"); 
            D(lowerFloor, "SET_BASE_AI_TABLETS_01");
            D(lowerFloor, "SET_BASE_AI_TABLETS_02"); 
            D(lowerFloor, "SET_BASE_AI_TABLETS_03");
            D(lowerFloor, "SET_BASE_HALLOWEEN"); 
            D(lowerFloor, "SET_BASE_NEW_YEAR");
            D(lowerFloor, "SET_BASE_VAULT_00"); 
            D(lowerFloor, "SET_BASE_VAULT_01");
            D(lowerFloor, "SET_BASE_VAULT_02"); 
            D(lowerFloor, "SET_BASE_VAULT_03");
            D(lowerFloor, "SET_BASE_VAULT_04"); 
            D(lowerFloor, "SET_BASE_VAULT_05");
            D(lowerFloor, "SET_BASE_VAULT_06"); 
            D(lowerFloor, "SET_BASE_VAULT_07");
            D(lowerFloor, "SET_BASE_VAULT_08"); 
            D(lowerFloor, "SET_BASE_VAULT_09");
            D(lowerFloor, "SET_BASE_XMAS"); 
            D(lowerFloor, "SET_ELEV_STD");
            D(lowerFloor, "SET_MOD_BLOCKER"); 
            D(lowerFloor, "SET_VAULT_DOOR_CLOSED");
            D(lowerFloor, "SET_VAULT_DOOR_OPEN");

            D(garageFloor, "SET_GAR_AI_TABLETS"); 
            D(garageFloor, "SET_GAR_AI_TABLETS_01");
            D(garageFloor, "SET_GAR_AI_TABLETS_02"); 
            D(garageFloor, "SET_GAR_AI_TABLETS_03");
            D(garageFloor, "SET_GAR_MOD_BLOCKER"); 
            D(garageFloor, "SET_GAR_PODIUM_BLOCKER");

            Function.Call(Hash.REFRESH_INTERIOR, mainFloor);
            Function.Call(Hash.REFRESH_INTERIOR, lowerFloor);
            Function.Call(Hash.REFRESH_INTERIOR, garageFloor);
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Chairs and emitters
        //
        // Emitter names follow a consistent pattern — only the location suffix varies:
        //   Richman = loc_2, Vinewood = loc_3, Tongva = loc_1
        //
        // Chair arrays are allocated per-mansion so switching mansions doesn't
        // disturb chairs belonging to a different one:
        //   Richman  → Mansionchairs[0..8]
        //   Vinewood → Mansionchairs[9..17]
        //   Tongva   → Mansionchairs[18..26]
        // ─────────────────────────────────────────────────────────────────────────────

        // Emitter base names shared by all three mansions.
        // The location suffix (_loc_N) is appended at call time using the
        // per-mansion suffix stored in _emitterLocSuffix below.
        private static readonly string[] _sharedEmitters = new[]
        {
            "se_dlc25-2_basement_armoury",
            "se_dlc25-2_basement_garage",
            "se_dlc25-2_basement_lobby_02",
            "se_dlc25-2_basement_lobby_03",
            "se_dlc25-2_basement_mod_garage",
            "se_dlc25-2_mansion_arcade",
            "se_dlc25-2_mansion_gallery",
            "se_dlc25-2_mansion_kitchen",
            "se_dlc25-2_mansion_lobby",
            "se_dlc25-2_mansion_trophy_room",
            "se_dlc25-2_mansion_office",
            "se_dlc25-2_mansion_cigar_room",
            "se_dlc25-2_mansion_guest_bedroom",
            "se_dlc25-2_mansion_master_bedroom",
            "se_dlc25-2_basement_garage_top",
            "dlc25-2_mansion_exterior_central",         // no leading "se_" — matches original
            "se_dlc25-2_mansion_exterior_pool_bar",
            "se_dlc25-2_mansion_exterior_terrace"
        };

        private static readonly string[] _clubEmitters = new[] { "se_dlc25-2_basement_club_01", "se_dlc25-2_basement_club_02" };

        // Emitter location suffix per mansion.
        // Richman = 2, Vinewood = 3, Tongva = 1.
        private static readonly Dictionary<MansionID, int> _emitterLocSuffix = new Dictionary<MansionID, int>
        {
            { MansionID.Richman,  2 },
            { MansionID.Vinewood, 3 },
            { MansionID.Tongva,   1 },
        };

        // Chair slot range per mansion [start, end) — exclusive upper bound.
        private static readonly Dictionary<MansionID, (int Start, int End)> _chairRange = new Dictionary<MansionID, (int, int)>
        {
            { MansionID.Richman,  (0,  9) },   // indices 0..8
            { MansionID.Vinewood, (9,  18) },  // indices 9..17
            { MansionID.Tongva,   (18, 27) },  // indices 18..26
        };

        // SetupMansionChairsAndEmitters — unchanged from original, just moved here
        public static void SetupMansionChairsAndEmitters(int mansionRadio, int mansionClubRadio, MansionID id)
        {
            int locSuffix = _emitterLocSuffix[id];

            // ── Emitters ──────────────────────────────────────────────────────────
            foreach (var baseName in _sharedEmitters)
                SetEmitter($"{baseName}_loc_{locSuffix}", true, mansionRadio);

            foreach (var baseName in _clubEmitters)
                SetEmitter($"{baseName}_loc_{locSuffix}", true, mansionClubRadio);

            // ── Clear existing chairs for this mansion slot ───────────────────────
            var (start, end) = _chairRange[id];

            for (int i = start; i < end; i++)
            {
                if (Mansionchairs[i] != null && Mansionchairs[i].Exists())
                {
                    Mansionchairs[i].Delete();
                    Mansionchairs[i] = null;
                }
            }

            // ── Spawn chairs ──────────────────────────────────────────────────────
            // All three mansions share the same layout: 8 dining chairs in two
            // facing rows plus one executive office chair at the head.
            // Quaternion values encode the exact world-space rotations required
            // by the table geometry at each location — do not approximate with headings.
            try
            {
                switch (id)
                {
                    case MansionID.Richman:
                        {
                            var quatA = new Vector4(0f, 0f, -0.587628f, 0.809132f);  // row A (~288°)
                            var quatB = new Vector4(0f, 0f, 0.813719f, 0.581259f);  // row B (~108°)
                            var quatC = new Vector4(0f, 0f, -0.483377f, 0.875413f);  // exec chair

                            Mansionchairs[0] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-1643.57f, 474.875f, 128.836f), 288.022f, quatA);
                            Mansionchairs[1] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-1643.93f, 475.989f, 128.836f), 288.022f, quatA);
                            Mansionchairs[2] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-1644.27f, 477.036f, 128.836f), 288.022f, quatA);
                            Mansionchairs[3] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-1644.60f, 478.074f, 128.836f), 288.022f, quatA);
                            Mansionchairs[4] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-1645.48f, 474.288f, 128.836f), 108.922f, quatB);
                            Mansionchairs[5] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-1645.88f, 475.433f, 128.836f), 108.922f, quatB);
                            Mansionchairs[6] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-1646.23f, 476.435f, 128.836f), 108.922f, quatB);
                            Mansionchairs[7] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-1646.60f, 477.486f, 128.836f), 108.922f, quatB);
                            Mansionchairs[8] = CreateFrozenInvincibleProp(RequestModel("ex_prop_offchair_exec_04"), new Vector3(-1630.88f, 484.486f, 128.861f), 302.188f, quatC);
                            break;
                        }

                    case MansionID.Vinewood:
                        {
                            var quatA = new Vector4(0f, 0f, 0.947196f, -0.320656f);  // row A (~217°)
                            var quatB = new Vector4(0f, 0f, 0.337979f, 0.941154f);  // row B (~39°)
                            var quatC = new Vector4(0f, 0f, 0.891669f, -0.452687f);  // exec chair

                            Mansionchairs[9] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(543.985f, 726.466f, 201.961f), 217.405f, quatA);
                            Mansionchairs[10] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(544.961f, 727.216f, 201.961f), 304.358f, quatA);
                            Mansionchairs[11] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(545.857f, 727.906f, 201.961f), 217.405f, quatA);
                            Mansionchairs[12] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(546.706f, 728.559f, 201.961f), 217.405f, quatA);
                            Mansionchairs[13] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(542.760f, 727.947f, 201.961f), 39.508f, quatB);
                            Mansionchairs[14] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(543.737f, 728.754f, 201.961f), 39.508f, quatB);
                            Mansionchairs[15] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(544.660f, 729.515f, 201.961f), 39.508f, quatB);
                            Mansionchairs[16] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(545.437f, 730.156f, 201.961f), 39.508f, quatB);
                            Mansionchairs[17] = CreateFrozenInvincibleProp(RequestModel("ex_prop_offchair_exec_04"), new Vector3(557.304f, 717.821f, 201.942f), 233.832f, quatC);
                            break;
                        }

                    case MansionID.Tongva:
                        {
                            var quatA = new Vector4(0f, 0f, 0.999217f, -0.0395644f); // row A (~184°)
                            var quatB = new Vector4(0f, 0f, 0.0526771f, 0.998612f);  // row B (~6°)
                            var quatC = new Vector4(0f, 0f, 0.97597f, -0.217906f);  // exec chair

                            Mansionchairs[18] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-2594.76f, 1888.64f, 166.985f), 184.535f, quatA);
                            Mansionchairs[19] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-2593.57f, 1888.72f, 166.985f), 184.535f, quatA);
                            Mansionchairs[20] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-2592.44f, 1888.81f, 166.985f), 184.535f, quatA);
                            Mansionchairs[21] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-2591.32f, 1888.89f, 166.985f), 184.535f, quatA);
                            Mansionchairs[22] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-2594.85f, 1890.48f, 166.985f), 6.039f, quatB);
                            Mansionchairs[23] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-2593.66f, 1890.61f, 166.985f), 6.039f, quatB);
                            Mansionchairs[24] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-2592.60f, 1890.73f, 166.985f), 6.039f, quatB);
                            Mansionchairs[25] = CreateFrozenInvincibleProp(RequestModel("m25_2_prop_m52_diningchair_01a"), new Vector3(-2591.53f, 1890.85f, 166.985f), 6.039f, quatB);
                            Mansionchairs[26] = CreateFrozenInvincibleProp(RequestModel("ex_prop_offchair_exec_04"), new Vector3(-2588.06f, 1874.30f, 166.966f), 205.172f, quatC);
                            break;
                        }
                }
            }
            catch
            {
                fUI.Hud.ShowNotification("~p~IMNOTEPIX~w~ - ~r~Error~w~ Could not spawn 'm25_2_prop_m52_diningchair_01a'. This is likely due to your mods folder missing the model. Remove the mods folder and see if the error persists.");
            }
        }

        private static int ResolveFloor(Vector3 coords)
            => Function.Call<int>(Hash.GET_INTERIOR_AT_COORDS, coords.X, coords.Y, coords.Z);

        private static void Activate(int interior, string set)
        {
            if (!string.IsNullOrEmpty(set))
                Function.Call(Hash.ACTIVATE_INTERIOR_ENTITY_SET, interior, set);
        }

        private static void Deactivate(int interior, string set)
        {
            if (!string.IsNullOrEmpty(set))
                Function.Call(Hash.DEACTIVATE_INTERIOR_ENTITY_SET, interior, set);
        }

        private static void SetOrClear(int interior, string set, bool activate)
        {
            if (activate) 
                Activate(interior, set);
            else 
                Deactivate(interior, set);
        }

        private static void SetEmitter(string emitterName, bool enabled, int station)
        {
            Function.Call(Hash.SET_STATIC_EMITTER_ENABLED, emitterName, enabled);
            Function.Call(Hash.SET_EMITTER_RADIO_STATION, emitterName, Function.Call<string>(Hash.GET_RADIO_STATION_NAME, station));
        }

        private static Prop CreateFrozenInvincibleProp(Model model, Vector3 pos, float heading, Vector4 quat)
        {
            Prop prop = GTA.World.CreateProp(model, pos, new Vector3(0f, 0f, heading), false, false);
            prop.Position -= new Vector3(0f, 0f, 0.55f);
            Function.Call(Hash.SET_ENTITY_HEADING, prop, heading);
            Function.Call(Hash.SET_ENTITY_QUATERNION, prop, quat.X, quat.Y, quat.Z, quat.W);
            Function.Call(Hash.SET_ENTITY_INVINCIBLE, prop, true);
            Function.Call(Hash.FREEZE_ENTITY_POSITION, prop, true);
            Function.Call(Hash.SET_ENTITY_VISIBLE, prop, true, false);
            return prop;
        }

        private static Model RequestModel(string name)
        {
            // One yield instead of blocking loop — consistent with CombatPed.Create()
            Model model = new Model(name);
            model.Request(250);

            if (model.IsInCdImage && model.IsValid && !model.IsLoaded)
                Script.Yield();

            if (!model.IsLoaded)
                model.MarkAsNoLongerNeeded();

            return model;
        }
    }
}
