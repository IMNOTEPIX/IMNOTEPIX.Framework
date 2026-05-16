using GTA;
using GTA.UI;
using System;

namespace IMNOTEPIX.Framework.fUI.Scaleforms
{
    /// <summary>
    /// Handles big message scaleform displays using the MP_BIG_MESSAGE_FREEMODE scaleform.
    /// Call <see cref="Update"/> every tick from your script's Tick event.
    /// </summary>
    public class BigMessageHandler
    {
        private Scaleform _sc;
        private int _start;
        private int _timer;

        public BigMessageHandler() { }

        public BigMessageHandler(bool load)
        {
            if (load)
                Load(true);
        }

        /// <summary>
        /// Returns true if the scaleform is not yet created, or if it's already loaded.
        /// </summary>
        public bool IsLoaded => _sc == null || _sc.IsLoaded;

        public void Load(bool waitToLoad)
        {
            if (_sc != null) return;

            // In SHVDN, the Scaleform constructor takes the name and begins loading immediately.
            _sc = Scaleform.RequestMovieIgnoreSuperWidescreenAdjustment("MP_BIG_MESSAGE_FREEMODE");

            if (waitToLoad)
            {
                var timeout = 1000;
                var start = DateTime.Now;
                while (!_sc.IsLoaded && DateTime.Now.Subtract(start).TotalMilliseconds < timeout)
                    Script.Wait(0);
            }
        }

        public void Dispose()
        {
            _sc?.Dispose();
            _sc = null;
        }

        public void ShowMissionPassedMessage(string msg, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_MISSION_PASSED_MESSAGE", msg, "", 100, true, 0, true);
            _timer = time;
        }

        public void ShowColoredShard(string msg, string desc, HudColor textColor, HudColor bgColor, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_SHARD_CENTERED_MP_MESSAGE", msg, desc, (int)textColor, (int)bgColor);
            _timer = time;
        }

        public void ShowOldMessage(string msg, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_MISSION_PASSED_MESSAGE", msg);
            _timer = time;
        }

        public void ShowSimpleShard(string title, string subtitle, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_SHARD_CREW_RANKUP_MP_MESSAGE", title, subtitle);
            _timer = time;
        }

        public void ShowRankupMessage(string msg, string subtitle, int rank, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_BIG_MP_MESSAGE", msg, subtitle, rank, "", "");
            _timer = time;
        }

        public void ShowWeaponPurchasedMessage(string bigMessage, string weaponName, WeaponHash weapon, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_WEAPON_PURCHASED", bigMessage, weaponName, unchecked((int)weapon), "", 100);
            _timer = time;
        }

        public void ShowMpMessageLarge(string msg, int time = 5000) => ShowMpMessageLarge(msg, string.Empty, time);

        public void ShowMpMessageLarge(string msg, string subtitle, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_CENTERED_MP_MESSAGE_LARGE", msg, subtitle, 100, true, 100);
            _sc.CallFunction("TRANSITION_IN");
            _timer = time;
        }

        public void ShowCustomShard(string funcName, params object[] parameters)
        {
            Load(true);
            _sc.CallFunction(funcName, parameters);
        }

        /// <summary>
        /// Call this every tick from your script's Tick event.
        /// Renders the scaleform and handles the transition-out timer.
        /// </summary>
        public void Update()
        {
            if (_sc == null) return;

            _sc.Render2D();

            if (_start != 0 && Game.GameTime - _start > _timer)
            {
                _sc.CallFunction("TRANSITION_OUT");
                _start = 0;
            }
        }
    }

    // NOTE: BigMessageThread has been intentionally removed.
    // SHVDN does not use GameFibers. Instead, create a BigMessageHandler instance in your
    // Script subclass and call MessageInstance.Update() inside your Tick event handler.
    //
    // Example:
    //
    //   public class MyScript : Script
    //   {
    //       private BigMessageHandler _bigMessage = new BigMessageHandler();
    //
    //       public MyScript()
    //       {
    //           Tick += OnTick;
    //       }
    //
    //       private void OnTick(object sender, EventArgs e)
    //       {
    //           _bigMessage.Update();
    //       }
    //   }
}
