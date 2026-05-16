using System.Windows.Forms;

namespace IMNOTEPIX.Framework.fUI.Scaleforms
{
    public static class InstructionalKeyExtension
    {
        /// <summary>
        /// Returns the scaleform button ID string for the given <see cref="InstructionalKey"/>.
        /// <para>
        /// NOTE: The original RAGE implementation used CTextFormat, an internal RAGE memory API,
        /// to dynamically resolve icon format strings for keyboard keys, mouse axes, and controller
        /// axes at runtime. That is not available in SHVDN.
        /// <br/>
        /// - Special symbol/controller combo keys: hardcoded (identical to the original).
        /// - Controller buttons: resolved via a lookup table of known scaleform "b_X" IDs.
        /// - Keyboard keys: resolved as text-label IDs (e.g. "t_F", "T_F5", "w_Enter") which
        ///   show the key name as text inside the button rather than a graphical key icon.
        /// - Mouse/controller axes, and anything else: returns the "b_995" fallback (red "???").
        /// </para>
        /// </summary>
        public static string GetId(this InstructionalKey key)
        {
            // --- Hardcoded special cases (identical to the original) ---
            switch (key)
            {
                case InstructionalKey.ControllerDPadNone: return "b_8";
                case InstructionalKey.ControllerDPadAll: return "b_9";
                case InstructionalKey.ControllerDPadUpDown: return "b_10";
                case InstructionalKey.ControllerDPadLeftRight: return "b_11";
                case InstructionalKey.ControllerLStickRotate: return "b_20";
                case InstructionalKey.ControllerRStickRotate: return "b_29";
                case InstructionalKey.SymbolBusySpinner: return "b_44";
                case InstructionalKey.SymbolPlus: return "b_998";
                case InstructionalKey.SymbolArrowUp: return "b_0";
                case InstructionalKey.SymbolArrowDown: return "b_1";
                case InstructionalKey.SymbolArrowLeft: return "b_2";
                case InstructionalKey.SymbolArrowRight: return "b_3";
                case InstructionalKey.SymbolArrowUpDown: return "b_45";
                case InstructionalKey.SymbolArrowLeftRight: return "b_46";
                case InstructionalKey.SymbolArrowAll: return "b_47";
                case InstructionalKey.Mouse: return "b_114";
            }

            uint parameter = (uint)key;

            // --- Controller digital buttons (0x2000 mask) ---
            // Lookup table of known GTA scaleform "b_X" IDs for controller buttons.
            if ((parameter & 0x2000) != 0)
            {
                switch (key)
                {
                    case InstructionalKey.ControllerLTrigger: return "b_194";
                    case InstructionalKey.ControllerRTrigger: return "b_193";
                    case InstructionalKey.ControllerLShoulder: return "b_192";
                    case InstructionalKey.ControllerRShoulder: return "b_191";
                    case InstructionalKey.ControllerA: return "b_196"; // Cross  / A
                    case InstructionalKey.ControllerB: return "b_197"; // Circle / B
                    case InstructionalKey.ControllerX: return "b_195"; // Square / X
                    case InstructionalKey.ControllerY: return "b_198"; // Triangle / Y
                    case InstructionalKey.ControllerStart: return "b_200";
                    case InstructionalKey.ControllerBack: return "b_199"; // Select
                    case InstructionalKey.ControllerLThumb: return "b_202"; // L3
                    case InstructionalKey.ControllerRThumb: return "b_203"; // R3
                    case InstructionalKey.ControllerDPadUp: return "b_172";
                    case InstructionalKey.ControllerDPadDown: return "b_173";
                    case InstructionalKey.ControllerDPadLeft: return "b_174";
                    case InstructionalKey.ControllerDPadRight: return "b_175";
                    default: return "b_995";
                }
            }

            // --- Mouse buttons (0x800 mask) ---
            if ((parameter & 0x800) != 0)
            {
                switch (key)
                {
                    case InstructionalKey.MouseLeft: return "b_239";
                    case InstructionalKey.MouseRight: return "b_240";
                    case InstructionalKey.MouseMiddle: return "b_241";
                    default: return "b_995";
                }
            }

            // --- Mouse wheel (0x400 mask) ---
            if ((parameter & 0x400) != 0)
            {
                switch (key)
                {
                    case InstructionalKey.MouseWheelUp: return "b_242";
                    case InstructionalKey.MouseWheelDown: return "b_243";
                    case InstructionalKey.MouseWheel: return "b_244";
                    default: return "b_995";
                }
            }

            // --- Mouse axes and controller axes ---
            // CTextFormat was used here in the original. No clean SHVDN equivalent exists.
            if ((parameter & 0x200) != 0 || (parameter & 0x1000) != 0)
                return "b_995";

            // --- Keyboard keys ---
            // CTextFormat was used here too. As a fallback, we generate a text-label ID
            // using the same prefix logic as InstructionalButtonId(string), which renders
            // the key name as text inside the button rather than a graphical icon.
            string keyName = GetKeyName(key);
            if (keyName == null) return "b_995";

            return keyName.Length switch
            {
                int n when n <= 2 => "t_" + keyName,
                int n when n <= 4 => "T_" + keyName,
                _ => "w_" + keyName,
            };
        }

