using GTA;
using GTA.Native;
using System.Drawing;

namespace IMNOTEPIX.Framework.fPlayer
{
    public static class Input
    {
        public static bool IsControlEnabled(int control, int action)
            => Function.Call<bool>(Hash.IS_CONTROL_ENABLED, control, action);

        public static bool IsControlPressed(int control, int action)
            => Function.Call<bool>(Hash.IS_CONTROL_PRESSED, control, action);

        public static bool IsControlReleased(int control, int action)
            => Function.Call<bool>(Hash.IS_CONTROL_RELEASED, control, action);

        public static bool IsControlJustPressed(int control, int action)
            => Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, control, action);

        public static bool IsControlJustReleased(int control, int action)
            => Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, control, action);

        public static bool IsControlHeldDown(int control, int action, int duration)
            => Function.Call<bool>((Hash)0xE9CB8C56E90D5079, control, action, duration);

        public static int GetControlValue(int control, int action)
            => Function.Call<int>(Hash.GET_CONTROL_VALUE, control, action);

        public static float GetControlNormal(int control, int action)
            => Function.Call<float>(Hash.GET_CONTROL_NORMAL, control, action);

        public static void SetUseAdjustedMouseCoords(bool toggle)
            => Function.Call(Hash.SET_USE_ADJUSTED_MOUSE_COORDS, toggle);

        public static float GetControlUnboundNormal(int control, int action)
            => Function.Call<float>(Hash.GET_​CONTROL_​UNBOUND_​NORMAL, control, action);

        public static bool SetControlValueNextFrame(int control, int action, float value)
            => Function.Call<bool>(Hash.SET_​CONTROL_​VALUE_​NEXT_​FRAME, control, action, value);

        public static bool IsDisabledControlPressed(int control, int action)
            => Function.Call<bool>(Hash.IS_​DISABLED_​CONTROL_​PRESSED, control, action);

        public static bool IsDisabledControlReleased(int control, int action)
            => Function.Call<bool>(Hash.IS_​DISABLED_​CONTROL_​RELEASED, control, action);

        public static bool IsDisabledControlJustPressed(int control, int action)
            => Function.Call<bool>(Hash.IS_​DISABLED_​CONTROL_​JUST_​PRESSED, control, action);

        public static bool IsDisabledControlJustReleased(int control, int action)
            => Function.Call<bool>(Hash.IS_​DISABLED_​CONTROL_​JUST_​RELEASED, control, action);

        public static float GetDisabledControlNormal(int control, int action)
            => Function.Call<float>(Hash.GET_​DISABLED_​CONTROL_​NORMAL, control, action);

        public static float GetDisabledControlUnboundNormal(int control, int action)
            => Function.Call<float>(Hash.GET_​DISABLED_​CONTROL_​UNBOUND_​NORMAL, control, action);

        public static int GetControlHowLongAgo(int control)
            => Function.Call<int>(Hash.GET_​CONTROL_​HOW_​LONG_​AGO, control);

        public static bool IsUsingKeyboardAndMouse(int control)
            => Function.Call<bool>(Hash.IS_USING_KEYBOARD_AND_MOUSE, control);

        public static bool IsUsingCursor(int control)
            => Function.Call<bool>(Hash.IS_​USING_​CURSOR, control);

        public static bool SetCursorPosition(float x, float y)
            => Function.Call<bool>(Hash.SET_​CURSOR_​POSITION, x, y);

        public static bool IsUsingRemotePlay(int control)
            => Function.Call<bool>(Hash.IS_​USING_​REMOTE_​PLAY, control);

        public static bool HaveControlsChanged(int control)
            => Function.Call<bool>(Hash.HAVE_​CONTROLS_​CHANGED, control);

        public static string GetControlInstructionalButtonsString(int control, int action, bool allowXOSwap)
            => Function.Call<string>(Hash.GET_​CONTROL_​INSTRUCTIONAL_​BUTTONS_​STRING, control, action, allowXOSwap);

        public static string GetControlGroupInstructionalButtonsString(int control, int controlGroup, bool allowXOSwap)
            => Function.Call<string>(Hash.GET_​CONTROL_​GROUP_​INSTRUCTIONAL_​BUTTONS_​STRING, control, controlGroup, allowXOSwap);

        public static void SetControlLightEffectColor(int control, int red, int green, int blue)
            => Function.Call(Hash.SET_​CONTROL_​LIGHT_​EFFECT_​COLOR, control, red, green, blue);

        public static void SetControlLightEffectColor(int control, Color color)
            => Function.Call(Hash.SET_​CONTROL_​LIGHT_​EFFECT_​COLOR, control, color.R, color.G, color.B);

        public static void ClearControlLightEffect(int control)
            => Function.Call(Hash.CLEAR_​CONTROL_​LIGHT_​EFFECT, control);

        public static void SetControlShake(int control, int duration, int frequency)
            => Function.Call(Hash.SET_CONTROL_SHAKE, control, duration, frequency);

        public static void SetControlTriggerShake(int control, int leftDuration, int leftFrequency, int rightDuration, int rightFrequency)
            => Function.Call(Hash.SET_​CONTROL_​TRIGGER_​SHAKE, control, leftDuration, leftFrequency, rightDuration, rightFrequency);

        public static void StopControlShake(int control)
            => Function.Call(Hash.STOP_​CONTROL_​SHAKE, control);

        public static void SetControlShakeSuppressedId(int control, int uniqueId)
            => Function.Call(Hash.SET_​CONTROL_​SHAKE_​SUPPRESSED_​ID, control, uniqueId);

        public static void ClearControlShakeSuppressedId(int control)
            => Function.Call(Hash.CLEAR_​CONTROL_​SHAKE_​SUPPRESSED_​ID, control);

        public static bool IsLookInverted()
            => Function.Call<bool>(Hash.IS_​LOOK_​INVERTED);

        public static bool IsMouseLookInverted()
            => Function.Call<bool>(Hash.IS_​MOUSE_​LOOK_​INVERTED);

        public static int GetLocalPlayerAimState()
            => Function.Call<int>(Hash.GET_​LOCAL_​PLAYER_​AIM_​STATE);

        public static int GetLocalPlayerGamepadAimState()
            => Function.Call<int>(Hash.GET_​LOCAL_​PLAYER_​GAMEPAD_​AIM_​STATE);

        public static bool GetIsUsingAlternateHandbrake()
            => Function.Call<bool>(Hash.GET_​IS_​USING_​ALTERNATE_​HANDBRAKE);

        public static bool GetIsUsingAlternateDriveby()
            => Function.Call<bool>(Hash.GET_​IS_​USING_​ALTERNATE_​DRIVEBY);

        public static bool GetAllowMovementWhileZoomed()
            => Function.Call<bool>(Hash.GET_​ALLOW_​MOVEMENT_​WHILE_​ZOOMED);

        public static void SetPlayerpadShakesWhenControllerDisabled(bool toggle)
            => Function.Call(Hash.SET_​PLAYERPAD_​SHAKES_​WHEN_​CONTROLLER_​DISABLED, toggle);

        public static void SetInputExclusive(int control, int action)
            => Function.Call(Hash.SET_​INPUT_​EXCLUSIVE, control, action);

        public static void DisableControlAction(int control, int action, bool disableRelatedActions)
            => Function.Call(Hash.DISABLE_CONTROL_ACTION, control, action, disableRelatedActions);

        public static void EnableControlAction(int control, int action, bool enableRelatedActions)
            => Function.Call(Hash.ENABLE_CONTROL_ACTION, control, action, enableRelatedActions);

        public static void DisableAllControlActions(int control)
            => Function.Call(Hash.DISABLE_​ALL_​CONTROL_​ACTIONS, control);

        public static void EnableAllControlActions(int control)
            => Function.Call(Hash.ENABLE_​ALL_​CONTROL_​ACTIONS, control);

        public static bool InitPcScriptedControls(string schemeName)
            => Function.Call<bool>(Hash.INIT_​PC_​SCRIPTED_​CONTROLS, schemeName);

        public static bool SwitchPcScriptControls(string schemeName)
            => Function.Call<bool>(Hash.SWITCH_​PC_​SCRIPTED_​CONTROLS, schemeName);

        public static void ShutdownPcScriptControls()
            => Function.Call(Hash.SHUTDOWN_​PC_​SCRIPTED_​CONTROLS);

        public static void AllowAlternativeScriptControlsLayout(int control)
            => Function.Call(Hash.ALLOW_​ALTERNATIVE_​SCRIPT_​CONTROLS_​LAYOUT, control);

        public static int GetGamepadType()
            => Function.Call<int>((Hash)0x18E474F40EF05F10);
    }
}
