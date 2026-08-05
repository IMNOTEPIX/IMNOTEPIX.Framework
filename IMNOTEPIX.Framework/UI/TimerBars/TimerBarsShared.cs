namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Replaces RAGENativeUI's Internals.Shared for the timer-bar fields only.
    /// The original backed these with a cross-process memory-mapped file so multiple
    /// independent RPH plugin copies (potentially separate AppDomains) could coordinate
    /// stacking of TimerBarPools. IMNOTEPIX.Framework is a single unified library, not many
    /// isolated copies needing to agree across process boundaries, so plain static fields
    /// are equivalent here - dropped the memory-mapped-file plumbing entirely.
    /// </summary>
    internal static class TimerBarsShared
    {
        public static uint TimerBarsLastFrame;
        public static float TimerBarsTotalHeight;

        /// <summary>
        /// Whether the game's own "ingamehud" script is currently loaded/running - only
        /// meaningful once per frame, refreshed in TimerBarPool.Draw() alongside
        /// TimerBarScriptGlobals.TimersBarsTotalHeightAvailable being true.
        /// </summary>
        public static bool IngamehudScriptExecuting;
    }
}