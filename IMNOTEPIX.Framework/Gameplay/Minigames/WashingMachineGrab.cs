using GTA;
using GTA.Math;
using GTA.Native;
using IMNOTEPIX.Framework.fInterfaces;
using IMNOTEPIX.Framework.fPlayer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screen = GTA.UI.Screen;

namespace IMNOTEPIX.Framework.fGameplay.Minigames
{
    public class WashingMachineGrab : CashGrabMinigame, IMinigame
    {
        private const float InteractionDistance = 1.35f;
        private const float BagAppearPhase = 0.34f;
        private const float BagRestorePhase = 0.7f;
        private const float SceneBlendRatio = 1148846100f;

        private const string AnimDict = "anim@scripted@cbr1@ig1_washmach_grab_cash@male@";
        private const string MachineModelName = "m23_2_prop_m32_prtmachine_dryer_op";
        private const string MachineMoneyPropModelName = "m23_2_prop_m32_money_dry_01a";
        private const string MoneyPropModelName = "m23_2_prop_m32_money_loose_01a";
        private const string AudioScene = "DLC_HEIST_MINIGAME_PAC_CASH_GRAB_SCENE";

        public Model machineModel { get; private init; }
        public Vector3 position { get; private init; }
        public Vector3 rotation { get; private init; }

        private fWorld.Anims.SynchronizedScene machineScene;
        private fWorld.Anims.SynchronizedScene playerScene;
        private fWorld.Anims.SynchronizedScene moneyScene;
        private GTA.Prop machineCol;
        private GTA.Prop machine;
        private GTA.Prop machineMoney;
        private GTA.Prop bag;
        private GTA.Prop moneyPropLeft;
        private GTA.Prop moneyPropRight;


        private static readonly List<WashingMachineGrab> ActiveMachines = [];

        public event MinigameValueAddedEventHandler ValueAdded;

        public static int GetHowManyAreLooted()
        {
            int amount = 0;
            for (int i = ActiveMachines.Count - 1; i >= 0; i--)
            {
                if (ActiveMachines[i].machine != null && ActiveMachines[i].IsLooted)
                    amount++;
            }
            return amount;
        }


        public static void ShowPromptNearAllMachines(string gxtEntry)
        {
            for (int i = ActiveMachines.Count - 1; i >= 0; i--)
            {
                if (ActiveMachines[i].machine != null)
                    ActiveMachines[i].ShowPromptNearMachine(gxtEntry);
            }
        }

        public static void UpdateAll()
        {
            for (int i = ActiveMachines.Count - 1; i >= 0; i--)
            {
                if (ActiveMachines[i].machine != null)
                    ActiveMachines[i].Update();
                if (ActiveMachines[i] == null)
                    ActiveMachines.RemoveAt(i);
            }
        }

        public static void DisposeAll()
        {
            for (int i = ActiveMachines.Count - 1; i >= 0; i--)
                ActiveMachines[i]?.Dispose();
            ActiveMachines.Clear();
        }

        public WashingMachineGrab(Vector3 pos, Vector3 rot = default, bool putOnGround = false)
        {
            rotation = rot;
            machineModel = new Model(fWorld.Misc.joaat(MachineModelName));

            if (putOnGround)
            {
                GTA.World.GetGroundHeight(pos, out var height);
                position = new Vector3(pos.X, pos.Y, height);
            }
            else
                position = pos;

            MaxRate = 1.5f;

            ActiveMachines.Add(this);
        }

        private void DisplayStartGrabText()
            => fUI.Hud.DisplayGXTHelpText("FMMC_INT_P2");

        private void DisplayGrabText()
        {
            bool isKbm = Input.IsUsingKeyboardAndMouse(2);
            string helpText = isKbm ? "MC_GRAB_3_MK_b" : "MC_GRAB_3_b";
            fUI.Hud.DisplayHelpTextThisFrame(helpText);
        }

