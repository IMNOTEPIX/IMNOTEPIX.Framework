using GTA.Native;
using IMNOTEPIX.Framework.fCore.Internals;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Replaces the earlier hardcoded-false stub now that the "ingamehud" script-global
    /// pattern scan is actually ported (see fCore.Internals.Memory.ScriptGlobals.cs).
    /// The RPH original read these via a raw pointer into Game.GetScriptGlobalVariableAddress(id);
    /// SHVDN exposes a managed equivalent, GTA.Native.GlobalVariable, so this uses
    /// GlobalVariable.Get(id).GetStructField(offset * 8).GetValue&lt;T&gt;()/SetValue&lt;T&gt;() instead -
    /// same addressing (offset is in 8-byte script-value units, hence * 8), no unsafe code needed.
    /// </summary>
    internal static class TimerBarScriptGlobals
    {
        public static bool TimersBarsTotalHeightAvailable =>
            Memory.TimershudSharedGlobalId != -1 &&
            Memory.TimershudSharedTimerbarsTotalHeightOffset != -1 &&
            Memory.TimerbarsPrevTotalHeightGlobalId != -1;

        public static float TimerBarsTotalHeight
        {
            get => GlobalVariable.Get(Memory.TimershudSharedGlobalId)
                                  .GetStructField(Memory.TimershudSharedTimerbarsTotalHeightOffset * 8)
                                  .Read<float>();
            set => GlobalVariable.Get(Memory.TimershudSharedGlobalId)
                                  .GetStructField(Memory.TimershudSharedTimerbarsTotalHeightOffset * 8)
                                  .Write(value);
        }

        public static float TimerBarsPrevTotalHeight
        {
            get => GlobalVariable.Get(Memory.TimerbarsPrevTotalHeightGlobalId).Read<float>();
            set => GlobalVariable.Get(Memory.TimerbarsPrevTotalHeightGlobalId).Write(value);
        }

        public static bool TimerBarsInstructionButtonsNumRowsAvailable =>
            Memory.TimershudSharedGlobalId != -1 &&
            Memory.TimershudSharedInstructionalButtonsNumRowsOffset != -1;

        public static int TimerBarsInstructionButtonsNumRows
        {
            get => GlobalVariable.Get(Memory.TimershudSharedGlobalId)
                                  .GetStructField(Memory.TimershudSharedInstructionalButtonsNumRowsOffset * 8)
                                  .Read<int>();
            set => GlobalVariable.Get(Memory.TimershudSharedGlobalId)
                                  .GetStructField(Memory.TimershudSharedInstructionalButtonsNumRowsOffset * 8)
                                  .Write(value);
        }
    }
}