using GTA;
using GTA.Math;
using GTA.Native;
using IMNOTEPIX.Framework.fInterfaces;
using IMNOTEPIX.Framework.fPlayer;
using System.Collections.Generic;
using Screen = GTA.UI.Screen;

namespace IMNOTEPIX.Framework.fGameplay.Minigames
{
    public enum StackType
    {
        Cash,
        Weed,
        Coke,
        Gold,
        ServerStack,
        HardDriveStack,
        BookStack
    }

    public class StackGrab : CashGrabMinigame, IMinigame
    {
        private const float InteractionDistance = 1.75f;
        private const float BagAppearPhase = 0.34f;
        private const float BagRestorePhase = 0.7f;
        private const float GoldMaxRate = 1.2f;
        private string AudioScene => "DLC_HEIST_MINIGAME_PAC_CASH_GRAB_SCENE";

        private const float SceneBlendRatio = 1148846100f;

        private static readonly HashSet<StackType> HackersDenDLCStacks =
            [StackType.ServerStack, StackType.HardDriveStack, StackType.BookStack];

        private static readonly HashSet<StackType> NonForceGrabTypes =
            [StackType.HardDriveStack, StackType.ServerStack, StackType.BookStack];

        private static readonly Dictionary<StackType, string> StackModels = new()
        {
            [StackType.Cash] = "h4_prop_h4_cash_stack_01a",
            [StackType.Weed] = "h4_prop_h4_weed_stack_01a",
            [StackType.Coke] = "h4_prop_h4_coke_stack_01a",
            [StackType.Gold] = "h4_prop_h4_gold_stack_01a",
            [StackType.ServerStack] = "m24_2_prop_m42_server_stack_01a",
            [StackType.HardDriveStack] = "m24_2_prop_m42_harddrive_stack_01a",
            [StackType.BookStack] = "m24_2_prop_m42_book_stack_01a",
        };

        private static readonly List<StackGrab> ActiveStacks = [];

        public static void UpdateAll()
        {
            for (int i = ActiveStacks.Count - 1; i >= 0; i--)
            {
                if (ActiveStacks[i].stack != null)
                    ActiveStacks[i].Update();
                if (ActiveStacks[i] == null)
                    ActiveStacks.RemoveAt(i);
            }
        }

        public static void DisposeAll()
        {
            for (int i = ActiveStacks.Count - 1; i >= 0; i--)
                ActiveStacks[i]?.Dispose();
            ActiveStacks.Clear();
        }

        public StackType Type { get; init; }
        public Model stackModel { get; private init; }
        public Vector3 position { get; private init; }
        public Vector3 rotation { get; private init; }
        public bool ForcedGrab { get; private init; }

        private bool DifferentAudioSceneRequired => HackersDenDLCStacks.Contains(Type);
        private bool IsGoldAnimFamily => HackersDenDLCStacks.Contains(Type) || Type == StackType.Gold;

        private string AnimDict => IsGoldAnimFamily ? "ANIM@SCRIPTED@PLAYER@MISSION@TUN_TABLE_GRAB@GOLD@" : "ANIM@SCRIPTED@PLAYER@MISSION@TUN_TABLE_GRAB@CASH@";

        private string EntityAnimName => IsGoldAnimFamily ? "grab_gold" : "grab_cash";

        private bool wantsToExit;
        private fWorld.Anims.SynchronizedScene playerScene;
        private fWorld.Anims.SynchronizedScene stackScene;
        private GTA.Prop stack;
        private GTA.Prop bag;

        public event MinigameValueAddedEventHandler ValueAdded;

