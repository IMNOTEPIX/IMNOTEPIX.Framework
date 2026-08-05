using GTA.Native;

namespace IMNOTEPIX.Framework.fCore
{
    public class Script
    {
        public static void RequestScript(string scriptName)
            => Function.Call(Hash.REQUEST_​SCRIPT, scriptName);

        public static void SetScriptAsNoLongerNeeded(string scriptName)
            => Function.Call(Hash.SET_​SCRIPT_​AS_​NO_​LONGER_​NEEDED, scriptName);

        public static bool HasScriptLoaded(string scriptName)
            => Function.Call<bool>(Hash.HAS_​SCRIPT_​LOADED, scriptName);

        public static bool DoesScriptExist(string scriptName)
            => Function.Call<bool>(Hash.DOES_​SCRIPT_​EXIST, scriptName);

        public static void RequestScriptWithNameHash(uint scriptHash)
            => Function.Call(Hash.REQUEST_​SCRIPT_​WITH_​NAME_​HASH, scriptHash);

        public static void SetScriptWithNameHashAsNoLongerNeeded(uint scriptHash)
            => Function.Call(Hash.SET_​SCRIPT_​WITH_​NAME_​HASH_​AS_​NO_​LONGER_​NEEDED, scriptHash);

        public static bool HasScriptWithNameHashLoaded(uint scriptHash)
            => Function.Call<bool>(Hash.HAS_​SCRIPT_​WITH_​NAME_​HASH_​LOADED, scriptHash);

        public static bool DoesScriptWithNameHashExist(uint scriptHash)
            => Function.Call<bool>(Hash.DOES_​SCRIPT_​WITH_​NAME_​HASH_​EXIST, scriptHash);

        public static void TerminateThread(int threadId)
            => Function.Call(Hash.TERMINATE_​THREAD, threadId);

        public static bool IsThreadActive(int threadId)
            => Function.Call<bool>(Hash.IS_​THREAD_​ACTIVE, threadId);

        public static string GetNameOfScriptWithThisId(int threadId)
            => Function.Call<string>(Hash.GET_​NAME_​OF_​SCRIPT_​WITH_​THIS_​ID, threadId);

        public static void ScriptThreadIteratorReset()
            => Function.Call(Hash.SCRIPT_​THREAD_​ITERATOR_​RESET);

        public static int ScriptThreadIteratorGetNextThreadId()
            => Function.Call<int>(Hash.SCRIPT_​THREAD_​ITERATOR_​GET_​NEXT_​THREAD_​ID);

        public static int GetIdOfThisThread()
            => Function.Call<int>(Hash.GET_​ID_​OF_​THIS_​THREAD);

        public static void TerminateThisThread()
            => Function.Call(Hash.TERMINATE_THIS_THREAD);

        public static int GetNumberOfThreadsRunningTheScriptWithThisHash(uint scriptHash)
            => Function.Call<int>(Hash.GET_​NUMBER_​OF_​THREADS_​RUNNING_​THE_​SCRIPT_​WITH_​THIS_​HASH, scriptHash);

        public static string GetThisScriptName()
            => Function.Call<string>(Hash.GET_​THIS_​SCRIPT_​NAME);

        public static uint GetHashOfThisScriptName()
            => Function.Call<uint>(Hash.GET_​HASH_​OF_​THIS_​SCRIPT_​NAME);

        public static int GetNumberOfEvents(int eventGroup)
            => Function.Call<int>(Hash.GET_​NUMBER_​OF_​EVENTS, eventGroup);

        public static bool GetEventExists(int eventGroup, int eventIndex)
            => Function.Call<bool>(Hash.GET_​EVENT_​EXISTS, eventGroup, eventIndex);

        public static int GetEventAtIndex(int eventGroup, int eventIndex)
            => Function.Call<int>(Hash.GET_​EVENT_​AT_​INDEX, eventGroup, eventIndex);

