using GTA;
using GTA.Native;
using IMNOTEPIX.Framework.fCore;
using IMNOTEPIX.Framework.fUI.Scaleforms;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Ported from RAGENativeUI's TimerBarPool. Behavior-preserving changes from the original:
    /// <list type="bullet">
    /// <item>N.* native wrapper calls -&gt; GTA.Native.Function.Call.</item>
    /// <item>Internals.Shared (cross-process memory-mapped fields) -&gt; TimerBarsShared (plain
    /// static fields) - see TimerBarsShared.cs for why that's equivalent here.</item>
    /// <item>Internals.ScriptGlobals (pattern-scanned into the game's "ingamehud" script) -&gt;
    /// TimerBarScriptGlobals, now a real port backed by GTA.Native.GlobalVariable
    /// (see TimerBarScriptGlobals.cs and fCore.Internals.Memory.ScriptGlobals.cs).</item>
    /// <item>The original's `ref float totalHeight = ref (ScriptGlobals field or Shared field)`
    /// aliasing trick transparently shared one of three possible backing floats through a
    /// single ref-local. C# can't alias a GlobalVariable-backed property the same way (it's not
    /// a real memory location from the caller's side), so this tracks which source was picked
    /// with a small enum and writes back explicitly at the end instead - same net read/write
    /// behavior, just without the ref-local indirection.</item>
    /// <item>InstructionalButtons.NumberOfRows - carried over by name; confirm this still matches
    /// whatever your ported InstructionalButtons class exposes.</item>
    /// </list>
    /// </summary>
    public class TimerBarPool : BaseCollection<TimerBarBase>
    {
        private enum TotalHeightSource { None, ScriptGlobalsTotal, ScriptGlobalsPrevTotal, Shared }

        private float lastY = TB.InitialY;

        public List<TimerBarBase> ToList()
        {
            return InternalList;
        }

        // added for backwards compatibility, BaseCollection.Remove returns a bool and this one doesn't return anything
        public new void Remove(TimerBarBase item)
        {
            base.Remove(item);
        }

        public void Draw()
        {
            if (InternalList.Count == 0)
            {
                return;
            }

            Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, TB.BgTextureDictionary, true);
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, TB.BgTextureDictionary))
            {
                return;
            }

            Function.Call(Hash.SET_SCRIPT_GFX_ALIGN, (int)'R', (int)'B');
            Function.Call(Hash.SET_SCRIPT_GFX_ALIGN_PARAMS, 0.0f, 0.0f, 0.952f, 0.949f);

            // NOTE: verify Game.FrameCount exists in your SHVDN build (used purely to detect
            // "first Draw() call this frame" across possibly multiple TimerBarPools).
            uint frame = (uint)Game.FrameCount;
            bool newFrame = frame != TimerBarsShared.TimerBarsLastFrame;
            if (newFrame)
            {
                TimerBarsShared.TimerBarsLastFrame = frame;

                if (TimerBarScriptGlobals.TimersBarsTotalHeightAvailable)
                {
                    // NOTE: verify GET_NUMBER_OF_REFERENCES_OF_SCRIPT_WITH_NAME_HASH is the exact
                    // Hash member name in your SHVDN build. 0xC45650F0 is "ingamehud"'s hash,
                    // carried over unchanged from the original.
                    TimerBarsShared.IngamehudScriptExecuting =
                        Function.Call<int>(Hash.GET_NUMBER_OF_THREADS_RUNNING_THE_SCRIPT_WITH_THIS_HASH, 0xC45650F0u) > 0;
                }

                Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 6); // VehicleName
                Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 7); // AreaName
                Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 8); // ?
                Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 9); // StreetName
            }

            // NOTE: InstructionalButtons.NumberOfRows - confirm this member name matches your
            // already-ported InstructionalButtons class.
            float x = TB.InitialX, y = TB.InitialY - (TB.LoadingPromptYOffset * InstructionalButtons.NumberOfRows);

            bool hasTotalHeight = true;
            var source = TotalHeightSource.None;
            float totalHeight = 0.0f;

            if (fCore.Internals.SHVDNConsole.IsOpen)
            {
                y = lastY;
                hasTotalHeight = false;
            }
            else if (TimerBarScriptGlobals.TimersBarsTotalHeightAvailable && TimerBarsShared.IngamehudScriptExecuting)
            {
                float timerBarsTotalHeight = TimerBarScriptGlobals.TimerBarsTotalHeight;
                float timerBarsPrevTotalHeight = TimerBarScriptGlobals.TimerBarsPrevTotalHeight;

                bool useTotal = timerBarsTotalHeight > timerBarsPrevTotalHeight;
                source = useTotal ? TotalHeightSource.ScriptGlobalsTotal : TotalHeightSource.ScriptGlobalsPrevTotal;
                totalHeight = useTotal ? timerBarsTotalHeight : timerBarsPrevTotalHeight;

                if (totalHeight > 0.0f)
                {
                    y -= totalHeight + 0.0075f;
                }
                else
                {
                    totalHeight = -0.0075f;
                }
            }
            else
            {
                source = TotalHeightSource.Shared;
                totalHeight = TimerBarsShared.TimerBarsTotalHeight;

                if (newFrame)
                {
                    totalHeight = 0.0f;
                }
                else
                {
                    y -= totalHeight;
                }
            }

            lastY = y;
            for (int i = 0; i < InternalList.Count; i++)
            {
                TimerBarBase b = InternalList[i];
                b.Draw(x, y);
                y -= b.Thin ? TB.SmallHeightWithGap : TB.DefaultHeightWithGap;
            }

            if (hasTotalHeight)
            {
                // update the total height so other TimerBarPools don't overlap
                totalHeight += lastY - y;

                switch (source)
                {
                    case TotalHeightSource.ScriptGlobalsTotal:
                        TimerBarScriptGlobals.TimerBarsTotalHeight = totalHeight;
                        break;
                    case TotalHeightSource.ScriptGlobalsPrevTotal:
                        TimerBarScriptGlobals.TimerBarsPrevTotalHeight = totalHeight;
                        break;
                    case TotalHeightSource.Shared:
                        TimerBarsShared.TimerBarsTotalHeight = totalHeight;
                        break;
                }
            }

            Function.Call(Hash.RESET_SCRIPT_GFX_ALIGN);
        }
    }
}