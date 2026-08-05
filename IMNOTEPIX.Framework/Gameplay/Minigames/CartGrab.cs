using GTA;
using GTA.Math;
using GTA.Native;
using IMNOTEPIX.Framework.fInterfaces;
using IMNOTEPIX.Framework.fPlayer;
using System.Collections.Generic;
using Screen = GTA.UI.Screen;

namespace IMNOTEPIX.Framework.fGameplay.Minigames
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

    public class CartGrab : CashGrabMinigame, IMinigame
    {
        private const float InteractionDistance = 1.75f;
        private const float BagAppearPhase = 0.34f;
        private const float BagRestorePhase = 0.7f;
        private const float GoldMaxRate = 1.2f;

        private const string AnimDict = "anim@heists@ornate_bank@grab_cash";
        private const string AudioScene = "DLC_HEIST_MINIGAME_PAC_CASH_GRAB_SCENE";

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

        private static readonly List<CartGrab> ActiveCarts = [];

        public static void UpdateAll()
        {
            for (int i = ActiveCarts.Count - 1; i >= 0; i--)
            {
                if (ActiveCarts[i].cart != null)
                    ActiveCarts[i].Update();
                if (ActiveCarts[i] == null)
                    ActiveCarts.RemoveAt(i);
            }
        }

        public static void DisposeAll()
        {
            for (int i = ActiveCarts.Count - 1; i >= 0; i--)
                ActiveCarts[i]?.Dispose();
            ActiveCarts.Clear();
        }

        public CartType Type { get; init; }
        public Model cartModel { get; private init; }
        public Vector3 position { get; private init; }
        public Vector3 rotation { get; private init; }
        public bool ForcedGrab { get; private init; }

        private bool wantsToExit;
        private fWorld.Anims.SynchronizedScene playerScene;
        private fWorld.Anims.SynchronizedScene cartScene;
        private GTA.Prop cart;
        private GTA.Prop bar;
        private GTA.Prop bag;

        public event MinigameValueAddedEventHandler ValueAdded;

        public CartGrab(CartType type, Vector3 pos, Vector3 rot = default, bool forcedGrab = false, bool putOnGround = false)
        {
            Type = type;
            rotation = rot;
            cartModel = new Model(fWorld.Misc.joaat(CartModels[type]));
            ForcedGrab = forcedGrab;

            if (putOnGround)
            {
                GTA.World.GetGroundHeight(pos, out var height);
                position = new Vector3(pos.X, pos.Y, height);
            }
            else
                position = pos;

            MaxRate = IsGoldType(type) ? GoldMaxRate : 1.5f;

            ActiveCarts.Add(this);
        }

        private static bool IsGoldType(CartType type)
            => type is CartType.Gold_a or CartType.Gold_b or CartType.Gold_c;

        private int GetBarModel()
            => Type switch
            {
                CartType.Cocaine => fWorld.Misc.joaat("imp_prop_impexp_coke_pile"),
                CartType.Gold_a or CartType.Gold_b or CartType.Gold_c => fWorld.Misc.joaat("ch_prop_gold_bar_01a"),
                CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c => fWorld.Misc.joaat("ch_prop_vault_dimaondbox_01a"),
                _ => fWorld.Misc.joaat("hei_prop_heist_cash_pile")
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
            fUI.Hud.DisplayHelpTextThisFrame(helpText);
        }

        private void DisplayGrabText(bool forcedGrab)
        {
            bool isKbm = Input.IsUsingKeyboardAndMouse(2);

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

            fUI.Hud.DisplayHelpTextThisFrame(helpText);
        }

        private void PlayIntroScene()
        {
            playerScene = new fWorld.Anims.SynchronizedScene(cart!.Position, cart.Rotation);
            playerScene.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "intro", 1.5f, -8f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1.5f);
            playerScene.PlayEntity(bag!, AnimDict, "bag_intro", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            cartScene = new fWorld.Anims.SynchronizedScene(cart);
        }

        private void PlayGrabScene()
        {
            playerScene!.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "grab", 4f, -4f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1000f);
            playerScene.PlayEntity(bag!, AnimDict, "bag_grab", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            playerScene.Phase = Data.CurrentPhase;

            cartScene!.Create(2000);
            cartScene.PlayEntity(cart!, AnimDict, "cart_cash_dissapear", 1000f, -4f, SyncedSceneFlags.UseKinematicPhysics, SceneBlendRatio);
            cartScene.Phase = Data.CurrentPhase;
        }

        private void PlayIdleScene()
        {
            playerScene!.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "grab_idle", 2f, -8f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1000f, AnimationIKControlFlags.LinkedFacial);
            playerScene.PlayEntity(bag!, AnimDict, "bag_grab_idle", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
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

            playerScene = new fWorld.Anims.SynchronizedScene(cart!);
            playerScene.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "exit", 1000f, -8f, SyncedSceneFlags.HideWeapon, RagdollBlockingFlags.PlayerImpact, 1000f);
            playerScene.PlayEntity(bag!, AnimDict, "bag_exit", 1000f, -8f, SyncedSceneFlags.None, 1000f);

            if (cartScene != null && cartScene.IsRunning)
                cartScene.Rate = 0f;

            State = 4;
        }

        public GTA.Blip GetBlip()
            => cart?.AttachedBlip ?? null;

        public CartGrab CreateBlip(BlipSprite sprite, BlipColor colour, string name = "", float scale = 1f, bool isShortRange = true)
        {
            if (cart == null || !cart.Exists())
                return this;

            var blip = cart.AddBlip();
            blip.Sprite = sprite;
            blip.Scale = scale;
            blip.Color = colour;
            blip.IsShortRange = isShortRange;
            blip.Name = name;

            return this;
        }

        public CartGrab ApplyBlipPreset(fBlip.BlipPreset preset)
        {
            CreateBlip(preset.Sprite, preset.Color, preset.Name, preset.Scale, preset.IsShortRange);
            return this;
        }

        public CartGrab CreateWithBlipPreset(fBlip.BlipPreset preset)
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

        private static fBlip.BlipPreset GetPresetForType(CartType type)
            => type switch
            {
                CartType.Gold_a or CartType.Gold_b or CartType.Gold_c => fBlip.BlipPresets.GoldCart,
                CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c => fBlip.BlipPresets.DiamondsCart,
                CartType.Cocaine => fBlip.BlipPresets.CocaineCart,
                _ => fBlip.BlipPresets.CashCart
            };

        public void Create()
        {
            if (cart == null)
            {
                IsLooted = false;
                cart = GTA.World.CreatePropNoOffset(cartModel, position, rotation, false);
            }
            else
            {
                cart.IsPositionFrozen = true;
                cartScene = new fWorld.Anims.SynchronizedScene(cart);
                cartScene.Create(2000);
                cartScene.PlayEntity(cart, AnimDict, "cart_cash_dissapear", 1000f, -4f, SyncedSceneFlags.UseKinematicPhysics, SceneBlendRatio);
                cartScene.Rate = 0f;
                cartScene.Phase = Data.CurrentPhase;
            }
        }

        public override void Update()
        {
            base.Update();

            if (CasinoTableCam)
                DisplayGrabText(ForcedGrab);

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
            if (IsLooted)
                return;

            Vector3 bottomOffset = fWorld.Entity.GetOffsetFromEntityInWorldCoords(cart, 0f, 0f, -0.45f);
            Vector3 topOffset = fWorld.Entity.GetOffsetFromEntityInWorldCoords(cart, 0f, 1.3f, 1.5f);
            Vector3 animStartPos = fWorld.Entity.GetAnimInitialOffsetPosition(AnimDict, "intro", cart!.Position, cart.Rotation);

            fCore.Streaming.RequestAnimDict(AnimDict);
            //fUI.Hud.RequestAdditionalText("MC_PLAY", 0);
            fUI.Hud.RequestAdditionalText("HACK", 3);

            if (!fCore.Streaming.HasAnimDictLoaded(AnimDict) /*|| !fUI.Hud.HasThisAdditionalTextLoaded("MC_PLAY", 0) */|| !fUI.Hud.HasAdditionalTextLoaded(3))
                return;

            if (fWorld.Cutscene.Cutscene.IsCutscenePlaying() || !Game.Player.CanControlCharacter)
                return;

            bool inInteractionArea = fWorld.Entity.IsEntityInAngledArea(fPlayer.Player.Character, bottomOffset, topOffset, InteractionDistance, false, true, 1) || fPlayer.Player.Character.Position.DistanceTo(animStartPos) < 1f;

            if (!inInteractionArea || !CanPlayerInteract())
                return;

            DisplayStartGrabText();

            if (GTA.Input.Controls.IsDisabledControlJustPressed(GTA.Input.ControlType.PlayerControl, GTA.Input.ControlAction.Context))
                StartGrabMinigame(animStartPos);
        }

        private static bool CanPlayerInteract()
            => !fPlayer.Player.Character.IsInVehicle() && fPlayer.Player.Character.IsAlive && !fPlayer.Player.Character.IsPerformingMeleeAction && !fPlayer.Player.Character.IsJumping && !fPlayer.Player.Character.IsSprinting && !fPlayer.Player.Character.IsRagdoll && !fPlayer.Player.Character.IsGettingUp;

        private void StartGrabMinigame(Vector3 targetPosition)
        {
            GameplayCamera.SetCamViewModeForContext(CamViewModeContext.OnFoot, CamViewMode.ThirdPersonNear);
            Screen.ClearHelpText();
            IsActive(true);
            Game.Player.SetControlState(false, SetPlayerControlFlags.LeaveCameraControlOn);
            fAudio.Audio.StartAudioScene(AudioScene);

            fWorld.Ped.SetPedCanLegIK(fPlayer.Player.Character, false);
            fPlayer.Player.Character.SetResetFlag(PedResetFlagToggles.CannotBeTargeted, false);
            fWorld.Entity.SetEntityProofs(fPlayer.Player.Character, false, false, false, true, false, true, false, true);
            fPlayer.Player.Character.CanSwitchWeapons = false;

            if (fPlayer.Player.Character.Weapons.Current != WeaponHash.Unarmed)
                PrevWeapon = fPlayer.Player.Character.Weapons.Current;

            fPlayer.Player.Character.Weapons.Select(WeaponHash.Unarmed);

            PrevBagType = PlayerBag.GetBagVariantFromPed(fPlayer.Player.Character);
            if (PrevBagType == PlayerBag.BagVariantTypes.Invalid)
            {
                PlayerBag.ApplyBag(fPlayer.Player.Character, PlayerBag.BagVariantTypes.OriginalHeists);
                PrevBagType = PlayerBag.BagVariantTypes.OriginalHeists;
            }

            bar = GTA.World.CreateProp(new Model(GetBarModel()), fPlayer.Player.Character.Position, false, false);
            if (bar != null && bar.Exists())
            {
                bar.IsCollisionEnabled = false;
                bar.IsVisible = false;
                bar.IsInvincible = true;
                bar.AttachTo(fPlayer.Player.Character.Bones[Bone.PHLeftHand]);
            }

            bag = PlayerBag.CreateBagPropFromPed(fPlayer.Player.Character);
            if (bag != null && bag.Exists())
            {
                bag.IsCollisionEnabled = false;
                bag.IsVisible = false;
                bag.IsInvincible = true;
            }

            CasinoTableCam = true;

            fWorld.Ped.SetMovementModeOverride(fPlayer.Player.Character, "DEFAULT_ACTION");

            float heading = fWorld.Entity.GetAnimInitialOffsetPosition(AnimDict, "intro", cart!.Position, cart.Rotation).Y;

            fPlayer.Player.Character.Task.GoStraightTo(targetPosition, 15000, heading, 0.75f);
            fPlayer.Player.Character.KeepTaskWhenMarkedAsNoLongerNeeded = true;

            if (cart.AttachedBlip != null)
                cart.AttachedBlip.Alpha = 0;

            State = 1;
        }

        private void UpdateApproachState()
        {
            if (fPlayer.Player.Character.GetScriptTaskStatus(ScriptTaskNameHash.GoStraightToCoord) == ScriptTaskStatus.Finished)
            {
                PlayIntroScene();
                State = 2;
            }
        }

        private void UpdateIntroState()
        {
            if (bag != null && bag.Exists() && playerScene!.Phase > BagAppearPhase)
            {
                PlayerBag.RemoveBag(fPlayer.Player.Character);
                if (fWorld.Ped.HaveAllStreamingRequestsCompleted(fPlayer.Player.Character))
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
            bool isPlayingGrab = fWorld.Entity.IsEntityPlayingAnim(fPlayer.Player.Character, AnimDict, "grab", 3);
            bool isCancelPressed = !ForcedGrab && GTA.Input.Controls.IsDisabledControlJustPressed(GTA.Input.ControlType.PlayerControl, Input.IsUsingKeyboardAndMouse(2) ? GTA.Input.ControlAction.CursorCancel : GTA.Input.ControlAction.FrontendCancel);

            if (isPlayingGrab)
            {
                if (isCancelPressed)
                    wantsToExit = true;

                UpdateGrabAnimation();
                HandleGrabEvents();
            }
            else
            {
                if (isCancelPressed)
                    PlayExitScene();

                if (!fWorld.Entity.IsEntityPlayingAnim(fPlayer.Player.Character, AnimDict, "grab_idle", 3))
                    PlayIdleScene();
                else if (Data.CurrentRate > base.MinRate)
                    PlayGrabScene();
            }
        }

        private void UpdateGrabAnimation()
        {
            if (playerScene!.IsRunning)
                playerScene.Rate = Data.CurrentRate;
            if (cartScene!.IsRunning)
                cartScene.Rate = Data.CurrentRate;
            Data.CurrentPhase = playerScene.Phase;
        }

        private void HandleGrabEvents()
        {
            lock (this)
            {
                if (fWorld.Entity.HasAnimEventFired(fPlayer.Player.Character, "CASH_APPEAR") && bar != null && bar.Exists() && !bar.IsVisible)
                    bar.IsVisible = true;

                if (fWorld.Entity.HasAnimEventFired(fPlayer.Player.Character, "RELEASE_CASH_DESTROY"))
                {
                    if (bar != null && bar.Exists() && bar.IsVisible)
                        bar.IsVisible = false;

                    PlayCashSound();

                    OnValueAdded(GetRandomCashAmount(), true);

                    if (wantsToExit)
                    {
                        if (!bar.IsVisible)
                        {
                            Data.CurrentRate = base.MinRate;
                            if (Data.CurrentPhase >= 1f)
                            {
                                fAudio.Audio.PlaySoundFrontend(-1, "REMOTE_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
                                Screen.ClearHelpText();
                                RemoveBlip();
                                IsLooted = true;
                            }
                            PlayExitScene();
                            wantsToExit = false;
                            return;
                        }
                    }

                    if (Data.CurrentRate == base.MinRate)
                    {
                        cartScene!.Rate = 0f;
                        PlayIdleScene();
                    }
                }

                if (Data.CurrentPhase >= 1f)
                {
                    fAudio.Audio.PlaySoundFrontend(-1, "REMOTE_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
                    Screen.ClearHelpText();
                    RemoveBlip();
                    IsLooted = true;
                    PlayExitScene();
                }
            }
        }

        private int GetRandomCashAmount()
        {
            var (min, max) = Type switch
            {
                CartType.Diamonds_a or CartType.Diamonds_b or CartType.Diamonds_c => (45000, 50000),
                CartType.Gold_a or CartType.Gold_b or CartType.Gold_c => (30000, 40000),
                CartType.Cocaine => (25000, 30000),
                CartType.Cash_a or CartType.Cash_b or CartType.Cash_c => (10000, 15000),
                CartType.LowCash_a or CartType.LowCash_b or CartType.LowCash_c => (5000, 8000),
                _ => (1000000, 1500000)
            };

            return Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, min, max);
        }

        protected virtual void OnValueAdded(int amount, bool isCash)
            => ValueAdded?.Invoke(this, new MinigameValueAddedArgs(amount, isCash));

        private void PlayCashSound()
        {
            if (UseRemoteCounterSound)
            {
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "REMOTE_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "REMOTE_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
            }
            else
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
        }

        private void UpdateExitState()
        {
            if (playerScene == null)
                return;

            if (playerScene.Phase >= BagRestorePhase)
            {
                if (bag != null && bag.Exists())
                {
                    bag.Delete();
                    bag = null;
                }
                PlayerBag.ApplyBag(fPlayer.Player.Character, PrevBagType);
            }

            if (playerScene.IsFinished)
                CleanupMinigame();
        }

        private void CleanupMinigame()
        {
            if (cart?.AttachedBlip != null)
                cart.AttachedBlip.Alpha = 255;

            fPlayer.Player.Character.CanSwitchWeapons = true;
            IsActive(false);
            fAudio.Audio.StopAudioScene(AudioScene);
            fCore.Streaming.RemoveAnimDict(AnimDict);
            fUI.Hud.ClearAdditionalText(0);
            fUI.Hud.ClearAdditionalText(3);
            Game.Player.SetControlState(true, SetPlayerControlFlags.None);
            fWorld.Ped.SetPedCanLegIK(fPlayer.Player.Character, true);
            fPlayer.Player.Character.SetResetFlag(PedResetFlagToggles.CannotBeTargeted, true);
            fWorld.Entity.SetEntityProofs(fPlayer.Player.Character, false, false, false, false, false, false, false, false);

            cartScene?.Dispose();
            cartScene = null;
            playerScene!.Dispose();
            playerScene = null;

            fPlayer.Player.Character.Task.ClearAllImmediately();
            State = 0;
        }

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
            if (bar != null && bar.Exists())
            {
                bar.Delete();
                bar = null;
            }
            if (bag != null && bag.Exists())
            {
                bag.Delete();
                bag = null;
            }
        }

        private void CleanupScenes()
        {
            CasinoTableCam = false;

            playerScene?.Dispose();
            playerScene = null;
            cartScene?.Dispose();
            cartScene = null;
        }

        private void CleanupResources()
        {
            if (fAudio.Audio.IsAudioSceneActive(AudioScene))
                fAudio.Audio.StopAudioScene(AudioScene);

            fCore.Streaming.RemoveAnimDict(AnimDict);

            if (fUI.Hud.HasAdditionalTextLoaded(0))
                fUI.Hud.ClearAdditionalText(0);
            if (fUI.Hud.HasAdditionalTextLoaded(3))
                fUI.Hud.ClearAdditionalText(3);
        }
    }
}
