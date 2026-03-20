using GTA;
using GTA.Native;

namespace IMNOTEPIX.Framework.Funcs
{
    public static class fScaleforms
    {
        // ─────────────────────────────────────────────────────────────────────
        // TXD wrapper — distinguishes texture name strings from plain strings
        // when passed through the untyped params object[] pipeline.
        // ─────────────────────────────────────────────────────────────────────

        public class TXD
        {
            public string Texture { get; }

            public TXD(string texture)
            {
                Texture = texture;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Queries
        // ─────────────────────────────────────────────────────────────────────

        public static bool HasScaleformMovieLoaded(int scaleID)
            => Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, scaleID);

        // ─────────────────────────────────────────────────────────────────────
        // Call wrappers
        // ─────────────────────────────────────────────────────────────────────

        public static void CallFunction(int handle, string name, params object[] args)
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, handle, name);
            PushArgs(args);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
        }

        public static bool CallFunctionBool(int handle, string name, params object[] args)
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, handle, name);
            PushArgs(args);
            int ret = Function.Call<int>(Hash.END_SCALEFORM_MOVIE_METHOD_RETURN_VALUE);
            while (!Function.Call<bool>(Hash.IS_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_READY, ret))
                Script.Yield();
            return Function.Call<bool>(Hash.GET_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_BOOL, ret);
        }

        public static int CallFunctionInt(int handle, string name, params object[] args)
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, handle, name);
            PushArgs(args);
            int ret = Function.Call<int>(Hash.END_SCALEFORM_MOVIE_METHOD_RETURN_VALUE);
            while (!Function.Call<bool>(Hash.IS_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_READY, ret))
                Script.Yield();
            return Function.Call<int>(Hash.GET_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_INT, ret);
        }

        public static string CallFunctionString(int handle, string name, params object[] args)
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, handle, name);
            PushArgs(args);
            int ret = Function.Call<int>(Hash.END_SCALEFORM_MOVIE_METHOD_RETURN_VALUE);
            while (!Function.Call<bool>(Hash.IS_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_READY, ret))
                Script.Yield();
            return Function.Call<string>(Hash.GET_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_STRING, ret);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Argument marshalling
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Pushes each argument into the active scaleform method call using the
        /// appropriate native for its type. TXD is pushed as a texture name string;
        /// string and char go through the text component pipeline; all others use
        /// their typed ADD_PARAM native.
        /// </summary>
        private static void PushArgs(object[] args)
        {
            foreach (object arg in args)
            {
                if (arg is int i)
                    Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, i);
                else if (arg is float f)
                    Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_FLOAT, f);
                else if (arg is double d)
                    Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_FLOAT, (float)d);
                else if (arg is bool b)
                    Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_BOOL, b);
                else if (arg is TXD txd)
                    Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_TEXTURE_NAME_STRING, txd.Texture);
                else if (arg is string s)
                    PushTextComponent(s);
                else if (arg is char c)
                    PushTextComponent(c.ToString());
            }
        }

        private static void PushTextComponent(string text)
        {
            Function.Call(Hash.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
            Function.Call(Hash.END_TEXT_COMMAND_SCALEFORM_STRING);
        }
    }
}