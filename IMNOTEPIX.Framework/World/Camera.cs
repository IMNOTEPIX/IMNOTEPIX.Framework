using GTA;
using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fWorld
{
    public class Camera
    {
        public static void SetGameplayCoordHint(float x, float y, float z, int duration, int blendOutDuration, int blendInDuration, int p6)
            => Function.Call(Hash.SET_GAMEPLAY_COORD_HINT, x, y, z, duration, blendOutDuration, blendInDuration, p6);
        public static void SetGameplayCoordHint(Vector3 xyz, int duration, int blendOutDuration, int blendInDuration, int p6)
            => Function.Call(Hash.SET_GAMEPLAY_COORD_HINT, xyz.X, xyz.Y, xyz.Z, duration, blendOutDuration, blendInDuration, p6);

        public static int GetCamViewModeForContext(int context)
            => Function.Call<int>(Hash.GET_CAM_VIEW_MODE_FOR_CONTEXT, context);

        /// <summary>Creates a named camera. Prefer the typed helpers below over calling this directly.</summary>
        public static GTA.Camera CreateCam(string camName, bool createCamera = true)
            => Function.Call<GTA.Camera>(Hash.CREATE_CAM, camName, createCamera);

        /// <summary>
        /// Creates and returns a <c>DEFAULT_SCRIPTED_CAMERA</c>, yielding one frame
        /// to allow the engine to finish initialising it.
        /// </summary>
        public static GTA.Camera CreateScriptedCam()
        {
            GTA.Camera cam = CreateCam("DEFAULT_SCRIPTED_CAMERA", true);

            if (cam != null && !cam.Exists())
                GTA.Script.Yield();

            return cam;
        }

        /// <summary>
        /// Creates and returns a <c>DEFAULT_ANIMATED_CAMERA</c>, yielding one frame
        /// to allow the engine to finish initialising it.
        /// </summary>
        public static GTA.Camera CreateAnimatedCam()
        {
            GTA.Camera cam = CreateCam("DEFAULT_ANIMATED_CAMERA", true);

            if (cam != null && !cam.Exists())
                GTA.Script.Yield();

            return cam;
        }

        /// <summary>Sets the field of view for <paramref name="cam"/> in degrees.</summary>
        public static void SetCamFov(GTA.Camera cam, float fov)
            => Function.Call(Hash.SET_CAM_FOV, cam, fov);

        /// <summary>
        /// Positions, rotates, and starts shaking <paramref name="cam"/> in one call.
        /// Detaches and stops any active point-at target first.
        /// </summary>
        public static void SetupMovingCam(GTA.Camera cam, Vector3 pos, Vector3 rot, float fov, CameraShake camShakeType, float camShakeIntensity)
        {
            if (cam == null) 
                return;

            cam.Detach();
            cam.StopPointing();
            cam.Position = pos;
            cam.Rotation = rot;
            Function.Call(Hash.SET_CAM_FOV, cam, fov);
            cam.Shake(camShakeType, camShakeIntensity);
        }

        /// <summary>Starts a named shake effect on <paramref name="cam"/>.</summary>
        public static void ShakeCam(GTA.Camera cam, string type, float amplitude)
            => Function.Call(Hash.SHAKE_CAM, cam, type, amplitude);

        /// <summary>Stops the shake effect on <paramref name="cam"/>.</summary>
        public static void StopCamShaking(GTA.Camera cam, bool p1)
            => Function.Call(Hash.STOP_CAM_SHAKING, cam, p1);

        /// <summary>
        /// Transitions from <paramref name="originCam"/> to <paramref name="destinationCam"/>
        /// over <paramref name="durationMs"/> milliseconds using the specified easing curves.
        /// </summary>
        public static void SetCamActiveWithInterp(GTA.Camera destinationCam, GTA.Camera originCam, int durationMs, CamGraphType graphTypePos = CamGraphType.SinAccelDecel,CamGraphType graphTypeRot = CamGraphType.SinAccelDecel)
           => Function.Call(Hash.SET_CAM_ACTIVE_WITH_INTERP, destinationCam, originCam, durationMs, (int)graphTypePos, (int)graphTypeRot);

        /// <summary>
        /// Enables or disables rendering of script cameras.
        /// </summary>
        /// <param name="render">Whether to render script cameras.</param>
        /// <param name="ease">Whether to blend from the game camera.</param>
        /// <param name="easeTimeMs">Blend duration in milliseconds.</param>
        /// <param name="lockInterpolationSource">Locks the source frame during interpolation.</param>
        /// <param name="applyAcrossAllThreads">Applies the change to all script threads.</param>
        /// <param name="renderingOptions">Additional behavioural flags.</param>
        public static void RenderScriptCams(bool render, bool ease, int easeTimeMs, bool lockInterpolationSource = true, bool applyAcrossAllThreads = false, RenderingOptionFlag renderingOptions = RenderingOptionFlag.NoOptions)
           => Function.Call(Hash.RENDER_SCRIPT_CAMS, render, ease, easeTimeMs, lockInterpolationSource, applyAcrossAllThreads, (int)renderingOptions);

        /// <summary>
        /// Stops rendering script cameras, blending back to the game camera via the spline smoothing type.
        /// </summary>
        /// <param name="applyAcrossAllThreads">Applies the change to all script threads.</param>
        /// <param name="distanceToBlend">Blend-out distance.</param>
        /// <param name="blendType">Easing curve for the blend-out transition.</param>
        public static void StopRenderingScriptCamsIntoFirstPerson(bool applyAcrossAllThreads, float distanceToBlend = 0f, CamSplineSmoothingFlags blendType = CamSplineSmoothingFlags.NoSmooth)
           => Function.Call(Hash.STOP_RENDERING_SCRIPT_CAMS_USING_CATCH_UP, applyAcrossAllThreads, distanceToBlend, (int)blendType);

        // ── Animation ────────────────────────────────────────────────────────

        /// <summary>Plays a named camera animation. Returns <c>true</c> on success.</summary>
        public static bool PlayCamAnim(GTA.Camera cam, string animName, string animDict, Vector3 pos, Vector3 rot, CamAnimationFlags animFlags, int rotOrder = 2)
            => Function.Call<bool>(Hash.PLAY_CAM_ANIM, cam, animName, animDict, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, (int)animFlags, rotOrder);

        /// <summary>
        /// Plays a camera animation synchronised to a scene.
        /// Returns <c>true</c> on success.
        /// </summary>
        public static bool PlaySynchronizedCamAnim(GTA.Camera cam, int sceneID, string animName, string animDictionary)
            => Function.Call<bool>(Hash.PLAY_SYNCHRONIZED_CAM_ANIM, cam, sceneID, animName, animDictionary);

        /// <returns><c>true</c> if <paramref name="cam"/> is currently playing the named animation.</returns>
        public static bool IsCamPlayingAnim(GTA.Camera cam, string animName, string animDictionary)
            => Function.Call<bool>(Hash.IS_CAM_PLAYING_ANIM, cam, animName, animDictionary);

        /// <summary>
        /// Controls how a camera spline node transitions in and out.
        /// Passed to <see cref="Camera.StopRenderingScriptCamsIntoFirstPerson"/>.
        /// </summary>
        public enum CamSplineSmoothingFlags
        {
            /// <summary>No smoothing — moves at a constant rate.</summary>
            NoSmooth = 0,
            /// <summary>Decelerates when approaching a node.</summary>
            SlowIn = 1,
            /// <summary>Accelerates slowly when leaving a node.</summary>
            SlowOut = 2,
            /// <summary>Decelerates approaching and accelerates leaving a node.</summary>
            SlowInOut = 3,
            VerySlowIn = 4,
            VerySlowOut = 5,
            VerySlowInSlowOut = 6,
            SlowInVerySlowOut = 7,
            VerySlowInVerySlowOut = 8,
            EaseIn = 9,
            EaseOut = 10,
            QuadraticEaseIn = 11,
            QuadraticEaseOut = 12,
            QuadraticEaseInOut = 13,
            CubicEaseIn = 14,
            CubicEaseOut = 15,
            CubicEaseInOut = 16,
            QuarticEaseIn = 17,
            QuarticEaseOut = 18,
            QuarticEaseInOut = 19,
            QuinticEaseIn = 20,
            QuinticEaseOut = 21,
            QuinticEaseInOut = 22,
            CircularEaseIn = 23,
            CircularEaseOut = 24,
            CircularEaseInOut = 25,
        }

        /// <summary>Easing curve applied to camera position or rotation during an interpolated transition.</summary>
        public enum CamGraphType
        {
            Linear,
            SinAccelDecel,
            Accel,
            Decel,
            SlowIn,
            SlowOut,
            SlowInOut,
            VerySlowIn,
            VerySlowOut,
            VerySlowInSlowOut,
            SlowInVerySlowOut,
            VerySlowInVerySlowOut,
            EaseIn,
            EaseOut,
            QuadraticEaseIn,
            QuadraticEaseOut,
            QuadraticEaseInOut,
            CubicEaseIn,
            CubicEaseOut,
            CubicEaseInOut,
            QuarticEaseIn,
            QuarticEaseOut,
            QuarticEaseInOut,
            QuinticEaseIn,
            QuinticEaseOut,
            QuinticEaseInOut,
            CircularEaseIn,
            CircularEaseOut,
            CircularEaseInOut,
            Max,
        }

        /// <summary>Flags passed to <see cref="Camera.PlayCamAnim"/>.</summary>
        public enum CamAnimationFlags
        {
            Default,
            Looping,
        }

        /// <summary>Additional rendering options for <see cref="Camera.RenderScriptCams"/>.</summary>
        public enum RenderingOptionFlag
        {
            NoOptions,
            StopWhenPlayerExitsIntoCover,
        }
    }
}
