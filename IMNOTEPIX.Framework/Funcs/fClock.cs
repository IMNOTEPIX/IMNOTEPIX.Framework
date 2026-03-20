using System;
using GTA;
using GTA.Native;

namespace IMNOTEPIX.Framework.Funcs
{
    public static class fClock
    {
        // ── In-game clock reads ───────────────────────────────────────────────

        public static int Hours => Function.Call<int>(Hash.GET_CLOCK_HOURS);
        public static int Minutes => Function.Call<int>(Hash.GET_CLOCK_MINUTES);
        public static int Seconds => Function.Call<int>(Hash.GET_CLOCK_SECONDS);
        public static int DayOfWeek => Function.Call<int>(Hash.GET_CLOCK_DAY_OF_WEEK);
        public static int DayOfMonth => Function.Call<int>(Hash.GET_CLOCK_DAY_OF_MONTH);
        public static int Month => Function.Call<int>(Hash.GET_CLOCK_MONTH);
        public static int Year => Function.Call<int>(Hash.GET_CLOCK_YEAR);

        /// <summary>
        /// Returns the next day-of-week index (0 = Sunday … 6 = Saturday),
        /// wrapping from Saturday back to Sunday.
        /// </summary>
        public static int NextDayOfWeek()
        {
            int day = DayOfWeek;
            return day == 6 ? 0 : day + 1;
        }

        // ── Clock control ─────────────────────────────────────────────────────

        /// <summary>Pauses or unpauses the in-game clock.</summary>
        public static void PauseClock(bool toggle)
            => Function.Call(Hash.PAUSE_CLOCK, toggle);

        /// <summary>Instantly sets the in-game clock to the given time.</summary>
        public static void SetClockTime(int hour = 0, int minute = 0, int second = 0)
            => Function.Call(Hash.SET_CLOCK_TIME, hour, minute, second);

        /// <summary>
        /// Advances the clock forward in real-time by repeatedly adding
        /// <paramref name="stepMinutes"/> (or <paramref name="stepSeconds"/>) per frame
        /// until both the target hour and minute are reached, then snaps to the exact target.
        /// </summary>
        /// <param name="easeByMinutes">
        /// If <c>true</c>, steps in minutes; otherwise steps in seconds.
        /// </param>
        public static void EaseClockTimeTo(
            int hour, int minute, int second = 0,
            bool easeByMinutes = true,
            int stepMinutes = 1, int stepSeconds = 1)
        {
            while (Hours != hour || Minutes != minute)
            {
                if (easeByMinutes)
                    AddToClockTime(0, stepMinutes, 0);
                else
                    AddToClockTime(0, 0, stepSeconds);

                Script.Yield();
            }

            // Snap to the exact target once the hour and minute align.
            SetClockTime(hour, minute, second);
        }

        /// <summary>Advances the clock to the given time, skipping any frames in between.</summary>
        public static void AdvanceClockTimeTo(int hour = 0, int minute = 0, int second = 0)
            => Function.Call(Hash.ADVANCE_CLOCK_TIME_TO, hour, minute, second);

        /// <summary>Adds the given offset to the current in-game clock.</summary>
        public static void AddToClockTime(int hours = 0, int minutes = 0, int seconds = 0)
            => Function.Call(Hash.ADD_TO_CLOCK_TIME, hours, minutes, seconds);

        /// <summary>Sets the in-game calendar date.</summary>
        public static void SetClockDate(int day, int month, int year)
            => Function.Call(Hash.SET_CLOCK_DATE, day, month, year);

        /// <summary>Returns how many real milliseconds elapse per in-game minute.</summary>
        public static int GetMillisecondsPerGameMinute()
            => Function.Call<int>(Hash.GET_MILLISECONDS_PER_GAME_MINUTE);

        // ── System / real-world time ──────────────────────────────────────────

        /// <summary>Returns the system POSIX time as a <see cref="DateTime"/>.</summary>
        public static unsafe DateTime GetPOSIXTime()
        {
            int year, month, day, hour, minute, second;
            Function.Call(Hash.GET_POSIX_TIME, &year, &month, &day, &hour, &minute, &second);
            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
        }

        /// <summary>Returns the current UTC time as a <see cref="DateTime"/>.</summary>
        public static unsafe DateTime GetUTCTime()
        {
            int year, month, day, hour, minute, second;
            Function.Call(Hash.GET_UTC_TIME, &year, &month, &day, &hour, &minute, &second);
            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        }

        /// <summary>Returns the local system time as a <see cref="DateTime"/>.</summary>
        public static unsafe DateTime GetLocalTime()
        {
            int year, month, day, hour, minute, second;
            Function.Call(Hash.GET_LOCAL_TIME, &year, &month, &day, &hour, &minute, &second);
            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
        }
    }
}