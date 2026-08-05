using GTA;
using GTA.Math;
using IMNOTEPIX.Framework.Abstracts;
using System;

namespace IMNOTEPIX.Framework.fWorld.Map.Interiors.CounterfeitGarages
{
    public class CounterfeitGarageC : AInterior
    {
        private const string LOG_CAT = "CounterfeitGarageC|AInterior";
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
                int CounterfeitGarageC = InteriorIndex ?? Interior.GetInteriorAtCoords(930.0f, -2183.36f, -50.0f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CounterfeitGarageC))
                {
                    InteriorIndex = CounterfeitGarageC;
                    fCore.Streaming.SetInteriorActive(CounterfeitGarageC, true);
                    Interior.PinInteriorInMemory(CounterfeitGarageC);
                    Interior.DisableInterior(CounterfeitGarageC, false);

                    if (Interior.IsInteriorCapped(CounterfeitGarageC))
                        Interior.CapInterior(CounterfeitGarageC, false);

                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageC, "Set_Door_Right");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageC, "Set_Door_Left");
                    if (SetDoorLeft)
                        Interior.ActivateInteriorEntitySet(CounterfeitGarageC, "Set_Door_Left");
                    else
                        Interior.DeactivateInteriorEntitySet(CounterfeitGarageC, "Set_Door_Left");
                    if (SetDoorRight)
                        Interior.ActivateInteriorEntitySet(CounterfeitGarageC, "Set_Door_Right");
                    else
                        Interior.DeactivateInteriorEntitySet(CounterfeitGarageC, "Set_Door_Right");
                    Interior.ActivateInteriorEntitySet(CounterfeitGarageC, "Set_Tints");
                    Interior.SetInteriorEntitySetTintIndex(CounterfeitGarageC, "Set_Tints", (int)color);

                    Interior.RefreshInterior(CounterfeitGarageC);
                }
                fUI.Graphics.FadeDecalsInRange(930.0f, -2183.36f, -50.0f, 17f, 0f);


                if (Interior.IsInteriorReady(CounterfeitGarageC))
                {
                    Vector3 outVector;
                    uint outHash;
                    Interior.GetInteriorLocationAndNamehash(CounterfeitGarageC, out outVector, out outHash);
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
                int CounterfeitGarageC = InteriorIndex ?? Interior.GetInteriorAtCoords(930.0f, -2183.36f, -50.0f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CounterfeitGarageC))
                {
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageC, "Set_Door_Right");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageC, "Set_Door_Left");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageC, "Set_Tints");

                    fCore.Streaming.SetInteriorActive(CounterfeitGarageC, false);
                    Interior.UnpinInterior(CounterfeitGarageC);
                    Interior.DisableInterior(CounterfeitGarageC, true);

                    if (!Interior.IsInteriorCapped(CounterfeitGarageC))
                        Interior.CapInterior(CounterfeitGarageC, true);

                    Interior.RefreshInterior(CounterfeitGarageC);
                }

                if (!Interior.IsInteriorReady(CounterfeitGarageC))
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
            => Game.Player.Character.Position = pos ?? new Vector3(930.0f, -2183.36f, -50.0f);
    }
}