        /// <summary>
        /// Returns a short display name for keyboard <see cref="InstructionalKey"/> values,
        /// used to build a text-label button ID. Returns <c>null</c> for non-keyboard keys.
        /// </summary>
        private static string GetKeyName(InstructionalKey key)
        {
            switch (key)
            {
                case InstructionalKey.Back: return "Bk";
                case InstructionalKey.Tab: return "Tab";
                case InstructionalKey.Return: return "Ent";
                case InstructionalKey.Pause: return "Pse";
                case InstructionalKey.Capital: return "Cap";
                case InstructionalKey.Escape: return "Esc";
                case InstructionalKey.Space: return "Spc";
                case InstructionalKey.PageUp: return "PgUp";
                case InstructionalKey.PageDown: return "PgDn";
                case InstructionalKey.End: return "End";
                case InstructionalKey.Home: return "Hme";
                case InstructionalKey.Left: return "←";
                case InstructionalKey.Up: return "↑";
                case InstructionalKey.Right: return "→";
                case InstructionalKey.Down: return "↓";
                case InstructionalKey.Insert: return "Ins";
                case InstructionalKey.Delete: return "Del";
                case InstructionalKey.D0: return "0";
                case InstructionalKey.D1: return "1";
                case InstructionalKey.D2: return "2";
                case InstructionalKey.D3: return "3";
                case InstructionalKey.D4: return "4";
                case InstructionalKey.D5: return "5";
                case InstructionalKey.D6: return "6";
                case InstructionalKey.D7: return "7";
                case InstructionalKey.D8: return "8";
                case InstructionalKey.D9: return "9";
                case InstructionalKey.A: return "A";
                case InstructionalKey.B: return "B";
                case InstructionalKey.C: return "C";
                case InstructionalKey.D: return "D";
                case InstructionalKey.E: return "E";
                case InstructionalKey.F: return "F";
                case InstructionalKey.G: return "G";
                case InstructionalKey.H: return "H";
                case InstructionalKey.I: return "I";
                case InstructionalKey.J: return "J";
                case InstructionalKey.K: return "K";
                case InstructionalKey.L: return "L";
                case InstructionalKey.M: return "M";
                case InstructionalKey.N: return "N";
                case InstructionalKey.O: return "O";
                case InstructionalKey.P: return "P";
                case InstructionalKey.Q: return "Q";
                case InstructionalKey.R: return "R";
                case InstructionalKey.S: return "S";
                case InstructionalKey.T: return "T";
                case InstructionalKey.U: return "U";
                case InstructionalKey.V: return "V";
                case InstructionalKey.W: return "W";
                case InstructionalKey.X: return "X";
                case InstructionalKey.Y: return "Y";
                case InstructionalKey.Z: return "Z";
                case InstructionalKey.NumPad0: return "N0";
                case InstructionalKey.NumPad1: return "N1";
                case InstructionalKey.NumPad2: return "N2";
                case InstructionalKey.NumPad3: return "N3";
                case InstructionalKey.NumPad4: return "N4";
                case InstructionalKey.NumPad5: return "N5";
                case InstructionalKey.NumPad6: return "N6";
                case InstructionalKey.NumPad7: return "N7";
                case InstructionalKey.NumPad8: return "N8";
                case InstructionalKey.NumPad9: return "N9";
                case InstructionalKey.Multiply: return "N*";
                case InstructionalKey.Add: return "N+";
                case InstructionalKey.Subtract: return "N-";
                case InstructionalKey.Decimal: return "N.";
                case InstructionalKey.Divide: return "N/";
                case InstructionalKey.NumPadEnter: return "NEnt";
                case InstructionalKey.F1: return "F1";
                case InstructionalKey.F2: return "F2";
                case InstructionalKey.F3: return "F3";
                case InstructionalKey.F4: return "F4";
                case InstructionalKey.F5: return "F5";
                case InstructionalKey.F6: return "F6";
                case InstructionalKey.F7: return "F7";
                case InstructionalKey.F8: return "F8";
                case InstructionalKey.F9: return "F9";
                case InstructionalKey.F10: return "F10";
                case InstructionalKey.F11: return "F11";
                case InstructionalKey.F12: return "F12";
                case InstructionalKey.F13: return "F13";
                case InstructionalKey.F14: return "F14";
                case InstructionalKey.F15: return "F15";
                case InstructionalKey.F16: return "F16";
                case InstructionalKey.F17: return "F17";
                case InstructionalKey.F18: return "F18";
                case InstructionalKey.F19: return "F19";
                case InstructionalKey.F20: return "F20";
                case InstructionalKey.F21: return "F21";
                case InstructionalKey.F22: return "F22";
                case InstructionalKey.F23: return "F23";
                case InstructionalKey.F24: return "F24";
                case InstructionalKey.NumLock: return "NLk";
                case InstructionalKey.Scroll: return "ScL";
                case InstructionalKey.LShift: return "LSh";
                case InstructionalKey.RShift: return "RSh";
                case InstructionalKey.LControl: return "LCt";
                case InstructionalKey.RControl: return "RCt";
                case InstructionalKey.LMenu: return "LAlt";
                case InstructionalKey.RMenu: return "RAlt";
                case InstructionalKey.LWin: return "LWin";
                case InstructionalKey.RWin: return "RWin";
                case InstructionalKey.Apps: return "App";
                case InstructionalKey.Oem1: return ";";
                case InstructionalKey.Plus: return "=";
                case InstructionalKey.Comma: return ",";
                case InstructionalKey.Minus: return "-";
                case InstructionalKey.Period: return ".";
                case InstructionalKey.Oem2: return "/";
                case InstructionalKey.Oem3: return "`";
                case InstructionalKey.Oem4: return "[";
                case InstructionalKey.Oem5: return "\\";
                case InstructionalKey.Oem6: return "]";
                case InstructionalKey.Oem7: return "'";
                case InstructionalKey.Oem102: return "\\";
                default: return null;
            }
        }

