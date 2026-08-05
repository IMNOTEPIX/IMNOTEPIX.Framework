using GTA;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IMNOTEPIX.Framework.fCore
{
    public class Debug
    {
        public static List<Vehicle> DebugVehicles = new List<Vehicle>();
        public static List<string> DebugAnimDicts = new List<string>();
        public static List<GTA.Prop> DebugProps = new List<GTA.Prop>();

        public static void CopyPlayerPosWithAddons()
        {
            var pos = Game.Player.Character.Position;
            string text = $"new Vector3({pos.X}f, {pos.Y}f, {pos.Z}f)";
            CopyToClipboard(text);
        }

        public static void CopyToClipboard(object text)
        {
            GTA.UI.Screen.ShowSubtitle("~p~Copied~s~: to clipboard!");
            var thread = new Thread(() => Clipboard.SetText(text.ToString()));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static void CopyPedVariationToClipboard(GTA.Ped ped)
        {
            var data = fWorld.Ped.GetPedVariationData(ped);
            if (data == null) 
                return;

            var sb = new StringBuilder();
            foreach (var kvp in data)
                sb.AppendLine($"{kvp.Key}:{kvp.Value.Item1},{kvp.Value.Item2}");

            CopyToClipboard(sb.ToString());
        }

        public static void CopyPedPropDataToClipboard(GTA.Ped ped)
        {
            var data = fWorld.Ped.GetPedPropData(ped);
            if (data == null) 
                return;

            var sb = new StringBuilder();
            foreach (var kvp in data)
                sb.AppendLine($"{kvp.Key}:{kvp.Value.Item1},{kvp.Value.Item2}");

            CopyToClipboard(sb.ToString());
        }

        public static void DoNothing() { }
    }
}
