using GTA;
using GTA.Native;

namespace IMNOTEPIX.Framework.Funcs
{
    public static class fScript
    {
        public static void TerminateThisThread()
            => Function.Call(Hash.TERMINATE_THIS_THREAD);

        public static void SetThisScriptCanBePaused(bool toggle)
            => Function.Call(Hash.SET_THIS_SCRIPT_CAN_BE_PAUSED, toggle);

        public static void SetThisScriptCanRemoveBlipsCreatedByAnyScript(bool toggle)
            => Function.Call(Hash.SET_THIS_SCRIPT_CAN_REMOVE_BLIPS_CREATED_BY_ANY_SCRIPT, toggle);

        /// <summary>
        /// Returns the thread ID of the newly started script, or 0 if the native fails.
        /// </summary>
        public static int StartNewScriptWithNameHashAndArgs(
            Hash scriptHash, int args, int argCount, int stackSize)
        {
            return Function.Call<int>(
                Hash.START_NEW_SCRIPT_WITH_NAME_HASH_AND_ARGS,
                scriptHash, args, argCount, stackSize);
        }

        /// <summary>
        /// Requests, waits for, starts, and releases a script by name.
        ///
        /// Fixed: original used fixed Script.Wait(100/500) regardless of load time.
        /// Now yields until the script is confirmed loaded before starting it.
        /// </summary>
        public static void StartScript(string scriptName, int stackSize)
        {
            while (!Function.Call<bool>(Hash.HAS_SCRIPT_LOADED, scriptName))
            {
                Function.Call(Hash.REQUEST_SCRIPT, scriptName);
                Script.Yield();
            }

            Function.Call(Hash.START_NEW_SCRIPT, scriptName, stackSize);
            Function.Call(Hash.SET_SCRIPT_AS_NO_LONGER_NEEDED, scriptName);
        }
    }
}