        /// <summary>
        /// Returns the scaleform button ID string for a Windows Forms <see cref="Keys"/> value.
        /// </summary>
        public static string GetInstructionalId(this Keys key) => key.GetInstructionalKey().GetId();

        /// <summary>
        /// Maps a Windows Forms <see cref="Keys"/> value to the corresponding <see cref="InstructionalKey"/>.
        /// </summary>
        public static InstructionalKey GetInstructionalKey(this Keys key)
            => key switch
            {
                Keys.LButton => InstructionalKey.MouseLeft,
                Keys.RButton => InstructionalKey.MouseRight,
                Keys.MButton => InstructionalKey.MouseMiddle,
                Keys.XButton1 => InstructionalKey.MouseExtra1,
                Keys.XButton2 => InstructionalKey.MouseExtra2,
                Keys.Back => InstructionalKey.Back,
                Keys.Tab => InstructionalKey.Tab,
                Keys.Return => InstructionalKey.Return,
                Keys.ShiftKey => InstructionalKey.LShift,
                Keys.ControlKey => InstructionalKey.LControl,
                Keys.Menu => InstructionalKey.LMenu,
                Keys.Pause => InstructionalKey.Pause,
                Keys.Capital => InstructionalKey.Capital,
                Keys.Escape => InstructionalKey.Escape,
                Keys.Space => InstructionalKey.Space,
                Keys.PageUp => InstructionalKey.PageUp,
                Keys.PageDown => InstructionalKey.PageDown,
                Keys.End => InstructionalKey.End,
                Keys.Home => InstructionalKey.Home,
                Keys.Left => InstructionalKey.Left,
                Keys.Up => InstructionalKey.Up,
                Keys.Right => InstructionalKey.Right,
                Keys.Down => InstructionalKey.Down,
                Keys.Snapshot => InstructionalKey.Snapshot,
                Keys.Insert => InstructionalKey.Insert,
                Keys.Delete => InstructionalKey.Delete,
                Keys.D0 => InstructionalKey.D0,
                Keys.D1 => InstructionalKey.D1,
                Keys.D2 => InstructionalKey.D2,
                Keys.D3 => InstructionalKey.D3,
                Keys.D4 => InstructionalKey.D4,
                Keys.D5 => InstructionalKey.D5,
                Keys.D6 => InstructionalKey.D6,
                Keys.D7 => InstructionalKey.D7,
                Keys.D8 => InstructionalKey.D8,
                Keys.D9 => InstructionalKey.D9,
                Keys.A => InstructionalKey.A,
                Keys.B => InstructionalKey.B,
                Keys.C => InstructionalKey.C,
                Keys.D => InstructionalKey.D,
                Keys.E => InstructionalKey.E,
                Keys.F => InstructionalKey.F,
                Keys.G => InstructionalKey.G,
                Keys.H => InstructionalKey.H,
                Keys.I => InstructionalKey.I,
                Keys.J => InstructionalKey.J,
                Keys.K => InstructionalKey.K,
                Keys.L => InstructionalKey.L,
                Keys.M => InstructionalKey.M,
                Keys.N => InstructionalKey.N,
                Keys.O => InstructionalKey.O,
                Keys.P => InstructionalKey.P,
                Keys.Q => InstructionalKey.Q,
                Keys.R => InstructionalKey.R,
                Keys.S => InstructionalKey.S,
                Keys.T => InstructionalKey.T,
                Keys.U => InstructionalKey.U,
                Keys.V => InstructionalKey.V,
                Keys.W => InstructionalKey.W,
                Keys.X => InstructionalKey.X,
                Keys.Y => InstructionalKey.Y,
                Keys.Z => InstructionalKey.Z,
                Keys.LWin => InstructionalKey.LWin,
                Keys.RWin => InstructionalKey.RWin,
                Keys.Apps => InstructionalKey.Apps,
                Keys.NumPad0 => InstructionalKey.NumPad0,
                Keys.NumPad1 => InstructionalKey.NumPad1,
                Keys.NumPad2 => InstructionalKey.NumPad2,
                Keys.NumPad3 => InstructionalKey.NumPad3,
                Keys.NumPad4 => InstructionalKey.NumPad4,
                Keys.NumPad5 => InstructionalKey.NumPad5,
                Keys.NumPad6 => InstructionalKey.NumPad6,
                Keys.NumPad7 => InstructionalKey.NumPad7,
                Keys.NumPad8 => InstructionalKey.NumPad8,
                Keys.NumPad9 => InstructionalKey.NumPad9,
                Keys.Multiply => InstructionalKey.Multiply,
                Keys.Add => InstructionalKey.Add,
                Keys.Subtract => InstructionalKey.Subtract,
                Keys.Decimal => InstructionalKey.Decimal,
                Keys.Divide => InstructionalKey.Divide,
                Keys.F1 => InstructionalKey.F1,
                Keys.F2 => InstructionalKey.F2,
                Keys.F3 => InstructionalKey.F3,
                Keys.F4 => InstructionalKey.F4,
                Keys.F5 => InstructionalKey.F5,
                Keys.F6 => InstructionalKey.F6,
                Keys.F7 => InstructionalKey.F7,
                Keys.F8 => InstructionalKey.F8,
                Keys.F9 => InstructionalKey.F9,
                Keys.F10 => InstructionalKey.F10,
                Keys.F11 => InstructionalKey.F11,
                Keys.F12 => InstructionalKey.F12,
                Keys.F13 => InstructionalKey.F13,
                Keys.F14 => InstructionalKey.F14,
                Keys.F15 => InstructionalKey.F15,
                Keys.F16 => InstructionalKey.F16,
                Keys.F17 => InstructionalKey.F17,
                Keys.F18 => InstructionalKey.F18,
                Keys.F19 => InstructionalKey.F19,
                Keys.F20 => InstructionalKey.F20,
                Keys.F21 => InstructionalKey.F21,
                Keys.F22 => InstructionalKey.F22,
                Keys.F23 => InstructionalKey.F23,
                Keys.F24 => InstructionalKey.F24,
                Keys.NumLock => InstructionalKey.NumLock,
                Keys.Scroll => InstructionalKey.Scroll,
                Keys.LShiftKey => InstructionalKey.LShift,
                Keys.RShiftKey => InstructionalKey.RShift,
                Keys.LControlKey => InstructionalKey.LControl,
                Keys.RControlKey => InstructionalKey.RControl,
                Keys.LMenu => InstructionalKey.LMenu,
                Keys.RMenu => InstructionalKey.RMenu,
                Keys.Oem1 => InstructionalKey.Oem1,
                Keys.Oemplus => InstructionalKey.Plus,
                Keys.Oemcomma => InstructionalKey.Comma,
                Keys.OemMinus => InstructionalKey.Minus,
                Keys.OemPeriod => InstructionalKey.Period,
                Keys.Oem2 => InstructionalKey.Oem2,
                Keys.Oem3 => InstructionalKey.Oem3,
                Keys.Oem4 => InstructionalKey.Oem4,
                Keys.Oem5 => InstructionalKey.Oem5,
                Keys.Oem6 => InstructionalKey.Oem6,
                Keys.Oem7 => InstructionalKey.Oem7,
                Keys.Oem102 => InstructionalKey.Oem102,
                _ => InstructionalKey.Unknown
            };