        public static unsafe bool GetEventData(int eventGroup, int eventIndex, object* eventData, int eventDataSize)
            => Function.Call<bool>(Hash.GET_​EVENT_​DATA, eventGroup, eventIndex, eventData, eventDataSize);

        public static unsafe void TriggerScriptEvent(int eventGroup, object* eventData, int eventDataSize, int playerBits)
            => Function.Call(Hash.TRIGGER_​SCRIPT_​EVENT, eventGroup, eventData, eventDataSize, playerBits);

        public static void ShutdownLoadingScreen()
            => Function.Call(Hash.SHUTDOWN_​LOADING_​SCREEN);

        public static void SetNoLoadingScreen(bool toggle)
            => Function.Call(Hash.SET_​NO_​LOADING_​SCREEN, toggle);

        public static bool GetNoLoadingScreen()
            => Function.Call<bool>(Hash.GET_​NO_​LOADING_​SCREEN);

        public static void CommitToLoadingscreenSelction()
            => Function.Call(Hash.COMMIT_​TO_​LOADINGSCREEN_​SELCTION);

        public static bool BgIsExitflagSet()
            => Function.Call<bool>(Hash.BG_​IS_​EXITFLAG_​SET);

        public static void BgSetExitflagResponse()
            => Function.Call(Hash.BG_​SET_​EXITFLAG_​RESPONSE);

        public static void BgStartContextHash(uint contextHash)
            => Function.Call(Hash.BG_​START_​CONTEXT_​HASH, contextHash);

        public static void BgEndContextHash(uint contextHash)
            => Function.Call(Hash.BG_​END_​CONTEXT_​HASH, contextHash);

        public static void BgStartContext(string contextName)
            => Function.Call(Hash.BG_​START_​CONTEXT, contextName);

        public static void BgEndContext(string contextName)
            => Function.Call(Hash.BG_​END_​CONTEXT, contextName);

        public static bool BgDoesLaunchParamExist(int scriptIndex, string p1)
            => Function.Call<bool>(Hash.BG_​DOES_​LAUNCH_​PARAM_​EXIST, scriptIndex, p1);

        public static int BgGetLaunchParamValue(int scriptIndex, string p1)
            => Function.Call<int>(Hash.BG_​GET_​LAUNCH_​PARAM_​VALUE, scriptIndex, p1);

        public static int BgGetScriptIdFromNameHash(uint p0)
            => Function.Call<int>(Hash.BG_​GET_​SCRIPT_​ID_​FROM_​NAME_​HASH, p0);

        public static unsafe void SendTuScriptEventNew(int eventGroup, object* eventData, int eventDataSize, int playerBits, uint eventType)
            => Function.Call((Hash)0x71A6F836422FDD2B, eventGroup, eventData, eventDataSize, playerBits, eventType);

        public static void SetThisThreadPriority(int priority)
            => Function.Call(Hash.SET_​THIS_​THREAD_​PRIORITY, priority);

        public static void SetThisThreadPriority(ThreadPrio priority)
            => Function.Call(Hash.SET_​THIS_​THREAD_​PRIORITY, (int)priority);

        public static void SetScriptCanStartCutscene(int threadId)
            => Function.Call(Hash.SET_​SCRIPT_​CAN_​START_​CUTSCENE, threadId);

        public static void SetScriptHighPrio(bool toggle)
            => Function.Call(Hash.SET_​SCRIPT_​HIGH_​PRIO, toggle);

        public static void SetThisIsATriggerScript(bool toggle)
            => Function.Call(Hash.SET_​THIS_​IS_​A_​TRIGGER_​SCRIPT, toggle);

        public static void SetThisScriptCanBePaused(bool toggle)
            => Function.Call(Hash.SET_THIS_SCRIPT_CAN_BE_PAUSED, toggle);

