using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using IMNOTEPIX.Framework.Funcs;

namespace IMNOTEPIX.Framework.Drawables.TimerBars
{
    public enum CheckpointState
    {
        InProgress,
        Completed,
        Failed
    }

    public class Checkpoint
    {
        public CheckpointState State { get; set; } = CheckpointState.InProgress;
        public bool IsCrossedOut { get; set; }
        public Color CompletedColor { get; set; } = Color.FromArgb(255, 114, 204, 114);
    }

    /// <summary>
    /// Timerbar displaying a row of checkpoint circles (heist objectives style).
    /// </summary>
    public class HudCheckpointsBar : HudBar
    {
        public List<Checkpoint> Checkpoints { get; }

        public HudCheckpointsBar(string label, int checkpointCount)
            : base(label, thin: true)
        {
            Checkpoints = Enumerable.Range(0, checkpointCount)
                .Select(_ => new Checkpoint())
                .ToList();
        }

        public override void Draw(float y)
        {
            base.Draw(y);

            if (Checkpoints.Count == 0) return;

            y += 0.012f;

            const float totalWidth = 0.069f;
            const float circleSize = 0.008f;
            const float startX = 0.879f;

            float spacing = totalWidth / (Checkpoints.Count + 1);

            for (int i = 0; i < Checkpoints.Count; i++)
            {
                var cp = Checkpoints[i];
                float circleX = startX + spacing * (i + 1);

                Color circleColor = cp.State switch
                {
                    CheckpointState.Completed => cp.CompletedColor,
                    CheckpointState.Failed => Color.FromArgb(255, 224, 50, 50),
                    _ => Color.FromArgb(155, 240, 240, 240)
                };

                fGraphics.DrawRect(circleX, y,
                    circleSize, circleSize,
                    circleColor.R, circleColor.G, circleColor.B, circleColor.A,
                    false);

                if (cp.IsCrossedOut)
                {
                    // Horizontal stroke
                    fGraphics.DrawRect(circleX, y, circleSize, 0.002f, 255, 255, 255, 255, false);
                    // Vertical stroke
                    fGraphics.DrawRect(circleX, y, 0.002f, circleSize, 255, 255, 255, 255, false);
                }
            }
        }
    }
}