        private void PlayIntroScene()
        {
            playerScene = new fWorld.Anims.SynchronizedScene(machine!);
            playerScene.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "enter", 1.5f, -8f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1.5f);
            playerScene.PlayEntity(bag!, AnimDict, "enter_bag", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            machineScene = new fWorld.Anims.SynchronizedScene(machine!);
            machineScene.Create(2000);
            machineScene.PlayEntity(machine!, AnimDict, "enter_dryer", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            machineScene.HoldLastFrame = true;
            moneyScene = new fWorld.Anims.SynchronizedScene(machine);
        }

        private void PlayGrabScene()
        {
            playerScene!.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "grab", 4f, -4f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1000f);
            playerScene.PlayEntity(bag!, AnimDict, "grab_bag", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            playerScene.Phase = Data.CurrentPhase;

            moneyScene!.Create(2000);
            moneyScene.PlayEntity(machineMoney!, AnimDict, "grab_dryermoney", 1000f, -4f, SyncedSceneFlags.UseKinematicPhysics, SceneBlendRatio);
            moneyScene.Phase = Data.CurrentPhase;
        }

        private void PlayIdleScene()
        {
            playerScene!.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "idle", 2f, -8f, (SyncedSceneFlags)13, RagdollBlockingFlags.PlayerImpact, 1000f, AnimationIKControlFlags.LinkedFacial);
            playerScene.PlayEntity(bag!, AnimDict, "idle_bag", 1000f, -1000f, SyncedSceneFlags.None, SceneBlendRatio);
            playerScene.IsLooping = true;
        }

        private void PlayExitScene()
        {
            CasinoTableCam = false;
            Screen.ClearHelpText();

            if (moneyPropLeft != null && moneyPropLeft.Exists())
            {
                moneyPropLeft.Delete();
                moneyPropLeft = null;
            }
            if (moneyPropRight != null && moneyPropRight.Exists())
            {
                moneyPropRight.Delete();
                moneyPropRight = null;
            }

            playerScene = new fWorld.Anims.SynchronizedScene(machine!);
            playerScene.Create(2000);
            playerScene.PlayPed(fPlayer.Player.Character, AnimDict, "exit", 1000f, -8f, SyncedSceneFlags.HideWeapon, RagdollBlockingFlags.PlayerImpact, 1000f);
            playerScene.PlayEntity(bag!, AnimDict, "exit_bag", 1000f, -8f, SyncedSceneFlags.None, 1000f);

            if (moneyScene != null && moneyScene.IsRunning)
                moneyScene.Rate = 0f;

            State = 4;
        }

        public GTA.Blip GetBlip()
            => machine?.AttachedBlip ?? null;

        public WashingMachineGrab CreateBlip(BlipSprite sprite, BlipColor colour, string name = "", float scale = 1f, bool isShortRange = true)
        {
            if (machine == null || !machine.Exists())
                return this;

            var blip = machine.AddBlip();
            blip.Sprite = sprite;
            blip.Scale = scale;
            blip.Color = colour;
            blip.IsShortRange = isShortRange;
            blip.Name = name;

            return this;
        }

        public WashingMachineGrab ApplyBlipPreset(fBlip.BlipPreset preset)
        {
            CreateBlip(preset.Sprite, preset.Color, preset.Name, preset.Scale, preset.IsShortRange);
            return this;
        }

        public WashingMachineGrab CreateWithBlipPreset(fBlip.BlipPreset preset)
        {
            Create();
            ApplyBlipPreset(preset);
            return this;
        }

        public WashingMachineGrab RemoveBlip()
        {
            if (machine?.AttachedBlip != null && machine.AttachedBlip.Exists())
                machine.AttachedBlip.Delete();
            return this;
        }

        public void Create()
        {
            if (machine == null && machineMoney == null && machineCol == null)
            {
                IsLooted = false;
                machine = GTA.World.CreatePropNoOffset(machineModel, position, rotation, false);
                machineMoney = GTA.World.CreatePropNoOffset(new Model(fWorld.Misc.joaat(MachineMoneyPropModelName)), position, rotation, false);
                machineCol = GTA.World.CreatePropNoOffset(new Model("m23_2_prop_m32_dryer_op_01a"), machine.Position, machine.Rotation, false);
            }
            while ((machine == null || !machine.Exists()) && (machineMoney == null && !machineMoney.Exists()) && machineCol == null && !machineCol.Exists())
                Script.Wait(0);
            machineCol.IsPositionFrozen = true;
            machineCol.IsVisible = false;
            machineCol.IsCollisionEnabled = false;
            machine.IsPositionFrozen = true;
            machineMoney.IsPositionFrozen = true;
            moneyScene = new fWorld.Anims.SynchronizedScene(machine);
            moneyScene.Create(2000);
            moneyScene.PlayEntity(machineMoney, AnimDict, "grab_dryermoney", 1000f, -4f, SyncedSceneFlags.UseKinematicPhysics, SceneBlendRatio);
            moneyScene.Rate = 0f;
            moneyScene.Phase = Data.CurrentPhase;
        }

