using GTA.Native;

namespace IMNOTEPIX.Framework.fPlayer
{
    public static class Input
    {
        public static bool IsUsingKeyboardAndMouse(int control)
            => Function.Call<bool>(Hash.IS_USING_KEYBOARD_AND_MOUSE, control);

        public static void SetControlShake(int control, int duration, int frequency)
            => Function.Call(Hash.SET_CONTROL_SHAKE, control, duration, frequency);
    }
}
