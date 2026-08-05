using GTA;
using GTA.Native;
using GTA.UI;

namespace IMNOTEPIX.Framework.fUI
{
    public static class BusySpinner
    {
        public static bool IsActive => Function.Call<bool>(Hash.BUSYSPINNER_IS_ON);

        public static void Show(string gxtEntry = null, LoadingSpinnerType busySpinnerType = LoadingSpinnerType.RegularClockwise)
        {
            Hide();
            if (string.IsNullOrEmpty(gxtEntry))
                Function.Call(Hash.BEGIN_TEXT_COMMAND_BUSYSPINNER_ON, string.Empty);
            else
                Function.Call(Hash.BEGIN_TEXT_COMMAND_BUSYSPINNER_ON, gxtEntry);

            Function.Call(Hash.END_TEXT_COMMAND_BUSYSPINNER_ON, (int)busySpinnerType);
        }

        public static void Hide()
        {
            if (IsActive)
                Function.Call(Hash.BUSYSPINNER_OFF);
        }
    }
}