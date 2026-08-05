using System;
using System.Drawing;
using GTA.Native;
using IMNOTEPIX.Framework.fCore.Internals;

namespace IMNOTEPIX.Framework.fUI
{
    public enum TextFont
    {
        ChaletLondon = 0,
        HouseScript = 1,
        Monospace = 2,
        ChaletComprimeCologne = 4,
        /// <summary>
        /// Monospaced version of <see cref="ChaletLondon"/>. Only numbers and some symbols are available.
        /// </summary>
        ChaletLondonFixedWidthNumbers = 5,
        Pricedown = 7
    }

    public enum TextJustification
    {
        /// <summary>The midpoint of the text is located at the drawing position.</summary>
        Center = 0,
        /// <summary>The left side of the text is located at the drawing position.</summary>
        Left = 1,
        /// <summary>The right side of the text is located at the border of the wrapping bounds.</summary>
        Right = 2,
    }

    /// <summary>
    /// Defines the appearance of text.
    /// Ported from RAGENativeUI's TextStyle. Apply()/Current use the CTextStyle fast path
    /// when available (see fCore.Internals.CTextStyle), falling back to natives otherwise -
    /// matches the original's behavior now that CTextStyle is ported.
    /// </summary>
    public struct TextStyle : IEquatable<TextStyle>
    {
        private TextFont font;
        private float scale;
        private float? characterHeight;

        public TextFont Font
        {
            readonly get => font;
            set
            {
                if (value != font)
                {
                    font = value;
                    characterHeight = null;
                }
            }
        }

        public Color Color { get; set; }

        public float Scale
        {
            readonly get => scale;
            set
            {
                if (value != scale)
                {
                    scale = value;
                    characterHeight = null;
                }
            }
        }

        public TextJustification Justification { get; set; }

        /// <summary>
        /// Gets or sets the bounds where the text will be wrapped, in relative coordinates along the X-axis.
        /// </summary>
        public (float Start, float End) Wrap { get; set; }

        public bool DropShadow { get; set; }

        public bool Outline { get; set; }

        /// <summary>
        /// Gets the height of a character in relative coordinates. Depends on <see cref="Font"/> and <see cref="Scale"/>.
        /// </summary>
        public float CharacterHeight =>
            characterHeight ?? (characterHeight = Function.Call<float>(Hash.GET_RENDERED_CHARACTER_HEIGHT, Scale, (int)Font)).Value;
        // NOTE: verify GET_TEXT_SCALE_HEIGHT is the exact Hash member name in your SHVDN build.

        private TextStyle(TextFont font, Color color, float scale, TextJustification justification,
                          (float Start, float End) wrap, bool dropShadow, bool outline, float? characterHeight = null)
            : this()
        {
            Font = font;
            Color = color;
            Scale = scale;
            Justification = justification;
            Wrap = wrap;
            DropShadow = dropShadow;
            Outline = outline;
            this.characterHeight = characterHeight;
        }

        public TextStyle(
            TextFont font,
            Color color,
            float scale = 1.12f,
            TextJustification justification = TextJustification.Left,
            float wrapStart = 0.0f, float wrapEnd = 1.0f,
            bool dropShadow = false,
            bool outline = false)
            : this(font, color, scale, justification, (wrapStart, wrapEnd), dropShadow, outline)
        {
        }

        /// <summary>
        /// Applies this style for the next text command. Uses the direct CTextStyle write
        /// when available (matches the original's fast path now that CTextStyle is ported -
        /// see fCore.Internals.CTextStyle), falling back to SET_TEXT_* natives otherwise.
        /// </summary>
        public readonly void Apply()
        {
            if (CTextStyle.ScriptStyleAvailable)
            {
                ref CTextStyle s = ref CTextStyle.ScriptStyle;
                s.Color = Color.ToArgb();
                s.Scale = Scale;
                s.Wrap = Wrap;
                s.Font = (int)Font;
                s.Justification = (byte)Justification;
                s.DropShadow = DropShadow;
                s.Outline = Outline;
                return;
            }

            Function.Call(Hash.SET_TEXT_FONT, (int)Font);
            Function.Call(Hash.SET_TEXT_COLOUR, Color.R, Color.G, Color.B, Color.A);
            Function.Call(Hash.SET_TEXT_SCALE, 0.0f, Scale);
            Function.Call(Hash.SET_TEXT_JUSTIFICATION, (int)Justification);
            Function.Call(Hash.SET_TEXT_WRAP, Wrap.Start, Wrap.End);

            if (Outline)
            {
                Function.Call(Hash.SET_TEXT_OUTLINE);
            }

            if (DropShadow)
            {
                Function.Call(Hash.SET_TEXT_DROP_SHADOW);
            }
        }

        /// <summary>
        /// Creates a copy of this style with the specified properties changed.
        /// </summary>
        public readonly TextStyle With(
            TextFont? font = null,
            Color? color = null,
            float? scale = null,
            TextJustification? justification = null,
            (float Start, float End)? wrap = null,
            bool? dropShadow = null,
            bool? outline = null)
        {
            TextFont newFont = font ?? Font;
            float newScale = scale ?? Scale;
            return new TextStyle(
                font: newFont,
                color: color ?? Color,
                scale: newScale,
                justification: justification ?? Justification,
                wrap: wrap ?? Wrap,
                dropShadow: dropShadow ?? DropShadow,
                outline: outline ?? Outline,
                characterHeight: (newFont != Font || newScale != Scale) ? null : characterHeight
            );
        }

        internal readonly TextStyle WithWrap(float start, float end)
            => new TextStyle(
                font: Font,
                color: Color,
                scale: Scale,
                justification: Justification,
                wrap: (start, end),
                dropShadow: DropShadow,
                outline: Outline,
                characterHeight: characterHeight
            );

        public override readonly string ToString() => (Font, Color, Scale, Justification, Wrap, DropShadow, Outline).ToString();

        public override readonly int GetHashCode() => (Font, Color, Scale, Justification, Wrap, DropShadow, Outline).GetHashCode();

        public override readonly bool Equals(object other) => other is TextStyle s && Equals(s);

        public readonly bool Equals(TextStyle other)
        {
            return Font == other.Font &&
                   Color == other.Color &&
                   Scale == other.Scale &&
                   Justification == other.Justification &&
                   Wrap == other.Wrap &&
                   DropShadow == other.DropShadow &&
                   Outline == other.Outline;
        }

        /// <summary>
        /// Represents the default text style.
        /// </summary>
        public static readonly TextStyle Default = new TextStyle(
            font: TextFont.ChaletLondon,
            color: Color.FromArgb(0xFF, 0xE1, 0xE1, 0xE1)
        );

        /// <summary>
        /// Gets or sets the text style currently applied. The setter is equivalent to <see cref="Apply"/>.
        /// </summary>
        public static TextStyle Current
        {
            get
            {
                if (CTextStyle.ScriptStyleAvailable)
                {
                    ref CTextStyle s = ref CTextStyle.ScriptStyle;
                    return new TextStyle(
                        (TextFont)s.Font,
                        Color.FromArgb(s.Color),
                        s.Scale,
                        (TextJustification)s.Justification,
                        s.Wrap.Start,
                        s.Wrap.End,
                        s.DropShadow,
                        s.Outline
                        );
                }
                else
                {
                    return Default;
                }
            }
            set => value.Apply();
        }
    }
}