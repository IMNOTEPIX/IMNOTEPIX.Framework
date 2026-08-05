using GTA;
using GTA.Math;
using IMNOTEPIX.Framework.Abstracts;
using System;

namespace IMNOTEPIX.Framework.fWorld.Map.Interiors.CounterfeitGarages
{
    public class CounterfeitGarageB : AInterior
    {
        private const string LOG_CAT = "CounterfeitGarageB|AInterior";
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
                int CounterfeitGarageB = InteriorIndex ?? Interior.GetInteriorAtCoords(930.0f, -2240.13f, -50.0f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CounterfeitGarageB))
                {
                    InteriorIndex = CounterfeitGarageB;
                    fCore.Streaming.SetInteriorActive(CounterfeitGarageB, true);
                    Interior.PinInteriorInMemory(CounterfeitGarageB);
                    Interior.DisableInterior(CounterfeitGarageB, false);

                    if (Interior.IsInteriorCapped(CounterfeitGarageB))
                        Interior.CapInterior(CounterfeitGarageB, false);

                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageB, "Set_Door_Right");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageB, "Set_Door_Left");
                    if (SetDoorLeft)
                        Interior.ActivateInteriorEntitySet(CounterfeitGarageB, "Set_Door_Left");
                    else
                        Interior.DeactivateInteriorEntitySet(CounterfeitGarageB, "Set_Door_Left");
                    if (SetDoorRight)
                        Interior.ActivateInteriorEntitySet(CounterfeitGarageB, "Set_Door_Right");
                    else
                        Interior.DeactivateInteriorEntitySet(CounterfeitGarageB, "Set_Door_Right");
                    Interior.ActivateInteriorEntitySet(CounterfeitGarageB, "Set_Tints");
                    Interior.SetInteriorEntitySetTintIndex(CounterfeitGarageB, "Set_Tints", (int)color);

                    Interior.RefreshInterior(CounterfeitGarageB);
                }
                fUI.Graphics.FadeDecalsInRange(930.0f, -2240.13f, -50.0f, 17f, 0f);


                if (Interior.IsInteriorReady(CounterfeitGarageB))
                {
                    Vector3 outVector;
                    uint outHash;
                    Interior.GetInteriorLocationAndNamehash(CounterfeitGarageB, out outVector, out outHash);
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
                int CounterfeitGarageB = InteriorIndex ?? Interior.GetInteriorAtCoords(930.0f, -2240.13f, -50.0f);
                Script.Wait(500);
                if (Interior.IsValidInterior(CounterfeitGarageB))
                {
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageB, "Set_Door_Right");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageB, "Set_Door_Left");
                    Interior.DeactivateInteriorEntitySet(CounterfeitGarageB, "Set_Tints");

                    fCore.Streaming.SetInteriorActive(CounterfeitGarageB, false);
                    Interior.UnpinInterior(CounterfeitGarageB);
                    Interior.DisableInterior(CounterfeitGarageB, true);

                    if (!Interior.IsInteriorCapped(CounterfeitGarageB))
                        Interior.CapInterior(CounterfeitGarageB, true);

                    Interior.RefreshInterior(CounterfeitGarageB);
                }

                if (!Interior.IsInteriorReady(CounterfeitGarageB))
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
            => Game.Player.Character.Position = pos ?? new Vector3(930.0f, -2240.13f, -50.0f);
    }
}
