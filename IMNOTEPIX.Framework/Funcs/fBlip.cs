using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

// Compiler shim to enable C# 9 records on .NET Framework 4.8.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace IMNOTEPIX.Framework.Funcs
{
    // ── Blip preset data ──────────────────────────────────────────────────────

    /// <summary>Immutable description of how a blip should look and behave on the map.</summary>
    public record BlipPreset(
        BlipSprite Sprite,
        BlipColor Color,
        string Name = "",
        float Scale = 1f,
        bool IsShortRange = true,
        BlipDisplayType DisplayType = BlipDisplayType.Default
    );

    /// <summary>Ready-made <see cref="BlipPreset"/> instances for common mission blips.</summary>
    public static class BlipPresets
    {
        public static readonly BlipPreset CashCart = new(
            (BlipSprite)272,
            BlipColor.Green,
            "Cash",
            Scale: 0.9f
        );

        public static readonly BlipPreset GoldCart = new(
            (BlipSprite)618,
            (BlipColor)28,
            "Gold",
            Scale: 0.8f
        );

        public static readonly BlipPreset DiamondsCart = new(
            (BlipSprite)617,
            (BlipColor)26,
            "Diamonds",
            Scale: 0.8f
        );

        public static readonly BlipPreset CocaineCart = new(
            (BlipSprite)514,
            BlipColor.Grey,
            "Cocaine",
            Scale: 0.8f
        );

        public static readonly BlipPreset VaultCart = new(
            BlipSprite.Standard,
            BlipColor.Green,
            "Cash",
            Scale: 0.75f
        );

        public static readonly BlipPreset Destination = new(
            BlipSprite.Standard,
            BlipColor.Yellow,
            "Destination",
            Scale: 1f,
            IsShortRange: false
        );

        public static readonly BlipPreset Torchable = new(
            BlipSprite.Standard,
            BlipColor.Red,
            Scale: 0.75f,
            DisplayType: BlipDisplayType.MiniMapOnly
        );

        public static readonly BlipPreset Template = new(
            BlipSprite.Standard,
            BlipColor.Red,
            "Template",
            Scale: 1f,
            IsShortRange: false,
            DisplayType: BlipDisplayType.Default
        );
    }

    // ── fBlip ─────────────────────────────────────────────────────────────────

    public static class fBlip
    {
        // ── Sprite classification ─────────────────────────────────────────────
        // Used to skip blips the game itself owns (waypoints, contacts, etc.)

        private static readonly HashSet<int> GameplayBlipSprites = new HashSet<int>
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            10, 11, 12, 13, 14, 15, 41, 42,
            162, /* Point of interest */
            185,
        };

        private static readonly HashSet<int> SpecificBlipSprites_0 = new HashSet<int>
        {
            71, 72, 73, 110, 162, 313,
        };

        private static bool IsBlipGameplayBlip(Blip blip)
            => GameplayBlipSprites.Contains((int)blip.Sprite);

        private static bool IsSpecificBlip_0(Blip blip)
            => SpecificBlipSprites_0.Contains((int)blip.Sprite);

        // ── Visibility helpers ────────────────────────────────────────────────

        /// <summary>
        /// Hides all non-gameplay blips (except <see cref="SpecificBlipSprites_0"/>),
        /// skipping any blip present in <paramref name="exclude"/>.
        /// </summary>
        public static void SetMostBlipsInvisible(List<Blip> exclude)
        {
            var excludeSet = exclude != null && exclude.Count > 0
                ? new HashSet<Blip>(exclude)
                : null;

            foreach (Blip blip in World.GetAllBlips())
            {
                if (blip == null) continue;
                if (IsBlipGameplayBlip(blip)) continue;
                if (IsSpecificBlip_0(blip)) continue;
                if (excludeSet != null && excludeSet.Contains(blip)) continue;

                blip.DisplayType = BlipDisplayType.NoDisplay;
            }
        }

        /// <summary>
        /// Hides all non-gameplay blips, skipping any blip present in <paramref name="exclude"/>.
        /// Unlike <see cref="SetMostBlipsInvisible"/>, this also hides blips in
        /// <see cref="SpecificBlipSprites_0"/>.
        /// </summary>
        public static void SetAllBlipsInvisible(List<Blip> exclude)
        {
            var excludeSet = exclude != null && exclude.Count > 0
                ? new HashSet<Blip>(exclude)
                : null;

            foreach (Blip blip in World.GetAllBlips())
            {
                if (blip == null) continue;
                if (IsBlipGameplayBlip(blip)) continue;
                if (excludeSet != null && excludeSet.Contains(blip)) continue;

                blip.DisplayType = BlipDisplayType.NoDisplay;
            }
        }

        /// <summary>
        /// Restores all blips to <see cref="BlipDisplayType.Default"/>,
        /// skipping any blip present in <paramref name="exclude"/>.
        /// </summary>
        public static void SetAllBlipsVisible(List<Blip> exclude)
        {
            var excludeSet = exclude != null && exclude.Count > 0
                ? new HashSet<Blip>(exclude)
                : null;

            foreach (Blip blip in World.GetAllBlips())
            {
                if (blip == null) continue;
                if (excludeSet != null && excludeSet.Contains(blip)) continue;

                blip.DisplayType = BlipDisplayType.Default;
            }
        }

        /// <summary>Restores every blip to <see cref="BlipDisplayType.Default"/> with no exclusions.</summary>
        public static void SetAllBlipsVisible()
        {
            foreach (Blip blip in World.GetAllBlips())
            {
                if (blip != null)
                    blip.DisplayType = BlipDisplayType.Default;
            }
        }

        // ── Long-range tracking ───────────────────────────────────────────────

        /// <summary>
        /// Returns a snapshot of all currently long-range blips.
        /// Call this at mission start — not at class-load time — so the world is ready.
        /// </summary>
        public static List<Blip> GetLongRangeBlips()
        {
            var list = new List<Blip>();
            foreach (Blip blip in World.GetAllBlips())
            {
                if (blip != null && !blip.IsShortRange)
                    list.Add(blip);
            }
            return list;
        }

        /// <summary>
        /// Temporarily forces all blips in <paramref name="longRangeBlips"/> to behave as
        /// short-range (or reverts them). Use with a snapshot from <see cref="GetLongRangeBlips"/>.
        /// </summary>
        public static void ToggleShortRangeForLongRangeBlips(List<Blip> longRangeBlips, bool isShortRange)
        {
            for (int i = 0; i < longRangeBlips.Count; i++)
                longRangeBlips[i].IsShortRange = isShortRange;
        }

        // ── Creation ──────────────────────────────────────────────────────────

        /// <summary>Creates a blip at <paramref name="pos"/> using a <see cref="BlipPreset"/>.</summary>
        public static Blip CreateBlipPreset(Vector3 pos, BlipPreset preset)
            => CreateBlipForCoordWithParams(
                pos, preset.Sprite, preset.Color,
                preset.Scale, preset.Name, preset.IsShortRange);

        /// <summary>Creates a coordinate blip and applies common display parameters.</summary>
        public static Blip CreateBlipForCoordWithParams(
            Vector3 pos, BlipSprite sprite, BlipColor color,
            float scale, string name, bool isShortRange = false, int alpha = 255)
        {
            Blip blip = AddBlipForCoord(pos);
            if (blip == null) return null;

            blip.Alpha = alpha;
            blip.Sprite = sprite;
            blip.Color = color;
            blip.Scale = scale;
            blip.Name = name;
            blip.IsShortRange = isShortRange;
            return blip;
        }

        /// <summary>Creates a coordinate blip with independent X/Y scale values.</summary>
        public static Blip CreateBlipForCoordWithParams(
            Vector3 pos, BlipSprite sprite, BlipColor color,
            float scaleX, float scaleY, string name, int alpha = 255)
        {
            Blip blip = AddBlipForCoord(pos);
            if (blip == null) return null;

            blip.Alpha = alpha;
            blip.Sprite = sprite;
            blip.Color = color;
            blip.ScaleX = scaleX;
            blip.ScaleY = scaleY;
            blip.Name = name;
            return blip;
        }

        /// <summary>Creates a radius (circle) blip centred on <paramref name="pos"/>.</summary>
        public static Blip CreateBlipForRadiusWithParams(Vector3 pos, float radius, BlipColor color)
        {
            Blip blip = AddBlipForRadius(pos, radius);
            if (blip == null) return null;

            blip.Color = color;
            return blip;
        }

        /// <summary>
        /// Creates a GPS-routed mission blip at <paramref name="pos"/> and immediately
        /// starts a multi-route line from the player to it.
        /// </summary>
        public static Blip CreateGPSBlip(Vector3 pos, int spriteID, BlipColor color)
        {
            Blip blip = World.CreateBlip(pos);

            if (blip != null && !blip.Exists())
                Script.Yield();

            blip.Sprite = (BlipSprite)spriteID;
            blip.Color = color;
            SetBlipGPS(blip, 156, drawFromPlayer: true, displayOnFoot: false);
            return blip;
        }

        // ── Deletion ──────────────────────────────────────────────────────────

        /// <summary>Deletes every blip in <paramref name="blipList"/> and clears the list.</summary>
        public static void DeleteListBlips(List<Blip> blipList)
        {
            for (int i = 0; i < blipList.Count; i++)
            {
                if (blipList[i] != null)
                    blipList[i].Delete();
            }
            blipList.Clear();
        }

        // ── GPS route ────────────────────────────────────────────────────────

        /// <summary>
        /// Clears any existing GPS route and starts a new multi-route line to <paramref name="blip"/>.
        /// </summary>
        public static void SetBlipGPS(Blip blip, int color, bool drawFromPlayer, bool displayOnFoot)
        {
            if (blip == null) return;

            Function.Call(Hash.CLEAR_GPS_MULTI_ROUTE);
            Function.Call(Hash.START_GPS_MULTI_ROUTE, color, drawFromPlayer, displayOnFoot);
            Function.Call(Hash.SET_GPS_MULTI_ROUTE_RENDER, true);
            Function.Call(Hash.ADD_POINT_TO_GPS_MULTI_ROUTE,
                blip.Position.X, blip.Position.Y, blip.Position.Z);
        }

        /// <summary>Shows or hides the GPS multi-route line without clearing it.</summary>
        public static void SetGPSMultiRouteRender(bool toggle)
            => Function.Call(Hash.SET_GPS_MULTI_ROUTE_RENDER, toggle);

        /// <summary>Clears the active GPS multi-route.</summary>
        public static void ClearGPSMultiRoute()
            => Function.Call(Hash.CLEAR_GPS_MULTI_ROUTE);

        // ── Misc ─────────────────────────────────────────────────────────────

        /// <summary>Shows or hides the tick/checkmark overlay on <paramref name="blip"/>.</summary>
        public static void ShowTickOnBlip(Blip blip, bool toggle)
            => Function.Call(Hash.SHOW_TICK_ON_BLIP, blip, toggle);

        /// <summary>Sets the heading arrow rotation on <paramref name="blip"/> in degrees.</summary>
        public static void SetBlipRotation(Blip blip, int rotation)
            => Function.Call(Hash.SET_BLIP_ROTATION, blip, rotation);

        // ── Raw native wrappers ───────────────────────────────────────────────

        public static Blip AddBlipForCoord(Vector3 pos)
            => Function.Call<Blip>(Hash.ADD_BLIP_FOR_COORD, pos.X, pos.Y, pos.Z);

        public static Blip AddBlipForRadius(Vector3 pos, float radius)
            => Function.Call<Blip>(Hash.ADD_BLIP_FOR_RADIUS, pos.X, pos.Y, pos.Z, radius);

        public static Blip AddBlipForArea(Vector3 areaCenter, float width, float height)
            => Function.Call<Blip>(Hash.ADD_BLIP_FOR_AREA, areaCenter.X, areaCenter.Y, areaCenter.Z, width, height);
    }
}
