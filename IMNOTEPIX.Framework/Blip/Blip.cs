using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fBlip
{
    public class Blip
    {
        // Used to skip blips the game itself owns (waypoints, contacts, etc.)

        private static readonly HashSet<int> GameplayBlipSprites = new HashSet<int>
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            10, 11, 12, 13, 14, 15, 41, 42,
            162, /* Point of interest */
            185,
        };

        private static readonly HashSet<int> SpecificBlipSprites_0 = new HashSet<int> { 71, 72, 73, 110, 162, 313 };

        private static bool IsBlipGameplayBlip(GTA.Blip blip)
            => GameplayBlipSprites.Contains((int)blip.Sprite);

        private static bool IsSpecificBlip(GTA.Blip blip)
            => SpecificBlipSprites_0.Contains((int)blip.Sprite);

        /// <summary>
        /// Hides all non-gameplay blips (except <see cref="SpecificBlipSprites_0"/>),
        /// skipping any blip present in <paramref name="exclude"/>.
        /// </summary>
        public static void SetMostBlipsInvisible(List<GTA.Blip> exclude)
        {
            var excludeSet = exclude != null && exclude.Count > 0 ? new HashSet<GTA.Blip>(exclude) : null;

            foreach (GTA.Blip blip in GTA.World.GetAllBlips())
            {
                if (blip == null)
                    continue;
                if (IsBlipGameplayBlip(blip))
                    continue;
                if (IsSpecificBlip(blip))
                    continue;
                if (excludeSet != null && excludeSet.Contains(blip))
                    continue;

                blip.DisplayType = BlipDisplayType.NoDisplay;
            }
        }

        /// <summary>
        /// Hides all non-gameplay blips, skipping any blip present in <paramref name="exclude"/>.
        /// Unlike <see cref="SetMostBlipsInvisible"/>, this also hides blips in
        /// <see cref="SpecificBlipSprites_0"/>.
        /// </summary>
        public static void SetAllBlipsInvisible(List<GTA.Blip> exclude)
        {
            var excludeSet = exclude != null && exclude.Count > 0 ? new HashSet<GTA.Blip>(exclude) : null;

            foreach (GTA.Blip blip in GTA.World.GetAllBlips())
            {
                if (blip == null)
                    continue;
                if (IsBlipGameplayBlip(blip))
                    continue;
                if (excludeSet != null && excludeSet.Contains(blip))
                    continue;

                blip.DisplayType = BlipDisplayType.NoDisplay;
            }
        }

        /// <summary>
        /// Restores all blips to <see cref="BlipDisplayType.Default"/>,
        /// skipping any blip present in <paramref name="exclude"/>.
        /// </summary>
        public static void SetAllBlipsVisible(List<GTA.Blip> exclude)
        {
            var excludeSet = exclude != null && exclude.Count > 0 ? new HashSet<GTA.Blip>(exclude) : null;

            foreach (GTA.Blip blip in GTA.World.GetAllBlips())
            {
                if (blip == null)
                    continue;
                if (excludeSet != null && excludeSet.Contains(blip))
                    continue;

                blip.DisplayType = BlipDisplayType.Default;
            }
        }

        /// <summary>Restores every blip to <see cref="BlipDisplayType.Default"/> with no exclusions.</summary>
        public static void SetAllBlipsVisible()
        {
            foreach (GTA.Blip blip in GTA.World.GetAllBlips())
            {
                if (blip != null)
                    blip.DisplayType = BlipDisplayType.Default;
            }
        }

        /// <summary>
        /// Returns a snapshot of all currently long-range blips.
        /// Call this at mission start — not at class-load time — so the world is ready.
        /// </summary>
        public static List<GTA.Blip> GetLongRangeBlips()
        {
            var list = new List<GTA.Blip>();
            foreach (GTA.Blip blip in GTA.World.GetAllBlips())
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
        public static void ToggleShortRangeForLongRangeBlips(List<GTA.Blip> longRangeBlips, bool isShortRange)
        {
            for (int i = 0; i < longRangeBlips.Count; i++)
                longRangeBlips[i].IsShortRange = isShortRange;
        }


        /// <summary>Creates a blip at <paramref name="pos"/> using a <see cref="BlipPreset"/>.</summary>
        public static GTA.Blip CreateBlipPreset(Vector3 pos, BlipPreset preset)
            => CreateBlipForCoordWithParams(pos, preset.Sprite, preset.Color, preset.Scale, preset.Name, preset.IsShortRange);

        /// <summary>Creates a coordinate blip and applies common display parameters.</summary>
        public static GTA.Blip CreateBlipForCoordWithParams(Vector3 pos, BlipSprite sprite, BlipColor color, float scale, string name, bool isShortRange = false, int alpha = 255)
        {
            GTA.Blip blip = AddBlipForCoord(pos);
            while (blip == null)
                GTA.Script.Wait(0);

            blip.Alpha = alpha;
            blip.Sprite = sprite;
            blip.Color = color;
            blip.Scale = scale;
            blip.Name = name;
            blip.IsShortRange = isShortRange;
            return blip;
        }

        /// <summary>Creates a coordinate blip with independent X/Y scale values.</summary>
        public static GTA.Blip CreateBlipForCoordWithParams(Vector3 pos, BlipSprite sprite, BlipColor color, float scaleX, float scaleY, string name, bool isShortRange = false, int alpha = 255)
        {
            GTA.Blip blip = AddBlipForCoord(pos);
            while (blip == null)
                GTA.Script.Wait(0);

            blip.Alpha = alpha;
            blip.Sprite = sprite;
            blip.Color = color;
            blip.ScaleX = scaleX;
            blip.ScaleY = scaleY;
            blip.Name = name;
            return blip;
        }

        /// <summary>Creates a radius (circle) blip centred on <paramref name="pos"/>.</summary>
        public static GTA.Blip CreateBlipForRadiusWithParams(Vector3 pos, float radius, BlipColor color)
        {
            GTA.Blip blip = AddBlipForRadius(pos, radius);
            while (blip == null)
                GTA.Script.Wait(0);

            blip.Color = color;
            return blip;
        }

        /// <summary>
        /// Creates a GPS-routed mission blip at <paramref name="pos"/> and immediately
        /// starts a multi-route line from the player to it.
        /// </summary>
        public static GTA.Blip CreateGPSBlip(Vector3 pos, int spriteID, BlipColor color)
        {
            GTA.Blip blip = GTA.World.CreateBlip(pos);

            if (blip != null && !blip.Exists())
                GTA.Script.Wait(0);

            blip.Sprite = (BlipSprite)spriteID;
            blip.Color = color;
            SetBlipGPS(blip, 156, drawFromPlayer: true, displayOnFoot: false);
            return blip;
        }

        /// <summary>
        /// <inheritdoc cref="CreateGPSBlip(Vector3, int, BlipColor)"/>
        /// </summary>
        public static GTA.Blip CreateGPSBlip(Vector3 pos, BlipPreset preset)
        {
            GTA.Blip blip = GTA.World.CreateBlip(pos);

            if (blip != null && !blip.Exists())
                GTA.Script.Wait(0);

            blip.Sprite = preset.Sprite;
            blip.Color = preset.Color;
            blip.Name = preset.Name;
            blip.Scale = preset.Scale;
            SetBlipGPS(blip, 156, drawFromPlayer: true, displayOnFoot: false);
            return blip;
        }

        /// <summary>Deletes every blip in <paramref name="blipList"/> and clears the list.</summary>
        public static void DeleteListBlips(List<GTA.Blip> blipList)
        {
            for (int i = 0; i < blipList.Count; i++)
            {
                if (blipList[i] != null)
                    blipList[i].Delete();
            }
            blipList.Clear();
        }

        /// <summary>
        /// Clears any existing GPS route and starts a new multi-route line to <paramref name="blip"/>.
        /// </summary>
        public static void SetBlipGPS(GTA.Blip blip, int color, bool drawFromPlayer, bool displayOnFoot)
        {
            if (blip == null)
                return;

            Function.Call(Hash.CLEAR_GPS_MULTI_ROUTE);
            blip.ShowRoute = true;
            /*
            Function.Call(Hash.START_GPS_MULTI_ROUTE, color, drawFromPlayer, displayOnFoot);
            Function.Call(Hash.SET_GPS_MULTI_ROUTE_RENDER, true);
            Function.Call(Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, blip.Position.X, blip.Position.Y, blip.Position.Z);*/
        }

        /// <summary>Shows or hides the GPS multi-route line without clearing it.</summary>
        public static void SetGPSMultiRouteRender(bool toggle)
            => Function.Call(Hash.SET_GPS_MULTI_ROUTE_RENDER, toggle);

        /// <summary>Clears the active GPS multi-route.</summary>
        public static void ClearGPSMultiRoute()
            => Function.Call(Hash.CLEAR_GPS_MULTI_ROUTE);

        /// <summary>Shows or hides the tick/checkmark overlay on <paramref name="blip"/>.</summary>
        public static void ShowTickOnBlip(GTA.Blip blip, bool toggle)
            => Function.Call(Hash.SHOW_TICK_ON_BLIP, blip, toggle);

        /// <summary>Sets the heading arrow rotation on <paramref name="blip"/> in degrees.</summary>
        public static void SetBlipRotation(GTA.Blip blip, int rotation)
            => Function.Call(Hash.SET_BLIP_ROTATION, blip, rotation);

        public static GTA.Blip AddBlipForCoord(Vector3 pos)
            => Function.Call<GTA.Blip>(Hash.ADD_BLIP_FOR_COORD, pos.X, pos.Y, pos.Z);

        public static GTA.Blip AddBlipForRadius(Vector3 pos, float radius)
            => Function.Call<GTA.Blip>(Hash.ADD_BLIP_FOR_RADIUS, pos.X, pos.Y, pos.Z, radius);

        public static GTA.Blip AddBlipForArea(Vector3 areaCenter, float width, float height)
            => Function.Call<GTA.Blip>(Hash.ADD_BLIP_FOR_AREA, areaCenter.X, areaCenter.Y, areaCenter.Z, width, height);

        public static bool DoesBlipExist(GTA.Blip blip)
            => Function.Call<bool>(Hash.DOES_BLIP_EXIST, blip);

        public static void SetBlipScale(GTA.Blip blip, float scale)
            => Function.Call(Hash.SET_BLIP_SCALE, blip, scale);

        public static void SetBlipPriority(GTA.Blip blip, int priority)
            => Function.Call(Hash.SET_​BLIP_​PRIORITY, blip, priority);

        public static GTA.Blip AddBlipForEntity(GTA.Entity entity)
            => Function.Call<GTA.Blip>(Hash.ADD_​BLIP_​FOR_​ENTITY, entity);

        public static void SetBlipAsFriendly(GTA.Blip blip, bool toggle)
            => Function.Call(Hash.SET_​BLIP_​AS_​FRIENDLY, blip, toggle);

        public static void SetBlipColour(GTA.Blip blip, int color)
            => Function.Call(Hash.SET_​BLIP_​COLOUR, blip, color);
    }
}
