using GTA;
using GTA.Math;
using GTA.Native;
using System;

namespace IMNOTEPIX.Framework.fWorld.Anims
{
    public class SynchronizedScene : IDisposable
    {
        public int Handle { get; private set; } = -1;
        public bool IsActive => Handle != -1;

        public Vector3 Position { get; protected set; }
        public Vector3 Rotation { get; protected set; }

        public GTA.Camera Camera { get; set; }

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

        public void Create(int timeout = 500)
        {
            Handle = Function.Call<int>(Hash.CREATE_SYNCHRONIZED_SCENE, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, 2);

            int startTime = Game.GameTime;
            while (!IsActive)
            {
                if (Game.GameTime - startTime > timeout)
                    throw new TimeoutException("Timed out waiting for synchronized scene to become active.");
                GTA.Script.Yield();
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

        public void PlayEntity(GTA.Entity entity, string animDict, string animName, float blendIn = 8f, float blendOut = 8f, SyncedSceneFlags flag = SyncedSceneFlags.None, float moveBlend = 1148846080)
        {
            if (!IsActive || entity == null || !entity.Exists())
                return;

            Function.Call(Hash.PLAY_SYNCHRONIZED_ENTITY_ANIM, entity, Handle, animName, animDict, blendIn, blendOut, (int)flag, moveBlend);
            Function.Call(Hash.FORCE_ENTITY_AI_AND_ANIMATION_UPDATE, entity);
        }

        public void PlayPed(GTA.Ped ped, string animDict, string animName, float blendIn = 8f, float blendOut = 8f, SyncedSceneFlags flag = SyncedSceneFlags.None, RagdollBlockingFlags ragdollBlockingFlags = RagdollBlockingFlags.None, float moveBlendRatio = 1148846080, AnimationIKControlFlags ikFlags = AnimationIKControlFlags.None)
        {
            if (!IsActive || ped == null || !ped.Exists())
                return;

            Function.Call(Hash.TASK_SYNCHRONIZED_SCENE, ped, Handle, animDict, animName, blendIn, blendOut, (int)flag, (int)ragdollBlockingFlags, moveBlendRatio, (int)ikFlags);
            Function.Call(Hash.FORCE_PED_AI_AND_ANIMATION_UPDATE, ped, false, false);
        }

        public void PlayCam(string animDict, string animName, int timeout = 500)
        {
            if (!IsActive)
                return;

            if (Camera == null)
                Camera = Function.Call<GTA.Camera>(Hash.CREATE_CAM, "DEFAULT_ANIMATED_CAMERA", true);

            int startTime = Game.GameTime;
            while (Camera == null)
            {
                if (Game.GameTime - startTime > timeout)
                    throw new TimeoutException("Timed out waiting for camera to be created.");
                GTA.Script.Wait(0);
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

        public bool PlayAudioEvent()
            => IsActive && Function.Call<bool>(Hash.PLAY_SYNCHRONIZED_AUDIO_EVENT, Handle);

        public void AttachToEntity(GTA.Entity entity, EntityBone bone)
        {
            if (IsActive && IsRunning)
                Function.Call(Hash.ATTACH_SYNCHRONIZED_SCENE_TO_ENTITY, Handle, entity, bone.Index);
        }

        public void DetachScene()
        {
            if (IsActive && IsRunning)
                Function.Call(Hash.DETACH_SYNCHRONIZED_SCENE, Handle);
        }
    }
}