        public static void SetThisScriptCanRemoveBlipsCreatedByAnyScript(bool toggle)
            => Function.Call(Hash.SET_THIS_SCRIPT_CAN_REMOVE_BLIPS_CREATED_BY_ANY_SCRIPT, toggle);

        public static void TerminateAllScriptsWithThisName(string scriptName)
            => Function.Call(Hash.TERMINATE_​ALL_​SCRIPTS_​WITH_​THIS_​NAME, scriptName);

        public static void StartScript(string scriptName, int stackSize)
        {
            Function.Call(Hash.REQUEST_SCRIPT, scriptName);

            while (!Function.Call<bool>(Hash.HAS_SCRIPT_LOADED, scriptName))
                GTA.Script.Wait(0);

            Function.Call(Hash.START_NEW_SCRIPT, scriptName, stackSize);
            Function.Call(Hash.SET_SCRIPT_AS_NO_LONGER_NEEDED, scriptName);
        }

        public static int StartNewScript(string scriptName, int stackSize)
            => Function.Call<int>(Hash.START_NEW_SCRIPT, scriptName, stackSize);

        public static unsafe void StartScriptWithArgs(string scriptName, object* args, int argCount, int stackSize)
        {
            Function.Call(Hash.REQUEST_SCRIPT, scriptName);

            while (!Function.Call<bool>(Hash.HAS_SCRIPT_LOADED, scriptName))
                GTA.Script.Wait(0);

            Function.Call(Hash.START_NEW_SCRIPT_WITH_ARGS, scriptName, args, argCount, stackSize);
            Function.Call(Hash.SET_SCRIPT_AS_NO_LONGER_NEEDED, scriptName);
        }

        public static unsafe int StartNewScriptWithArgs(string scriptName, object* args, int argCount, int stackSize)
            => Function.Call<int>(Hash.START_​NEW_​SCRIPT_​WITH_​ARGS, scriptName, args, argCount, stackSize);

        public static void StartScriptWithNameHash(uint scriptHash, int stackSize)
        {
            Function.Call(Hash.REQUEST_SCRIPT_WITH_NAME_HASH, scriptHash);

            while (!Function.Call<bool>(Hash.HAS_SCRIPT_WITH_NAME_HASH_LOADED, scriptHash))
                GTA.Script.Wait(0);

            Function.Call(Hash.START_NEW_SCRIPT_WITH_NAME_HASH, scriptHash, stackSize);
            Function.Call(Hash.SET_SCRIPT_WITH_NAME_HASH_AS_NO_LONGER_NEEDED, scriptHash);
        }

        public static int StartNewScriptWithNameHash(uint scriptHash, int stackSize)
            => Function.Call<int>(Hash.START_​NEW_​SCRIPT_​WITH_​NAME_​HASH, scriptHash, stackSize);

        public static unsafe void StartScriptWithNameHashAndArgs(uint scriptHash, object* args, int argCount, int stackSize)
        {
            Function.Call(Hash.REQUEST_SCRIPT_WITH_NAME_HASH, scriptHash);

            while (!Function.Call<bool>(Hash.HAS_SCRIPT_WITH_NAME_HASH_LOADED, scriptHash))
                GTA.Script.Wait(0);

            Function.Call(Hash.START_NEW_SCRIPT_WITH_NAME_HASH_AND_ARGS, scriptHash, args, argCount, stackSize);
            Function.Call(Hash.SET_SCRIPT_WITH_NAME_HASH_AS_NO_LONGER_NEEDED, scriptHash);
        }

        public static unsafe int StartNewScriptWithNameHashAndArgs(uint scriptHash, object* args, int argCount, int stackSize)
            => Function.Call<int>(Hash.START_​NEW_​SCRIPT_​WITH_​NAME_​HASH_​AND_​ARGS, scriptHash, args, argCount, stackSize);

        public enum ThreadPrio
        {
            Highest = 0,
            Normal = 1,
            Lowest = 2,
            ManualUpdate = 100,
        }
    }
}
