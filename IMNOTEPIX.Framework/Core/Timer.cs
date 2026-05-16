using GTA.Native;

namespace IMNOTEPIX.Framework.fCore
{
    public class Timer
    {
        public static int TimerA() => Function.Call<int>(Hash.TIMERA);
        public static int TimerB() => Function.Call<int>(Hash.TIMERB);
        public static void SetTimerA(int value) => Function.Call(Hash.SETTIMERA, value);
        public static void SetTimerB(int value) => Function.Call(Hash.SETTIMERB, value);
    }
}