        public override void Update()
        {
            base.Update();

            if (CasinoTableCam)
                DisplayGrabText();

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

            Vector3 bottomOffset = fWorld.Entity.GetOffsetFromEntityInWorldCoords(machine, 0f, 0f, -0.45f);
            Vector3 topOffset = fWorld.Entity.GetOffsetFromEntityInWorldCoords(machine, 0f, 1.3f, 1.5f);
            Vector3 animStartPos = fWorld.Entity.GetAnimInitialOffsetPosition(AnimDict, "enter", machine!.Position, machine.Rotation);

            fCore.Streaming.RequestAnimDict(AnimDict);
            fUI.Hud.RequestAdditionalText("MC_PLAY", 0);
            fUI.Hud.RequestAdditionalText("HACK", 3);

            if (!fCore.Streaming.HasAnimDictLoaded(AnimDict) || !fUI.Hud.HasThisAdditionalTextLoaded("MC_PLAY", 0) || !fUI.Hud.HasAdditionalTextLoaded(3))
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

            var moneyModel = new Model(fWorld.Misc.joaat(MoneyPropModelName));
            moneyPropLeft = GTA.World.CreateProp(moneyModel, fPlayer.Player.Character.Position, false, false);
            if (moneyPropLeft != null && moneyPropLeft.Exists())
            {
                moneyPropLeft.IsVisible = false;
                moneyPropLeft.IsCollisionEnabled = false;
                moneyPropLeft.IsInvincible = true;
                moneyPropLeft.AttachTo(fPlayer.Player.Character.Bones[Bone.PHLeftHand]);
            }

            moneyPropRight = GTA.World.CreateProp(moneyModel, fPlayer.Player.Character.Position, false, false);
            if (moneyPropRight != null && moneyPropRight.Exists())
            {
                moneyPropRight.IsVisible = false; 
                moneyPropRight.IsCollisionEnabled = false;
                moneyPropRight.IsInvincible = true;
                moneyPropRight.AttachTo(fPlayer.Player.Character.Bones[Bone.PHRightHand]);
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

            float heading = fWorld.Entity.GetAnimInitialOffsetPosition(AnimDict, "enter", machine!.Position, machine.Rotation).Y;

            fPlayer.Player.Character.Task.GoStraightTo(targetPosition, 15000, heading, 0.75f);
            fPlayer.Player.Character.KeepTaskWhenMarkedAsNoLongerNeeded = true;

            if (machine.AttachedBlip != null)
                machine.AttachedBlip.Alpha = 0;

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
                machineCol.IsCollisionEnabled = true;
                Data.CurrentRate = MinRate;
                Data.TargetRate = MinRate;
                State = 3;
            }
        }

        private void UpdateGrabState()
        {
            bool isPlayingGrab = fWorld.Entity.IsEntityPlayingAnim(fPlayer.Player.Character, AnimDict, "grab", 3);

            if (isPlayingGrab)
            {
                UpdateGrabAnimation();
                HandleGrabEvents();
            }
            else
            {
                if (!fWorld.Entity.IsEntityPlayingAnim(fPlayer.Player.Character, AnimDict, "idle", 3))
                    PlayIdleScene();
                else if (Data.CurrentRate > base.MinRate)
                    PlayGrabScene();
            }
        }

        private void UpdateGrabAnimation()
        {
            fWorld.Entity.CreateModelSwap(machine.Position, 0.03f, machineModel, new Model(1928143059), true);
            if (playerScene!.IsRunning)
                playerScene.Rate = Data.CurrentRate;
            if (moneyScene!.IsRunning)
                moneyScene.Rate = Data.CurrentRate;
            Data.CurrentPhase = playerScene.Phase;
        }

