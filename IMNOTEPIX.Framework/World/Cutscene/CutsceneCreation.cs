using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMNOTEPIX.Framework.fWorld.Cutscene
{
    /// <summary>
    /// Manages the setup, playback, and cleanup of a single scripted cutscene.
    ///
    /// Typical usage:
    ///   var cs = new CutsceneCreation("cs_name", pos, rot, setPlayerModel: true);
    ///   cs.AddEntity(myPed, "PLAYER0", fCutscene.CutsceneUsage.Entity, 0, flag);
    ///   // position player / camera using cs.Pos / cs.Rot before calling:
    ///   cs.StartCutscene();       // returns once the cutscene begins playing
    ///   // or:
    ///   cs.StartCutsceneTillEnd(); // blocks until the cutscene finishes
    ///   cs.Cleanup();
    /// </summary>
    public class CutsceneCreation
    {
        public string CutsceneName { get; set; }

        /// <summary>
        /// World position used by the caller to place the player/camera before
        /// starting the cutscene. Not read internally.
        /// </summary>
        public Vector3 Pos { get; set; }

        /// <summary>
        /// World rotation used by the caller to orient the player/camera before
        /// starting the cutscene. Not read internally.
        /// </summary>
        public Vector3 Rot { get; set; }

        public bool SetPlayerModel { get; set; }

        // Fade flags — all default false
        public bool FadeOutAtStart { get; set; }
        public bool FadeInAtStart { get; set; }
        public bool FadeOutAtEnd { get; set; }
        public bool FadeInAtEnd { get; set; }

        public IReadOnlyList<RegisterEntityChunk> Entities => _entities;
        private readonly List<RegisterEntityChunk> _entities = new List<RegisterEntityChunk>();

        /// <param name="cutsceneName">Internal cutscene asset name.</param>
        /// <param name="pos">Caller-managed spawn / camera position.</param>
        /// <param name="rot">Caller-managed spawn / camera rotation.</param>
        /// <param name="setPlayerModel">
        /// If true, temporarily applies the player model to the cutscene ped
        /// and restores it after playback begins.
        /// </param>
        public CutsceneCreation(string cutsceneName, Vector3 pos, Vector3 rot, bool setPlayerModel, bool fadeOutAtStart = false, bool fadeInAtStart = false, bool fadeOutAtEnd = false, bool fadeInAtEnd = false)
        {
            CutsceneName = cutsceneName;
            Pos = pos;
            Rot = rot;
            SetPlayerModel = setPlayerModel;
            FadeOutAtStart = fadeOutAtStart;
            FadeInAtStart = fadeInAtStart;
            FadeOutAtEnd = fadeOutAtEnd;
            FadeInAtEnd = fadeInAtEnd;
        }

        /// <summary>Registers a scripted entity (by handle int) with this cutscene.</summary>
        public void AddEntity(int entityHandle, string cHandle, Cutscene.CutsceneUsage usage, int modelNames, Cutscene.CutsceneEntityOptionFlag entityOptionsFlag)
        {
            _entities.Add(new RegisterEntityChunk(entityHandle, cHandle, usage, modelNames, entityOptionsFlag));
        }

        /// <summary>Registers a GTA Entity object with this cutscene.</summary>
        public void AddEntity(GTA.Entity entity, string cHandle, Cutscene.CutsceneUsage usage, int modelNames, Cutscene.CutsceneEntityOptionFlag entityOptionsFlag)
        {
            _entities.Add(new RegisterEntityChunk(entity, cHandle, usage, modelNames, entityOptionsFlag));
        }

        /// <summary>
        /// Loads, registers entities, starts the cutscene, then returns as soon as
        /// it begins playing. Outfit capture/restore is performed around playback.
        ///
        /// Fade values are applied after StartCutscene — the native supports this.
        /// </summary>
        public void StartCutscene()
        {
            // Capture ped outfits before the cutscene takes control
            foreach (var chunk in _entities)
            {
                if (chunk.Ent?.EntityType == EntityType.Ped)
                    chunk.GetCutscenePedOutfit((GTA.Ped)chunk.Ent);
            }

            Cutscene.LoadCutscene(CutsceneName);

            if (SetPlayerModel)
                fPlayer.Player.PlayerModelSet(Game.Player.Character);

            RegisterEntities();

            Cutscene.StartCutscene(Cutscene.CutscenePlaybackFlags.CUTSCENE_REQUESTED_IN_MISSION);

            // Fade values can be set after StartCutscene — native processes them on next frame
            if (FadeOutAtStart || FadeInAtStart || FadeOutAtEnd || FadeInAtEnd)
                Cutscene.SetCutsceneFadeValues(FadeOutAtStart, FadeInAtStart, FadeOutAtEnd, FadeInAtEnd);

            // Coroutine yield — wait for the cutscene to begin before returning
            while (!Cutscene.IsCutscenePlaying())
                GTA.Script.Wait(0);

            // Restore outfits once the cutscene has taken control of the peds
            foreach (var chunk in _entities)
            {
                if (chunk.Ent?.EntityType == EntityType.Ped)
                    chunk.SetCutscenePedOutfit((GTA.Ped)chunk.Ent);
            }

            if (SetPlayerModel)
                fPlayer.Player.PlayerModelSetBack(Game.Player.Character);
        }

        /// <summary>
        /// Loads, registers entities, starts the cutscene, then blocks until it
        /// finishes completely.
        ///
        /// Note: outfit capture/restore is intentionally omitted here — this path
        /// is used for full-screen cutscenes where ped appearance is irrelevant.
        /// If you need outfit management, use StartCutscene() instead.
        /// </summary>
        public void StartCutsceneTillEnd()
        {
            Cutscene.LoadCutscene(CutsceneName);

            if (SetPlayerModel)
                fPlayer.Player.PlayerModelSet(Game.Player.Character);

            RegisterEntities();

            Cutscene.StartCutscene(Cutscene.CutscenePlaybackFlags.CUTSCENE_REQUESTED_IN_MISSION);
            GTA.Script.Wait(50);

            if (SetPlayerModel)
                fPlayer.Player.PlayerModelSetBack(Game.Player.Character);

            // Coroutine yield — block until cutscene finishes
            while (!Cutscene.HasCutsceneFinished())
                GTA.Script.Wait(0);
        }

        public void Cleanup()
        {
            _entities.Clear();
            Cutscene.RemoveCutscene();
        }

        private void RegisterEntities()
        {
            foreach (var chunk in _entities)
            {
                if (chunk.EntInt != -1)
                    Cutscene.RegisterEntityForCutscene(0, chunk.CHandle, (int)chunk.Usage, chunk.ModelNames, (int)chunk.EntityOptionsFlag);
                else
                    Cutscene.RegisterEntityForCutscene(chunk.Ent, chunk.CHandle, (int)chunk.Usage, chunk.ModelNames, (int)chunk.EntityOptionsFlag);
            }
        }
    }
}
