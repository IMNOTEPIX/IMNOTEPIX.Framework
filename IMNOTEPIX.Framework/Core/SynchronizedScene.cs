using GTA;
using GTA.Math;
using GTA.Native;
using System;

namespace IMNOTEPIX.Framework.Core
{
    public class SynchronizedScene : IDisposable
    {
        // ─────────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────────

        public int Handle { get; private set; } = -1;
        public bool IsActive => Handle != -1;

        public Vector3 Position { get; protected set; }
        public Vector3 Rotation { get; protected set; }

        public Camera Camera { get; set; }

        // ─────────────────────────────────────────────────────────────────────
        // Native-backed properties
        // ─────────────────────────────────────────────────────────────────────

        public bool IsRunning
            => Function.Call<bool>(Hash.IS_SYNCHRONIZED_SCENE_RUNNING, Handle);

        public float Phase
        {
            get => Function.Call<float>(Hash.GET_SYNCHRONIZED_SCENE_PHASE, Handle);
            set => Function.Call(Hash.SET_SYNCHRONIZED_SCENE_PHASE, Handle, value);
        }

        public float Rate
        {
            get => Function.Call<float>(Hash.GET_SYNCHRONIZED_SCENE_RATE, Handle);
            set => Function.Call(Hash.SET_SYNCHRONIZED_SCENE_RATE, Handle, value);
        }

        public bool IsLooping
        {
            get => Function.Call<bool>(Hash.IS_SYNCHRONIZED_SCENE_LOOPED, Handle);
            set => Function.Call(Hash.SET_SYNCHRONIZED_SCENE_LOOPED, Handle, value);
        }

        public bool HoldLastFrame
        {
            get => Function.Call<bool>(Hash.IS_SYNCHRONIZED_SCENE_HOLD_LAST_FRAME, Handle);
            set => Function.Call(Hash.SET_SYNCHRONIZED_SCENE_HOLD_LAST_FRAME, Handle, value);
        }

        public bool IsFinished => Phase >= 1.0f;

        // ─────────────────────────────────────────────────────────────────────
        // Construction
        // ─────────────────────────────────────────────────────────────────────

        public SynchronizedScene(Vector3 position, float heading = 0f)
        {
            Position = position;
            Rotation = new Vector3(0f, 0f, heading);
        }

        public SynchronizedScene(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public SynchronizedScene(Prop prop)
        {
            Position = prop.Position;
            Rotation = prop.Rotation;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Lifecycle
        // ─────────────────────────────────────────────────────────────────────

        public void Create(int timeout = 500)
        {
            Handle = Function.Call<int>(Hash.CREATE_SYNCHRONIZED_SCENE,
                Position.X, Position.Y, Position.Z,
                Rotation.X, Rotation.Y, Rotation.Z,
                2);

            int startTime = Game.GameTime;
            while (!IsActive)
            {
                if (Game.GameTime - startTime > timeout)
                    throw new TimeoutException("Timed out waiting for synchronized scene to become active.");
                Script.Yield();
            }
        }

        public void Dispose()
        {
            if (IsActive)
            {
                Function.Call(Hash.TAKE_OWNERSHIP_OF_SYNCHRONIZED_SCENE, Handle);
                Handle = -1;
                DeleteCam(stopRendering: true);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Playback
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Plays an animation on a non-ped entity within this scene.
        ///
        /// moveBlend default WARNING: 1148846080 is a suspicious magic value.
        /// As a float literal this compiles to ~1.149e9, not the intended 1000.0f
        /// (whose IEEE 754 bits are 0x447A0000 = 1148846080). If you need 1000.0,
        /// pass 1000f explicitly. Standard blend ratios are 0.0–8.0.
        /// </summary>
        public void PlayEntity(
            Entity entity,
            string animDict,
            string animName,
            float blendIn = 8f,
            float blendOut = 8f,
            SyncedSceneFlags flag = SyncedSceneFlags.None,
            float moveBlend = 1148846080)
        {
            if (!IsActive || entity == null || !entity.Exists()) return;

            Function.Call(Hash.PLAY_SYNCHRONIZED_ENTITY_ANIM,
                entity, Handle, animName, animDict,
                blendIn, blendOut, (int)flag, moveBlend);
            Function.Call(Hash.FORCE_ENTITY_AI_AND_ANIMATION_UPDATE, entity);
        }

        /// <summary>
        /// Assigns a synchronized scene task to a ped.
        ///
        /// moveBlendRatio default WARNING: same magic value issue as PlayEntity.
        /// </summary>
        public void PlayPed(
            Ped ped,
            string animDict,
            string animName,
            float blendIn = 8f,
            float blendOut = 8f,
            SyncedSceneFlags flag = SyncedSceneFlags.None,
            RagdollBlockingFlags ragdollBlockingFlags = RagdollBlockingFlags.None,
            float moveBlendRatio = 1148846080,
            AnimationIKControlFlags ikFlags = AnimationIKControlFlags.None)
        {
            if (!IsActive || ped == null || !ped.Exists()) return;

            Function.Call(Hash.TASK_SYNCHRONIZED_SCENE,
                ped, Handle, animDict, animName,
                blendIn, blendOut,
                (int)flag, (int)ragdollBlockingFlags,
                moveBlendRatio, (int)ikFlags);
            Function.Call(Hash.FORCE_PED_AI_AND_ANIMATION_UPDATE, ped, false, false);
        }

        /// <summary>
        /// Creates or reuses the scene camera and plays its animation.
        /// Times out after 500ms if the camera handle is not returned by the native.
        /// </summary>
        public void PlayCam(string animDict, string animName, int timeout = 500)
        {
            if (!IsActive) return;

            if (Camera == null)
                Camera = Function.Call<Camera>(Hash.CREATE_CAM, "DEFAULT_ANIMATED_CAMERA", true);

            int startTime = Game.GameTime;
            while (Camera == null)
            {
                if (Game.GameTime - startTime > timeout)
                    throw new TimeoutException("Timed out waiting for camera to be created.");
                Script.Wait(0);
            }

            Function.Call(Hash.PLAY_SYNCHRONIZED_CAM_ANIM, Camera, Handle, animName, animDict);
            ScriptCameraDirector.StartRendering();
        }

        public void DeleteCam(bool stopRendering)
        {
            if (Camera != null && Camera.Exists())
            {
                if (stopRendering)
                    ScriptCameraDirector.StopRendering(false);
                Camera.Delete();
                Camera = null;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Audio / attachment
        // ─────────────────────────────────────────────────────────────────────

        public bool PlayAudioEvent()
            => IsActive && Function.Call<bool>(Hash.PLAY_SYNCHRONIZED_AUDIO_EVENT, Handle);

        public void AttachToEntity(Entity entity, EntityBone bone)
        {
            if (IsActive && IsRunning)
                Function.Call(Hash.ATTACH_SYNCHRONIZED_SCENE_TO_ENTITY,
                    Handle, entity, bone.Index);
        }

        public void DetachScene()
        {
            if (IsActive && IsRunning)
                Function.Call(Hash.DETACH_SYNCHRONIZED_SCENE, Handle);
        }
    }
}