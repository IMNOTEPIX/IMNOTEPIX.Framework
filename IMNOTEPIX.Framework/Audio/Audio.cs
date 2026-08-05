using System.Collections.Generic;
using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fAudio
{
    public class Audio
    {
        // Shared lookup used by ChangeMusicEventIntensity and PrepareMusicEventIntensity.
        private static readonly Dictionary<MusicEventIntensity, string> MusicEventNames = new()
        {
            {MusicEventIntensity.Idle, "CH_IDLE"},
            {MusicEventIntensity.Silent, "CH_SILENT"},
            {MusicEventIntensity.Suspense, "CH_SUSPENSE"},
            {MusicEventIntensity.MedIntensity, "CH_MED_INTENSITY"},
            {MusicEventIntensity.Gunfight, "CH_GUNFIGHT"},
            {MusicEventIntensity.VehicleAction, "CH_VEHICLE_ACTION"},
            {MusicEventIntensity.Delivering, "CH_DELIVERING"},
            {MusicEventIntensity.Fail, "CH_FAIL"},
            {MusicEventIntensity.MusicStop, "CH_MUSIC_STOP"},
        };

        public static int GetSoundId() 
            => Function.Call<int>(Hash.GET_SOUND_ID);

        public static void StopSound(int soundId) 
            => Function.Call(Hash.STOP_SOUND, soundId);

        /// <summary>Releases <paramref name="soundId"/> back to the engine.</summary>
        public static void ReleaseSoundId(int soundId) 
            => Function.Call(Hash.RELEASE_SOUND_ID, soundId);

        public static void PlaySoundFromCoord(int soundId, string audioName, Vector3 coord, string audioRef, bool isNetwork = false, int range = 0, bool p8 = false)
           => Function.Call(Hash.PLAY_SOUND_FROM_COORD, soundId, audioName, coord.X, coord.Y, coord.Z, audioRef, isNetwork, range, p8);

        public static void PlaySoundFromEntity(int soundId, string audioName, GTA.Entity entity, string audioRef, bool isNetwork = false, int p5 = 0)
           => Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, soundId, audioName, entity, audioRef, isNetwork, p5);

        public static void PlaySoundFrontend(int soundId, string audioName, string audioRef, bool p3 = false)
            => Function.Call(Hash.PLAY_SOUND_FRONTEND, soundId, audioName, audioRef, p3);

        public static void PlaySoundFrontend(string audioName, string audioRef)
            => Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, audioName, audioRef, false);

        public static void PlayStreamFrontend()
            => Function.Call(Hash.PLAY_STREAM_FRONTEND);

        /// <returns><c>true</c> on successful triggering.</returns>
        public static bool TriggerMusicEvent(string eventName)
            => Function.Call<bool>(Hash.TRIGGER_MUSIC_EVENT, eventName);

        /// <summary>Prepares (pre-caches) a music event without triggering it yet.</summary>
        /// <returns><c>true</c> on successful preparation.</returns>
        public static bool PrepareMusicEvent(string eventName)
            => Function.Call<bool>(Hash.PREPARE_MUSIC_EVENT, eventName);

        /// <returns><c>true</c> on successful canceling.</returns>
        public static bool CancelMusicEvent(string eventName)
            => Function.Call<bool>(Hash.CANCEL_MUSIC_EVENT, eventName);

        public static bool IsMusicPlaying()
            => Function.Call<bool>(Hash.AUDIO_IS_MUSIC_PLAYING);

        public static bool ChangeMusicEventIntensity(MusicEventIntensity intensity)
        {
            if (MusicEventNames.TryGetValue(intensity, out string eventName))
                return TriggerMusicEvent(eventName);
            return false;
        }

        public static bool PrepareMusicEventIntensity(MusicEventIntensity intensity)
        {
            if (MusicEventNames.TryGetValue(intensity, out string eventName))
                return PrepareMusicEvent(eventName);
            return false;
        }

        public static bool CancelMusicEventIntensity(MusicEventIntensity intensity)
        {
            if (MusicEventNames.TryGetValue(intensity, out string eventName))
                return CancelMusicEvent(eventName);
            return false;
        }

        /// <returns><c>true</c> once alarm is ready.</returns>
        public static bool PrepareAlarm(string alarmName)
            => Function.Call<bool>(Hash.PREPARE_ALARM, alarmName);

        /// <param name="playUntilCancelled">If <c>true</c>, the alarm loops until explicitly stopped.</param>
        public static void StartAlarm(string alarmName, bool playUntilCancelled)
            => Function.Call(Hash.START_ALARM, alarmName, playUntilCancelled);

        public static void StopAlarm(string alarmName)
            => Function.Call(Hash.STOP_ALARM, alarmName, true);

        /// <returns><c>true</c> once stream is loaded.</returns>
        public static bool LoadStream(string streamName, string soundSet)
            => Function.Call<bool>(Hash.LOAD_STREAM, streamName, soundSet);

        /// <inheritdoc cref="LoadStream(string, string)"/>
        public static bool LoadStream(string streamName, int soundSet)
            => Function.Call<bool>(Hash.LOAD_STREAM, streamName, soundSet);

        /// <returns><c>true</c> once started.</returns>
        public static bool StartAudioScene(string scene)
            => Function.Call<bool>(Hash.START_AUDIO_SCENE, scene);

        public static void StopAudioScene(string scene)
            => Function.Call(Hash.STOP_AUDIO_SCENE, scene);

        public static void StopAllAudioScenes()
            => Function.Call(Hash.STOP_AUDIO_SCENES);

        public static bool IsAudioSceneActive(string scene)
            => Function.Call<bool>(Hash.IS_AUDIO_SCENE_ACTIVE, scene);

        /// <summary>Activates a named slow-motion audio mode (e.g. pitch/reverb effects during bullet-time).</summary>
        public static void ActivateAudioSlowmoMode(string mode)
            => Function.Call(Hash.ACTIVATE_AUDIO_SLOWMO_MODE, mode);

        public static void SetUserRadioControlEnabled(bool enabled)
            => Function.Call(Hash.SET_USER_RADIO_CONTROL_ENABLED, enabled);

        public static void SetVehicleRadioStation(GTA.Vehicle vehicle, string radioStation)
            => Function.Call(Hash.SET_VEH_RADIO_STATION, vehicle, radioStation);

        public static void SetVehicleRadioEnabled(GTA.Vehicle vehicle, bool enabled)
            => Function.Call(Hash.SET_VEHICLE_RADIO_ENABLED, vehicle, enabled);

        public static void PlayPedAmbientSpeech(GTA.Ped ped, string speechName, string speechParam)
            => Function.Call(Hash.PLAY_PED_AMBIENT_SPEECH_NATIVE, ped, speechName, speechParam);

        public static bool RequestScriptAudioBank(string audioBank, bool bOverNetwork, int playerBits = -1)
            => Function.Call<bool>(Hash.REQUEST_SCRIPT_AUDIO_BANK, audioBank, bOverNetwork, playerBits);

        public static void ReleaseNamedScriptAudioBank(string audioBank)
            => Function.Call(Hash.RELEASE_NAMED_SCRIPT_AUDIO_BANK, audioBank);

        public static bool HasSoundFinished(int soundId)
            => Function.Call<bool>(Hash.HAS_SOUND_FINISHED, soundId);

        public static bool IsAlarmPlaying(string alarmName)
            => Function.Call<bool>(Hash.IS_ALARM_PLAYING, alarmName);

        public static void DisablePedPainAudio(GTA.Ped ped, bool toggle)
            => Function.Call(Hash.DISABLE_PED_PAIN_AUDIO, ped, toggle);

        public static void StopPedSpeaking(GTA.Ped ped, bool shaking)
            => Function.Call(Hash.STOP_PED_SPEAKING, ped, shaking);

        public static void SetFrontendRadioActive(bool active)
            => Function.Call(Hash.SET_​FRONTEND_​RADIO_​ACTIVE, active);

        public static void SetSirenWithNoDriver(GTA.Vehicle vehicle, bool toggle)
            => Function.Call(Hash.SET_​SIREN_​WITH_​NO_​DRIVER, vehicle, toggle);

        public static void CreateNewScriptedConversation()
            => Function.Call(Hash.CREATE_NEW_SCRIPTED_CONVERSATION);

        public static void AddPedToConversation(int speakerConversationIndex, GTA.Ped ped, string voiceName)
            => Function.Call(Hash.ADD_PED_TO_CONVERSATION, speakerConversationIndex, ped, voiceName);

        public static void AddLineToConversation(int speakerConversationIndex, string context, string subtitle, int listenerNumber, int volumeType, bool isRandom, bool interruptible, bool ducksRadio, bool ducksScore, int audibility, bool headset, bool dontInterruptForSpecialAbility, bool isPadSpeakerRoute)
            => Function.Call(Hash.ADD_LINE_TO_CONVERSATION, speakerConversationIndex, context, subtitle, listenerNumber, volumeType, isRandom, interruptible, ducksRadio, ducksScore, audibility, headset, dontInterruptForSpecialAbility, isPadSpeakerRoute);

        public static void AddLineToConversation(int speakerConversationIndex, string context, string subtitle, int listenerNumber, int volumeType, bool isRandom, bool interruptible, bool ducksRadio, bool ducksScore, eAudibility audibility, bool headset, bool dontInterruptForSpecialAbility, bool isPadSpeakerRoute)
            => Function.Call(Hash.ADD_LINE_TO_CONVERSATION, speakerConversationIndex, context, subtitle, listenerNumber, volumeType, isRandom, interruptible, ducksRadio, ducksScore, (int)audibility, headset, dontInterruptForSpecialAbility, isPadSpeakerRoute);

        public static void StartScriptConversation(bool displaySubtitles, bool addToBriefScreen, bool cloneConversation, bool interruptible)
            => Function.Call(Hash.START_SCRIPT_CONVERSATION, displaySubtitles, addToBriefScreen, cloneConversation, interruptible);

        public static void PlayPedAmbientSpeechWithVoiceNative(GTA.Ped ped, string speechName, string voiceName, string speechParam, bool p4 = false)
            => Function.Call(Hash.PLAY_PED_AMBIENT_SPEECH_WITH_VOICE_NATIVE, ped, speechName, voiceName, speechParam, p4);


        public enum eAudibility
        {
            AUD_AUDIBILITY_NORMAL = 0,
            AUD_AUDIBILITY_CLEAR = 1,
            AUD_AUDIBILITY_CRITICAL = 2,
            AUD_AUDIBILITY_LEAD_IN = 3
        }

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
    }
}