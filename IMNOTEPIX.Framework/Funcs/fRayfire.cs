using GTA;
using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.Funcs
{
    public static class fRayfire
    {
        public static Prop GetRayfireMapObject(Vector3 xyz, float radius, string name)
            => Function.Call<Prop>(Hash.GET_RAYFIRE_MAP_OBJECT, xyz.X, xyz.Y, xyz.Z, radius, name);

        public static void SetStateOfRayfireMapObject(Prop prop, int state)
            => Function.Call(Hash.SET_STATE_OF_RAYFIRE_MAP_OBJECT, prop, state);

        public static int GetStateOfRayfireMapObject(Prop prop)
            => Function.Call<int>(Hash.GET_STATE_OF_RAYFIRE_MAP_OBJECT, prop);

        public static bool DoesRayfireMapObjectExist(Prop prop)
            => Function.Call<bool>(Hash.DOES_RAYFIRE_MAP_OBJECT_EXIST, prop);

        public static float GetRayfireMapObjectAnimPhase(Prop prop)
            => Function.Call<float>(Hash.GET_RAYFIRE_MAP_OBJECT_ANIM_PHASE, prop);
    }
}