using System;
using System.Collections.Generic;
using System.Drawing;
using GTA.Native;

namespace IMNOTEPIX.Framework.fUI.TimerBars
{
    /// <summary>
    /// Represents a timer bar containing a list of checkpoints.
    /// <para>
    /// Each checkpoint is drawn as a circle: greyed out when not completed, black when failed,
    /// or a specific color when completed. Can additionally be crossed out.
    /// </para>
    /// </summary>
    /// <see cref="TimerBarCheckpoint"/>
    /// <see cref="TimerBarCheckpointState"/>
    public class CheckpointsTimerBar : TimerBarBase
    {
        private IList<TimerBarCheckpoint> checkpoints;

        /// <summary>
        /// Shown from right to left: index 0 is the right-most circle, the last item is the left-most.
        /// </summary>
        public IList<TimerBarCheckpoint> Checkpoints
        {
            get => checkpoints;
            set => checkpoints = value ?? throw new ArgumentNullException(nameof(value));
        }

        public CheckpointsTimerBar(string label, int numberOfCheckpoints) : base(label)
        {
            if (numberOfCheckpoints < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfCheckpoints), numberOfCheckpoints, $"{nameof(numberOfCheckpoints)} is negative");
            }

            Thin = true;

            Checkpoints = new List<TimerBarCheckpoint>();
            for (int i = 0; i < numberOfCheckpoints; i++)
            {
                Checkpoints.Add(new TimerBarCheckpoint());
            }
        }

        /// <inheritdoc/>
        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            if (Checkpoints.Count > 0)
            {
                x += TB.CheckpointXOffset;
                y += TB.CheckpointYOffset;

                bool requestedCrossTxd = false;
                foreach (TimerBarCheckpoint cp in Checkpoints)
                {
                    if (cp != null)
                    {
                        Color circleColor = cp.State switch
                        {
                            TimerBarCheckpointState.Completed => cp.Color,
                            TimerBarCheckpointState.Failed => Color.Black,
                            _ => Color.FromArgb(51, 255, 255, 255),
                        };

                        Function.Call(Hash.DRAW_SPRITE, TB.CheckpointTextureDictionary, TB.CheckpointTextureName, x, y, TB.CheckpointWidth, TB.CheckpointHeight, 0.0f,
                                    circleColor.R, circleColor.G, circleColor.B, circleColor.A);

                        if (cp.IsCrossedOut)
                        {
                            if (!requestedCrossTxd)
                            {
                                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, TB.CrossTextureDictionary, true);
                                if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, TB.CrossTextureDictionary))
                                {
                                    break;
                                }

                                requestedCrossTxd = true;
                            }

                            int crossColor = cp.State == TimerBarCheckpointState.Failed ? 255 : 0;
                            Function.Call(Hash.DRAW_SPRITE, TB.CrossTextureDictionary, TB.CrossTextureName, x, y, TB.CheckpointWidth, TB.CheckpointHeight, 0.0f,
                                        crossColor, crossColor, crossColor, 250);
                        }
                    }

                    x -= TB.CheckpointPadding;
                }
            }
        }
    }

    public enum TimerBarCheckpointState
    {
        /// <summary>Unfinished objective. Shown as a grey circle.</summary>
        InProgress = 0,
        /// <summary>Successfully completed objective. Shown as a circle with <see cref="TimerBarCheckpoint.Color"/>.</summary>
        Completed,
        /// <summary>Failed objective. Shown as a black circle.</summary>
        Failed,
    }

    /// <summary>
    /// Represents a checkpoint of a <see cref="CheckpointsTimerBar"/>.
    /// </summary>
    public class TimerBarCheckpoint
    {
        public TimerBarCheckpointState State { get; set; }

        /// <summary>
        /// If <c>true</c>, a cross is drawn on top of the checkpoint circle.
        /// </summary>
        public bool IsCrossedOut { get; set; }

        /// <summary>
        /// Color used when <see cref="State"/> is <see cref="TimerBarCheckpointState.Completed"/>.
        /// </summary>
        public Color Color { get; set; }

        public TimerBarCheckpoint(Color color)
        {
            State = TimerBarCheckpointState.InProgress;
            IsCrossedOut = false;
            Color = color;
        }

        public TimerBarCheckpoint() : this(Color.White)
        {
        }
    }
}