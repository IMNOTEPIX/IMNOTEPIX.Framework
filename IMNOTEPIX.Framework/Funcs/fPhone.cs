using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace IMNOTEPIX.Framework.Funcs
{
    public static class fPhone
    {
        // ─────────────────────────────────────────────────────────────────────
        // Scaleform movie name per character model
        //
        // Michael and all non-story peds use ifruit.
        // Franklin → badger, Trevor → facade.
        // ─────────────────────────────────────────────────────────────────────

        private static readonly Dictionary<uint, string> _scaleformByModel
            = new Dictionary<uint, string>
        {
            { 2602752943U, "cellphone_badger"  }, // Franklin
            { 2608926626U, "cellphone_facade"  }, // Trevor
        };

        private const string DefaultScaleform = "cellphone_ifruit"; // Michael + freemode

        // ─────────────────────────────────────────────────────────────────────
        // Sound set per character model
        // ─────────────────────────────────────────────────────────────────────

        private static readonly Dictionary<uint, string> _soundSetByModel
            = new Dictionary<uint, string>
        {
            { 225514697U,  "Phone_SoundSet_Michael"  },
            { 2602752943U, "Phone_SoundSet_Franklin" },
            { 2608926626U, "Phone_SoundSet_Trevor"   },
        };

        private const string DefaultSoundSet = "Phone_SoundSet_Default";

        // ─────────────────────────────────────────────────────────────────────
        // Handle — cached after first load
        //
        // Previously requested and waited on every single call, which caused
        // multiple blocking yields per scaleform method sequence.
        // ─────────────────────────────────────────────────────────────────────

        private static int _handle = -1;

        /// <summary>
        /// Returns the scaleform movie handle for the current player's phone.
        /// Loads and caches on first access; use InvalidateHandle() if the
        /// player model changes mid-session.
        /// </summary>
        public static int Handle
        {
            get
            {
                if (_handle != -1) return _handle;

                uint modelHash = (uint)Game.Player.Character.Model.Hash;
                string movieName = _scaleformByModel.TryGetValue(modelHash, out var name)
                    ? name
                    : DefaultScaleform;

                int handle = Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, movieName);

                while (!Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, handle))
                    Script.Yield();

                _handle = handle;
                return _handle;
            }
        }

        /// <summary>
        /// Clears the cached handle. Call if the player model changes.
        /// </summary>
        public static void InvalidateHandle() => _handle = -1;

        // ─────────────────────────────────────────────────────────────────────
        // Audio
        // ─────────────────────────────────────────────────────────────────────

        public static void PlayTextArriveTone()
        {
            uint modelHash = (uint)Game.Player.Character.Model.Hash;
            string soundSet = _soundSetByModel.TryGetValue(modelHash, out var s)
                ? s
                : DefaultSoundSet;

            Function.Call(Hash.PLAY_SOUND_FRONTEND,
                -1, "Text_Arrive_Tone", soundSet, true);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Scaleform methods
        // ─────────────────────────────────────────────────────────────────────

        public static void SetTextHeader(string text)
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, Handle, "SET_HEADER");
            Function.Call(Hash.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PHONE_NUMBER, text, -1);
            Function.Call(Hash.END_TEXT_COMMAND_SCALEFORM_STRING);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
        }

        public static void SetNewText(
            string contact,
            string message,
            string contactTXD = "char_default")
        {
            SetSoftKeyIcon(1, SoftKeyIcons.Yes);
            SetSoftKeyColor(1, Color.Green);
            SetSoftKeyIcon(2, SoftKeyIcons.Blank);
            SetSoftKeyIcon(3, SoftKeyIcons.No);
            SetTextHeader("Texts");

            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, Handle, "SET_DATA_SLOT_EMPTY");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 7);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);

            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, Handle, "SET_DATA_SLOT");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 7);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 0);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_PLAYER_NAME_STRING, contact);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_PLAYER_NAME_STRING,
                $"<textarea rows=\"1000\" cols=\"1000\">{message}</textarea>");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_PLAYER_NAME_STRING, contactTXD);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);

            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, Handle, "DISPLAY_VIEW");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 7);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 0);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, -1082130432);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, -1082130432);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, -1082130432);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
        }

        public static int GetSelectedIndex()
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, Handle, "GET_CURRENT_SELECTION");
            int methodReturn = Function.Call<int>(Hash.END_SCALEFORM_MOVIE_METHOD_RETURN_VALUE);

            while (!Function.Call<bool>(Hash.IS_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_READY, methodReturn))
                Script.Wait(0);

            return Function.Call<int>(Hash.GET_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_INT, methodReturn);
        }

        public static void SetSoftKeyColor(int buttonID, Color color)
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, Handle, "SET_SOFT_KEYS_COLOUR");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, buttonID);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, (int)color.R);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, (int)color.G);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, (int)color.B);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
        }

        public static void SetSoftKeyIcon(int buttonID, SoftKeyIcons icon)
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, Handle, "SET_SOFT_KEYS");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, buttonID);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_BOOL, true);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, (int)icon);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Enums
        // ─────────────────────────────────────────────────────────────────────

        public enum SoftKeyIcons
        {
            Blank = 1,
            Select,
            Pages,
            Back,
            Call,
            Hangup,
            HangupHuman,
            Week,
            Keypad,
            Open,
            Reply,
            Delete,
            Yes,
            No,
            Sort,
            Website,
            Police,
            Ambulance,
            Fire,
            Pages2
        }
    }
}