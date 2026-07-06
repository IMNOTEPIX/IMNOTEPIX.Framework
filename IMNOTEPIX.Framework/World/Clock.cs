using System;
using GTA.Native;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Clock
    {
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

        /// <summary>Pauses or unpauses the in-game clock.</summary>
        public static void PauseClock(bool toggle)
            => Function.Call(Hash.PAUSE_CLOCK, toggle);

        /// <summary>Instantly sets the in-game clock to the given time.</summary>
        public static void SetClockTime(int hour = 0, int minute = 0, int second = 0)
            => Function.Call(Hash.SET_CLOCK_TIME, hour, minute, second);

        /// <summary>
        /// Eases the in-game clock forward toward the given time over real-world time,
        /// then snaps to the exact target. Always moves forward, wrapping past midnight
        /// if the target is "behind" the current time.
        /// </summary>
        /// <param name="stepDelayMs">
        /// Real-world milliseconds to wait between each step. Raise this (or lower
        /// stepMinutes/stepSeconds) to slow the visible easing down.
        /// </param>
        public static void EaseClockTimeTo(int hour, int minute, int second = 0, bool easeByMinutes = true, int stepMinutes = 1, int stepSeconds = 1, int stepDelayMs = 15)
        {
            int currentTotalMinutes = Hours * 60 + Minutes;
            int targetTotalMinutes = hour * 60 + minute;

            int minutesToAdd = targetTotalMinutes - currentTotalMinutes;
            if (minutesToAdd < 0)
                minutesToAdd += 24 * 60;

            if (easeByMinutes)
            {
                int step = Math.Max(1, stepMinutes);
                int steps = minutesToAdd / step;

                for (int i = 0; i < steps; i++)
                {
                    AddToClockTime(0, step, 0);
                    GTA.Script.Wait(stepDelayMs);
                }
            }
            else
            {
                int step = Math.Max(1, stepSeconds);
                int steps = (minutesToAdd * 60) / step;

                for (int i = 0; i < steps; i++)
                {
                    AddToClockTime(0, 0, step);
                    GTA.Script.Wait(stepDelayMs);
                }
            }

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