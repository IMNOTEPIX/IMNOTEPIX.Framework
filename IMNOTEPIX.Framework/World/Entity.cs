using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using static IMNOTEPIX.Framework.fWorld.Pathfind;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Entity
    {
        public static bool HasEntityClearLOSToEntityAdjustForCover(GTA.Entity entity1, GTA.Entity entity2, int traceType)
            => Function.Call<bool>(Hash.HAS_​ENTITY_​CLEAR_​LOS_​TO_​ENTITY_​ADJUST_​FOR_​COVER, entity1, entity2, traceType);

        public static bool HasEntityClearLOSToEntity(GTA.Entity entity1, GTA.Entity entity2, int traceType)
            => Function.Call<bool>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY, entity1, entity2, traceType);

        public static void CreateModelSwap(float x, float y, float z, float radius, Model originalModel, Model newModel, bool bSurviveMapReload)
            => Function.Call(Hash.CREATE_​MODEL_​SWAP, x, y, z, radius, (int)originalModel, (int)newModel, bSurviveMapReload);

        public static void CreateModelSwap(Vector3 xyz, float radius, Model originalModel, Model newModel, bool bSurviveMapReload)
            => Function.Call(Hash.CREATE_​MODEL_​SWAP, xyz.X, xyz.Y, xyz.Z, radius, (int)originalModel, (int)newModel, bSurviveMapReload);

        public static void EnableEntityBulletCollision(GTA.Entity entity)
            => Function.Call(Hash.ENABLE_​ENTITY_​BULLET_​COLLISION, entity);

        public static void SetEntityNoCollisionEntity(GTA.Entity entity1, GTA.Entity entity2, bool thisFrameOnly)
            => Function.Call(Hash.SET_​ENTITY_​NO_​COLLISION_​ENTITY, entity1, entity2, thisFrameOnly);

        public static void RemoveModelHide(float x, float y, float z, float radius, Model model, bool lazy)
            => Function.Call(Hash.REMOVE_MODEL_HIDE, x, y, z, radius, (int)model, lazy);

        public static void RemoveModelHide(Vector3 xyz, float radius, Model model, bool lazy)
            => Function.Call(Hash.REMOVE_MODEL_HIDE, xyz.X, xyz.Y, xyz.Z, radius, (int)model, lazy);

        public static GTA.Ped GetPedIndexFromEntityIndex(GTA.Entity entity)
            => Function.Call<GTA.Ped>(Hash.GET_​PED_​INDEX_​FROM_​ENTITY_​INDEX, entity);

        public static float GetEntitySpeed(GTA.Entity entity)
            => Function.Call<float>(Hash.GET_​ENTITY_​SPEED, entity);

        public static bool IsEntityAnObject(GTA.Entity entity)
            => Function.Call<bool>(Hash.IS_​ENTITY_​AN_​OBJECT, entity);

        public static bool IsEntityAPed(GTA.Entity entity)
            => Function.Call<bool>(Hash.IS_​ENTITY_​A_​PED, entity);

        public static bool IsEntityAVehicle(GTA.Entity entity)
            => Function.Call<bool>(Hash.IS_​ENTITY_​A_​VEHICLE, entity);

        public static bool IsEntityTouchingEntity(GTA.Entity entity, GTA.Entity targetEntity)
            => Function.Call<bool>(Hash.IS_​ENTITY_​TOUCHING_​ENTITY, entity, targetEntity);

        public static bool IsEntityOnScreen(GTA.Entity entity)
            => Function.Call<bool>(Hash.IS_​ENTITY_​ON_​SCREEN, entity);

        public static void SetEntityRotation(GTA.Entity entity, float pitch, float roll, float yaw, int rotationOrder, bool p5)
            => Function.Call(Hash.SET_​ENTITY_​ROTATION, entity, pitch, roll, yaw, rotationOrder, p5);

        public static void SetEntityRotation(GTA.Entity entity, Vector3 rot, int rotationOrder, bool p5)
            => Function.Call(Hash.SET_​ENTITY_​ROTATION, entity, rot.X, rot.Y, rot.Z, rotationOrder, p5);

        public static void SetCanAutoVaultOnEntity(GTA.Entity entity, bool toggle)
            => Function.Call(Hash.SET_​CAN_​AUTO_​VAULT_​ON_​ENTITY, entity, toggle);
        public static bool IsEntityInCameraView(GTA.Entity entity)
        {
            if (entity == null || !entity.Exists()) return false;

            if (Game.Player.Character.IsInVehicle() &&
                Game.Player.Character.CurrentVehicle == entity)
                return true;

            return entity.IsOnScreen;
        }

        public static void CreateModelHide(Vector3 xyz, float radius, Model model, bool p5)
            => Function.Call(Hash.CREATE_​MODEL_​HIDE, xyz.X, xyz.Y, xyz.Z, radius, (int)model, p5);

        public static void CreateModelHide(float x, float y, float z, float radius, Model model, bool p5)
            => Function.Call(Hash.CREATE_​MODEL_​HIDE, x, y, z, radius, (int)model, p5);

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

        /// <summary>
        /// transportMode: Checks the transport mode the ped is using, only does something if entity is a ped.Transport modes are: 0 (any), 1 (on foot), 2 (in vehicle)
        /// </summary>
        public static bool IsEntityAtCoord(GTA.Entity entity, float xPos, float yPos, float zPos, float xSize, float ySize, float zSize, bool highlightArea, bool do3dCheck, int transportMode)
            => Function.Call<bool>(Hash.IS_ENTITY_AT_COORD, entity, xPos, yPos, zPos, xSize, ySize, zSize, highlightArea, do3dCheck, transportMode);

        /// <summary>
        /// <inheritdoc cref="IsEntityAtCoord"/>
        /// </summary>
        public static bool IsEntityAtCoord(GTA.Entity entity, Vector3 xyzPos, Vector3 xyzSize, bool highlightArea, bool do3dCheck, int transportMode)
            => Function.Call<bool>(Hash.IS_ENTITY_AT_COORD, entity, xyzPos.X, xyzPos.Y, xyzPos.Z, xyzSize.X, xyzSize.Y, xyzSize.Z, highlightArea, do3dCheck, transportMode);

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
