using GTA;
using GTA.Math;
using GTA.Native;
using GTA.NaturalMotion;
using GTA.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using IMNOTEPIX.Framework.Core;
using IMNOTEPIX.Framework.Funcs;
using IMNOTEPIX.Framework.Interfaces;
using Control = GTA.Control;
using Screen = GTA.UI.Screen;
using System.Linq;

namespace IMNOTEPIX.Framework.Minigames
{
    public enum CartType
    {
        Standard,
        Cocaine,
        Cash_a, Cash_b, Cash_c,
        Gold_a, Gold_b, Gold_c,
        LowCash_a, LowCash_b, LowCash_c,
        Diamonds_a, Diamonds_b, Diamonds_c
    }

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

    // ─────────────────────────────────────────────────────────────────────────
    // Base class — animation rate management and shared minigame state
    // ─────────────────────────────────────────────────────────────────────────

    public abstract class CashGrabMinigame
    {
        private const float SmoothSpeed = 6f;
        private const float EasyIncrease = 0.10f;
        private const float MediumIncrease = 0.09f;
        private const float HardIncrease = 0.08f;
        private const float EasyDecay = 0.30f;
        private const float MediumDecay = 0.375f;
        private const float HardDecay = 0.405f;

        // MinRate and MaxRate are configuration values set per-instance/type.
        // They are intentionally NOT reset in ResetValues() — resetting them to
        // default(float) = 0f would break UpdateAnimationRate() and discard the
        // type-specific MaxRate set in CartGrab's constructor.
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
        public BagManager.BagVariantTypes PrevBagType { get; set; }
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
            if (Function.Call<bool>(Hash.IS_PAUSE_MENU_ACTIVE)) return;

            float deltaTime = Function.Call<float>(Hash.TIMESTEP);
            bool inputPressed = IsGrabInputPressed();

