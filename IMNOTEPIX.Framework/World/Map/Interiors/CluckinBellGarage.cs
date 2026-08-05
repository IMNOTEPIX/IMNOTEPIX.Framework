using GTA;
using GTA.Math;
using IMNOTEPIX.Framework.Abstracts;
using System;

namespace IMNOTEPIX.Framework.fWorld.Map.Interiors
{
    public class CluckinBellGarage : AInterior
    {
        private const string LOG_CAT = "CluckinBellGarage|AInterior";
        private int? InteriorIndex = null;
        public uint? Hash { get; private set; } = null;
        public Vector3? Position { get; private set; } = null;
        protected override void OnLoad() {
            fCore.Logger.Info(LOG_CAT, "Trying To Load...");
            try {
                fCore.Streaming.RequestIpl("m23_2_int_placement_m23_2_interior_3_int_warehouse_milo_");
                int CluckinBellGarage = InteriorIndex ?? Interior.GetInteriorAtCoords(1220f, -2280f, -50f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CluckinBellGarage))
                {
                    InteriorIndex = CluckinBellGarage;
                    fCore.Streaming.SetInteriorActive(CluckinBellGarage, true);
                    Interior.PinInteriorInMemory(CluckinBellGarage);
                    Interior.DisableInterior(CluckinBellGarage, false);

                    if (Interior.IsInteriorCapped(CluckinBellGarage))
                        Interior.CapInterior(CluckinBellGarage, false);

                    Interior.ActivateInteriorEntitySet(CluckinBellGarage, "mp2023_02_DLC_INT_6_CB");

                    Interior.RefreshInterior(CluckinBellGarage);
                }
                if (Interior.IsInteriorReady(CluckinBellGarage) && fCore.Streaming.IsIplActive("m23_2_int_placement_m23_2_interior_3_int_warehouse_milo_"))
                {
                    Vector3 outVector;
                    uint outHash;
                    Interior.GetInteriorLocationAndNamehash(CluckinBellGarage, out outVector, out outHash);
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
                int CluckinBellGarage = InteriorIndex ?? Interior.GetInteriorAtCoords(1220f, -2280f, -50f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CluckinBellGarage))
                {
                    Interior.DeactivateInteriorEntitySet(CluckinBellGarage, "mp2023_02_DLC_INT_6_CB");

                    fCore.Streaming.SetInteriorActive(CluckinBellGarage, false);
                    Interior.UnpinInterior(CluckinBellGarage);
                    Interior.DisableInterior(CluckinBellGarage, true);

                    if (!Interior.IsInteriorCapped(CluckinBellGarage))
                        Interior.CapInterior(CluckinBellGarage, true);

                    Interior.RefreshInterior(CluckinBellGarage);
                }
                fCore.Streaming.RemoveIpl("m23_2_int_placement_m23_2_interior_3_int_warehouse_milo_");

                if (!Interior.IsInteriorReady(CluckinBellGarage) && !fCore.Streaming.IsIplActive("m23_2_int_placement_m23_2_interior_3_int_warehouse_milo_"))
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
        {
            Game.Player.Character.Position = pos ?? new Vector3(1220f, -2280f, -50f);
        }
    }
}
