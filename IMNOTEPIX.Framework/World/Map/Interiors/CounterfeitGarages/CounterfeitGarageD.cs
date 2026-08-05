using GTA;
using GTA.Math;
using IMNOTEPIX.Framework.Abstracts;
using System;

namespace IMNOTEPIX.Framework.fWorld.Map.Interiors.CounterfeitGarages
{
    public class CounterfeitGarageD : AInterior
    {
        private const string LOG_CAT = "CounterfeitGarageD|AInterior";
        private int? InteriorIndex = null;
        public uint? Hash { get; private set; } = null;
        public Vector3? Position { get; private set; } = null;
        public bool SetDoorLeft { get; set; } = false;
        public bool SetDoorRight { get; set; } = false;
        public GarageColor color { get; set; } = 0;

        protected override void OnLoad()
        {
            fCore.Logger.Info(LOG_CAT, "Trying To Load...");
            try
            {
                int CounterfeitGarageD = InteriorIndex ?? Interior.GetInteriorAtCoords(930.0f, -2213.2f, -50.0f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CounterfeitGarageD))
                {
                    InteriorIndex = CounterfeitGarageD;
                    fCore.Streaming.SetInteriorActive(CounterfeitGarageD, true);
                    Interior.PinInteriorInMemory(CounterfeitGarageD);
                    Interior.DisableInterior(CounterfeitGarageD, false);

                    if (Interior.IsInteriorCapped(CounterfeitGarageD))
                        Interior.CapInterior(CounterfeitGarageD, false);

                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageD, "Set_Door_Right");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageD, "Set_Door_Left");
                    if (SetDoorLeft)
                        Interior.ActivateInteriorEntitySet(CounterfeitGarageD, "Set_Door_Left");
                    else
                        Interior.DeactivateInteriorEntitySet(CounterfeitGarageD, "Set_Door_Left");
                    if (SetDoorRight)
                        Interior.ActivateInteriorEntitySet(CounterfeitGarageD, "Set_Door_Right");
                    else
                        Interior.DeactivateInteriorEntitySet(CounterfeitGarageD, "Set_Door_Right");
                    Interior.ActivateInteriorEntitySet(CounterfeitGarageD, "Set_Tints");
                    Interior.SetInteriorEntitySetTintIndex(CounterfeitGarageD, "Set_Tints", (int)color);

                    Interior.RefreshInterior(CounterfeitGarageD);
                }
                fUI.Graphics.FadeDecalsInRange(930.0f, -2213.2f, -50.0f, 17f, 0f);


                if (Interior.IsInteriorReady(CounterfeitGarageD))
                {
                    Vector3 outVector;
                    uint outHash;
                    Interior.GetInteriorLocationAndNamehash(CounterfeitGarageD, out outVector, out outHash);
                    Position = outVector;
                    Hash = outHash;
                    fCore.Logger.Info(LOG_CAT, "Loaded Successfully.");
                }
                else
                    fCore.Logger.Info(LOG_CAT, "Failed To Load The Interior. (One of the issues might be MP Maps not being loaded in.)");
            }
            catch (Exception ex)
            {
                fCore.Logger.Error(LOG_CAT, $"Caught An Exeption When Trying To Load. Exeption: {ex}");
            }
        }
        protected override void OnUnload()
        {
            fCore.Logger.Info(LOG_CAT, "Trying To Unload...");
            try
            {
                int CounterfeitGarageD = InteriorIndex ?? Interior.GetInteriorAtCoords(930.0f, -2213.2f, -50.0f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CounterfeitGarageD))
                {
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageD, "Set_Door_Right");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageD, "Set_Door_Left");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageD, "Set_Tints");

                    fCore.Streaming.SetInteriorActive(CounterfeitGarageD, false);
                    Interior.UnpinInterior(CounterfeitGarageD);
                    Interior.DisableInterior(CounterfeitGarageD, true);

                    if (!Interior.IsInteriorCapped(CounterfeitGarageD))
                        Interior.CapInterior(CounterfeitGarageD, true);

                    Interior.RefreshInterior(CounterfeitGarageD);
                }

                if (!Interior.IsInteriorReady(CounterfeitGarageD))
                {
                    InteriorIndex = null;
                    Position = null;
                    Hash = null;
                    fCore.Logger.Info(LOG_CAT, "Unloaded Successfully.");
                }
                else
                    fCore.Logger.Info(LOG_CAT, "Failed To Unload The Interior. (One of the issues might be MP Maps being loaded in.)");
            }
            catch (Exception ex)
            {
                fCore.Logger.Error(LOG_CAT, $"Caught An Exeption When Trying To Load. Exeption: {ex}");
            }
        }
        public void Teleport(Vector3? pos = null)
            => Game.Player.Character.Position = pos ?? new Vector3(930.0f, -2213.2f, -50.0f);
    }
}