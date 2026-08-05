using GTA;
using GTA.Math;
using IMNOTEPIX.Framework.Abstracts;
using System;

namespace IMNOTEPIX.Framework.fWorld.Map.Interiors
{
    public class AInteriorTemplate : AInterior
    {
        private const string LOG_CAT = "AInteriorTemplate|AInterior";
        private int? InteriorIndex = null;
        public uint? Hash { get; private set; } = null;
        public Vector3? Position { get; private set; } = null;
        protected override void OnLoad() { 
            fCore.Logger.Info(LOG_CAT, "Trying To Load...");
            try { 
                // REQUEST/REMOVE IPL'S
                // fCore.Streaming.RequestIpl();
                int template = InteriorIndex ?? Interior.GetInteriorAtCoords(Vector3.Zero);
                Script.Wait(500);
                if (Interior.IsValidInterior(template))
                {
                    InteriorIndex = template;

                    fCore.Streaming.SetInteriorActive(template, true);
                    Interior.PinInteriorInMemory(template);
                    Interior.DisableInterior(template, false);

                    if (Interior.IsInteriorCapped(template))
                        Interior.CapInterior(template, false);

                    // INTERIOR SETS

                    Interior.RefreshInterior(template);
                }
                // FADE DECALS
                // fUI.Graphics.FadeDecalsInRange();


                if (Interior.IsInteriorReady(template)/* && fCore.Streaming.IsIplActive()*/)
                {
                    Vector3 outVector;
                    uint outHash;
                    Interior.GetInteriorLocationAndNamehash(template, out outVector, out outHash);
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
            try
            {
                int template = InteriorIndex ?? Interior.GetInteriorAtCoords(Vector3.Zero);
                Script.Wait(500);
                if (Interior.IsValidInterior(template))
                {
                    // INTERIOR SETS

                    fCore.Streaming.SetInteriorActive(template, false);
                    Interior.UnpinInterior(template);
                    Interior.DisableInterior(template, true);

                    if (!Interior.IsInteriorCapped(template))
                        Interior.CapInterior(template, true);

                    Interior.RefreshInterior(template);
                }
                // REMOVE/REQUEST IPL'S
                // fCore.Streaming.RemoveIpl();

                if (!Interior.IsInteriorReady(template)/* && !fCore.Streaming.IsIplActive()*/)
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
            => Game.Player.Character.Position = pos ?? new Vector3();
    }
}
