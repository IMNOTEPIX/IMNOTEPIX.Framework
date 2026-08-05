using System;
using System.Collections.Generic;
using System.Text;
using GTA.Native;

namespace IMNOTEPIX.Framework.fUI
{
    /// <summary>
    /// Helper class for text command natives.
    /// Ported from RAGENativeUI's TextCommands. The original used Rage.Native.NativeString
    /// (RPH-specific unsafe pointer marshaling) to walk the string byte-by-byte and split it
    /// into &lt;=98-byte chunks without breaking a '~' token, feeding each chunk to
    /// ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME. SHVDN's Function.Call marshals managed
    /// strings itself, so the chunking here works directly over UTF-8 bytes of the managed
    /// string instead of an unsafe sbyte* buffer - same splitting behavior, no unsafe code needed.
    /// </summary>
    internal static class TextCommands
    {
        private const string ShortStringFormat = "STRING";
        private const string LongStringFormat = "CELL_EMAIL_BCON";
        private const int MaxSubstringLength = 98;

        /// <summary>
        /// Splits <paramref name="text"/> into &lt;=<see cref="MaxSubstringLength"/>-byte (UTF-8) chunks,
        /// avoiding splitting a '~' token in half - mirrors the original's NextSubstring.
        /// </summary>
        private static IEnumerable<string> SplitIntoSubstrings(string text)
        {
            const byte TokenAffix = (byte)'~';

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            int pos = 0;
            while (pos < bytes.Length)
            {
                int length = Math.Min(MaxSubstringLength, bytes.Length - pos);

                int tokenStart = -1;
                for (int i = 0; i < length; i++)
                {
                    if (bytes[pos + i] == TokenAffix)
                    {
                        tokenStart = tokenStart == -1 ? i : -1;
                    }
                }

                int actualLength = (tokenStart != -1 && tokenStart > 0) ? tokenStart : length;

                yield return Encoding.UTF8.GetString(bytes, pos, actualLength);
                pos += actualLength;
            }
        }

        private static void PushText(Action<string> beginCommand, string text)
        {
            if (Encoding.UTF8.GetByteCount(text) <= MaxSubstringLength)
            {
                beginCommand(ShortStringFormat);
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
            }
            else
            {
                beginCommand(LongStringFormat);
                foreach (string chunk in SplitIntoSubstrings(text))
                {
                    Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, chunk);
                }
            }
        }

        public static float GetWidth(string text, in TextStyle style)
        {
            style.Apply();
            return GetWidth(text);
        }

        public static float GetWidth(string text)
        {
            PushText(s => Function.Call(Hash.BEGIN_TEXT_COMMAND_GET_SCREEN_WIDTH_OF_DISPLAY_TEXT, s), text);
            return Function.Call<float>(Hash.END_TEXT_COMMAND_GET_SCREEN_WIDTH_OF_DISPLAY_TEXT, true);
        }

        public static void Display(string text, in TextStyle style, float x, float y)
        {
            style.Apply();
            Display(text, x, y);
        }

        public static void Display(string text, float x, float y)
        {
            PushText(s => Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, s), text);
            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y);
        }

        public static int GetLineCount(string text, in TextStyle style, float x, float y)
        {
            style.Apply();
            return GetLineCount(text, x, y);
        }

        public static int GetLineCount(string text, float x, float y)
        {
            // NOTE: verify BEGIN_TEXT_COMMAND_LINE_COUNT / END_TEXT_COMMAND_LINE_COUNT
            // are the exact Hash member names in your SHVDN build.
            PushText(s => Function.Call(Hash.BEGIN_TEXT_COMMAND_GET_NUMBER_OF_LINES_FOR_STRING, s), text);
            return Function.Call<int>(Hash.END_TEXT_COMMAND_GET_NUMBER_OF_LINES_FOR_STRING, x, y);
        }
    }
}