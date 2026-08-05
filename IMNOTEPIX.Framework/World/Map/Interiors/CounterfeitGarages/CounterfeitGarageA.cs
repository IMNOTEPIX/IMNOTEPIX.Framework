using GTA;
using GTA.Math;
using IMNOTEPIX.Framework.Abstracts;
using System;

namespace IMNOTEPIX.Framework.fWorld.Map.Interiors.CounterfeitGarages
{
    public class CounterfeitGarageA : AInterior
    {
        private const string LOG_CAT = "CounterfeitGarageA|AInterior";
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
                int CounterfeitGarageA = InteriorIndex ?? Interior.GetInteriorAtCoords(930f, -2270f, -50f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CounterfeitGarageA))
                {
                    InteriorIndex = CounterfeitGarageA;
                    fCore.Streaming.SetInteriorActive(CounterfeitGarageA, true);
                    Interior.PinInteriorInMemory(CounterfeitGarageA);
                    Interior.DisableInterior(CounterfeitGarageA, false);

                    if (Interior.IsInteriorCapped(CounterfeitGarageA))
                        Interior.CapInterior(CounterfeitGarageA, false);

                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageA, "Set_Door_Right");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageA, "Set_Door_Left");
                    if (SetDoorLeft)
                        Interior.ActivateInteriorEntitySet(CounterfeitGarageA, "Set_Door_Left");
                    else
                        Interior.DeactivateInteriorEntitySet(CounterfeitGarageA, "Set_Door_Left");
                    if (SetDoorRight)
                        Interior.ActivateInteriorEntitySet(CounterfeitGarageA, "Set_Door_Right");
                    else
                        Interior.DeactivateInteriorEntitySet(CounterfeitGarageA, "Set_Door_Right");
                    Interior.ActivateInteriorEntitySet(CounterfeitGarageA, "Set_Tints");
                    Interior.SetInteriorEntitySetTintIndex(CounterfeitGarageA, "Set_Tints", (int)color);

                    Interior.RefreshInterior(CounterfeitGarageA);
                }
                fUI.Graphics.FadeDecalsInRange(930f, -2270f, -50f, 17f, 0f);


                if (Interior.IsInteriorReady(CounterfeitGarageA))
                {
                    Vector3 outVector;
                    uint outHash;
                    Interior.GetInteriorLocationAndNamehash(CounterfeitGarageA, out outVector, out outHash);
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
                int CounterfeitGarageA = InteriorIndex ?? Interior.GetInteriorAtCoords(930f, -2270f, -50f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CounterfeitGarageA))
                {
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageA, "Set_Door_Right");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageA, "Set_Door_Left");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageA, "Set_Tints");

                    fCore.Streaming.SetInteriorActive(CounterfeitGarageA, false);
                    Interior.UnpinInterior(CounterfeitGarageA);
                    Interior.DisableInterior(CounterfeitGarageA, true);

                    if (!Interior.IsInteriorCapped(CounterfeitGarageA))
                        Interior.CapInterior(CounterfeitGarageA, true);

                    Interior.RefreshInterior(CounterfeitGarageA);
                }

                if (!Interior.IsInteriorReady(CounterfeitGarageA))
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
            => Game.Player.Character.Position = pos ?? new Vector3(930f, -2270f, -50f);
    }
}