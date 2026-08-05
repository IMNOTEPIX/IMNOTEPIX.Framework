using System;
using System.Linq;
using System.Reflection;

namespace IMNOTEPIX.Framework.fCore.Internals
{
    /// <summary>
    /// Reflection-based access to SHVDN.Console.IsOpen. Mirrors the exact resolution pattern
    /// already proven working in fCore.ConsoleLog: the console instance is held in a static
    /// field named "console" on a type literally named "ScriptHookVDotNet" (the C++/CLI bridge -
    /// per DllMain.cpp it can land outside the SHVDN namespace, hence searching by type name
    /// across all loaded assemblies rather than assuming a namespace).
    /// </summary>
    internal static class SHVDNConsole
    {
        private static readonly object consoleInstance;
        private static readonly PropertyInfo isOpenProperty;

        static SHVDNConsole()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type bridgeType = asm.GetTypes().FirstOrDefault(t => t.Name == "ScriptHookVDotNet");
                if (bridgeType == null)
                {
                    continue;
                }

                FieldInfo consoleField = bridgeType.GetField("console",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (consoleField == null)
                {
                    continue;
                }

                consoleInstance = consoleField.GetValue(null);

                Type consoleType = asm.GetType("SHVDN.Console");
                isOpenProperty = consoleType?.GetProperty("IsOpen", BindingFlags.Public | BindingFlags.Instance);
                break;
            }

            if (consoleInstance == null || isOpenProperty == null)
            {
                ConsoleLog.Debug("[[IMNOTEPIX]] [SHVDNConsole] ERROR - could not resolve SHVDN.Console instance or IsOpen property.");
            }
        }

        public static bool IsOpen => isOpenProperty != null && (bool)isOpenProperty.GetValue(consoleInstance);
    }
}