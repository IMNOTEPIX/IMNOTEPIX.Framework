namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Internal timer bars constants. Direct 1:1 port - pure layout data from the game scripts,
    /// no natives or RPH dependencies involved.
    /// </summary>
    internal static class TB
    {
        internal const float InitialX = 0.795f;
        internal const float InitialY = 0.925f - 0.002f;
        internal const float LoadingPromptYOffset = 0.036f;

        internal const string BgTextureDictionary = "timerbars";
        internal const string BgTextureName = "all_black_bg";
        internal const string BgHighlightTextureName = "all_white_bg";
        internal const float BgXOffset = 0.079f;
        internal const float BgDefaultYOffset = 0.008f;
        internal const float BgSmallYOffset = 0.012f;
        internal const float BgWidth = 0.157f;
        internal const float BgDefaultHeight = 0.036f;
        internal const float BgSmallHeight = 0.028f;
        internal const float DefaultHeightWithGap = ((0.025f + 0.006f) + 0.0009f) + 0.008f;
        internal const float SmallHeightWithGap = ((0.025f + 0.006f) + 0.0009f);

        internal const float LabelInitialWrapEnd = ((((0.88f - 0.062f) + 0.026f) + 0.027f) + 0.03f) - 0.034f;
        internal const float LabelScale = 0.202f;
        internal const float LabelSize = 0.288f;

        internal const float TextYOffset = ((((-0.01f - 0.005f) + 0.004f) - 0.001f) + 0.001f);
        internal const float TextWrapEnd = (((((0.95f - 0.047f) + 0.001f) + 0.047f) - 0.002f) + 0.001f);
        internal const float TextScale = 0.332f;
        internal const float TextSize = ((((((0.469f + 0.096f) - 0.017f) + 0.022f) - 0.062f) - 0.001f) - 0.013f);

        internal const float BarXOffset = 0.118f;
        internal const float BarYOffset = ((((0.013f - 0.002f) + 0.001f) + 0.001f) - 0.001f);
        internal const float BarWidth = 0.069f;
        internal const float BarHeight = 0.011f;

        internal const float AccentXOffset = BgWidth - AccentWidth * 0.5f;
        internal const float AccentWidth = 0.002f;

        internal const string MarkersTextureDictionary = "timerbar_lines";
        /// <summary>
        /// We use the texture with the marker at 50% because the game only has textures for every 10% interval,
        /// but here we support any percentage and the 50% texture is the easiest one to offset.
        /// </summary>
        internal const string MarkerTextureName = "linemarker50_128";

        internal const string CheckpointTextureDictionary = "timerbars";
        internal const string CheckpointTextureName = "circle_checkpoints";
        internal const string CrossTextureDictionary = "cross";
        internal const string CrossTextureName = "circle_checkpoints_cross";

        internal const float CheckpointXOffset = 0.1495f;
        internal const float CheckpointYOffset = ((((0.013f - 0.002f) + 0.001f) + 0.001f) - 0.001f);
        internal const float CheckpointWidth = 0.012f;
        internal const float CheckpointHeight = 0.023f;
        internal const float CheckpointPadding = 0.0094f;

        internal const float IconXOffset = 0.145f + 0.001f;
        internal const float IconYOffset = 0.016f * 0.5f;
        internal const float IconWidth = 0.016f + 0.003f;
        internal const float IconHeight = 0.032f + 0.004f;
    }
}