        // NOTE: GetInstructionalId(this ControllerButtons) and GetInstructionalKey(this ControllerButtons)
        // have been removed. ControllerButtons is a RAGE-specific enum with no equivalent in SHVDN.
        // Use the InstructionalKey.ControllerXxx values directly instead, e.g.:
        //   new InstructionalButton(InstructionalKey.ControllerA, "Confirm")
    }


    // ---------------------------------------------------------------------------
    // InstructionalKey enum — unchanged from the original (it's just values).
    // ---------------------------------------------------------------------------

    public enum InstructionalKey // from rage::ioMapperParameter enum
    {
        Unknown = 0x0,
        Back = 0x8,
        Tab = 0x9,
        Return = 0xD,
        Pause = 0x13,
        Capital = 0x14,
        CapsLock = Capital,
        Escape = 0x1B,
        Space = 0x20,
        PageUp = 0x21,
        Prior = PageUp,
        PageDown = 0x22,
        Next = PageDown,
        End = 0x23,
        Home = 0x24,
        Left = 0x25,
        Up = 0x26,
        Right = 0x27,
        Down = 0x28,
        Snapshot = 0x2C,
        PrintScreen = Snapshot,
        SysRq = Snapshot,
        Insert = 0x2D,
        Delete = 0x2E,
        D0 = 0x30,
        D1 = 0x31,
        D2 = 0x32,
        D3 = 0x33,
        D4 = 0x34,
        D5 = 0x35,
        D6 = 0x36,
        D7 = 0x37,
        D8 = 0x38,
        D9 = 0x39,
        A = 0x41,
        B = 0x42,
        C = 0x43,
        D = 0x44,
        E = 0x45,
        F = 0x46,
        G = 0x47,
        H = 0x48,
        I = 0x49,
        J = 0x4A,
        K = 0x4B,
        L = 0x4C,
        M = 0x4D,
        N = 0x4E,
        O = 0x4F,
        P = 0x50,
        Q = 0x51,
        R = 0x52,
        S = 0x53,
        T = 0x54,
        U = 0x55,
        V = 0x56,
        W = 0x57,
        X = 0x58,
        Y = 0x59,
        Z = 0x5A,
        LWin = 0x5B,
        RWin = 0x5C,
        Apps = 0x5D,
        NumPad0 = 0x60,
        NumPad1 = 0x61,
        NumPad2 = 0x62,
        NumPad3 = 0x63,
        NumPad4 = 0x64,
        NumPad5 = 0x65,
        NumPad6 = 0x66,
        NumPad7 = 0x67,
        NumPad8 = 0x68,
        NumPad9 = 0x69,
        Multiply = 0x6A,
        Add = 0x6B,
        Subtract = 0x6D,
        Decimal = 0x6E,
        Divide = 0x6F,
        F1 = 0x70,
        F2 = 0x71,
        F3 = 0x72,
        F4 = 0x73,
        F5 = 0x74,
        F6 = 0x75,
        F7 = 0x76,
        F8 = 0x77,
        F9 = 0x78,
        F10 = 0x79,
        F11 = 0x7A,
        F12 = 0x7B,
        F13 = 0x7C,
        F14 = 0x7D,
        F15 = 0x7E,
        F16 = 0x7F,
        F17 = 0x80,
        F18 = 0x81,
        F19 = 0x82,
        F20 = 0x83,
        F21 = 0x84,
        F22 = 0x85,
        F23 = 0x86,
        F24 = 0x87,
        NumLock = 0x90,
        Scroll = 0x91,
        NumPadEquals = 0x92,
        LShift = 0xA0,
        RShift = 0xA1,
        LControl = 0xA2,
        RControl = 0xA3,
        LMenu = 0xA4,
        RMenu = 0xA5,
        Semicolon = 0xBA,
        Oem1 = Semicolon,
        Plus = 0xBB,
        Equals = Plus,
        Comma = 0xBC,
        Minus = 0xBD,
        Period = 0xBE,
        Slash = 0xBF,
        Oem2 = Slash,
        Grave = 0xC0,
        Oem3 = Grave,
        LBracket = 0xDB,
        Oem4 = LBracket,
        Backslash = 0xDC,
        Oem5 = Backslash,
        RBracket = 0xDD,
        Oem6 = RBracket,
        Apostrophe = 0xDE,
        Oem7 = Apostrophe,
        Oem102 = 0xE2,
        NumPadEnter = 0xFD,

