namespace IMNOTEPIX.Framework.fCore
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class ConsoleLog
    {
        private static object s_consoleInstance;
        private static MethodInfo s_printMessage;
        private static MethodInfo s_printInfo;
        private static MethodInfo s_printError;
        private static MethodInfo s_printWarning;
        private static MethodInfo s_printDebug;

        static ConsoleLog()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                // The class name is literally "ScriptHookVDotNet" per DllMain.cpp -
                // check the decompiler for its actual namespace (if any); C++/CLI
                // classes sometimes land in the global namespace.
                Type bridgeType = asm.GetTypes()
                    .FirstOrDefault(t => t.Name == "ScriptHookVDotNet");
                if (bridgeType == null) continue;

                FieldInfo consoleField = bridgeType.GetField("console",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (consoleField == null) continue;

                s_consoleInstance = consoleField.GetValue(null);

                Type consoleType = asm.GetType("SHVDN.Console");
                s_printMessage = consoleType.GetMethod("PrintMessage", new[] { typeof(string), typeof(string) });
                s_printInfo = consoleType.GetMethod("PrintInfo", new[] { typeof(string) });
                s_printError = consoleType.GetMethod("PrintError", new[] { typeof(string) });
                s_printWarning = consoleType.GetMethod("PrintWarning", new[] { typeof(string) });
                s_printDebug = consoleType.GetMethod("PrintDebug", new[] { typeof(string) });
                break;
            }
        }

        public static void Message(string headerStr, string msg) => s_printMessage?.Invoke(s_consoleInstance, [headerStr, msg]);
        public static void Message(string headerStr, string msg, params object[] args) => s_printMessage?.Invoke(s_consoleInstance, [headerStr, msg, args]);
        public static void Info(string msg) => s_printInfo?.Invoke(s_consoleInstance, [msg]);
        public static void Error(string msg) => s_printError?.Invoke(s_consoleInstance, [msg]);
        public static void Warning(string msg) => s_printWarning?.Invoke(s_consoleInstance, [msg]);
        public static void Debug(string msg) => s_printDebug?.Invoke(s_consoleInstance, [msg]);
    }
}
