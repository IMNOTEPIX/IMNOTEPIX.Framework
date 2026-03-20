using GTA;
using GTA.Native;
using IMNOTEPIX.Framework.Interfaces;
using IMNOTEPIX.Framework.Funcs;

namespace IMNOTEPIX.Framework.Scaleforms
{
    public class MissionShard : IScaleform
    {
        // ─────────────────────────────────────────────────────────────────────
        // Sound configuration — private constants; not intended to be mutated
        // per-instance. Override by subclassing if variation is needed.
        // ─────────────────────────────────────────────────────────────────────

        private const string ShardInSoundName = "Shard_Appear";
        private const string ShardInSoundSet = "GTAO_FM_Events_Soundset";
        private const string ShardOutSoundName = "Shard_Disappear";
        private const string ShardOutSoundSet = "GTAO_FM_Events_Soundset";
        private const string ShardFailAudioBank = "DLC_MP2023_1/DLC_MP2023_1_Bicycle_Race";
        private const string ShardFailSoundName = "Fail";
        private const string ShardFailSoundSet = "Bike_Time_Trials_Soundset";

        // ─────────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────────

        private int _scaleId = 0;

        // ─────────────────────────────────────────────────────────────────────
        // IScaleform
        // ─────────────────────────────────────────────────────────────────────

        public void RequestScaleform()
        {
            _scaleId = Function.Call<int>(
                Hash.REQUEST_SCALEFORM_MOVIE_WITH_IGNORE_SUPER_WIDESCREEN,
                "MIDSIZED_MESSAGE");

            while (!fScaleforms.HasScaleformMovieLoaded(_scaleId))
                Script.Wait(0);
        }

        /// <summary>
        /// Releases the scaleform handle.
        /// SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED takes a pointer and zeroes it —
        /// a local copy is used so the native zeroes the copy, not the field,
        /// allowing us to explicitly reset _scaleId to 0 ourselves.
        /// </summary>
        public unsafe void DeleteScaleform()
        {
            int handle = _scaleId;
            Function.Call(Hash.SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED, &handle);
            _scaleId = 0;
        }

        public void DrawScaleform()
        {
            if (!Game.Player.Character.IsDead)
                Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN,
                    _scaleId, 255, 255, 255, 255, 0);
        }

        public void Dispose()
        {
            if (_scaleId != 0)
                DeleteScaleform();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Shard display
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Plays the full shard in → hold → out sequence. This is a blocking call —
        /// it must be called from a SHVDN coroutine (Script.OnTick or similar).
        ///
        /// <paramref name="color"/> and <paramref name="colorOut"/> are GTA V shard
        /// colour palette indices (0 = white/default, 2 = gold/mission-complete, etc.).
        /// </summary>
        public void ShardIn(
            string shardName,
            string shardDescription,
            int color = 2,
            float speed = 0.5f,
            int colorOut = 0,
            bool failShard = false)
        {
            DeleteScaleform();
            RequestScaleform();

            // Brief settle wait after scaleform load before pushing content.
            Script.Wait(500);

            fScaleforms.CallFunction(_scaleId,
                "SHOW_SHARD_MIDSIZED_MESSAGE",
                shardName, shardDescription, color, false, true);

            if (failShard)
            {
                Function.Call(Hash.REQUEST_SCRIPT_AUDIO_BANK, ShardFailAudioBank, true, -1);
                Function.Call(Hash.PLAY_SOUND_FRONTEND,
                    -1, ShardFailSoundName, ShardFailSoundSet, true);
            }
            else
            {
                Function.Call(Hash.PLAY_SOUND_FRONTEND,
                    -1, ShardInSoundName, ShardInSoundSet, true);
            }

            // Hold the shard on screen for 7 seconds
            int endTime = Game.GameTime + 7000;
            while (Game.GameTime < endTime)
            {
                DrawScaleform();
                Script.Wait(0);
            }

            ShardOut(colorOut, speed);

            // Allow the out animation to finish before cleanup
            endTime = Game.GameTime + 2000;
            while (Game.GameTime < endTime)
            {
                DrawScaleform();
                Script.Wait(0);
            }

            DeleteScaleform();
            Function.Call(Hash.RELEASE_NAMED_SCRIPT_AUDIO_BANK, ShardFailAudioBank);
        }

        /// <summary>
        /// Triggers the shard out animation and plays the dismiss sound.
        /// </summary>
        public void ShardOut(int color, float speed)
        {
            fScaleforms.CallFunction(_scaleId, "SHARD_ANIM_OUT", color, speed);
            Function.Call(Hash.PLAY_SOUND_FRONTEND,
                -1, ShardOutSoundName, ShardOutSoundSet, true);
        }
    }
}