        MouseAxisX = 0x200,
        MouseAxisY = 0x201,
        MouseAxisXLeft = 0x205,
        MouseAxisXRight = 0x206,
        MouseAxisYUp = 0x207,
        MouseAxisYDown = 0x208,

        MouseWheelUp = 0x400,
        MouseWheelDown = 0x401,
        MouseWheel = 0x402,

        MouseLeft = 0x801,
        MouseRight = 0x802,
        MouseMiddle = 0x804,
        MouseExtra1 = 0x808,
        MouseExtra2 = 0x810,
        MouseExtra3 = 0x820,
        MouseExtra4 = 0x840,
        MouseExtra5 = 0x880,

        ControllerAxisLX = 0x1000,
        ControllerAxisLY = 0x1001,
        ControllerAxisRX = 0x1002,
        ControllerAxisRY = 0x1003,
        ControllerAxisLYUp = 0x1016,
        ControllerAxisLYDown = 0x1017,
        ControllerAxisLXLeft = 0x1018,
        ControllerAxisLXRight = 0x1019,
        ControllerAxisRYUp = 0x101A,
        ControllerAxisRYDown = 0x101B,
        ControllerAxisRXLeft = 0x101C,
        ControllerAxisRXRight = 0x101D,

        ControllerLTrigger = 0x2000,
        ControllerRTrigger = 0x2001,
        ControllerLShoulder = 0x2002,
        ControllerRShoulder = 0x2003,
        ControllerRUp = 0x2004,
        ControllerRRight = 0x2005,
        ControllerRDown = 0x2006,
        ControllerRLeft = 0x2007,
        ControllerSelect = 0x2008,
        ControllerLThumb = 0x2009,
        ControllerLStick = ControllerLThumb,
        ControllerRThumb = 0x200A,
        ControllerRStick = ControllerRThumb,
        ControllerStart = 0x200B,
        ControllerLUp = 0x200C,
        ControllerLRight = 0x200D,
        ControllerLDown = 0x200E,
        ControllerLLeft = 0x200F,

