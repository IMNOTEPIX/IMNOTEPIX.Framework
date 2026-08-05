using System;
using System.IO;
using System.Runtime.Serialization;

namespace IMNOTEPIX.Framework.fCore
{
    public static class DataStore
    {
        private const string LOG_CAT = "DataStore";
        private static readonly string BasePath = @"scripts\IMNOTEPIX\Data";

        public static void Save<T>(string key, T data)
        {
            Directory.CreateDirectory(BasePath);
            string path = GetPath(key);
            string tempPath = path + ".tmp";

            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    using (var writer = new FileStream(tempPath, FileMode.Create))
                    {
                        serializer.WriteObject(writer, data);
                    }

                    if (File.Exists(path)) File.Replace(tempPath, path, null);
                    else File.Move(tempPath, path);

                    Logger.Debug(LOG_CAT, $"Saved '{key}' -> {path}");
                    return;
                }
                catch (IOException) when (attempt < 2)
                {
                    System.Threading.Thread.Sleep(15);
                }
                catch (Exception ex)
                {
                    Logger.Error(LOG_CAT, $"Failed to save '{key}': {ex}");
                    return;
                }
            }
        }

        public static T Load<T>(string key, T fallback = default(T))
        {
            string path = GetPath(key);
            if (!File.Exists(path)) return fallback;

            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    using (var reader = new FileStream(path, FileMode.Open))
                    {
                        return (T)serializer.ReadObject(reader);
                    }
                }
                catch (IOException) when (attempt < 2)
                {
                    System.Threading.Thread.Sleep(15);
                }
                catch (Exception ex)
                {
                    Logger.Error(LOG_CAT, $"Failed to load '{key}': {ex}");
                    return fallback;
                }
            }
            return fallback;
        }

        public static bool Exists(string key) => File.Exists(GetPath(key));
        public static void Delete(string key)
        {
            string path = GetPath(key);
            if (File.Exists(path)) File.Delete(path);
        }

        private static string GetPath(string key) => Path.Combine(BasePath, key + ".xml");
    }
}