        public StackGrab(StackType type, Vector3 pos, Vector3 rot = default, bool forcedGrab = false, bool putOnGround = false)
        {
            Type = type;
            rotation = rot;
            stackModel = new Model(fWorld.Misc.joaat(StackModels[type]));
            if (!NonForceGrabTypes.Contains(type))
                ForcedGrab = forcedGrab;
            else if (forcedGrab)
                fCore.Logger.Error("StackGrab", "Forced grab is not supported for stacks that are Non-ForceGrabbable.");

            if (putOnGround)
            {
                GTA.World.GetGroundHeight(pos, out var height);
                position = new Vector3(pos.X, pos.Y, height);
            }
            else
                position = pos;

            MaxRate = Type == StackType.Gold ? GoldMaxRate : 1.5f;

            ActiveStacks.Add(this);
        }

        private void DisplayStartGrabText()
        {
            string helpText = Type switch
            {
                StackType.Weed or StackType.Coke => "MC_GRAB_6",
                StackType.Gold => "MC_GRAB_2",
                StackType.ServerStack => "WBFIN_INT_GRAB3",
                StackType.HardDriveStack => "WBFIN_INT_GRAB1",
                StackType.BookStack => "WBFIN_INT_GRAB2",
                _ => "MC_GRAB_1"
            };
            fUI.Hud.DisplayHelpTextThisFrame(helpText);
        }

        private void DisplayGrabText(bool forcedGrab)
        {
            bool isKbm = Input.IsUsingKeyboardAndMouse(2);

            string helpText = Type switch
            {
                StackType.HardDriveStack => isKbm ? "MC_GRAB_8b" : "MC_GRAB_8",
                StackType.ServerStack => isKbm ? "MC_GRAB_9b" : "MC_GRAB_9",
                StackType.BookStack => isKbm ? "MC_GRAB_10b" : "MC_GRAB_10",
                _ => (Type, forcedGrab, isKbm) switch
                {
                    (StackType.Weed or StackType.Coke, true, true) => "MC_GRAB_5_MK_b",
                    (StackType.Weed or StackType.Coke, true, false) => "MC_GRAB_5_b",
                    (StackType.Weed or StackType.Coke, false, true) => "MC_GRAB_5_MK",
                    (StackType.Weed or StackType.Coke, false, false) => "MC_GRAB_5",

                    (StackType.Gold, true, true) => "MC_GRAB_4b_b",
                    (StackType.Gold, true, false) => "MC_GRAB_4_b",
                    (StackType.Gold, false, true) => "MC_GRAB_4b",
                    (StackType.Gold, false, false) => "MC_GRAB_4",

                    (_, true, true) => "MC_GRAB_3_MK_b",
                    (_, true, false) => "MC_GRAB_3_b",
                    (_, false, true) => "MC_GRAB_3_MK",
                    _ => "MC_GRAB_3"
                }
            };

            fUI.Hud.DisplayHelpTextThisFrame(helpText);
        }

        private void PlayIntroScene()
        {
            playerScene = new fWorld.Anims.SynchronizedScene(stack!.Position, stack.Rotation);
            playerScene.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "enter", 1.5f, -8f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1.5f);
            playerScene.PlayEntity(bag!, AnimDict, "enter_bag", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            stackScene = new fWorld.Anims.SynchronizedScene(stack);
        }

        private void PlayGrabScene()
        {
            playerScene!.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "grab", 4f, -4f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1000f);
            playerScene.PlayEntity(bag!, AnimDict, "grab_bag", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            playerScene.Phase = Data.CurrentPhase;

            stackScene!.Create(2000);
            stackScene.PlayEntity(stack!, AnimDict, EntityAnimName, 1000f, -4f, SyncedSceneFlags.UseKinematicPhysics, SceneBlendRatio);
            stackScene.Phase = Data.CurrentPhase;
        }

        private void PlayIdleScene()
        {
            playerScene!.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "grab_idle", 2f, -8f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1000f, AnimationIKControlFlags.LinkedFacial);
            playerScene.PlayEntity(bag!, AnimDict, "grab_idle_bag", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            playerScene.IsLooping = true;
        }

