using GTA;

namespace IMNOTEPIX.Framework.fGameplay.Minigames { public class CashGrabMinigameDisposal : GTA.Script { public CashGrabMinigameDisposal() { Aborted += OnAborted; }
        private void OnAborted(object sender, System.EventArgs e) {
            CartGrab.DisposeAll();
            StackGrab.DisposeAll();
            WashingMachineGrab.DisposeAll();

            fPlayer.Player.Character.CanSwitchWeapons = true;
            Game.Player.SetControlState(true, SetPlayerControlFlags.None);
            fWorld.Ped.SetPedCanLegIK(fPlayer.Player.Character, true);
            fPlayer.Player.Character.SetResetFlag(PedResetFlagToggles.CannotBeTargeted, true);
            fWorld.Entity.SetEntityProofs(fPlayer.Player.Character, false, false, false, false, false, false, false, false);
        }
    }
}