        // Aliases matching RPH's ControllerButtons names
        ControllerBack = ControllerSelect,
        ControllerDPadDown = ControllerLDown,
        ControllerDPadRight = ControllerLRight,
        ControllerDPadLeft = ControllerLLeft,
        ControllerDPadUp = ControllerLUp,
        ControllerA = ControllerRDown,
        ControllerB = ControllerRRight,
        ControllerX = ControllerRLeft,
        ControllerY = ControllerRUp,

        // Special cases (not in the original ioMapperParameter enum)
        ControllerDPadNone = 0x40000,
        ControllerDPadAll = 0x40001,
        ControllerDPadUpDown = 0x40002,
        ControllerDPadLeftRight = 0x40003,
        ControllerLThumbRotate = 0x40004,
        ControllerLStickRotate = ControllerLThumbRotate,
        ControllerRThumbRotate = 0x40005,
        ControllerRStickRotate = ControllerRThumbRotate,
        SymbolBusySpinner = 0x40006,
        SymbolPlus = 0x40007,
        SymbolArrowUp = 0x40008,
        SymbolArrowDown = 0x40009,
        SymbolArrowLeft = 0x4000A,
        SymbolArrowRight = 0x4000B,
        SymbolArrowUpDown = 0x4000C,
        SymbolArrowLeftRight = 0x4000D,
        SymbolArrowAll = 0x4000E,
        Mouse = 0x4000F,
    }
}