using IMNOTEPIX.Framework.fUI.TimerBars;

namespace IMNOTEPIX.Framework.fExtension
{
    public static class CashGrabExtensions
    {
        public static int AmountAdded = 0;

        /// <summary>
        /// Connects a cart to a HUD value bar so that every ValueAdded event
        /// increments the bar automatically.
        /// </summary>
        public static fGameplay.Minigames.CartGrab ConnectToValueBar(this fGameplay.Minigames.CartGrab cart, TextTimerBar bar)
        {
            cart.ValueAdded += (sender, e) => bar.Text = $"${AmountAdded += e.Amount:N0}";
            return cart;
        }

        /// <summary>
        /// Connects a stack to a HUD value bar so that every ValueAdded event
        /// increments the bar automatically.
        /// </summary>
        public static fGameplay.Minigames.StackGrab ConnectToValueBar(this fGameplay.Minigames.StackGrab stack, TextTimerBar bar)
        {
            stack.ValueAdded += (sender, e) => bar.Text = $"${AmountAdded += e.Amount:N0}";
            return stack;
        }

        /// <summary>
        /// Connects a washing machine to a HUD value bar so that every ValueAdded event
        /// increments the bar automatically.
        /// </summary>
        public static fGameplay.Minigames.WashingMachineGrab ConnectToValueBar(this fGameplay.Minigames.WashingMachineGrab mach, TextTimerBar bar)
        {
            mach.ValueAdded += (sender, e) => bar.Text = $"${AmountAdded += e.Amount:N0}";
            return mach;
        }
    }
}
