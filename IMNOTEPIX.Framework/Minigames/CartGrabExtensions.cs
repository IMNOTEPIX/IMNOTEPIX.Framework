using IMNOTEPIX.Framework.Drawables.TimerBars;

namespace IMNOTEPIX.Framework.Minigames
{
    public static class CartGrabExtensions
    {
        /// <summary>
        /// Connects a cart to a HUD value bar so that every ValueAdded event
        /// increments the bar automatically.
        /// </summary>
        public static CartGrab ConnectToValueBar(this CartGrab cart, HudValueBar bar)
        {
            cart.ValueAdded += (sender, e) => bar.Value += e.Amount;
            return cart;
        }
    }
}
