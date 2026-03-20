using GTA.Native;

namespace IMNOTEPIX.Framework.Funcs
{
    public class fClouds
    {
        public static float GetCloudsAlpha => Function.Call<float>(Hash.GET_CLOUDS_ALPHA);
        public static void PreloadCloudHat(string name) => Function.Call(Hash.PRELOAD_CLOUD_HAT, name);
        public static void LoadCloudHat(string name, float transitionTime) => Function.Call(Hash.LOAD_CLOUD_HAT, name, transitionTime);
        public static void UnloadCloudHat(string name, float p1) => Function.Call(Hash.UNLOAD_CLOUD_HAT, name, p1);
        public static void UnloadAllCloudHats() => Function.Call(Hash.UNLOAD_ALL_CLOUD_HATS);
        public static void SetCloudsAlpha(float opacity) => Function.Call(Hash.SET_CLOUDS_ALPHA, opacity);
    }
}
