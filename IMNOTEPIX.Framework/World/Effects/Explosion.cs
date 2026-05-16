using GTA;
using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fWorld.Effects
{
    public class Explosion
    {
        public static void CreateExplosion(Vector3 position, ExplosionType type, float damage, bool isAudible, bool isInvisible, float cameraShake, bool damagePlayer)
           => Function.Call(Hash.ADD_EXPLOSION, position.X, position.Y, position.Z, (int)type, damage, isAudible, isInvisible, cameraShake, damagePlayer);

        public static void CreatePedOwnedExplosion(GTA.Ped ped, Vector3 position, ExplosionType type, float damage, bool isAudible, bool isInvisible, float cameraShake)
            => Function.Call(Hash.ADD_OWNED_EXPLOSION, ped, position.X, position.Y, position.Z, (int)type, damage, isAudible, isInvisible, cameraShake);

        public static void CreateExplosionWithCustomVfx(int customVfx, Vector3 position, ExplosionType type, float damage, bool isAudible, bool isInvisible, float cameraShake)
            => Function.Call(Hash.ADD_EXPLOSION_WITH_USER_VFX, position.X, position.Y, position.Z, (int)type, customVfx, damage, isAudible, isInvisible, cameraShake);

        public static void CreateExplosionWithCustomVfx(string customVfx, Vector3 position, ExplosionType type, float damage, bool isAudible, bool isInvisible, float cameraShake)
            => Function.Call(Hash.ADD_EXPLOSION_WITH_USER_VFX, position.X, position.Y, position.Z, (int)type, Function.Call<int>(Hash.GET_HASH_KEY, customVfx), damage, isAudible, isInvisible, cameraShake);

        /// <summary>
        /// Checks if an explosion of the given type exists within an axis-aligned box.
        /// Pass (ExplosionType)(-1) to match any type.
        /// </summary>
        public static bool IsInArea(ExplosionType type, Vector3 min, Vector3 max)
            => Function.Call<bool>(Hash.IS_EXPLOSION_IN_AREA, (int)type, min.X, min.Y, min.Z, max.X, max.Y, max.Z);

        /// <summary>
        /// Like IsInArea, but only returns true while the explosion is actively dealing damage.
        /// </summary>
        public static bool IsActiveInArea(ExplosionType type, Vector3 min, Vector3 max)
            => Function.Call<bool>(Hash.IS_EXPLOSION_ACTIVE_IN_AREA, (int)type, min.X, min.Y, min.Z, max.X, max.Y, max.Z);

        /// <summary>
        /// Checks if an explosion of the given type exists within a sphere.
        /// Pass (ExplosionType)(-1) to match any type.
        /// </summary>
        public static bool IsInSphere(ExplosionType type, Vector3 center, float radius)
            => Function.Call<bool>(Hash.IS_EXPLOSION_IN_SPHERE, (int)type, center.X, center.Y, center.Z, radius);

        /// <summary>
        /// Returns the owner Entity of any explosion found within a sphere, or null if none.
        /// </summary>
        public static GTA.Entity GetOwnerInSphere(ExplosionType type, Vector3 center, float radius)
        {
            int handle = Function.Call<int>(Hash.GET_OWNER_OF_EXPLOSION_IN_SPHERE, (int)type, center.X, center.Y, center.Z, radius);

            return handle != 0 ? GTA.Entity.FromHandle(handle) : null;
        }

        /// <summary>
        /// Checks if an explosion of the given type exists within a rectangle defined
        /// by two points and a width.
        /// </summary>
        public static bool IsInAngledArea(ExplosionType type, Vector3 origin, Vector3 edge, float width)
            => Function.Call<bool>(Hash.IS_EXPLOSION_IN_ANGLED_AREA, (int)type, origin.X, origin.Y, origin.Z, edge.X, edge.Y, edge.Z, width);

        /// <summary>
        /// Returns the owner Entity of any explosion found within an angled area, or null if none.
        /// </summary>
        public static GTA.Entity GetOwnerInAngledArea(ExplosionType type, Vector3 origin, Vector3 edge, float width)
        {
            int handle = Function.Call<int>(Hash.GET_OWNER_OF_EXPLOSION_IN_ANGLED_AREA, (int)type, origin.X, origin.Y, origin.Z, edge.X, edge.Y, edge.Z, width);

            return handle != 0 ? GTA.Entity.FromHandle(handle) : null;
        }
    }
}
