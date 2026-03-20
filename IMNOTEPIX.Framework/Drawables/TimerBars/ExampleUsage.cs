using System.Drawing;
using GTA;
using GTA.UI;

namespace IMNOTEPIX.Framework.Drawables.TimerBars
{
    /// <summary>
    /// Demonstrates how to create and register each HUD bar type.
    /// Not used at runtime — reference only.
    /// </summary>
    public class ExampleUsage
    {
        public ExampleUsage()
        {
            // Value bar with highlight and accent
            var takeBar = new HudValueBar("TAKE", isMoney: true)
            {
                Value = 1164724,
                ValueColor = Color.FromArgb(255, 114, 204, 114),
                Highlight = Color.FromArgb(100, 0, 100, 0),
                Accent = Color.Green
            };
            HudBarController.Register(takeBar);

            // Progress bar with markers
            var progressBar = new HudProgressBar("PROGRESS", 0.75f)
            {
                ForegroundColor = Color.FromArgb(255, 114, 204, 114),
                Highlight = Color.FromArgb(100, 0, 100, 0)
            };
            progressBar.Markers.Add(new ProgressBarMarker(0.25f));
            progressBar.Markers.Add(new ProgressBarMarker(0.5f, Color.Yellow));
            progressBar.Markers.Add(new ProgressBarMarker(0.75f, Color.Green));
            HudBarController.Register(progressBar);

            // Checkpoints bar (heist objectives style)
            var checkpointsBar = new HudCheckpointsBar("OBJECTIVES", 5);
            checkpointsBar.Checkpoints[0].State = CheckpointState.Completed;
            checkpointsBar.Checkpoints[1].State = CheckpointState.Completed;
            checkpointsBar.Checkpoints[2].State = CheckpointState.InProgress;
            checkpointsBar.Checkpoints[3].IsCrossedOut = true;
            HudBarController.Register(checkpointsBar);

            // Countdown timer
            var timerBar = new HudCountdownBar("TIME", 300000) // 5 minutes
            {
                TimeColor = Color.FromArgb(255, 224, 50, 50),
                Accent = Color.Red,
                OnFinished = () => Notification.PostTicker("Time's up!", false, false)
            };
            HudBarController.Register(timerBar);

            // Player bar
            var playerBar = new HudPlayerBar("xXCoolGamer420Xx", "15 KILLS")
            {
                RightTextColor = Color.FromArgb(255, 114, 204, 114)
            };
            HudBarController.Register(playerBar);
        }
    }
}
