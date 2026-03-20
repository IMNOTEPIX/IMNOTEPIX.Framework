using GTA;
using GTA.Math;
using GTA.Native;
using System;

namespace IMNOTEPIX.Framework.Funcs
{
    /// <summary>
    /// Data container and trigger for a scripted explosion.
    ///
    /// Usage:
    ///   new Explosion(pos, ExplosionType.Grenade, 1f, true, false, 0.5f, false).Add();
    ///   new Explosion(ped, pos, ExplosionType.RPG, 2f, true, false, 1f).AddPedOwned();
    ///   new Explosion("vfxName", pos, ExplosionType.Car, 1f, true, false, 0f).AddWithCustomVfx();
    /// </summary>
    public class Explosion
    {
        // ─────────────────────────────────────────────────────────────────────
        // Properties
        // ─────────────────────────────────────────────────────────────────────

        public GTA.Ped Ped { get; private set; }
        public Vector3 Position { get; private set; }
        public ExplosionType TypeEnum { get; private set; }
        public float DamageAmount { get; private set; }
        public bool IsAudible { get; private set; }
        public bool IsInvisible { get; private set; }
        public float CameraShake { get; private set; }
        public bool DamagePlayer { get; private set; }

        /// <summary>Joaat hash of the custom VFX asset name. 0 if not set.</summary>
        public int CustomVfx { get; private set; }

        // ─────────────────────────────────────────────────────────────────────
        // Construction — all overloads delegate to the canonical constructor
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Standard explosion with explicit damage-player flag.</summary>
        public Explosion(
            Vector3 position,
            ExplosionType type,
            float damage,
            bool isAudible,
            bool isInvisible,
            float cameraShake,
            bool damagePlayer)
        {
            Position = position;
            TypeEnum = type;
            DamageAmount = damage;
            IsAudible = isAudible;
            IsInvisible = isInvisible;
            CameraShake = cameraShake;
            DamagePlayer = damagePlayer;
        }

        /// <summary>
        /// Ped-owned explosion. DamagePlayer defaults to false — use the
        /// full constructor if you need to override this.
        /// </summary>
        public Explosion(
            GTA.Ped ped,
            Vector3 position,
            ExplosionType type,
            float damage,
            bool isAudible,
            bool isInvisible,
            float cameraShake)
            : this(position, type, damage, isAudible, isInvisible, cameraShake, damagePlayer: false)
        {
            Ped = ped;
        }

        /// <summary>
        /// Custom VFX explosion. DamagePlayer defaults to false — use the
        /// full constructor if you need to override this.
        /// </summary>
        public Explosion(
            string customVfx,
            Vector3 position,
            ExplosionType type,
            float damage,
            bool isAudible,
            bool isInvisible,
            float cameraShake)
            : this(position, type, damage, isAudible, isInvisible, cameraShake, damagePlayer: false)
        {
            CustomVfx = Function.Call<int>(Hash.GET_HASH_KEY, customVfx);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Trigger methods
        // ─────────────────────────────────────────────────────────────────────

        public void Add()
        {
            Function.Call(Hash.ADD_EXPLOSION,
                Position.X, Position.Y, Position.Z,
                (int)TypeEnum, DamageAmount, IsAudible, IsInvisible, CameraShake, DamagePlayer);
        }

        /// <exception cref="InvalidOperationException">
        /// Thrown if no valid Ped was provided at construction.
        /// </exception>
        public void AddPedOwned()
        {
            if (Ped == null || !Ped.Exists())
                throw new InvalidOperationException(
                    "A valid Ped must be assigned before calling AddPedOwned.");

            Function.Call(Hash.ADD_OWNED_EXPLOSION,
                Ped, Position.X, Position.Y, Position.Z,
                (int)TypeEnum, DamageAmount, IsAudible, IsInvisible, CameraShake);
        }

        /// <exception cref="InvalidOperationException">
        /// Thrown if no custom VFX hash was set at construction.
        /// </exception>
        public void AddWithCustomVfx()
        {
            if (CustomVfx == 0)
                throw new InvalidOperationException(
                    "CustomVfx is not set. Use the customVfx constructor overload.");

            Function.Call(Hash.ADD_EXPLOSION_WITH_USER_VFX,
                Position.X, Position.Y, Position.Z,
                (int)TypeEnum, CustomVfx, DamageAmount, IsAudible, IsInvisible, CameraShake);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Static detection
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Checks if an explosion of the given type exists within an axis-aligned box.
        /// Pass (ExplosionType)(-1) to match any type.
        /// </summary>
        public static bool IsInArea(ExplosionType type, Vector3 min, Vector3 max)
        {
            return Function.Call<bool>(Hash.IS_EXPLOSION_IN_AREA,
                (int)type,
                min.X, min.Y, min.Z,
                max.X, max.Y, max.Z);
        }

        /// <summary>
        /// Like IsInArea, but only returns true while the explosion is actively dealing damage.
        /// </summary>
        public static bool IsActiveInArea(ExplosionType type, Vector3 min, Vector3 max)
        {
            return Function.Call<bool>(Hash.IS_EXPLOSION_ACTIVE_IN_AREA,
                (int)type,
                min.X, min.Y, min.Z,
                max.X, max.Y, max.Z);
        }

        /// <summary>
        /// Checks if an explosion of the given type exists within a sphere.
        /// Pass (ExplosionType)(-1) to match any type.
        /// </summary>
        public static bool IsInSphere(ExplosionType type, Vector3 center, float radius)
        {
            return Function.Call<bool>(Hash.IS_EXPLOSION_IN_SPHERE,
                (int)type,
                center.X, center.Y, center.Z,
                radius);
        }

        /// <summary>
        /// Returns the owner Entity of any explosion found within a sphere, or null if none.
        /// </summary>
        public static Entity GetOwnerInSphere(ExplosionType type, Vector3 center, float radius)
        {
            int handle = Function.Call<int>(Hash.GET_OWNER_OF_EXPLOSION_IN_SPHERE,
                (int)type,
                center.X, center.Y, center.Z,
                radius);

            return handle != 0 ? Entity.FromHandle(handle) : null;
        }

        /// <summary>
        /// Checks if an explosion of the given type exists within a rectangle defined
        /// by two points and a width.
        /// </summary>
        public static bool IsInAngledArea(
            ExplosionType type, Vector3 origin, Vector3 edge, float width)
        {
            return Function.Call<bool>(Hash.IS_EXPLOSION_IN_ANGLED_AREA,
                (int)type,
                origin.X, origin.Y, origin.Z,
                edge.X, edge.Y, edge.Z,
                width);
        }

        /// <summary>
        /// Returns the owner Entity of any explosion found within an angled area, or null if none.
        /// </summary>
        public static Entity GetOwnerInAngledArea(
            ExplosionType type, Vector3 origin, Vector3 edge, float width)
        {
            int handle = Function.Call<int>(Hash.GET_OWNER_OF_EXPLOSION_IN_ANGLED_AREA,
                (int)type,
                origin.X, origin.Y, origin.Z,
                edge.X, edge.Y, edge.Z,
                width);

            return handle != 0 ? Entity.FromHandle(handle) : null;
        }
    }
}