        private void PlayExitScene()
        {
            CasinoTableCam = false;
            Screen.ClearHelpText();

            playerScene = new fWorld.Anims.SynchronizedScene(stack!);
            playerScene.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "exit", 1000f, -8f, SyncedSceneFlags.HideWeapon, RagdollBlockingFlags.PlayerImpact, 1000f);
            playerScene.PlayEntity(bag!, AnimDict, "exit_bag", 1000f, -8f, SyncedSceneFlags.None, 1000f);

            if (stackScene != null && stackScene.IsRunning)
                stackScene.Rate = 0f;

            State = 4;
        }

        public GTA.Blip GetBlip()
          => stack?.AttachedBlip ?? null;

        public StackGrab CreateBlip(BlipSprite sprite, BlipColor colour, string name = "", float scale = 1f, bool isShortRange = true)
        {
            if (stack == null || !stack.Exists())
                return this;

            var blip = stack.AddBlip();
            blip.Sprite = sprite;
            blip.Scale = scale;
            blip.Color = colour;
            blip.IsShortRange = isShortRange;
            blip.Name = name;

            return this;
        }

        public StackGrab ApplyBlipPreset(fBlip.BlipPreset preset)
        {
            CreateBlip(preset.Sprite, preset.Color, preset.Name, preset.Scale, preset.IsShortRange);
            return this;
        }

        public StackGrab CreateWithBlipPreset(fBlip.BlipPreset preset)
        {
            Create();
            ApplyBlipPreset(preset);
            return this;
        }

        public StackGrab CreateWithAutoBlip()
        {
            Create();
            ApplyBlipPreset(GetPresetForType(Type));
            return this;
        }

        public StackGrab RemoveBlip()
        {
            if (stack?.AttachedBlip != null && stack.AttachedBlip.Exists())
                stack.AttachedBlip.Delete();
            return this;
        }

        private static fBlip.BlipPreset GetPresetForType(StackType type)
            => type switch
            {
                StackType.Weed => fBlip.BlipPresets.WeedStack,
                StackType.Gold => fBlip.BlipPresets.GoldStack,
                StackType.BookStack => fBlip.BlipPresets.BookStack,
                StackType.ServerStack => fBlip.BlipPresets.ServerStack,
                StackType.HardDriveStack => fBlip.BlipPresets.HardDriveStack,
                StackType.Cash => fBlip.BlipPresets.CashStack,
                StackType.Coke => fBlip.BlipPresets.CokeStack,
            };

