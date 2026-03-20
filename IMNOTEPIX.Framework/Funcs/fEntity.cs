namespace IMNOTEPIX.Framework.Funcs
{
    using GTA;
    using GTA.Math;
    using GTA.Native;
    using System;

    public static class fEntity
    {
        // ─────────────────────────────────────────────────────────────────────
        // Attachment
        // ─────────────────────────────────────────────────────────────────────

        public static void DetachEntity(Entity entity, bool dynamic, bool collision)
        {
            Function.Call(Hash.DETACH_ENTITY, entity, dynamic, collision);
        }

        /// <summary>
        /// Attaches entity1 to entity2 at the given bone.
        /// p9       = isPedAttachingToVehicle (usually false)
        /// vertexIndex = 0 for most cases
        /// p15      = unknown, usually 0
        /// </summary>
        public static void AttachEntityToEntity(
            Entity entity1,
            Entity entity2,
            int boneIndex,
            Vector3 pos,
            Vector3 rot,
            bool p9,
            bool useSoftPinning,
            bool collision,
            bool isPed,
            int vertexIndex,
            bool fixedRot,
            int p15)
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY,
                entity1, entity2, boneIndex,
                pos.X, pos.Y, pos.Z,
                rot.X, rot.Y, rot.Z,
                p9, useSoftPinning, collision, isPed, vertexIndex, fixedRot, p15);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Proofs / damage
        // ─────────────────────────────────────────────────────────────────────

        public static void SetEntityProofs(
            Entity entity,
            bool bulletProof,
            bool fireProof,
            bool explosionProof,
            bool collisionProof,
            bool meleeProof,
            bool steamProof,
            bool dontResetOnCleanup,
            bool waterProof)
        {
            Function.Call(Hash.SET_ENTITY_PROOFS,
                entity,
                bulletProof, fireProof, explosionProof,
                collisionProof, meleeProof, steamProof,
                dontResetOnCleanup, waterProof);
        }

        public static void SetEntityCanBeDamaged(Entity entity, bool toggle)
        {
            Function.Call(Hash.SET_ENTITY_CAN_BE_DAMAGED, entity, toggle);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Health
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns entity health as int (cast from float internally by the native).
        /// Ranges: Player 0–200 | Ped 100–200 | Vehicle 0–1000 | Object 0–1000
        /// </summary>
        public static int GetEntityHealth(Entity entity)
        {
            return Function.Call<int>(Hash.GET_ENTITY_HEALTH, entity);
        }

        /// <summary>
        /// Returns the raw float health by reading directly from entity memory.
        ///
        /// WARNING: offset 0x280 is specific to a particular game build.
        /// Verify this offset against your target GTA V / SHVDN version before use.
        /// An incorrect offset will return garbage data silently.
        /// </summary>
        public static unsafe float GetEntityHealthFloat(Entity entity)
        {
            return *(float*)(entity.MemoryAddress + 0x280);
        }

        /// <summary>
        /// Sets entity health with a specific instigator and weapon type.
        /// health range: Player/Ped 0–200 | Vehicle/Object 0–1000
        /// </summary>
        public static void SetEntityHealth(
            Entity entity, int health, Entity instigator, Hash weaponType)
        {
            Function.Call(Hash.SET_ENTITY_HEALTH, entity, health, instigator, weaponType);
        }

        /// <summary>
        /// Sets entity health with no instigator.
        /// Omits weaponType — use the four-parameter overload if weapon attribution matters.
        /// </summary>
        public static void SetEntityHealth(Entity entity, int health)
        {
            Function.Call(Hash.SET_ENTITY_HEALTH, entity, health, 0);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Position / coords
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Axis flags = invert axis. Pass false/false/false for standard teleport.
        /// </summary>
        public static void SetEntityCoordsNoOffset(
            Entity entity, Vector3 coords, bool xAxis, bool yAxis, bool zAxis)
        {
            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET,
                entity, coords.X, coords.Y, coords.Z, xAxis, yAxis, zAxis);
        }

        public static Vector3 GetOffsetFromEntityInWorldCoords(Entity entity, Vector3 offset)
        {
            return Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS,
                entity, offset.X, offset.Y, offset.Z);
        }

        public static Vector3 GetOffsetFromEntityInWorldCoords(
            Entity entity, float offsetX, float offsetY, float offsetZ)
        {
            return Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS,
                entity, offsetX, offsetY, offsetZ);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Animation
        // ─────────────────────────────────────────────────────────────────────

        public static Vector3 GetAnimInitialOffsetPosition(
            string animDict, string animName,
            Vector3 pos, Vector3 rot,
            float p8 = 0f, int p9 = 2)
        {
            return Function.Call<Vector3>(Hash.GET_ANIM_INITIAL_OFFSET_POSITION,
                animDict, animName,
                pos.X, pos.Y, pos.Z,
                rot.X, rot.Y, rot.Z,
                p8, p9);
        }

        public static Vector3 GetAnimInitialOffsetPosition(
            string animDict, string animName,
            float x, float y, float z,
            float xRot, float yRot, float zRot,
            float p8 = 0f, int p9 = 2)
        {
            return Function.Call<Vector3>(Hash.GET_ANIM_INITIAL_OFFSET_POSITION,
                animDict, animName,
                x, y, z, xRot, yRot, zRot,
                p8, p9);
        }

        public static bool IsEntityPlayingAnim(
            Entity entity, string animDict, string animName, int taskFlag = 3)
        {
            return Function.Call<bool>(
                Hash.IS_ENTITY_PLAYING_ANIM, entity, animDict, animName, taskFlag);
        }

        public static bool HasAnimEventFired(Entity entity, string actionHash)
        {
            return Function.Call<bool>(Hash.HAS_ANIM_EVENT_FIRED,
                entity, Function.Call<int>(Hash.GET_HASH_KEY, actionHash));
        }

        public static bool HasAnimEventFired(Entity entity, int actionHash)
        {
            return Function.Call<bool>(Hash.HAS_ANIM_EVENT_FIRED, entity, actionHash);
        }

        public static bool HasAnimEventFired(Entity entity, Hash actionHash)
        {
            return Function.Call<bool>(Hash.HAS_ANIM_EVENT_FIRED, entity, actionHash);
        }

        public static void ForceEntityAiAndAnimationUpdate(Entity entity)
        {
            Function.Call(Hash.FORCE_ENTITY_AI_AND_ANIMATION_UPDATE, entity);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Collision / area
        // ─────────────────────────────────────────────────────────────────────

        public static bool HasCollisionLoadedAroundEntity(Entity entity)
        {
            return Function.Call<bool>(Hash.HAS_COLLISION_LOADED_AROUND_ENTITY, entity);
        }

        public static void SetEntityLoadCollisionFlag(Entity entity, bool toggle, int p2 = 1)
        {
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, entity, toggle, p2);
        }

        public static void SetCanClimbOnEntity(Entity entity, bool toggle)
        {
            Function.Call(Hash.SET_CAN_CLIMB_ON_ENTITY, entity, toggle);
        }

        public static bool IsEntityInArea(
            Entity entity, Vector3 xyz1, Vector3 xyz2,
            bool p7 = false, bool p8 = true, int p9 = 0)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_IN_AREA,
                entity,
                xyz1.X, xyz1.Y, xyz1.Z,
                xyz2.X, xyz2.Y, xyz2.Z,
                p7, p8, p9);
        }

        /// <summary>
        /// "Angled area" is a rectangle defined by two corner points and a width.
        /// </summary>
        public static bool IsEntityInAngledArea(
            Entity entity, Vector3 xyz1, Vector3 xyz2, float width,
            bool debug = false, bool includeZ = true, int p10 = 0)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_IN_ANGLED_AREA,
                entity,
                xyz1.X, xyz1.Y, xyz1.Z,
                xyz2.X, xyz2.Y, xyz2.Z,
                width, debug, includeZ, p10);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Removed
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Removed — use entity.Exists() directly instead.
        /// </summary>
        [Obsolete("Use entity.Exists() or entity != null and entity.Exists() directly.")]
        public static bool DoesEntityExist(Entity entity)
        {
            return entity != null && entity.Exists();
        }
    }
}
