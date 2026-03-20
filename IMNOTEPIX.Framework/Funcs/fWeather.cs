using GTA.Native;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.Funcs
{
    public static class fWeather
    {
        // ─────────────────────────────────────────────────────────────────────
        // WeatherTypes enum
        //
        // Renamed to PascalCase for C# conventions. The game-facing string names
        // are stored in _weatherStrings below — enum values are not used as
        // string keys directly.
        // ─────────────────────────────────────────────────────────────────────

        public enum WeatherTypes
        {
            ExtraSunny,
            Clear,
            Clouds,
            Smog,
            Foggy,
            Overcast,
            Rain,
            Thunder,
            Clearing,
            Neutral,
            Snow,
            Blizzard,
            SnowLight,
            Halloween,
            SnowHalloween,
            RainHalloween,
        }

        // ─────────────────────────────────────────────────────────────────────
        // Lookup tables
        //
        // Replaces two parallel public mutable arrays.
        // _weatherStrings: enum → game string name (used by SET_* natives)
        // _hashToWeather:  hash → enum (used by GET_*_HASH_NAME reverse lookup)
        //
        // Hash values are the joaat hashes of the string names.
        // ─────────────────────────────────────────────────────────────────────

        private static readonly Dictionary<WeatherTypes, string> _weatherStrings
            = new Dictionary<WeatherTypes, string>
        {
            { WeatherTypes.ExtraSunny,    "EXTRASUNNY"     },
            { WeatherTypes.Clear,         "CLEAR"          },
            { WeatherTypes.Clouds,        "CLOUDS"         },
            { WeatherTypes.Smog,          "smog"           },
            { WeatherTypes.Foggy,         "foggy"          },
            { WeatherTypes.Overcast,      "OVERCAST"       },
            { WeatherTypes.Rain,          "Rain"           },
            { WeatherTypes.Thunder,       "THUNDER"        },
            { WeatherTypes.Clearing,      "Clearing"       },
            { WeatherTypes.Neutral,       "NEUTRAL"        },
            { WeatherTypes.Snow,          "Snow"           },
            { WeatherTypes.Blizzard,      "BLIZZARD"       },
            { WeatherTypes.SnowLight,     "SNOWLIGHT"      },
            { WeatherTypes.Halloween,     "Halloween"      },
            { WeatherTypes.SnowHalloween, "SNOW_HALLOWEEN" },
            { WeatherTypes.RainHalloween, "RAIN_HALLOWEEN" },
        };

        private static readonly Dictionary<uint, WeatherTypes> _hashToWeather
            = new Dictionary<uint, WeatherTypes>
        {
            { 2544503417U, WeatherTypes.ExtraSunny    },
            { 916995460U,  WeatherTypes.Clear         },
            { 821931868U,  WeatherTypes.Clouds        },
            { 282916021U,  WeatherTypes.Smog          },
            { 2926802500U, WeatherTypes.Foggy         },
            { 3146353965U, WeatherTypes.Overcast      },
            { 1420204096U, WeatherTypes.Rain          },
            { 3061285535U, WeatherTypes.Thunder       },
            { 1840358669U, WeatherTypes.Clearing      },
            { 2764706598U, WeatherTypes.Neutral       },
            { 4021743606U, WeatherTypes.Snow          },
            { 669657108U,  WeatherTypes.Blizzard      },
            { 603685163U,  WeatherTypes.SnowLight     },
            { 3373937154U, WeatherTypes.Halloween     },
            { 2144126041U, WeatherTypes.SnowHalloween },
            { 385726482U,  WeatherTypes.RainHalloween },
        };

        // ─────────────────────────────────────────────────────────────────────
        // Current / next weather queries
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the current weather string name, or null if the hash is unrecognised.
        /// </summary>
        public static string GetCurrentWeatherStringName()
        {
            var type = GetCurrentWeatherEnum();
            return _weatherStrings.TryGetValue(type, out var s) ? s : null;
        }

        /// <summary>
        /// Returns the next weather string name, or null if the hash is unrecognised.
        /// </summary>
        public static string GetNextWeatherStringName()
        {
            var type = GetNextWeatherEnum();
            return _weatherStrings.TryGetValue(type, out var s) ? s : null;
        }

        /// <summary>
        /// Returns the current weather as a WeatherTypes enum value.
        /// Falls back to ExtraSunny if the hash is not in the lookup table —
        /// this matches the game's own fallback behaviour.
        /// </summary>
        public static WeatherTypes GetCurrentWeatherEnum()
        {
            uint hash = (uint)Function.Call<Hash>(Hash.GET_PREV_WEATHER_TYPE_HASH_NAME);
            return _hashToWeather.TryGetValue(hash, out var type) ? type : WeatherTypes.ExtraSunny;
        }

        /// <summary>
        /// Returns the next weather as a WeatherTypes enum value.
        /// Falls back to ExtraSunny if the hash is not in the lookup table.
        /// </summary>
        public static WeatherTypes GetNextWeatherEnum()
        {
            uint hash = (uint)Function.Call<Hash>(Hash.GET_NEXT_WEATHER_TYPE_HASH_NAME);
            return _hashToWeather.TryGetValue(hash, out var type) ? type : WeatherTypes.ExtraSunny;
        }

        public static Hash GetCurrentWeatherTypeHash()
            => Function.Call<Hash>(Hash.GET_PREV_WEATHER_TYPE_HASH_NAME);

        public static Hash GetNextWeatherTypeHash()
            => Function.Call<Hash>(Hash.GET_NEXT_WEATHER_TYPE_HASH_NAME);

        public static bool IsCurrentWeatherType(WeatherTypes weatherType)
            => Function.Call<bool>(Hash.IS_PREV_WEATHER_TYPE, ToGameString(weatherType));

        public static bool IsNextWeatherType(WeatherTypes weatherType)
            => Function.Call<bool>(Hash.IS_NEXT_WEATHER_TYPE, ToGameString(weatherType));

        // ─────────────────────────────────────────────────────────────────────
        // Set weather
        // ─────────────────────────────────────────────────────────────────────

        public static void SetWeatherTypePersist(WeatherTypes weatherType)
            => Function.Call(Hash.SET_WEATHER_TYPE_PERSIST, ToGameString(weatherType));

        public static void SetWeatherTypeNowPersist(WeatherTypes weatherType)
            => Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, ToGameString(weatherType));

        public static void SetWeatherTypeNow(WeatherTypes weatherType)
            => Function.Call(Hash.SET_WEATHER_TYPE_NOW, ToGameString(weatherType));

        public static void SetWeatherTypeOvertimePersist(WeatherTypes weatherType, float time)
            => Function.Call(Hash.SET_WEATHER_TYPE_OVERTIME_PERSIST, ToGameString(weatherType), time);

        public static void SetRandomWeatherType()
            => Function.Call(Hash.SET_RANDOM_WEATHER_TYPE);

        public static void ClearWeatherTypePersist()
            => Function.Call(Hash.CLEAR_WEATHER_TYPE_PERSIST);

        public static void SetOverrideWeather(WeatherTypes weatherType)
            => Function.Call(Hash.SET_OVERRIDE_WEATHER, ToGameString(weatherType));

        /// <summary>
        /// Identical to SET_OVERRIDE_WEATHER but has an additional bool param
        /// that sets some weather var to 0 if true.
        /// </summary>
        public static void SetOverrideWeatherEx(WeatherTypes weatherType, bool p1)
            => Function.Call(Hash.SET_OVERRIDE_WEATHEREX, ToGameString(weatherType), p1);

        public static void ClearOverrideWeather()
            => Function.Call(Hash.CLEAR_OVERRIDE_WEATHER);

        // ─────────────────────────────────────────────────────────────────────
        // Misc
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>https://nativedb.dotindustries.dev/gta5/natives/0x643E26EA6E024D92</summary>
        public static void SetRain(float intensity)
            => Function.Call(Hash.SET_RAIN, intensity);

        public static unsafe void GetCurrWeatherState(
            Hash* weatherType1, Hash* weatherType2, float* percentWeather2)
        {
            Function.Call(Hash.GET_CURR_WEATHER_STATE,
                weatherType1, weatherType2, percentWeather2);
        }

        /// <summary>https://nativedb.dotindustries.dev/gta5/natives/0x578C752848ECFA0C</summary>
        public static void SetCurrWeatherState(
            Hash weatherType1, Hash weatherType2, float percentWeather2)
        {
            Function.Call(Hash.SET_CURR_WEATHER_STATE,
                weatherType1, weatherType2, percentWeather2);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────

        private static string ToGameString(WeatherTypes weatherType)
            => _weatherStrings.TryGetValue(weatherType, out var s) ? s : null;
    }
}
