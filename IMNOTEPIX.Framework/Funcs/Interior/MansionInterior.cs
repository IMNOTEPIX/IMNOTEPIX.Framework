namespace IMNOTEPIX.Framework.Funcs.Interior
{
    using GTA;
    using GTA.Math;
    using GTA.Native;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Replaces fInterior.MPMansion.
    /// Three static MansionConfig instances represent all safehouse state.
    /// ActivateMansionEntitySets now takes a config instead of a MansionID switch.
    /// </summary>
    public static class MansionInterior
    {
        // ─────────────────────────────────────────────────────────────────────
        // Config instances — edit these to change mansion state
        // ─────────────────────────────────────────────────────────────────────

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

        // ─────────────────────────────────────────────────────────────────────
        // Static data
        // ─────────────────────────────────────────────────────────────────────

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

        // ─────────────────────────────────────────────────────────────────────
        // IPL manifest — keeps RequestMansion body clean
        // ─────────────────────────────────────────────────────────────────────

        // IPLs to REMOVE when enabling the DLC mansion
        private static readonly Dictionary<MansionID, string[]> _enableRemove
            = new Dictionary<MansionID, string[]>
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
            },
            };

        // IPLs to REQUEST when enabling the DLC mansion
        private static readonly Dictionary<MansionID, string[]> _enableRequest
            = new Dictionary<MansionID, string[]>
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
            },
            };

        // IPLs to REMOVE when restoring the original mansion
        private static readonly Dictionary<MansionID, string[]> _restoreRemove
            = new Dictionary<MansionID, string[]>
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
            },
            };

        // IPLs to REQUEST when restoring the original mansion
        private static readonly Dictionary<MansionID, string[]> _restoreRequest
            = new Dictionary<MansionID, string[]>
            {
                [MansionID.Richman] = new[]
            {
                "hei_ch1_06e_original_terrain",
                "hei_ch1_06e_mansion_original",
                "hei_ch1_06e_props_original",
                "hei_ch1_roads_original",
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
            },
            };

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        public static void RequestMansions()
        {
            RequestMansion(Richman);
            RequestMansion(Vinewood);
            RequestMansion(Tongva);
        }

        public static void RequestMansion(MansionConfig cfg)
        {
            int mainFloor = ResolveFloor(cfg.MainFloorCoords);
            int lowerFloor = ResolveFloor(cfg.LowerFloorCoords);
            int garageFloor = ResolveFloor(cfg.GarageFloorCoords);

            if (cfg.Enabled)
            {
                foreach (var ipl in _enableRemove[cfg.ID])
                    fInterior.RemoveIpl(ipl);

                foreach (var ipl in _enableRequest[cfg.ID])
                    fInterior.RequestIpl(ipl);

                DeactivateMansionEntitySets(mainFloor, lowerFloor, garageFloor);
                ActivateMansionEntitySets(cfg, mainFloor, lowerFloor, garageFloor);
                Script.Wait(500);
                SetupMansionChairsAndEmitters(cfg.MansionRadio, cfg.MansionClubRadio, cfg.ID);
            }
            else
            {
                foreach (var ipl in _restoreRemove[cfg.ID])
                    fInterior.RemoveIpl(ipl);

                foreach (var ipl in _restoreRequest[cfg.ID])
                    fInterior.RequestIpl(ipl);
            }
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

        public static void ActivateMansionEntitySets(
            MansionConfig cfg,
            int mainFloor,
            int lowerFloor,
            int garageFloor)
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
            if (cfg.KennelCat) Activate(mainFloor, "SET_PET_CAT");
            if (cfg.KennelDog) Activate(mainFloor, "SET_PET_DOG");

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
            Function.Call(Hash.SET_INTERIOR_ENTITY_SET_TINT_INDEX,
                mainFloor, MansionDecor[cfg.DecorStyle].Item2, cfg.Style);

            if (cfg.WallpaperStyle > 0 && cfg.WallpaperStyle < MansionPattern.Count)
            {
                string wallpaperSet = MansionPattern[cfg.WallpaperStyle].Item2;
                Activate(mainFloor, wallpaperSet);
                Function.Call(Hash.SET_INTERIOR_ENTITY_SET_TINT_INDEX,
                    mainFloor, wallpaperSet, cfg.Style);
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
            void D(int floor, string set) => Function.Call(Hash.DEACTIVATE_INTERIOR_ENTITY_SET, floor, set);

            D(mainFloor, "SET_AFTERPARTY"); D(mainFloor, "SET_AI_TABLETS_01");
            D(mainFloor, "SET_AI_TABLETS_02"); D(mainFloor, "SET_AI_TABLETS_03");
            D(mainFloor, "SET_ARCADE_BLOCKER"); D(mainFloor, "SET_ART_COASTAL");
            D(mainFloor, "SET_ART_LOFT"); D(mainFloor, "SET_ART_REGENCY");
            D(mainFloor, "SET_BIRTHDAY"); D(mainFloor, "SET_ELEV_CALI");
            D(mainFloor, "SET_ELEV_HOLLY"); D(mainFloor, "SET_ELEV_LOFT");
            D(mainFloor, "SET_HALLOWEEN"); D(mainFloor, "SET_LOFT_ARCADE_BLOCKER");
            D(mainFloor, "SET_LOFT_PODIUM_BLOCKER"); D(mainFloor, "SET_LOFT_SHELVING_PLANTER");
            D(mainFloor, "SET_LOFT_TROPHY_PLANTER"); D(mainFloor, "SET_LUNAR");
            D(mainFloor, "SET_MICHAEL_CARD"); D(mainFloor, "SET_MICHAEL_POSTER");
            D(mainFloor, "SET_PET_CAT"); D(mainFloor, "SET_PET_DOG");
            D(mainFloor, "SET_PODIUM_BLOCKER"); D(mainFloor, "SET_REG_ARCADE_BLOCKER");
            D(mainFloor, "SET_REG_PODIUM_BLOCKER"); D(mainFloor, "SET_REG_SHELVING_PLANTER");
            D(mainFloor, "SET_REG_TROPHY_PLANTER"); D(mainFloor, "SET_SHELVING_PLANTER");
            D(mainFloor, "SET_STEP_COLLISION"); D(mainFloor, "SET_STYLE_CALI");
            D(mainFloor, "SET_STYLE_CALI_TINT"); D(mainFloor, "SET_STYLE_HOLLY");
            D(mainFloor, "SET_STYLE_LOFT"); D(mainFloor, "SET_STYLE_LOFT_TINT");
            D(mainFloor, "SET_STYLE_REG_TINT"); D(mainFloor, "SET_TROPHY_PLANTER");
            D(mainFloor, "SET_TROPHY_SHELVES"); D(mainFloor, "SET_WALLPAPER_COASTAL");
            D(mainFloor, "SET_WALLPAPER_DECO"); D(mainFloor, "SET_WALLPAPER_POPART");
            D(mainFloor, "SET_WALLPAPER_RUSTIC"); D(mainFloor, "SET_WALLPAPER_SAFARI");
            D(mainFloor, "SET_WALLPAPER_SUBTLE"); D(mainFloor, "SET_XMAS");

            D(lowerFloor, "SET_ACCESS_BLOCKER"); D(lowerFloor, "SET_ARMORY_BLOCKER");
            D(lowerFloor, "SET_BASE_AI_TABLETS"); D(lowerFloor, "SET_BASE_AI_TABLETS_01");
            D(lowerFloor, "SET_BASE_AI_TABLETS_02"); D(lowerFloor, "SET_BASE_AI_TABLETS_03");
            D(lowerFloor, "SET_BASE_HALLOWEEN"); D(lowerFloor, "SET_BASE_NEW_YEAR");
            D(lowerFloor, "SET_BASE_VAULT_00"); D(lowerFloor, "SET_BASE_VAULT_01");
            D(lowerFloor, "SET_BASE_VAULT_02"); D(lowerFloor, "SET_BASE_VAULT_03");
            D(lowerFloor, "SET_BASE_VAULT_04"); D(lowerFloor, "SET_BASE_VAULT_05");
            D(lowerFloor, "SET_BASE_VAULT_06"); D(lowerFloor, "SET_BASE_VAULT_07");
            D(lowerFloor, "SET_BASE_VAULT_08"); D(lowerFloor, "SET_BASE_VAULT_09");
            D(lowerFloor, "SET_BASE_XMAS"); D(lowerFloor, "SET_ELEV_STD");
            D(lowerFloor, "SET_MOD_BLOCKER"); D(lowerFloor, "SET_VAULT_DOOR_CLOSED");
            D(lowerFloor, "SET_VAULT_DOOR_OPEN");

            D(garageFloor, "SET_GAR_AI_TABLETS"); D(garageFloor, "SET_GAR_AI_TABLETS_01");
            D(garageFloor, "SET_GAR_AI_TABLETS_02"); D(garageFloor, "SET_GAR_AI_TABLETS_03");
            D(garageFloor, "SET_GAR_MOD_BLOCKER"); D(garageFloor, "SET_GAR_PODIUM_BLOCKER");

            Function.Call(Hash.REFRESH_INTERIOR, mainFloor);
            Function.Call(Hash.REFRESH_INTERIOR, lowerFloor);
            Function.Call(Hash.REFRESH_INTERIOR, garageFloor);
        }

        // SetupMansionChairsAndEmitters — unchanged from original, just moved here
        public static void SetupMansionChairsAndEmitters(
            int mansionRadio, int mansionClubRadio, MansionID id)
        {
            // ... paste original method body here unchanged ...
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────

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
            if (activate) Activate(interior, set);
            else Deactivate(interior, set);
        }

        private static void SetEmitter(string emitterName, bool enabled, int station)
        {
            Function.Call(Hash.SET_STATIC_EMITTER_ENABLED, emitterName, enabled);
            Function.Call(Hash.SET_EMITTER_RADIO_STATION, emitterName,
                Function.Call<string>(Hash.GET_RADIO_STATION_NAME, station));
        }

        private static Prop CreateFrozenInvincibleProp(
            Model model, Vector3 pos, float heading, Vector4 quat)
        {
            Prop prop = World.CreateProp(model, pos, new Vector3(0f, 0f, heading), false, false);
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
