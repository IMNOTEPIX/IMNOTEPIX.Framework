using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fCore
{
    public class Math
    {
        /// <summary>
        /// Converts a speed from meters per second to kilometers per hour, rounded to the nearest whole number.
        /// </summary>
        /// <param name="metersPerSecond">The speed in meters per second (e.g. as returned by GTA.Vehicle.Speed).</param>
        /// <returns>The equivalent speed in kilometers per hour, rounded to the nearest integer.</returns>
        public static int ConvertMetersPerSecondToKilometersPerHourRounded(float metersPerSecond)
            => (int)System.Math.Round(metersPerSecond * 3.6f);

        public static Vector3 RotationToDirection(Vector3 rotation)
        {
            double pitch = rotation.X * System.Math.PI / 180.0;
            double yaw = rotation.Z * System.Math.PI / 180.0;

            return new Vector3(
                (float)(-System.Math.Sin(yaw) * System.Math.Abs(System.Math.Cos(pitch))),
                (float)(System.Math.Cos(yaw) * System.Math.Abs(System.Math.Cos(pitch))),
                (float)(System.Math.Sin(pitch))
            );
        }

        /// <summary>
        /// Clamps <paramref name="value"/> to [min, max].
        /// Note: .NET's Math.Clamp&lt;float&gt; does the same thing — prefer that
        /// for new code. This exists for callsites that predate .NET Standard 2.1.
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) 
                return min;
            if (value > max) 
                return max;
            return value;
        }

        /// <summary>
        /// Returns the ceiling of <paramref name="value"/> via the game's CEIL native.
        /// Equivalent to (int)Math.Ceiling(value) for most inputs.
        /// </summary>
        public static int Ceil(float value)
            => Function.Call<int>(Hash.CEIL, value);

        /// <summary>
        /// Converts a duration in minutes to whole milliseconds.
        /// </summary>
        public static int MinutesToMilliseconds(float minutes)
        {
            return (int)(minutes * 60f * 1000f);
        }

        /// <summary>
        /// Converts MM.SS notation (matching how the bomb display reads) to
        /// milliseconds. The whole part is minutes, the two digits after the
        /// decimal point are literal seconds - NOT a fraction of a minute.
        /// e.g. 3.30 -> 3 min 30 sec -> 210000ms (displays as "03:30").
        /// e.g. 1.51 -> 1 min 51 sec -> 111000ms (displays as "01:51").
        /// This is different from MinutesToMilliseconds, which would treat
        /// 3.30 as 3.3 minutes (198000ms) instead.
        /// Note: this doesn't validate that the seconds part is 00-59 - e.g.
        /// 1.75 is read as 1 min 75 sec and simply rolls over to 135 sec
        /// (2:15) rather than being rejected. Round-trip precision on the
        /// fractional part is handled via rounding to guard against float
        /// representation error (e.g. 3.30f not being stored as exactly
        /// 3.30), not verified against every possible input.
        /// </summary>
        public static int MinutesSecondsToMilliseconds(float minutesDotSeconds)
        {
            int minutes = (int)System.Math.Floor(minutesDotSeconds);
            int seconds = (int)System.Math.Round((minutesDotSeconds - minutes) * 100f);
            return (minutes * 60 + seconds) * 1000;
        }

        /// <summary>
        /// Given a desired real-world duration and a countdown speed
        /// multiplier (see BombCountdownDisplay.SpeedMultiplier), returns the
        /// starting countdown value in milliseconds so the display reaches
        /// zero after exactly that much real time has passed.
        /// e.g. RealDurationToCountdownMilliseconds(1.8f, 60f) == 108000
        /// (1:48 on the display), which ticks to zero in 1.8 real seconds
        /// at a 60x multiplier.
        /// </summary>
        public static int RealDurationToCountdownMilliseconds(float realSeconds, float speedMultiplier)
        {
            return (int)(realSeconds * 1000f * speedMultiplier);
        }
    }
}
