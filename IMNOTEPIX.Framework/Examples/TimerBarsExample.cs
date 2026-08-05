using GTA;
using GTA.Native;
using GTA.UI;
using IMNOTEPIX.Framework.fCore;
using IMNOTEPIX.Framework.fUI;
using IMNOTEPIX.Framework.fUI.TimerBars;
using System;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace IMNOTEPIX.Framework.fExamples
{
    /// <summary>
    /// Ported from RAGENativeUI's TimerBarsExample. Structural changes from the original:
    /// <list type="bullet">
    /// <item>RPH's GameFiber.StartNew(...) + [ConsoleCommand] plugin-entry model has no SHVDN
    /// equivalent - SHVDN scripts are GTA.Script subclasses driven by the Tick event instead.
    /// The two fibers (Main's update loop, ProcessTimerBars' draw loop) both ran once per
    /// frame anyway, so they're merged into a single Tick handler here.</item>
    /// <item>[ConsoleCommand] dropped - SHVDN's own in-game console can already call any public
    /// static method directly (e.g. typing the method's fully-qualified name), no attribute needed.</item>
    /// <item>Game.Console.Print(...) -&gt; GTA.UI.Notification.Show(...), shown once on start.</item>
    /// <item>Rage.Native.NativeFunction.Natives.GetWeapontypeGroup&lt;uint&gt;(...) -&gt;
    /// GTA.Native.Function.Call&lt;uint&gt;(Hash.GET_WEAPONTYPE_GROUP, ...).</item>
    /// <item>playerPed.Inventory.EquippedWeapon (RPH) -&gt; playerPed.Weapons.Current (SHVDN).</item>
    /// </list>
    /// NOTE: verify these SHVDN members against your build - I'm reasonably confident they're
    /// correct but haven't cross-checked them the way GTA.Game.FindPattern/GlobalVariable were:
    /// World.CurrentTimeOfDay, Game.LastFrameTime, Entity.Speed, Ped.Weapons.Current,
    /// Player.WantedLevel, Hash.GET_WEAPONTYPE_GROUP.
    /// </summary>
    public class TimerBarsExample// : GTA.Script
    {
        private readonly TimerBarPool pool;
        private readonly BarTimerBar healthTB;
        private readonly TextTimerBar speedTB;
        private readonly TextTimerBar clockTB;
        private readonly IconsTimerBar weaponTB;
        private readonly CheckpointsTimerBar wantedLevelTB;

        private float lastHealth;
        private float healthDamage;
        private const float HealthDamageIndicatorDuration = 2.0f; // seconds

        public TimerBarsExample()
        {
            // create the pool that handles drawing the timer bars
            pool = new TimerBarPool();

            // timer bar that shows the player health as a progress bar and is highlighted in red when the player is damaged
            healthTB = new BarTimerBar("HEALTH");

            // timer bar that shows the player speed
            speedTB = new TextTimerBar("SPEED", "0 km/h");

            // timer bar that shows the time of day
            clockTB = new TextTimerBar("CLOCK", "00:00:00");
            // use monospace font for the time text
            clockTB.TextStyle = clockTB.TextStyle.With(font: TextFont.ChaletLondonFixedWidthNumbers);

            // timer bar that shows the player equipped weapon as an icon
            weaponTB = new IconsTimerBar("WEAPON");
            weaponTB.Icons.Add(new TimerBarIcon("commonmenu", "arrowleft")
            {
                Size = new SizeF(TimerBarIcon.DefaultSize.Width * 0.825f, TimerBarIcon.DefaultSize.Height * 0.825f),
                Spacing = TimerBarIcon.DefaultSpacing * 2.75f
            });
            weaponTB.Icons.Add(new TimerBarIcon("mpkillquota", "weapon_unarmed"));

            // timer bar that shows the wanted level as checkpoints
            Color checkpointsColor = fUI.HudColor.Gold.GetColor();
            wantedLevelTB = new CheckpointsTimerBar("WANTED", 5) { Accent = checkpointsColor };
            foreach (TimerBarCheckpoint cp in wantedLevelTB.Checkpoints)
            {
                cp.Color = checkpointsColor;
            }

            // add all the timer bars to the pool
            pool.Add(healthTB, speedTB, clockTB, weaponTB, wantedLevelTB);

            Notification.Show(
                $"~y~{healthTB.Label}~s~: shows the player health.\n" +
                $"~y~{speedTB.Label}~s~: shows the player speed in kilometers per hour\n" +
                $"~y~{clockTB.Label}~s~: shows the current time of day\n" +
                $"~y~{weaponTB.Label}~s~: shows the type of the weapon currently equipped\n" +
                $"~y~{wantedLevelTB.Label}~s~: shows the player wanted level.");

            //Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            Ped playerPed = Game.Player.Character;

            // update speedometer timer bar
            {
                int speed = fCore.Math.ConvertMetersPerSecondToKilometersPerHourRounded(playerPed.Speed);
                speedTB.Text = $"{speed} km/h";
            }

            // update clock timer bar
            {
                // NOTE: verify World.CurrentTimeOfDay is the exact member name/type in your SHVDN build.
                clockTB.Text = World.CurrentTimeOfDay.ToString();
            }

            // update health timer bar
            {
                float health = (float)(playerPed.Health - 100) / (playerPed.MaxHealth - 100);
                bool hasReceivedDamage = health < lastHealth;
                lastHealth = health;

                healthTB.Percentage = health;

                healthTB.ForegroundColor = (health < 0.25 ? fUI.HudColor.RadarDamage : fUI.HudColor.RadarHealth).GetColor();
                healthTB.BackgroundColor = Color.FromArgb(120, healthTB.ForegroundColor);

                if (healthDamage > 0.0f)
                {
                    // set highlight and fade it out
                    healthTB.Highlight = Color.FromArgb((int)(healthDamage * 255), fUI.HudColor.RadarDamage.GetColor());
                    // NOTE: verify Game.LastFrameTime is the exact member name in your SHVDN build.
                    healthDamage -= (1.0f / HealthDamageIndicatorDuration) * Game.LastFrameTime;
                }
                else
                {
                    healthTB.Highlight = null;

                    if (hasReceivedDamage)
                    {
                        healthDamage = 1.0f;
                    }
                }
            }

            // update weapon timer bar
            {
                // choose a new texture based on the equipped weapon
                (string, string, float, float) tex;
                if (playerPed.CurrentVehicle != null && playerPed.CurrentVehicle.Exists())
                {
                    tex = ("mpcarhud", "transport_car_icon", 1.1f, 1.1f);
                }
                else
                {
                    // NOTE: verify Ped.Weapons.Current is the exact member path in your SHVDN build
                    // (RPH's playerPed.Inventory.EquippedWeapon has no direct SHVDN equivalent).
                    Weapon current = playerPed.Weapons.Current;
                    if (current == null || current.Hash == WeaponHash.Unarmed)
                    {
                        tex = ("mpkillquota", "weapon_unarmed", 2.75f, 1.25f);
                    }
                    else
                    {
                        // NOTE: verify Hash.GET_WEAPONTYPE_GROUP is the exact Hash member name.
                        uint group = Function.Call<uint>(Hash.GET_WEAPONTYPE_GROUP, (uint)current.Hash);
                        tex = group switch
                        {
                            0x18D5FA97u /* group_pistol  */ => ("mpkillquota", "weapon_pistol", 2.75f, 1.25f),
                            0x33431399u /* group_shotgun */ => ("mpkillquota", "weapon_shotgun_pump", 2.7f, 1.3f),
                            _ => ("mpkillquota", "weapon_rifle_carbine", 2.75f, 1.25f)
                        };
                    }
                }

                TimerBarIcon icon = weaponTB.Icons[1];
                icon.TextureDictionary = tex.Item1;
                icon.TextureName = tex.Item2;
                SizeF s = TimerBarIcon.DefaultSize;
                icon.Size = new SizeF(s.Width * tex.Item3, s.Height * tex.Item4);
            }

            // update wanted level timer bar
            {
                // NOTE: verify Game.Player.WantedLevel is the exact member path in your SHVDN build.
                int level = Game.Player.WantedLevel;
                for (int i = 0; i < wantedLevelTB.Checkpoints.Count; i++)
                {
                    wantedLevelTB.Checkpoints[i].State = i < level ? TimerBarCheckpointState.Completed : TimerBarCheckpointState.InProgress;
                    wantedLevelTB.Checkpoints[i].IsCrossedOut = i < level - 1;
                }
            }

            // draw the timer bars every frame, same as the original's separate ProcessTimerBars fiber
            pool.Draw();
        }
    }
}