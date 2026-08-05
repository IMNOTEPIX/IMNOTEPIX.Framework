using GTA;
using GTA.Native;
using IMNOTEPIX.Framework.fInterfaces;
using IMNOTEPIX.Framework.fPlayer;
using System;

namespace IMNOTEPIX.Framework.fGameplay.Minigames
{
    /// <summary>Event args for when loot value is added in a minigame.</summary>
    public class MinigameValueAddedArgs : EventArgs
    {
        public int Amount { get; }
        public bool IsCash { get; }

        public MinigameValueAddedArgs(int amount, bool isCash)
        {
            Amount = amount;
            IsCash = isCash;
        }
    }

    public delegate void MinigameValueAddedEventHandler(object sender, MinigameValueAddedArgs e);

    public abstract class CashGrabMinigame
    {
        private const float SmoothSpeed = 6f;
        private const float EasyIncrease = 0.10f;
        private const float MediumIncrease = 0.09f;
        private const float HardIncrease = 0.08f;
        private const float EasyDecay = 0.30f;
        private const float MediumDecay = 0.375f;
        private const float HardDecay = 0.405f;

        public virtual float MinRate { get; set; } = 0.65f;
        public virtual float MaxRate { get; set; } = 1.5f;

        public int State { get; set; }
        public bool IsLooted { get; set; }
        public bool CasinoTableCam { get; set; }
        public bool UseRemoteCounterSound { get; set; } = true;

        public bool IsMinigameActive
        {
            get => Function.Call<bool>(Hash.IS_MINIGAME_IN_PROGRESS);
            set => Function.Call(Hash.SET_MINIGAME_IN_PROGRESS, value);
        }

        public bool IsScriptActive { get; set; }
        public PlayerBag.BagVariantTypes PrevBagType { get; set; }
        public WeaponHash PrevWeapon { get; set; }
        public AnimationRateData Data;

        public struct AnimationRateData
        {
            public int DifficultyLevel;
            public float CurrentRate;
            public float TargetRate;
            public float CurrentPhase;
        }

        public void IsActive(bool toggle)
        {
            IsMinigameActive = toggle;
            IsScriptActive = toggle;
        }

        private void UpdateAnimationRate(ref AnimationRateData data, float minRate, float maxRate)
        {
            if (Function.Call<bool>(Hash.IS_PAUSE_MENU_ACTIVE))
                return;

            float deltaTime = Function.Call<float>(Hash.TIMESTEP);
            bool inputPressed = IsGrabInputPressed();

            if (inputPressed)
            {
                data.TargetRate += GetRateIncrease(data.DifficultyLevel);
                data.TargetRate = fCore.Math.Clamp(data.TargetRate, minRate, maxRate);
            }
            else if (data.TargetRate > minRate)
            {
                data.TargetRate -= GetRateDecay(data.DifficultyLevel) * deltaTime;
                if (data.TargetRate < minRate)
                    data.TargetRate = minRate;
            }

            data.CurrentRate = SmoothTowardTarget(data.CurrentRate, data.TargetRate, deltaTime);
        }

        private static bool IsGrabInputPressed()
        {
            bool isKeyboardMouse = Function.Call<bool>(Hash.IS_USING_KEYBOARD_AND_MOUSE, 2);
            int control = isKeyboardMouse ? 237 : 201;
            return Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 2, control);
        }

        private static float GetRateIncrease(int difficulty)
            => difficulty switch { 0 => EasyIncrease, 1 => MediumIncrease, 2 => HardIncrease, _ => EasyIncrease };

        private static float GetRateDecay(int difficulty)
            => difficulty switch { 0 => EasyDecay, 1 => MediumDecay, 2 => HardDecay, _ => EasyDecay };

        private static float SmoothTowardTarget(float current, float target, float deltaTime)
        {
            float step = SmoothSpeed * deltaTime;
            if (System.Math.Abs(current - target) <= step)
                return target;
            return current < target ? current + step : current - step;
        }

        /// <summary>
        /// Resets runtime state. MinRate and MaxRate are preserved —
        /// they are configuration, not per-run state.
        /// </summary>
        public void ResetValues()
        {
            Data = default;
            State = 0;
            IsLooted = false;
            PrevBagType = PlayerBag.BagVariantTypes.Invalid;
            PrevWeapon = default;
            IsActive(false);
        }

        public virtual void Update()
        {
            if (CasinoTableCam)
                Function.Call(Hash.SET_TABLE_GAMES_CAMERA_THIS_UPDATE, fWorld.Misc.joaat("CASINO_BLACKJACK_CAMERA"));

            if (fPlayer.Player.Character.IsDead && IsScriptActive)
                PushResetOnDeath();

            UpdateAnimationRate(ref Data, MinRate, MaxRate);
        }

        public virtual void Dispose() => ResetValues();

        public virtual void PushResetOnDeath()
        {
            State = 0;
            IsActive(false);
            fPlayer.Player.Character.CanSwitchWeapons = true;
            Game.Player.SetControlState(true, SetPlayerControlFlags.None);
            fPlayer.Player.Character.Task.ClearAllImmediately();
            PlayerBag.ApplyBag(fPlayer.Player.Character, PrevBagType);
        }
    }
}