        public void Create()
        {
            if (stack == null)
            {
                IsLooted = false;
                stack = GTA.World.CreatePropNoOffset(stackModel, position, rotation, false);
            }
            else
            {
                stack.IsPositionFrozen = true;
                stackScene = new fWorld.Anims.SynchronizedScene(stack);
                stackScene.Create(2000);
                stackScene.PlayEntity(stack, AnimDict, EntityAnimName, 1000f, -4f, SyncedSceneFlags.UseKinematicPhysics, SceneBlendRatio);
                stackScene.Rate = 0f;
                stackScene.Phase = Data.CurrentPhase;
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

            Vector3 bottomOffset = fWorld.Entity.GetOffsetFromEntityInWorldCoords(stack, 0f, 0f, -0.45f);
            Vector3 topOffset = fWorld.Entity.GetOffsetFromEntityInWorldCoords(stack, 0f, 1.3f, 1.5f);
            Vector3 animStartPos = fWorld.Entity.GetAnimInitialOffsetPosition(AnimDict, "enter", stack!.Position, stack.Rotation);

            fCore.Streaming.RequestAnimDict(AnimDict);
            //fUI.Hud.RequestAdditionalText("MC_PLAY", 0);
            fUI.Hud.RequestAdditionalText("HACK", 3);
            fAudio.Audio.RequestScriptAudioBank("DLC_HEI4/DLCHEI4_GENERIC_01", false);

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
            if (!DifferentAudioSceneRequired)
                fAudio.Audio.StartAudioScene(AudioScene);
            else
            {
                fAudio.Audio.StartAudioScene(AudioScene);
                fAudio.Audio.StartAudioScene("DLC_24-2_Loot_Grabbing_Minigame_Mute_Gold_Scene");
            }

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

            bag = PlayerBag.CreateBagPropFromPed(fPlayer.Player.Character);
            if (bag != null && bag.Exists())
            {
                bag.IsCollisionEnabled = false;
                bag.IsVisible = false;
                bag.IsInvincible = true;
            }

            CasinoTableCam = true;

            fWorld.Ped.SetMovementModeOverride(fPlayer.Player.Character, "DEFAULT_ACTION");

            float heading = fWorld.Entity.GetAnimInitialOffsetPosition(AnimDict, "enter", stack!.Position, stack.Rotation).Y;

            fPlayer.Player.Character.Task.GoStraightTo(targetPosition, 15000, heading, 0.75f);
            fPlayer.Player.Character.KeepTaskWhenMarkedAsNoLongerNeeded = true;

            if (stack.AttachedBlip != null)
                stack.AttachedBlip.Alpha = 0;

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
            if (stackScene!.IsRunning)
                stackScene.Rate = Data.CurrentRate;
            Data.CurrentPhase = playerScene.Phase;
        }

        private void HandleGrabEvents()
        {
            lock (this)
            {
                if (fWorld.Entity.HasAnimEventFired(fPlayer.Player.Character, "LOOTED"))
                {
                    PlayCashSound();

                    OnValueAdded(GetRandomAmount(), true);

                    if (wantsToExit)
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

                    if (Data.CurrentRate == base.MinRate)
                    {
                        stackScene!.Rate = 0f;
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

        private int GetRandomAmount()
        {
            var (min, max) = Type switch
            {
                StackType.Gold => (20000, 28000),
                StackType.ServerStack or StackType.HardDriveStack or StackType.BookStack => (15000, 22000),
                StackType.Coke => (12000, 18000),
                StackType.Weed => (8000, 12000),
                _ => (6000, 10000) // Cash
            };

            return Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, min, max);
        }

        protected virtual void OnValueAdded(int amount, bool isCash)
            => ValueAdded?.Invoke(this, new MinigameValueAddedArgs(amount, isCash));

        private void PlayCashSound()
        {
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
            if (stack?.AttachedBlip != null)
                stack.AttachedBlip.Alpha = 255;

            fPlayer.Player.Character.CanSwitchWeapons = true;
            IsActive(false);
            fAudio.Audio.StopAudioScene(AudioScene);
            fCore.Streaming.RemoveAnimDict(AnimDict);
            fUI.Hud.ClearAdditionalText(0);
            fUI.Hud.ClearAdditionalText(3);
            fAudio.Audio.ReleaseNamedScriptAudioBank("DLC_HEI4/DLCHEI4_GENERIC_01");
            Game.Player.SetControlState(true, SetPlayerControlFlags.None);
            fWorld.Ped.SetPedCanLegIK(fPlayer.Player.Character, true);
            fPlayer.Player.Character.SetResetFlag(PedResetFlagToggles.CannotBeTargeted, true);
            fWorld.Entity.SetEntityProofs(fPlayer.Player.Character, false, false, false, false, false, false, false, false);

            stackScene?.Dispose();
            stackScene = null;
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

            if (stack != null && stack.Exists())
            {
                stack.AttachedBlip?.Delete();
                stack.Delete();
                stack = null;
            }

            CleanupProps();
            CleanupScenes();
            CleanupResources();

            ActiveStacks.Remove(this);
        }

        private void CleanupProps()
        {
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
            stackScene?.Dispose();
            stackScene = null;
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

            fAudio.Audio.ReleaseNamedScriptAudioBank("DLC_HEI4/DLCHEI4_GENERIC_01");
        }
    }
}