            if (inputPressed)
            {
                data.TargetRate += GetRateIncrease(data.DifficultyLevel);
                data.TargetRate = fMath.Clamp(data.TargetRate, minRate, maxRate);
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

        private static float GetRateIncrease(int difficulty) => difficulty switch
        {
            0 => EasyIncrease,
            1 => MediumIncrease,
            2 => HardIncrease,
            _ => EasyIncrease
        };

        private static float GetRateDecay(int difficulty) => difficulty switch
        {
            0 => EasyDecay,
            1 => MediumDecay,
            2 => HardDecay,
            _ => EasyDecay
        };

        private static float SmoothTowardTarget(float current, float target, float deltaTime)
        {
            float step = SmoothSpeed * deltaTime;
            if (Math.Abs(current - target) <= step) return target;
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
            PrevBagType = BagManager.BagVariantTypes.Invalid;
            PrevWeapon = default;
            IsActive(false);
        }

        public virtual void Update()
        {
            if (CasinoTableCam)
            {
                // SET_TABLE_GAMES_CAMERA_THIS_UPDATE sets the camera perspective
                // used while the player interacts with a casino-style table.
                Function.Call(Hash.SET_TABLE_GAMES_CAMERA_THIS_UPDATE,
                    fMisc.joaat("CASINO_BLACKJACK_CAMERA"));
            }

            if (fPlayer.Character.IsDead && IsScriptActive)
                PushResetOnDeath();

            UpdateAnimationRate(ref Data, MinRate, MaxRate);
        }

        public virtual void Dispose() => ResetValues();

        public virtual void PushResetOnDeath()
        {
            State = 0;
            IsActive(false);
            fPlayer.Character.CanSwitchWeapons = true;
            Game.Player.SetControlState(true, SetPlayerControlFlags.None);
            fPlayer.Character.Task.ClearAllImmediately();
            BagManager.ApplyBag(fPlayer.Character, PrevBagType);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CartGrab — loot cart interaction minigame
    // ─────────────────────────────────────────────────────────────────────────

    public class CartGrab : CashGrabMinigame, IMinigame
    {
        private const float InteractionDistance = 1.75f;
        private const float BagAppearPhase = 0.34f;
        private const float BagRestorePhase = 0.7f;
        private const float GoldMaxRate = 1.2802f;
        private const string AnimDict = "anim@heists@ornate_bank@grab_cash";
        private const string AudioScene = "DLC_HEIST_MINIGAME_PAC_CASH_GRAB_SCENE";

        // moveBlendRatio values of 1148846080f / 1148846100f passed to SynchronizedScene
        // methods appear to be type-confusion artifacts — as float literals these are
        // ~1.149e9, not standard blend ratios (0.0–8.0). Preserved to match Rockstar
        // script behaviour; verify against the original animation data before changing.
        private const float SceneBlendRatio = 1148846100f;

        private static readonly Dictionary<CartType, string> CartModels = new()
        {
            [CartType.Standard] = "hei_prop_hei_cash_trolly_01",
            [CartType.Cocaine] = "imp_prop_impexp_coke_trolly",
            [CartType.Cash_a] = "ch_prop_ch_cash_trolly_01a",
            [CartType.Cash_b] = "ch_prop_ch_cash_trolly_01b",
            [CartType.Cash_c] = "ch_prop_ch_cash_trolly_01c",
            [CartType.Gold_a] = "ch_prop_gold_trolly_01a",
            [CartType.Gold_b] = "ch_prop_gold_trolly_01b",
            [CartType.Gold_c] = "ch_prop_gold_trolly_01c",
            [CartType.LowCash_a] = "ch_prop_cash_low_trolly_01a",
            [CartType.LowCash_b] = "ch_prop_cash_low_trolly_01b",
            [CartType.LowCash_c] = "ch_prop_cash_low_trolly_01c",
            [CartType.Diamonds_a] = "ch_prop_diamond_trolly_01a",
            [CartType.Diamonds_b] = "ch_prop_diamond_trolly_01b",
            [CartType.Diamonds_c] = "ch_prop_diamond_trolly_01c",
        };

        // ─────────────────────────────────────────────────────────────────────
        // Static management
        // ─────────────────────────────────────────────────────────────────────

        private static readonly List<CartGrab> ActiveCarts = new List<CartGrab>().ToList();

        /// <summary>
        /// Ticks all registered carts. The cart prop null guard was removed —
        /// Update() → case 0 → UpdateIdleState() calls Create() internally,
        /// so Update must run even before the prop exists.
        /// </summary>
        public static void UpdateAll()
        {
            for (int i = ActiveCarts.Count - 1; i >= 0; i--)
            {
                // CartGrab instances are added via constructor (this) so null
                // entries are impossible — no null check needed on the instance.
                if (ActiveCarts[i].cart != null)
                {
                    ActiveCarts[i].Update();
                }
                if (ActiveCarts[i] == null)
                {
                    ActiveCarts.RemoveAt(i);
                }
            }
        }

        public static void DisposeAll()
        {
            for (int i = ActiveCarts.Count - 1; i >= 0; i--)
            {
                ActiveCarts[i]?.Dispose();
            }
            ActiveCarts.Clear();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Instance
        // ─────────────────────────────────────────────────────────────────────

        public CartType Type { get; init; }
        public Model cartModel { get; private init; }
        public Vector3 position { get; private init; }
        public Vector3 rotation { get; private init; }

        private bool wantsToExit;
        private SynchronizedScene? playerScene;
        private SynchronizedScene? cartScene;
        private Prop? cart;
        private Prop? bar;
        private Prop? bag;

        public event MinigameValueAddedEventHandler ValueAdded;

        public CartGrab(
            CartType type,
            Vector3 pos,
            bool putOnGround = false,
            Vector3 rot = default)
        {
            Type = type;
            rotation = rot;
            cartModel = new Model(fMisc.joaat(
                CartModels.TryGetValue(type, out var m) ? m : "hei_prop_hei_cash_trolly_01"));

            if (putOnGround)
            {
                // Fixed: was passing the uninitialized `position` field (Vector3.Zero)
                // instead of the `pos` parameter — ground height was sampled at world origin.
                World.GetGroundHeight(pos, out var height);
                position = new Vector3(pos.X, pos.Y, height);
            }
            else
            {
                position = pos;
            }

            MaxRate = IsGoldType(type) ? GoldMaxRate : 1.5f;

            ActiveCarts.Add(this);
        }

        private static bool IsGoldType(CartType type)
            => type is CartType.Gold_a or CartType.Gold_b or CartType.Gold_c;

        private int GetBarModel() => Type switch
        {
            CartType.Cocaine
                => fMisc.joaat("imp_prop_impexp_coke_pile"),
            CartType.Gold_a or CartType.Gold_b or CartType.Gold_c
                => fMisc.joaat("ch_prop_gold_bar_01a"),
            CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c
                => fMisc.joaat("ch_prop_vault_dimaondbox_01a"),
            _
                => fMisc.joaat("hei_prop_heist_cash_pile")
        };

        private void DisplayStartGrabText()
        {
            string helpText = Type switch
            {
                CartType.Cocaine => "MC_GRAB_6",
                CartType.Gold_a or CartType.Gold_b or CartType.Gold_c => "MC_GRAB_2",
                CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c => "MC_GRAB_7",
                _ => "MC_GRAB_1"
            };
            Function.Call(Hash.DISPLAY_HELP_TEXT_THIS_FRAME, helpText, true);
        }

        private void DisplayGrabText(bool forcedGrab)
        {
            bool isKbm = fGamePad.IsUsingKeyboardAndMouse(2);

            string helpText = (Type, forcedGrab, isKbm) switch
            {
                (CartType.Cocaine, true, true) => "MC_GRAB_5_MK_b",
                (CartType.Cocaine, true, false) => "MC_GRAB_5_b",
                (CartType.Cocaine, false, true) => "MC_GRAB_5_MK",
                (CartType.Cocaine, false, false) => "MC_GRAB_5",

                (CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c, true, true) => "MC_GRAB_7_MK_b",
                (CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c, true, false) => "MC_GRAB_7_TP_b",
                (CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c, false, true) => "MC_GRAB_7_MK",
                (CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c, false, false) => "MC_GRAB_7_TP",

                (CartType.Gold_a or CartType.Gold_b or CartType.Gold_c, true, true) => "MC_GRAB_4b_b",
                (CartType.Gold_a or CartType.Gold_b or CartType.Gold_c, true, false) => "MC_GRAB_4_b",
                (CartType.Gold_a or CartType.Gold_b or CartType.Gold_c, false, true) => "MC_GRAB_4b",
                (CartType.Gold_a or CartType.Gold_b or CartType.Gold_c, false, false) => "MC_GRAB_4",

                (_, true, true) => "MC_GRAB_3_MK_b",
                (_, true, false) => "MC_GRAB_3_b",
                (_, false, true) => "MC_GRAB_3_MK",
                _ => "MC_GRAB_3"
            };

            Function.Call(Hash.DISPLAY_HELP_TEXT_THIS_FRAME, helpText, true);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Scene management
        // ─────────────────────────────────────────────────────────────────────

        private void PlayIntroScene()
        {
            playerScene = new SynchronizedScene(cart!.Position, cart.Rotation);
            playerScene.Create(2000);
            playerScene.PlayPed(fPlayer.Character, AnimDict, "intro",
                1.5f, -8f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1.5f);
            playerScene.PlayEntity(bag!, AnimDict, "bag_intro",
                1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            cartScene = new SynchronizedScene(cart);
        }

        private void PlayGrabScene()
        {
            playerScene!.Create(2000);
            playerScene.PlayPed(fPlayer.Character, AnimDict, "grab",
                4f, -4f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1000f);
            playerScene.PlayEntity(bag!, AnimDict, "bag_grab",
                1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            playerScene.Phase = Data.CurrentPhase;

            cartScene!.Create(2000);
            cartScene.PlayEntity(cart!, AnimDict, "cart_cash_dissapear",
                1000f, -4f, SyncedSceneFlags.UseKinematicPhysics, SceneBlendRatio);
            cartScene.Phase = Data.CurrentPhase;
        }

        private void PlayIdleScene()
        {
            playerScene!.Create(2000);
            playerScene.PlayPed(fPlayer.Character, AnimDict, "grab_idle",
                2f, -8f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact,
                1000f, AnimationIKControlFlags.LinkedFacial);
            playerScene.PlayEntity(bag!, AnimDict, "bag_grab_idle",
                1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            playerScene.IsLooping = true;
        }

        private void PlayExitScene()
        {
            CasinoTableCam = false;
            Screen.ClearHelpText();

            if (bar != null && bar.Exists())
            {
                bar.Delete();
                bar = null;
            }

            playerScene = new SynchronizedScene(cart!);
            playerScene.Create(2000);
            playerScene.PlayPed(fPlayer.Character, AnimDict, "exit",
                1000f, -8f, SyncedSceneFlags.HideWeapon, RagdollBlockingFlags.PlayerImpact, 1000f);
            playerScene.PlayEntity(bag!, AnimDict, "bag_exit",
                1000f, -8f, SyncedSceneFlags.None, 1000f);

            if (cartScene != null && cartScene.IsRunning)
                cartScene.Rate = 0f;

            State = 4;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Blip management
        // ─────────────────────────────────────────────────────────────────────

        public Blip GetBlip()
            => cart?.AttachedBlip ?? null;

        public CartGrab CreateBlip(
            BlipSprite sprite,
            BlipColor colour,
            string name = "",
            float scale = 1f,
            bool isShortRange = true)
        {
            if (cart == null || !cart.Exists()) return this;

            var blip = cart.AddBlip();
            blip.Sprite = sprite;
            blip.Scale = scale;
            blip.Color = colour;
            blip.IsShortRange = isShortRange;
            blip.Name = name;

            return this;
        }

        public CartGrab ApplyBlipPreset(BlipPreset preset)
        {
            CreateBlip(preset.Sprite, preset.Color, preset.Name, preset.Scale, preset.IsShortRange);
            return this;
        }

        public CartGrab CreateWithBlipPreset(BlipPreset preset)
        {
            Create();
            ApplyBlipPreset(preset);
            return this;
        }

        public CartGrab CreateWithAutoBlip()
        {
            Create();
            ApplyBlipPreset(GetPresetForType(Type));
            return this;
        }

        public CartGrab RemoveBlip()
        {
            if (cart?.AttachedBlip != null && cart.AttachedBlip.Exists())
                cart.AttachedBlip.Delete();
            return this;
        }

        private static BlipPreset GetPresetForType(CartType type) => type switch
        {
            CartType.Gold_a or CartType.Gold_b or CartType.Gold_c
                => BlipPresets.GoldCart,
            CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c
                => BlipPresets.DiamondsCart,
            CartType.Cocaine
                => BlipPresets.CocaineCart,
            _
                => BlipPresets.CashCart
        };

        // ─────────────────────────────────────────────────────────────────────
        // Spawn
        // ─────────────────────────────────────────────────────────────────────

        public void Create()
        {
            if (cart == null)
            {
                IsLooted = false;
                cart = World.CreatePropNoOffset(cartModel, position, rotation, false);
            }
            else
            {
                // Cart already exists (e.g. re-entering after partial loot) —
                // set up a frozen scene at the current phase.
                cart.IsPositionFrozen = true;
                cartScene = new SynchronizedScene(cart);
                cartScene.Create(2000);
                cartScene.PlayEntity(cart, AnimDict, "cart_cash_dissapear",
                    1000f, -4f, SyncedSceneFlags.UseKinematicPhysics, SceneBlendRatio);
                cartScene.Rate = 0f;
                cartScene.Phase = Data.CurrentPhase;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Update loop
        // ─────────────────────────────────────────────────────────────────────

        public override void Update()
        {
            base.Update();

            if (CasinoTableCam)
                DisplayGrabText(false);

            switch (State)
            {
                case 0: UpdateIdleState(); break;
                case 1: UpdateApproachState(); break;
                case 2: UpdateIntroState(); break;
                case 3: UpdateGrabState(); break;
                case 4: UpdateExitState(); break;
            }
        }

        private void UpdateIdleState()
        {
            Create();
            if (IsLooted) return;

            Vector3 bottomOffset = fEntity.GetOffsetFromEntityInWorldCoords(cart!, 0f, 0f, -0.45f);
            Vector3 topOffset = fEntity.GetOffsetFromEntityInWorldCoords(cart!, 0f, 1.3f, 1.5f);
            Vector3 animStartPos = fEntity.GetAnimInitialOffsetPosition(
                AnimDict, "intro",
                cart!.Position.X, cart.Position.Y, cart.Position.Z,
                cart.Rotation.X, cart.Rotation.Y, cart.Rotation.Z,
                0f, 2);

            fStreaming.RequestAnimDict(AnimDict);
            fHud.RequestAdditionalText("MC_PLAY", 0);
            fHud.RequestAdditionalText("HACK", 3);

            if (!fStreaming.HasAnimDictLoaded(AnimDict)
             || !fHud.HasThisAdditionalTextLoaded("MC_PLAY", 0)
             || !fHud.HasAdditionalTextLoaded(3))
                return;

            if (fCutscene.IsCutscenePlaying() || !Game.Player.CanControlCharacter)
                return;

            bool inInteractionArea =
                fEntity.IsEntityInAngledArea(
                    fPlayer.Character, bottomOffset, topOffset,
                    InteractionDistance, false, true, 1)
                || fPlayer.Character.Position.DistanceTo(animStartPos) < 1f;

            if (!inInteractionArea || !CanPlayerInteract()) return;

            DisplayStartGrabText();

            if (Game.IsControlJustPressed(Control.Context))
                StartGrabMinigame(animStartPos);
        }

        private static bool CanPlayerInteract() =>
            !fPlayer.Character.IsInVehicle()
            && fPlayer.Character.IsAlive
            && !fPlayer.Character.IsPerformingMeleeAction
            && !fPlayer.Character.IsJumping
            && !fPlayer.Character.IsSprinting
            && !fPlayer.Character.IsRagdoll
            && !fPlayer.Character.IsGettingUp;

        private void StartGrabMinigame(Vector3 targetPosition)
        {
            GameplayCamera.SetCamViewModeForContext(
                CamViewModeContext.OnFoot, CamViewMode.ThirdPersonNear);
            Screen.ClearHelpText();
            IsActive(true);
            Game.Player.SetControlState(false, SetPlayerControlFlags.LeaveCameraControlOn);
            fAudio.StartAudioScene(AudioScene);

            Funcs.Ped.fPed.SetPedCanLegIK(fPlayer.Character, false);
            fPlayer.Character.SetResetFlag(PedResetFlagToggles.CannotBeTargeted, false);
            fEntity.SetEntityProofs(fPlayer.Character, false, false, false, true, false, true, false, true);
            fPlayer.Character.CanSwitchWeapons = false;

            if (fPlayer.Character.Weapons.Current != WeaponHash.Unarmed)
                PrevWeapon = fPlayer.Character.Weapons.Current;

            fPlayer.Character.Weapons.Select(WeaponHash.Unarmed);

            PrevBagType = BagManager.GetBagVariantFromPed(fPlayer.Character);
            if (PrevBagType == BagManager.BagVariantTypes.Invalid)
            {
                BagManager.ApplyBag(fPlayer.Character, BagManager.BagVariantTypes.OriginalHeists);
                PrevBagType = BagManager.BagVariantTypes.OriginalHeists;
            }

            bar = World.CreateProp(new Model(GetBarModel()), fPlayer.Character.Position, false, false);
            if (bar != null && bar.Exists())
            {
                bar.IsCollisionEnabled = false;
                bar.IsVisible = false;
                bar.IsInvincible = true;
                bar.AttachTo(fPlayer.Character.Bones[Bone.PHLeftHand]);
            }

            bag = BagManager.CreateBagPropFromPed(fPlayer.Character);
            if (bag != null && bag.Exists())
            {
                bag.IsCollisionEnabled = false;
                bag.IsVisible = false;
                bag.IsInvincible = true;
            }

            CasinoTableCam = true;
            Funcs.Ped.fPed.SetMovementModeOverride(fPlayer.Character, "DEFAULT_ACTION");

            // WARNING: GetAnimInitialOffsetPosition returns a world-space Vector3
            // position. Taking .Y as a heading (an angle in degrees) is suspicious —
            // the world Y coordinate (~-5207) is not a valid heading value.
            // This may need to use GetAnimInitialOffsetRotation(...).Z instead.
            // Preserved to maintain existing behaviour until verified.
            float heading = fEntity.GetAnimInitialOffsetPosition(
                AnimDict, "intro",
                cart!.Position.X, cart.Position.Y, cart.Position.Z,
                cart.Rotation.X, cart.Rotation.Y, cart.Rotation.Z,
                0f, 2).Y;

            fPlayer.Character.Task.GoStraightTo(targetPosition, 15000, heading, 0.75f);
            fPlayer.Character.KeepTaskWhenMarkedAsNoLongerNeeded = true;

            if (cart.AttachedBlip != null)
                cart.AttachedBlip.Alpha = 0;

            State = 1;
        }

        private void UpdateApproachState()
        {
            if (fPlayer.Character.GetScriptTaskStatus(ScriptTaskNameHash.GoStraightToCoord)
                == ScriptTaskStatus.Finished)
            {
                PlayIntroScene();
                State = 2;
            }
        }

        private void UpdateIntroState()
        {
            if (bar != null && bar.Exists() && playerScene!.Phase > BagAppearPhase)
            {
                BagManager.RemoveBag(fPlayer.Character);
                if (Funcs.Ped.fPed.HaveAllStreamingRequestsCompleted(fPlayer.Character))
                    bag!.IsVisible = true;
            }

            if (playerScene!.IsFinished)
            {
                Data.CurrentRate = MinRate;
                Data.TargetRate = MinRate;
                State = 3;
            }
        }

        private void UpdateGrabState()
        {
            bool isPlayingGrab = fEntity.IsEntityPlayingAnim(fPlayer.Character, AnimDict, "grab", 3);
            bool isCancelPressed = Game.IsControlJustPressed(
                fGamePad.IsUsingKeyboardAndMouse(2) ? Control.CursorCancel : Control.FrontendCancel);

            if (isPlayingGrab)
            {
                if (isCancelPressed)
                {
                    wantsToExit = true;
                    return;
                }
                UpdateGrabAnimation();
                HandleGrabEvents();
            }
            else
            {
                if (isCancelPressed)
                {
                    PlayExitScene();
                    return;
                }

                if (!fEntity.IsEntityPlayingAnim(fPlayer.Character, AnimDict, "grab_idle", 3))
                    PlayIdleScene();
                else if (Data.CurrentRate > MinRate)
                    PlayGrabScene();
            }
        }

        private void UpdateGrabAnimation()
        {
            if (playerScene!.IsRunning) playerScene.Rate = Data.CurrentRate;
            if (cartScene!.IsRunning) cartScene.Rate = Data.CurrentRate;
            Data.CurrentPhase = playerScene.Phase;
        }

        private void HandleGrabEvents()
        {
            if (fEntity.HasAnimEventFired(fPlayer.Character, "CASH_APPEAR")
                && bar != null && bar.Exists() && !bar.IsVisible)
            {
                bar.IsVisible = true;
            }

            if (fEntity.HasAnimEventFired(fPlayer.Character, "RELEASE_CASH_DESTROY"))
            {
                if (bar != null && bar.Exists() && bar.IsVisible)
                    bar.IsVisible = false;

                PlayCashSound();

                int cashAmount = GetRandomCashAmount();
                OnValueAdded(cashAmount, true);

                if (wantsToExit)
                {
                    Data.CurrentRate = MinRate;
                    if (Data.CurrentPhase >= 1f)
                    {
                        Screen.ClearHelpText();
                        RemoveBlip();
                        IsLooted = true;
                    }
                    PlayExitScene();
                    wantsToExit = false;
                    return;
                }

                if (Data.CurrentRate == MinRate)
                {
                    cartScene!.Rate = 0f;
                    PlayIdleScene();
                }
            }

            if (Data.CurrentPhase >= 1f)
            {
                Screen.ClearHelpText();
                RemoveBlip();
                IsLooted = true;
                PlayExitScene();
            }
        }

        /// <summary>
        /// Returns a random loot value for this cart type via the game's RNG.
        /// NOTE: The Standard/default case returns a very large range (1M–1.5M) —
        /// this appears to be a placeholder/debug value.
        /// </summary>
        private int GetRandomCashAmount()
        {
            var (min, max) = Type switch
            {
                CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c
                    => (45000, 50000),
                CartType.Gold_a or CartType.Gold_b or CartType.Gold_c
                    => (30000, 40000),
                CartType.Cocaine
                    => (25000, 30000),
                CartType.Cash_a or CartType.Cash_b or CartType.Cash_c
                    => (10000, 15000),
                CartType.LowCash_a or CartType.LowCash_b or CartType.LowCash_c
                    => (5000, 8000),
                _ // Standard — placeholder value, verify before shipping
                    => (1000000, 1500000)
            };

            return Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, min, max);
        }

        protected virtual void OnValueAdded(int amount, bool isCash)
            => ValueAdded?.Invoke(this, new MinigameValueAddedArgs(amount, isCash));

        private void PlayCashSound()
        {
            if (UseRemoteCounterSound)
            {
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1,
                    "REMOTE_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1,
                    "REMOTE_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
            }
            else
            {
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1,
                    "LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
            }
        }

        private void UpdateExitState()
        {
            if (playerScene == null) return;

            if (playerScene.Phase >= BagRestorePhase)
            {
                if (bag != null && bag.Exists())
                {
                    bag.Delete();
                    bag = null;
                }
                BagManager.ApplyBag(fPlayer.Character, PrevBagType);
            }

            if (playerScene.IsFinished)
                CleanupMinigame();
        }

        private void CleanupMinigame()
        {
            if (cart?.AttachedBlip != null)
                cart.AttachedBlip.Alpha = 255;

            fPlayer.Character.CanSwitchWeapons = true;
            IsActive(false);
            fAudio.StopAudioScene(AudioScene);
            fStreaming.RemoveAnimDict(AnimDict);
            fHud.ClearAdditionalText(0);
            fHud.ClearAdditionalText(3);
            Game.Player.SetControlState(true, SetPlayerControlFlags.None);
            Funcs.Ped.fPed.SetPedCanLegIK(fPlayer.Character, true);
            fPlayer.Character.SetResetFlag(PedResetFlagToggles.CannotBeTargeted, true);
            fEntity.SetEntityProofs(fPlayer.Character,
                false, false, false, false, false, false, false, false);

            cartScene?.Dispose(); cartScene = null;
            playerScene!.Dispose(); playerScene = null;

            fPlayer.Character.Task.ClearAllImmediately();
            State = 0;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Cleanup
        // ─────────────────────────────────────────────────────────────────────

        public override void PushResetOnDeath()
        {
            base.PushResetOnDeath();
            CleanupProps();
            CleanupScenes();
            CleanupResources();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (cart != null && cart.Exists())
            {
                cart.AttachedBlip?.Delete();
                cart.Delete();
                cart = null;
            }

            CleanupProps();
            CleanupScenes();
            CleanupResources();

            ActiveCarts.Remove(this);
        }

        private void CleanupProps()
        {
            CasinoTableCam = false;

            if (bar != null && bar.Exists()) { bar.Delete(); bar = null; }
            if (bag != null && bag.Exists()) { bag.Delete(); bag = null; }
        }

        private void CleanupScenes()
        {
            playerScene?.Dispose(); playerScene = null;
            cartScene?.Dispose(); cartScene = null;
        }

        private void CleanupResources()
        {
            if (fAudio.IsAudioSceneActive(AudioScene))
                fAudio.StopAudioScene(AudioScene);

            fStreaming.RemoveAnimDict(AnimDict);

            if (fHud.HasAdditionalTextLoaded(0)) fHud.ClearAdditionalText(0);
            if (fHud.HasAdditionalTextLoaded(3)) fHud.ClearAdditionalText(3);
        }
    }
}