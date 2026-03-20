using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.Funcs
{
    /// <summary>
    /// Intensity levels that map to named music events on the CH ambient music system.
    /// Used with <see cref="fAudio.ChangeMusicEventIntensity"/> and
    /// <see cref="fAudio.PrepareMusicEventIntensity"/>.
    /// </summary>
    public enum MusicEventIntensity
    {
        Idle,
        Silent,
        Suspense,
        MedIntensity,
        Gunfight,
        VehicleAction,
        Delivering,
        Fail,
        MusicStop,
    }

    public static class fAudio
    {
        // Shared lookup used by ChangeMusicEventIntensity and PrepareMusicEventIntensity.
        private static readonly Dictionary<MusicEventIntensity, string> MusicEventNames =
            new Dictionary<MusicEventIntensity, string>
            {
                { MusicEventIntensity.Idle,            "CH_IDLE"           },
                { MusicEventIntensity.Silent,          "CH_SILENT"         },
                { MusicEventIntensity.Suspense,        "CH_SUSPENSE"       },
                { MusicEventIntensity.MedIntensity,    "CH_MED_INTENSITY"  },
                { MusicEventIntensity.Gunfight,        "CH_GUNFIGHT"       },
                { MusicEventIntensity.VehicleAction,   "CH_VEHICLE_ACTION" },
                { MusicEventIntensity.Delivering,      "CH_DELIVERING"     },
                { MusicEventIntensity.Fail,            "CH_FAIL"           },
                { MusicEventIntensity.MusicStop,       "CH_MUSIC_STOP"     },
            };

        // ── Sound IDs ────────────────────────────────────────────────────────────

        /// <summary>Allocates a new sound ID slot for subsequent play/stop calls.</summary>
        public static int GetSoundId()
            => Function.Call<int>(Hash.GET_SOUND_ID);

        /// <summary>Stops any sound currently playing on <paramref name="soundId"/>.</summary>
        public static void StopSound(int soundId)
            => Function.Call(Hash.STOP_SOUND, soundId);

        /// <summary>
        /// Releases <paramref name="soundId"/> back to the engine.
        /// Always call this after you are finished with a slot obtained from <see cref="GetSoundId"/>.
        /// </summary>
        public static void ReleaseSoundId(int soundId)
            => Function.Call(Hash.RELEASE_SOUND_ID, soundId);

        // ── Positional / entity sounds ───────────────────────────────────────────

        /// <summary>Plays a named sound originating from a world position.</summary>
        public static void PlaySoundFromCoord(
            int soundId, string audioName, Vector3 coord,
            string audioRef, bool isNetwork = false, int range = 0, bool p8 = false)
        {
            Function.Call(Hash.PLAY_SOUND_FROM_COORD,
                soundId, audioName, coord.X, coord.Y, coord.Z,
                audioRef, isNetwork, range, p8);
        }

        /// <summary>Plays a named sound attached to <paramref name="entity"/>.</summary>
        public static void PlaySoundFromEntity(
            int soundId, string audioName, Entity entity,
            string audioRef, bool isNetwork = false, int p5 = 0)
        {
            Function.Call(Hash.PLAY_SOUND_FROM_ENTITY,
                soundId, audioName, entity, audioRef, isNetwork, p5);
        }

        // ── Frontend sounds ──────────────────────────────────────────────────────

        /// <summary>
        /// Plays a frontend (UI/non-positional) sound into an existing <paramref name="soundId"/> slot.
        /// Pass the result of <see cref="GetSoundId"/> when you need to stop it later;
        /// pass <c>-1</c> to fire-and-forget without tracking the slot.
        /// </summary>
        public static void PlaySoundFrontend(int soundId, string audioName, string audioRef, bool enableOnReplay = true)
            => Function.Call(Hash.PLAY_SOUND_FRONTEND, soundId, audioName, audioRef, enableOnReplay);

        /// <summary>
        /// Fire-and-forget variant: plays a frontend sound without tracking its slot.
        /// Use the overload with an explicit <c>soundId</c> if you need to stop it early.
        /// </summary>
        public static void PlaySoundFrontend(string audioName, string audioRef)
            => Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, audioName, audioRef, true);

        /// <summary>
        /// Plays a previously loaded audio stream through the frontend channel.
        /// Call <see cref="LoadStream"/> first to prepare the stream.
        /// </summary>
        public static void PlayStreamFrontend()
            => Function.Call(Hash.PLAY_STREAM_FRONTEND);

        // ── Music ────────────────────────────────────────────────────────────────

        /// <summary>Fires a music event by name. Returns <c>true</c> on success.</summary>
        public static bool TriggerMusicEvent(string eventName)
            => Function.Call<bool>(Hash.TRIGGER_MUSIC_EVENT, eventName);

        /// <summary>Prepares (pre-caches) a music event without triggering it yet.</summary>
        public static bool PrepareMusicEvent(string eventName)
            => Function.Call<bool>(Hash.PREPARE_MUSIC_EVENT, eventName);

        /// <summary>Cancels a currently active music event.</summary>
        public static bool CancelMusicEvent(string eventName)
            => Function.Call<bool>(Hash.CANCEL_MUSIC_EVENT, eventName);

        /// <returns><c>true</c> if any music track is currently playing.</returns>
        public static bool IsMusicPlaying()
            => Function.Call<bool>(Hash.AUDIO_IS_MUSIC_PLAYING);

        /// <summary>Immediately triggers the CH music event matching <paramref name="intensity"/>.</summary>
        public static void ChangeMusicEventIntensity(MusicEventIntensity intensity)
        {
            if (MusicEventNames.TryGetValue(intensity, out string eventName))
                TriggerMusicEvent(eventName);
        }

        /// <summary>Pre-caches the CH music event matching <paramref name="intensity"/> without triggering it.</summary>
        public static void PrepareMusicEventIntensity(MusicEventIntensity intensity)
        {
            if (MusicEventNames.TryGetValue(intensity, out string eventName))
                PrepareMusicEvent(eventName);
        }

        // ── Alarms ───────────────────────────────────────────────────────────────

        /// <summary>Prepares an alarm for playback. Returns <c>true</c> once ready.</summary>
        public static bool PrepareAlarm(string alarmName)
            => Function.Call<bool>(Hash.PREPARE_ALARM, alarmName);

        /// <param name="playUntilCancelled">If <c>true</c>, the alarm loops until explicitly stopped.</param>
        public static void StartAlarm(string alarmName, bool playUntilCancelled)
            => Function.Call(Hash.START_ALARM, alarmName, playUntilCancelled);

        // ── Streams ──────────────────────────────────────────────────────────────

        /// <summary>Loads a named audio stream from a sound set. Returns <c>true</c> on success.</summary>
        public static bool LoadStream(string streamName, string soundSet)
            => Function.Call<bool>(Hash.LOAD_STREAM, streamName, soundSet);

        /// <inheritdoc cref="LoadStream(string, string)"/>
        public static bool LoadStream(string streamName, int soundSet)
            => Function.Call<bool>(Hash.LOAD_STREAM, streamName, soundSet);

        // ── Audio scenes ─────────────────────────────────────────────────────────

        /// <summary>Starts a named audio scene (ambient audio context). Returns <c>true</c> on success.</summary>
        public static bool StartAudioScene(string scene)
            => Function.Call<bool>(Hash.START_AUDIO_SCENE, scene);

        /// <summary>Stops a specific active audio scene.</summary>
        public static void StopAudioScene(string scene)
            => Function.Call(Hash.STOP_AUDIO_SCENE, scene);

        /// <summary>Stops all currently active audio scenes.</summary>
        public static void StopAllAudioScenes()
            => Function.Call(Hash.STOP_AUDIO_SCENES);

        /// <returns><c>true</c> if the named audio scene is currently active.</returns>
        public static bool IsAudioSceneActive(string scene)
            => Function.Call<bool>(Hash.IS_AUDIO_SCENE_ACTIVE, scene);

        // ── Slowmo ───────────────────────────────────────────────────────────────

        /// <summary>Activates a named slow-motion audio mode (e.g. pitch/reverb effects during bullet-time).</summary>
        public static void ActivateAudioSlowmoMode(string mode)
            => Function.Call(Hash.ACTIVATE_AUDIO_SLOWMO_MODE, mode);

        // ── Radio ────────────────────────────────────────────────────────────────

        /// <summary>Enables or disables the player's ability to change radio stations.</summary>
        public static void SetUserRadioControlEnabled(bool enabled)
            => Function.Call(Hash.SET_USER_RADIO_CONTROL_ENABLED, enabled);

        /// <summary>Sets the radio station playing in <paramref name="vehicle"/> by name.</summary>
        public static void SetVehicleRadioStation(Vehicle vehicle, string radioStation)
            => Function.Call(Hash.SET_VEH_RADIO_STATION, vehicle, radioStation);

        /// <summary>Enables or disables the radio in <paramref name="vehicle"/> entirely.</summary>
        public static void SetVehicleRadioEnabled(Vehicle vehicle, bool enabled)
            => Function.Call(Hash.SET_VEHICLE_RADIO_ENABLED, vehicle, enabled);

        // ── Ped speech ───────────────────────────────────────────────────────────

        /// <summary>
        /// Triggers a named ambient speech line on <paramref name="ped"/>.
        /// <paramref name="speechParam"/> controls interruption behaviour
        /// (e.g. <c>"SPEECH_PARAMS_FORCE"</c>).
        /// </summary>
        public static void PlayPedAmbientSpeech(GTA.Ped ped, string speechName, string speechParam, int unk = 1)
            => Function.Call(Hash.PLAY_PED_AMBIENT_SPEECH_NATIVE, ped, speechName, speechParam, unk);
    }
}