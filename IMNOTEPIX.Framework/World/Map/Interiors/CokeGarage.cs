using GTA;
using GTA.Math;
using IMNOTEPIX.Framework.Abstracts;
using System;
using System.Windows.Forms;

namespace IMNOTEPIX.Framework.fWorld.Map.Interiors
{
    public class CokeGarage : AInterior
    {
        private const string LOG_CAT = "CokeGarage|AInterior";
        private int? InteriorIndex = null;
        public uint? Hash { get; private set; } = null;
        public Vector3? Position { get; private set; } = null;
        protected override void OnLoad() {
            fCore.Logger.Info(LOG_CAT, "Trying To Load...");
            try {
                fCore.Streaming.RequestIpl("m23_2_int_placement_m23_2_interior_8_dlc_int_warehouse2_milo_");
                fCore.Streaming.RequestIpl("m23_2_int_placement_m23_2_interior_7_dlc_int_warehouse2_milo_");
                int CokeGarage = InteriorIndex ?? Interior.GetInteriorAtCoords(1000f, -2200f, -50f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CokeGarage))
                {
                    InteriorIndex = CokeGarage;
                    fCore.Streaming.SetInteriorActive(CokeGarage, true);
                    Interior.PinInteriorInMemory(CokeGarage);
                    Interior.DisableInterior(CokeGarage, false);

                    if (Interior.IsInteriorCapped(CokeGarage))
                        Interior.CapInterior(CokeGarage, false);

                    Interior.RefreshInterior(CokeGarage);
                }
                fUI.Graphics.FadeDecalsInRange(1000f, -2200f, -50f, 18f, 0f);


                if (Interior.IsInteriorReady(CokeGarage) && fCore.Streaming.IsIplActive("m23_2_int_placement_m23_2_interior_8_dlc_int_warehouse2_milo_") && fCore.Streaming.IsIplActive("m23_2_int_placement_m23_2_interior_7_dlc_int_warehouse2_milo_"))
                {
                    Vector3 outVector;
                    uint outHash;
                    Interior.GetInteriorLocationAndNamehash(CokeGarage, out outVector, out outHash);
                    Position = outVector;
                    Hash = outHash;
                    fCore.Logger.Info(LOG_CAT, "Loaded Successfully.");
                }
                else
                    fCore.Logger.Info(LOG_CAT, "Failed To Load The Interior. (One of the issues might be MP Maps not being loaded in.)");
            }
            catch (Exception ex) {
                fCore.Logger.Error(LOG_CAT, $"Caught An Exeption When Trying To Load. Exeption: {ex}");
            }
        }
        protected override void OnUnload() {
            fCore.Logger.Info(LOG_CAT, "Trying To Unload...");
            try {
                int CokeGarage = InteriorIndex ?? Interior.GetInteriorAtCoords(1000f, -2200f, -50f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CokeGarage))
                {
                    fCore.Streaming.SetInteriorActive(CokeGarage, false);
                    Interior.UnpinInterior(CokeGarage);
                    Interior.DisableInterior(CokeGarage, true);

                    if (!Interior.IsInteriorCapped(CokeGarage))
                        Interior.CapInterior(CokeGarage, true);

                    Interior.RefreshInterior(CokeGarage);
                }
                fCore.Streaming.RemoveIpl("m23_2_int_placement_m23_2_interior_8_dlc_int_warehouse2_milo_");
                fCore.Streaming.RemoveIpl("m23_2_int_placement_m23_2_interior_7_dlc_int_warehouse2_milo_");

                if (!Interior.IsInteriorReady(CokeGarage) && !fCore.Streaming.IsIplActive("m23_2_int_placement_m23_2_interior_8_dlc_int_warehouse2_milo_") && !fCore.Streaming.IsIplActive("m23_2_int_placement_m23_2_interior_7_dlc_int_warehouse2_milo_"))
                {
                    InteriorIndex = null;
                    Position = null;
                    Hash = null;
                    fCore.Logger.Info(LOG_CAT, "Unloaded Successfully.");
                }
                else
                    fCore.Logger.Info(LOG_CAT, "Failed To Unload The Interior. (One of the issues might be MP Maps being loaded in.)");
            }
            catch (Exception ex) {
                fCore.Logger.Error(LOG_CAT, $"Caught An Exeption When Trying To Load. Exeption: {ex}");
            }
        }
        public void Teleport(Vector3? pos = null)
            => Game.Player.Character.Position = pos ?? new Vector3(1000f, -2200f, -50f);
    }
}