        private void HandleGrabEvents()
        {
            lock (this)
            {
                if (!fWorld.Entity.HasAnimEventFired(fPlayer.Player.Character, "LOOTED"))
                {
                    if (moneyPropLeft != null && moneyPropLeft.Exists() && !moneyPropLeft.IsVisible)
                        moneyPropLeft.IsVisible = true;
                    if (moneyPropRight != null && moneyPropRight.Exists() && !moneyPropRight.IsVisible)
                        moneyPropRight.IsVisible = true;
                }
                else
                {
                    if (moneyPropLeft != null && moneyPropLeft.Exists() && moneyPropLeft.IsVisible)
                        moneyPropLeft.IsVisible = false;
                    if (moneyPropRight != null && moneyPropRight.Exists() && moneyPropRight.IsVisible)
                        moneyPropRight.IsVisible = false;

                    PlayCashSound();

                    OnValueAdded(GetRandomCashAmount(), true);

                    if (Data.CurrentRate == base.MinRate)
                    {
                        moneyScene!.Rate = 0f;
                        PlayIdleScene();
                    }
                }

                if (Data.CurrentPhase >= 1f)
                {
                    fAudio.Audio.PlaySoundFrontend(-1, "REMOTE_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
                    OnValueAdded(GetRandomCashAmount(), true); OnValueAdded(GetRandomCashAmount(), true);
                    Screen.ClearHelpText();
                    RemoveBlip();
                    IsLooted = true;
                    PlayExitScene();
                }
            }
        }
        

        private int GetRandomCashAmount()
            => Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 4000, 10000);

        protected virtual void OnValueAdded(int amount, bool isCash)
            => ValueAdded?.Invoke(this, new MinigameValueAddedArgs(amount, isCash));

        private void PlayCashSound()
            => Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);

        private void ShowPromptNearMachine(string gxtEntry)
        {
            fCore.Streaming.RequestAnimDict(AnimDict);

            Vector3 bottomOffset = fWorld.Entity.GetOffsetFromEntityInWorldCoords(machine, 0f, 0f, -0.45f);
            Vector3 topOffset = fWorld.Entity.GetOffsetFromEntityInWorldCoords(machine, 0f, 1.3f, 1.5f);
            Vector3 animStartPos = fWorld.Entity.GetAnimInitialOffsetPosition(AnimDict, "enter", machine!.Position, machine.Rotation);

            bool inInteractionArea = fWorld.Entity.IsEntityInAngledArea(fPlayer.Player.Character, bottomOffset, topOffset, InteractionDistance, false, true, 1) || fPlayer.Player.Character.Position.DistanceTo(animStartPos) < 1f;

            if (inInteractionArea && CanPlayerInteract())
                fUI.Hud.DisplayHelpTextThisFrame(gxtEntry);
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
            if (machine?.AttachedBlip != null)
                machine.AttachedBlip.Alpha = 255;

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

            moneyScene?.Dispose();
            moneyScene = null;
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

            if (machine != null && machine.Exists())
            {
                machine.AttachedBlip?.Delete();
                machine.Delete();
                machine = null;
            }
            if (machineMoney != null && machineMoney.Exists())
            {
                machineMoney.Delete();
                machineMoney = null;
            }
            if (machineCol != null && machineCol.Exists())
            {
                machineCol.Delete();
                machineCol = null;
            }

            CleanupProps();
            CleanupScenes();
            CleanupResources();

            ActiveMachines.Remove(this);
        }

        private void CleanupProps()
        {
            if (bag != null && bag.Exists())
            {
                bag.Delete();
                bag = null;
            }
            if (moneyPropLeft != null && moneyPropLeft.Exists())
            {
                moneyPropLeft.Delete();
                moneyPropLeft = null;
            }
            if (moneyPropRight != null && moneyPropRight.Exists())
            {
                moneyPropRight.Delete();
                moneyPropRight = null;
            }
        }

        private void CleanupScenes()
        {
            CasinoTableCam = false;
            machineScene?.Dispose();
            machineScene = null;
            playerScene?.Dispose();
            playerScene = null;
            moneyScene?.Dispose();
            moneyScene = null;
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