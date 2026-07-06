using GTA;
using GTA.Math;
using GTA.Native;
using System;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace IMNOTEPIX.Framework.fWorld
{
    /// <summary>
    /// Replicates R*'s hint camera system from the decompiled YSC scripts.
    /// Handles showing a help message, focusing in on a target entity when the
    /// player presses the cinematic cam button, and focusing back out on a second press.
    /// </summary>
    public class HintCamera
    {
        // ---------------------------------------------------------------
        // Configuration
        // ---------------------------------------------------------------

        /// <summary>The entity the hint camera will focus on.</summary>
        public GTA.Entity TargetEntity { get; set; }

        /// <summary>
        /// Offset from the entity's origin that the camera focuses on.
        /// Use Vector3.Zero to focus on the entity's root position.
        /// For peds you typically want (0, 0, 1) to aim at the upper body.
        /// </summary>
        public Vector3 TargetOffset { get; set; } = Vector3.Zero;

        /// <summary>
        /// GXT key of the help message shown while the hint is inactive.
        /// Defaults to CMN_HINT which reads "Press [button] to toggle focus on the target."
        /// </summary>
        public string HelpMessageGxt { get; set; } = "CMN_HINT";

        /// <summary>
        /// String of the help message shown while the hint is inactive, used if the HelpMessageGxt is unavailable or not fit for a certain context.
        /// Defaults to "~s~Press ~INPUT_VEH_CIN_CAM~ to toggle focus on the target."
        /// </summary>
        public string[] HelpMessage { get; set; } =
        [
            "~s~Press ~INPUT_VEH_CIN_CAM~ to toggle focus on the target."
        ];

        /// <summary>How long in milliseconds the hint camera holds after firing.</summary>
        public int HintDuration { get; set; } = 3000;

        /// <summary>Cooldown in milliseconds before the hint can fire again.</summary>
        public int HintCooldown { get; set; } = 3000;

        /// <summary>
        /// The gameplay hint type passed to SET_GAMEPLAY_ENTITY_HINT.
        /// Use GameplayHintType enum values — e.g. GameplayHintType.Default,
        /// GameplayHintType.VehicleHighZoom, etc.
        /// </summary>
        public CameraHintHelperNameHash HintHash { get; set; } = 0;

        // ---------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------

        /// <param name="Target"><inheritdoc cref="fWorld.HintCamera.TargetEntity"/></param>
        /// <param name="Offset"><inheritdoc cref="fWorld.HintCamera.TargetOffset"/></param>
        /// <param name="HelpMessageGXT"><inheritdoc cref="fWorld.HintCamera.HelpMessageGxt"/></param>
        /// <param name="HintDuration"><inheritdoc cref="fWorld.HintCamera.HintDuration"/></param>
        /// <param name="HintCooldown"><inheritdoc cref="fWorld.HintCamera.HintCooldown"/></param>
        /// <param name="HintHash"><inheritdoc cref="fWorld.HintCamera.HintHash"/></param>
        public HintCamera(GTA.Entity Target, Vector3 Offset = default, string HelpMessageGXT = "CMN_HINT", int HintDuration = 3000, int HintCooldown = 3000, CameraHintHelperNameHash HintHash = 0)
        {
            TargetEntity = Target;
            TargetOffset = Offset;
            HelpMessageGxt = string.IsNullOrEmpty(HelpMessageGXT) ? HelpMessageGxt : HelpMessageGXT;
            this.HintDuration = HintDuration;
            this.HintCooldown = HintCooldown;
            this.HintHash = HintHash;
        }

        /// <param name="Target"><inheritdoc cref="fWorld.HintCamera.TargetEntity"/></param>
        /// <param name="Offset"><inheritdoc cref="fWorld.HintCamera.TargetOffset"/></param>
        /// <param name="HelpMessage"><inheritdoc cref="fWorld.HintCamera.HelpMessage"/></param>
        /// <param name="HintDuration"><inheritdoc cref="fWorld.HintCamera.HintDuration"/></param>
        /// <param name="HintCooldown"><inheritdoc cref="fWorld.HintCamera.HintCooldown"/></param>
        /// <param name="HintHash"><inheritdoc cref="fWorld.HintCamera.HintHash"/></param>
        public HintCamera(GTA.Entity Target, Vector3 Offset = default, int HintDuration = 3000, int HintCooldown = 3000, CameraHintHelperNameHash HintHash = 0, params string[] HelpMessage)
        {
            TargetEntity = Target;
            TargetOffset = Offset;
            this.HelpMessage = HelpMessage.Length <= 0 ? this.HelpMessage : HelpMessage;
            HelpMessageGxt = "";
            this.HintDuration = HintDuration;
            this.HintCooldown = HintCooldown;
            this.HintHash = HintHash;
        }

        // ---------------------------------------------------------------
        // State
        // ---------------------------------------------------------------

        /// <summary>Whether the hint camera is currently active.</summary>
        public bool IsHintActive => GameplayCamera.IsHintActive;

        // ---------------------------------------------------------------
        // Internal
        // ---------------------------------------------------------------

        private enum ButtonState { Idle, Down, Held }

        private ButtonState _btnState = ButtonState.Idle;
        private int _btnTimestamp = 0;
        private bool _fxPlayed = false;
        private bool _helpShown = false;

        // ---------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------

        /// <summary>
        /// Call this every tick inside your OnTick handler.
        /// Handles help text, input detection, and camera transitions automatically.
        /// </summary>
        public void Update()
        {
            if (TargetEntity == null || !TargetEntity.Exists() || !IsEntityAlive(TargetEntity))
            {
                Reset();
                return;
            }

            if (IsHintActive)
                UpdateActive();
            else
                UpdateIdle();
        }

        /// <summary>
        /// Force stops the hint camera and resets all internal state.
        /// Safe to call at any time, e.g. when a mission ends or the player dies.
        /// </summary>
        public void Reset()
        {
            StopHint(immediate: false);
            _btnState = ButtonState.Idle;
            _helpShown = false;
        }

        /// <summary>
        /// Force stops the hint camera and resets all internal state. 
        /// Disposes of all configuration.
        /// </summary>
        public void Dispose()
        {
            StopHint(immediate: false);
            _btnState = ButtonState.Idle;
            _helpShown = false;

            TargetEntity = null;
            TargetOffset = default;
            HelpMessage = null;
            HelpMessageGxt = null;
            HintDuration = default;
            HintCooldown = default;
            HintHash = default;
        }

        // ---------------------------------------------------------------
        // Core update paths
        // ---------------------------------------------------------------

        /// <summary>
        /// Runs while the hint camera is NOT active.
        /// Refreshes the help text every tick and listens for button press.
        /// </summary>
        private void UpdateIdle()
        {
            if (!AreHintsAllowed())
            {
                _btnState = ButtonState.Idle;
                return;
            }

            // Help text must be submitted every tick to stay visible,
            // same as how R*'s script calls it continuously in the update loop.
            if (TargetEntity.IsOnScreen && IsEntityVisible(TargetEntity))
            {
                if (!string.IsNullOrEmpty(HelpMessageGxt))
                {
                    fUI.Hud.DisplayGXTHelpText(HelpMessageGxt);
                    _helpShown = true;
                }
                else
                {
                    fUI.Hud.DisplayHelpText(HelpMessage);
                    _helpShown = true;
                }
            }
            else if (_helpShown)
            {
                Function.Call(Hash.CLEAR_HELP, false);
                _helpShown = false;
            }

            if (TickButtonFSM())
                FireHint();
        }

        /// <summary>
        /// Runs while the hint camera IS active.
        /// Listens for a second button press to focus back out.
        /// </summary>
        private void UpdateActive()
        {
            if (!AreHintsAllowed())
            {
                StopHint(immediate: true);
                return;
            }

            if (TickButtonFSM())
                StopHint(immediate: false);
        }

        // ---------------------------------------------------------------
        // Button FSM
        // ---------------------------------------------------------------

        /// <summary>
        /// Tracks the cinematic cam button state and returns true
        /// the frame a confirmed press (tap or hold-release) is detected.
        /// Mirrors the logic of func_4230 from the decompiled script.
        /// </summary>
        private bool TickButtonFSM()
        {
            // Suppress the button so the game doesn't handle it natively
            GTA.Input.Controls.DisableControlActionThisFrame(GTA.Input.ControlType.PlayerControl, GTA.Input.ControlAction.VehicleCinCam);

            if (!Game.Player.Character.IsInVehicle())
                GTA.Input.Controls.DisableControlActionThisFrame(GTA.Input.ControlType.PlayerControl, GTA.Input.ControlAction.MeleeAttackLight);

            bool pressed = GTA.Input.Controls.IsDisabledControlPressed(GTA.Input.ControlType.PlayerControl, GTA.Input.ControlAction.VehicleCinCam);
            int now = Game.GameTime;

            switch (_btnState)
            {
                case ButtonState.Idle:
                    if (pressed)
                    {
                        _btnTimestamp = now;
                        _btnState = ButtonState.Down;
                    }
                    break;

                case ButtonState.Down:
                    if (!pressed)
                    {
                        // Released within 500ms = confirmed tap
                        _btnState = ButtonState.Idle;
                        return true;
                    }
                    if (now - _btnTimestamp > 500)
                        _btnState = ButtonState.Held;
                    break;

                case ButtonState.Held:
                    if (!pressed)
                    {
                        // Released from a hold = also confirmed
                        _btnState = ButtonState.Idle;
                        return true;
                    }
                    break;
            }

            return false;
        }

        // ---------------------------------------------------------------
        // Focus in — mirrors func_4238
        // ---------------------------------------------------------------

        /// <summary>
        /// Fires the hint camera, plays FocusIn effects, and starts the audio scene.
        /// Mirrors func_4238 from the decompiled script.
        /// </summary>
        private void FireHint()
        {
            if (_helpShown)
            {
                Function.Call(Hash.CLEAR_HELP, true);
                _helpShown = false;
            }

            int duration = HintDuration;
            int cooldown = HintCooldown;

            // R*'s script enforces a minimum of 1500ms for the high zoom hint
            if ((int)HintHash == (int)StringHash.AtStringHashUtf8("VEHICLE_HIGH_ZOOM_HINT_HELPER"))
            {
                duration = Math.Max(duration, 1500);
                cooldown = Math.Max(cooldown, 1500);
            }

            Function.Call(Hash.SET_CINEMATIC_BUTTON_ACTIVE, false);

            GameplayCamera.SetEntityHint(TargetEntity, TargetOffset, true, duration, cooldown, 90000, HintHash);

            Game.Player.Character.Task.LookAt(TargetEntity, -1, (LookAtFlags)2048, (LookAtPriority)3);

            Function.Call(Hash.ANIMPOSTFX_PLAY, "FocusIn", 0, false);
            Function.Call(Hash.START_AUDIO_SCENE, "HINT_CAM_SCENE");
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "FocusIn", "HintCamSounds", true);

            _fxPlayed = true;
            _btnState = ButtonState.Idle;
        }

        // ---------------------------------------------------------------
        // Focus out — mirrors func_4220 / func_4240
        // ---------------------------------------------------------------

        /// <summary>
        /// Stops the hint camera, plays FocusOut effects, and cleans up.
        /// Mirrors func_4220 and func_4240 from the decompiled script.
        /// </summary>
        /// <param name="immediate">
        /// If true, cuts the camera immediately with no blend.
        /// If false, blends back to gameplay camera smoothly.
        /// </param>
        private void StopHint(bool immediate)
        {
            GTA.Ped ped = Game.Player.Character;

            if (ped != null && ped.Exists())
                Function.Call(Hash.TASK_CLEAR_LOOK_AT, ped.Handle);

            if (IsHintActive)
            {
                Function.Call(Hash.SET_CINEMATIC_BUTTON_ACTIVE, true);
                GameplayCamera.StopGameplayHint(immediate);
                Function.Call(Hash.STOP_AUDIO_SCENE, "HINT_CAM_SCENE");
                Function.Call(Hash.ANIMPOSTFX_STOP, "FocusIn");

                if (_fxPlayed)
                {
                    Function.Call(Hash.ANIMPOSTFX_PLAY, "FocusOut", 0, false);
                    Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "FocusOut", "HintCamSounds", true);
                    _fxPlayed = false;
                }
            }

            _btnState = ButtonState.Idle;
            _helpShown = false;
        }

        // ---------------------------------------------------------------
        // Guards
        // ---------------------------------------------------------------

        /// <summary>
        /// Returns false in situations where hints should be suppressed,
        /// such as player switch, typing in chat, or first-person/cinematic cam.
        /// </summary>
        private static bool AreHintsAllowed()
        {
            if (!Game.Player.CanControlCharacter)
                return false;

            if (Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS))
                return false;

            if (Function.Call<bool>(Hash.NETWORK_TEXT_CHAT_IS_TYPING))
                return false;

            GTA.Ped ped = Game.Player.Character;
            if (ped.IsInVehicle())
            {
                if (GameplayCamera.IsLookingBehind)
                    return false;

                int camMode = GetVehicleCamMode(ped);
                if (camMode == 3 || camMode == 4)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the active camera view mode for the player's current vehicle type.
        /// Context IDs match those used in the decompiled R* scripts.
        /// </summary>
        private static int GetVehicleCamMode(GTA.Ped ped)
        {
            if (ped.IsInBoat) return Function.Call<int>(Hash.GET_CAM_VIEW_MODE_FOR_CONTEXT, 3);
            if (ped.IsInHeli) return Function.Call<int>(Hash.GET_CAM_VIEW_MODE_FOR_CONTEXT, 6);
            if (ped.IsInPlane) return Function.Call<int>(Hash.GET_CAM_VIEW_MODE_FOR_CONTEXT, 4);
            if (ped.IsOnBike) return Function.Call<int>(Hash.GET_CAM_VIEW_MODE_FOR_CONTEXT, 2);
            return Function.Call<int>(Hash.GET_FOLLOW_VEHICLE_CAM_VIEW_MODE);
        }

        // ---------------------------------------------------------------
        // Entity helpers
        // ---------------------------------------------------------------

        /// <summary>
        /// Returns false if the entity is dead or undriveable,
        /// which mirrors the func_4221 check in the decompiled script.
        /// </summary>
        private static bool IsEntityAlive(GTA.Entity entity)
        {
            if (entity is GTA.Ped ped) return !ped.IsDead;
            if (entity is GTA.Vehicle veh) return veh.IsDriveable;
            return true;
        }

        /// <summary>
        /// Returns true if the entity is currently visible on screen.
        /// Uses tracked visibility for peds and vehicles where available,
        /// falling back to IsOnScreen for objects.
        /// </summary>
        private static bool IsEntityVisible(GTA.Entity entity)
        {
            if (entity is GTA.Ped ped)
            {
                int pedIndex = Function.Call<int>(Hash.GET_PED_INDEX_FROM_ENTITY_INDEX, ped.Handle);
                Function.Call(Hash.REQUEST_PED_VISIBILITY_TRACKING, pedIndex);
                return Function.Call<bool>(Hash.IS_TRACKED_PED_VISIBLE, pedIndex);
            }

            if (entity is GTA.Vehicle veh)
            {
                int vehIndex = Function.Call<int>(Hash.GET_VEHICLE_INDEX_FROM_ENTITY_INDEX, veh.Handle);
                Function.Call(Hash.TRACK_VEHICLE_VISIBILITY, vehIndex);
                return Function.Call<bool>(Hash.IS_VEHICLE_VISIBLE, vehIndex);
            }

            return entity.IsOnScreen;
        }
    }
}