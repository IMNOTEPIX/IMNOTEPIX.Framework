using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Entity
    {
        public static bool IsEntityInCameraView(GTA.Entity entity)
        {
            if (entity == null || !entity.Exists()) return false;

            if (Game.Player.Character.IsInVehicle() &&
                Game.Player.Character.CurrentVehicle == entity)
                return true;

            var outMin = new OutputArgument();
            var outMax = new OutputArgument();

            Function.Call(Hash.GET_MODEL_DIMENSIONS, entity.Model.Hash, outMin, outMax);

            Vector3 min = outMin.GetResult<Vector3>();
            Vector3 max = outMax.GetResult<Vector3>();

            Vector3[] localCorners = new Vector3[]
            {
        new Vector3(min.X, min.Y, min.Z),
        new Vector3(min.X, min.Y, max.Z),
        new Vector3(min.X, max.Y, min.Z),
        new Vector3(min.X, max.Y, max.Z),
        new Vector3(max.X, min.Y, min.Z),
        new Vector3(max.X, min.Y, max.Z),
        new Vector3(max.X, max.Y, min.Z),
        new Vector3(max.X, max.Y, max.Z),
            };

            foreach (Vector3 localOffset in localCorners)
            {
                Vector3 worldPos = Function.Call<Vector3>(
                    Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS,
                    entity,
                    localOffset.X,
                    localOffset.Y,
                    localOffset.Z
                );

                var outX = new OutputArgument();
                var outY = new OutputArgument();

                bool onScreen = Function.Call<bool>(
                    Hash.GET_SCREEN_COORD_FROM_WORLD_COORD,
                    worldPos.X, worldPos.Y, worldPos.Z,
                    outX, outY
                );

                if (!onScreen) continue;

                float sx = outX.GetResult<float>();
                float sy = outY.GetResult<float>();

                if (sx >= 0f && sx <= 1f && sy >= 0f && sy <= 1f)
                    return true;
            }

            return false;
        }

        public static void CreateModelHide(Vector3 xyz, float radius, int modelHash, bool p5)
            => Function.Call(Hash.CREATE_​MODEL_​HIDE, xyz.X, xyz.Y, xyz.Z, radius, modelHash, p5);

        public static void CreateModelHide(float x, float y, float z, float radius, int modelHash, bool p5)
            => Function.Call(Hash.CREATE_​MODEL_​HIDE, x, y, z, radius, modelHash, p5);

        public static Vector3 GetEntityCoords(GTA.Entity entity, bool alive)
            => Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, entity, alive);

        public static unsafe void SetPedAsNoLongerNeeded(GTA.Ped ped)
            => Function.Call(Hash.SET_PED_AS_NO_LONGER_NEEDED, &ped);

        public static void SetEntityShouldFreezeWaitingOnCollision(GTA.Entity entity, bool toggle)
            => Function.Call(Hash.SET_​ENTITY_​SHOULD_​FREEZE_​WAITING_​ON_​COLLISION, entity, toggle);

        public static void FreezeEntityPosition(GTA.Entity entity, bool toggle)
            => Function.Call(Hash.FREEZE_​ENTITY_​POSITION, entity, toggle);

        public static void SetEntityOnlyDamagedByPlayer(GTA.Entity entity, bool toggle)
            => Function.Call(Hash.SET_ENTITY_ONLY_DAMAGED_BY_PLAYER, entity, toggle);

        public static bool IsEntityDead(GTA.Entity entity, bool p1)
            => Function.Call<bool>(Hash.IS_ENTITY_DEAD, entity, p1);

        public static void SetEntityAnimSpeed(GTA.Entity entity, string animDictionary, string animName, float speedMultiplier)
            => Function.Call(Hash.SET_ENTITY_ANIM_SPEED, entity, animDictionary, animName, speedMultiplier);

        public static void SetEntityAnimCurrentTime(GTA.Entity entity, string animDictionary, string animName, float time)
            => Function.Call(Hash.SET_ENTITY_ANIM_CURRENT_TIME, entity, animDictionary, animName, time);

        public static bool PlayEntityAnim(GTA.Entity entity, string animName, string animDict, float p3, bool loop, bool stayInAnim, bool p6, float delta, int bitset = 0)
            => Function.Call<bool>(Hash.PLAY_ENTITY_ANIM, entity, animName, animDict, p3, loop, stayInAnim, p6, delta, bitset);

        public static bool IsEntityAtCoord(GTA.Entity entity, float xPos, float yPos, float zPos, float xSize, float ySize, float zSize, bool p7, bool p8, int p9)
            => Function.Call<bool>(Hash.IS_ENTITY_AT_COORD, entity, xPos, yPos, zPos, xSize, ySize, zSize, p7, p8, p9);

        public static bool IsEntityAtCoord(GTA.Entity entity, Vector3 xyzPos, Vector3 xyzSize, bool p7, bool p8, int p9)
            => Function.Call<bool>(Hash.IS_ENTITY_AT_COORD, entity, xyzPos.X, xyzPos.Y, xyzPos.Z, xyzSize.X, xyzSize.Y, xyzSize.Z, p7, p8, p9);

        public static void DetachEntity(GTA.Entity entity, bool dynamic, bool collision)
        {
            Function.Call(Hash.DETACH_ENTITY, entity, dynamic, collision);
        }

        public static void AttachEntityToEntity(GTA.Entity entity1, GTA.Entity entity2, int boneIndex, Vector3 pos, Vector3 rot, bool isPedAttachingToVehicle, bool useSoftPinning, bool collision, bool isPed, int vertexIndex = 0, bool fixedRot = true, int p15 = 0)
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, entity1, entity2, boneIndex, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, isPedAttachingToVehicle, useSoftPinning, collision, isPed, vertexIndex, fixedRot, p15);
        }
        
        public static void AttachEntityToEntity(GTA.Entity entity1, GTA.Entity entity2, int boneIndex, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, bool isPedAttachingToVehicle, bool useSoftPinning, bool collision, bool isPed, int vertexIndex = 0, bool fixedRot = true, int p15 = 0)
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, entity1, entity2, boneIndex, posX, posY, posZ, rotX, rotY, rotZ, isPedAttachingToVehicle, useSoftPinning, collision, isPed, vertexIndex, fixedRot, p15);
        }

        public static void SetEntityProofs(GTA.Entity entity, bool bulletProof, bool fireProof, bool explosionProof, bool collisionProof, bool meleeProof, bool steamProof, bool dontResetOnCleanup, bool waterProof)
        {
            Function.Call(Hash.SET_ENTITY_PROOFS, entity, bulletProof, fireProof, explosionProof, collisionProof, meleeProof, steamProof, dontResetOnCleanup, waterProof);
        }

        public static void SetEntityCanBeDamaged(GTA.Entity entity, bool toggle)
        {
            Function.Call(Hash.SET_ENTITY_CAN_BE_DAMAGED, entity, toggle);
        }

        /// <summary>
        /// Returns entity health as int (cast from float internally by the native).
        /// Ranges: Player 0–200 | Ped 100–200 | Vehicle 0–1000 | Object 0–1000
        /// </summary>
        public static int GetEntityHealth(GTA.Entity entity)
        {
            return Function.Call<int>(Hash.GET_ENTITY_HEALTH, entity);
        }

        /// <summary>
        /// Returns the raw float health by reading directly from entity memory.
        /// </summary>
        /// 
        /// <remarks>
        /// <c>WARNING:</c> offset 0x280 is specific to a particular game build.
        /// Verify this offset against your target GTA V / SHVDN version before use.
        /// An incorrect offset will return garbage data silently.
        /// </remarks>
        public static unsafe float GetEntityHealthFloat(GTA.Entity entity)
        {
            return *(float*)(entity.MemoryAddress + 0x280);
        }

        /// <summary>
        /// Sets entity health with a specific instigator and weapon type.
        /// </summary>
        /// 
        /// <remarks>
        /// health range: Player/Ped 0–200 | Vehicle/Object 0–1000
        /// </remarks>
        public static void SetEntityHealth(GTA.Entity entity, int health, GTA.Entity instigator, int weaponHash)
        {
            Function.Call(Hash.SET_ENTITY_HEALTH, entity, health, instigator, weaponHash);
        }

        // Sets entity health with no instigator.
        public static void SetEntityHealth(GTA.Entity entity, int health)
        {
            Function.Call(Hash.SET_ENTITY_HEALTH, entity, health, 0);
        }

        // Axis flags = invert axis.
        public static void SetEntityCoordsNoOffset(GTA.Entity entity, Vector3 coords, bool xAxis, bool yAxis = false, bool zAxis = false)
        {
            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, entity, coords.X, coords.Y, coords.Z, xAxis, yAxis, zAxis);
        }

        public static void SetEntityCoordsNoOffset(GTA.Entity entity, float X, float Y, float Z, bool xAxis, bool yAxis = false, bool zAxis = false)
        {
            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, entity, X, Y, Z, xAxis, yAxis, zAxis);
        }

        public static Vector3 GetOffsetFromEntityInWorldCoords(GTA.Entity entity, Vector3 offset)
        {
            return Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, entity, offset.X, offset.Y, offset.Z);
        }

        public static Vector3 GetOffsetFromEntityInWorldCoords(GTA.Entity entity, float offsetX, float offsetY, float offsetZ)
        {
            return Function.Call<Vector3>(Hash.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS, entity, offsetX, offsetY, offsetZ);
        }

        public static Vector3 GetAnimInitialOffsetPosition(string animDict, string animName, Vector3 pos, Vector3 rot, float p8 = 0f, int p9 = 2)
        {
            return Function.Call<Vector3>(Hash.GET_ANIM_INITIAL_OFFSET_POSITION, animDict, animName, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, p8, p9);
        }

        public static Vector3 GetAnimInitialOffsetPosition(string animDict, string animName, float x, float y, float z, float xRot, float yRot, float zRot, float p8 = 0f, int p9 = 2)
        {
            return Function.Call<Vector3>(Hash.GET_ANIM_INITIAL_OFFSET_POSITION, animDict, animName, x, y, z, xRot, yRot, zRot, p8, p9);
        }

        public static bool IsEntityPlayingAnim(GTA.Entity entity, string animDict, string animName, int taskFlag = 3)
        {
            return Function.Call<bool>(
                Hash.IS_ENTITY_PLAYING_ANIM, entity, animDict, animName, taskFlag);
        }

        public static bool HasAnimEventFired(GTA.Entity entity, string actionHash)
        {
            return Function.Call<bool>(Hash.HAS_ANIM_EVENT_FIRED, entity, Function.Call<int>(Hash.GET_HASH_KEY, actionHash));
        }

        public static bool HasAnimEventFired(GTA.Entity entity, int actionHash)
        {
            return Function.Call<bool>(Hash.HAS_ANIM_EVENT_FIRED, entity, actionHash);
        }

        public static bool HasAnimEventFired(GTA.Entity entity, uint actionHash)
        {
            return Function.Call<bool>(Hash.HAS_ANIM_EVENT_FIRED, entity, actionHash);
        }

        public static void ForceEntityAiAndAnimationUpdate(GTA.Entity entity)
        {
            Function.Call(Hash.FORCE_ENTITY_AI_AND_ANIMATION_UPDATE, entity);
        }

        public static bool HasCollisionLoadedAroundEntity(GTA.Entity entity)
        {
            return Function.Call<bool>(Hash.HAS_COLLISION_LOADED_AROUND_ENTITY, entity);
        }

        public static void SetEntityLoadCollisionFlag(GTA.Entity entity, bool toggle, int p2 = 1)
        {
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, entity, toggle, p2);
        }

        public static void SetCanClimbOnEntity(GTA.Entity entity, bool toggle)
        {
            Function.Call(Hash.SET_CAN_CLIMB_ON_ENTITY, entity, toggle);
        }

        public static bool IsEntityInArea(GTA.Entity entity, Vector3 xyz1, Vector3 xyz2, bool p7 = false, bool p8 = true, int p9 = 0)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_IN_AREA, entity, xyz1.X, xyz1.Y, xyz1.Z, xyz2.X, xyz2.Y, xyz2.Z, p7, p8, p9);
        }

        public static bool IsEntityInArea(GTA.Entity entity, float x1, float y1, float z1, float x2, float y2, float z2, bool p7 = false, bool p8 = true, int p9 = 0)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_IN_AREA, entity, x1, y1, z1, x2, y2, z2, p7, p8, p9);
        }

        /// <summary>
        /// "Angled area" is a rectangle defined by two corner points and a width.
        /// </summary>
        public static bool IsEntityInAngledArea(GTA.Entity entity, Vector3 xyz1, Vector3 xyz2, float width, bool debug = false, bool includeZ = true, int p10 = 0)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_IN_ANGLED_AREA, entity, xyz1.X, xyz1.Y, xyz1.Z, xyz2.X, xyz2.Y, xyz2.Z, width, debug, includeZ, p10);
        }

        public static bool IsEntityInAngledArea(GTA.Entity entity, float x1, float y1, float z1, float x2, float y2, float z2, float width, bool debug = false, bool includeZ = true, int p10 = 0)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_IN_ANGLED_AREA, entity, x1, y1, z1, x2, y2, z2, width, debug, includeZ, p10);
        }

        //[Obsolete("Use entity.Exists() or entity != null and entity.Exists() directly.")]
        public static bool DoesEntityExist(GTA.Entity entity)
        {
            return entity != null && entity.Exists();
        }
    }
}
