using GTA.Math;
using GTA.Native;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fWorld.Cutscene
{
    /// <summary>
    /// Provides utility functions for managing GTA V cutscenes, including loading, playback control,
    /// entity registration, and ped outfit management for cutscene characters.
    /// </summary>
    public class Cutscene
    {
        /// <summary>
        /// Gets the total duration of the currently loaded cutscene in milliseconds.
        /// </summary>
        /// <returns>Total cutscene duration in milliseconds</returns>
        public static int GetCutsceneTotalDuration()
            => Function.Call<int>(Hash.GET_CUTSCENE_TOTAL_DURATION);

        /// <summary>
        /// Gets the current playback time of the active cutscene in milliseconds.
        /// </summary>
        /// <returns>Current cutscene time in milliseconds</returns>
        public static int GetCutsceneTime()
            => Function.Call<int>(Hash.GET_CUTSCENE_TIME);

        /// <summary>
        /// Loads a cutscene with specific playback flags and waits until it's fully loaded.
        /// Yields execution until the cutscene is ready.
        /// </summary>
        /// <param name="cutscene">Name of the cutscene to load</param>
        /// <param name="playbackflag">Playback flag controlling cutscene behavior (see CutscenePlaybackFlags enum)</param>
        /// <returns>The cutscene name that was loaded</returns>
        public static string LoadCutsceneWithFlag(string cutscene, int playbackflag)
        {
            // Wait until cutscene is fully loaded into memory
            while (!Function.Call<bool>(Hash.HAS_CUTSCENE_LOADED, cutscene))
            {
                Function.Call(Hash.REQUEST_CUTSCENE_WITH_PLAYBACK_LIST, cutscene, playbackflag, 8);
                GTA.Script.Yield();
            }
            return cutscene;
        }

        /// <summary>
        /// Loads a standard cutscene and waits until it's fully loaded.
        /// Yields execution until the cutscene is ready.
        /// </summary>
        /// <param name="cutscene">Name of the cutscene to load</param>
        /// <returns>The cutscene name that was loaded</returns>
        public static string LoadCutscene(string cutscene)
        {
            // Wait until cutscene is fully loaded into memory
            while (!Function.Call<bool>(Hash.HAS_CUTSCENE_LOADED, cutscene))
            {
                Function.Call(Hash.REQUEST_CUTSCENE, cutscene, 8);
                GTA.Script.Yield();
            }
            return cutscene;
        }

        /// <summary>
        /// Loads a cutscene file (.cut file) and waits until it's fully loaded.
        /// Cut files contain cutscene data and concatenation information.
        /// </summary>
        /// <param name="cutscene">Name of the cut file to load</param>
        /// <returns>The cut file name that was loaded</returns>
        public static string LoadCutfile(string cutscene)
        {
            // Wait until cut file is fully loaded into memory
            while (!Function.Call<bool>(Hash.HAS_CUT_FILE_LOADED, cutscene))
            {
                Function.Call(Hash.REQUEST_CUT_FILE, cutscene);
                GTA.Script.Yield();
            }
            return cutscene;
        }

        /// <summary>
        /// Removes/unloads a cutscene file from memory and waits until it's fully removed.
        /// </summary>
        /// <param name="cutscene">Name of the cut file to remove</param>
        /// <returns>The cut file name that was removed</returns>
        public static string RemoveCutfile(string cutscene)
        {
            // Wait until cut file is fully unloaded from memory
            while (Function.Call<bool>(Hash.HAS_CUT_FILE_LOADED, cutscene))
            {
                Function.Call(Hash.REMOVE_CUT_FILE, cutscene);
                GTA.Script.Yield();
            }
            return cutscene;
        }

        /// <summary>
        /// Gets the number of concatenated sections in a cutscene file.
        /// Cutscenes can be split into multiple sections that play sequentially.
        /// </summary>
        /// <param name="cutsceneName">Name of the cutscene</param>
        /// <returns>Number of concatenated sections, or 0 if the file couldn't be loaded</returns>
        public static int GetNumOfConcats(string cutsceneName)
        {
            // Request the cut file
            Function.Call(Hash.REQUEST_CUT_FILE, cutsceneName);
            int result;

            // Check if the file loaded successfully
            if (Function.Call<bool>(Hash.HAS_CUT_FILE_LOADED, cutsceneName))
            {
                // Get the concatenation count and clean up
                int num = Function.Call<int>(Hash.GET_CUT_FILE_CONCAT_COUNT, cutsceneName);
                Function.Call(Hash.REMOVE_CUT_FILE, cutsceneName);
                result = num;
            }
            else
                result = 0; // File failed to load
            return result;
        }

        /// <summary>
        /// Checks if the current cutscene has finished playing.
        /// </summary>
        /// <returns>True if cutscene has completed, false otherwise</returns>
        public static bool HasCutsceneFinished()
            => Function.Call<bool>(Hash.HAS_CUTSCENE_FINISHED);

        /// <summary>
        /// Checks if a cutscene has been loaded and is ready to play.
        /// </summary>
        /// <returns>True if cutscene is loaded, false otherwise</returns>
        public static bool HasCutsceneLoaded()
            => Function.Call<bool>(Hash.HAS_CUTSCENE_LOADED);

        /// <summary>
        /// Checks if a cutscene is currently playing.
        /// </summary>
        /// <returns>True if cutscene is actively playing, false otherwise</returns>
        public static bool IsCutscenePlaying()
            => Function.Call<bool>(Hash.IS_CUTSCENE_PLAYING);

        /// <summary>
        /// Starts playing the loaded cutscene with the specified flag.
        /// </summary>
        /// <param name="flag">Playback flag (0 for default behavior)</param>
        public static void StartCutscene(int flag = 0)
           => Function.Call(Hash.START_CUTSCENE, flag);

        /// <summary>
        /// Starts playing the loaded cutscene with the specified playback flags.
        /// </summary>
        /// <param name="flag">Cutscene playback flags enum value</param>
        public static void StartCutscene(CutscenePlaybackFlags flag = 0)
           => Function.Call(Hash.START_CUTSCENE, flag);

        /// <summary>
        /// Immediately stops the currently playing cutscene without fade or transition.
        /// </summary>
        public static void StopCutsceneImmediately()
           => Function.Call(Hash.STOP_CUTSCENE_IMMEDIATELY);

        /// <summary>
        /// Removes/unloads the current cutscene from memory.
        /// Should be called after a cutscene finishes to free resources.
        /// </summary>
        public static void RemoveCutscene()
           => Function.Call(Hash.REMOVE_CUTSCENE);

        /// <summary>
        /// Stops the audio playback of the current cutscene.
        /// Useful when you want to end the cutscene but keep the audio silent.
        /// </summary>
        public static void StopCutsceneAudio()
           => Function.Call(Hash.STOP_CUTSCENE_AUDIO, 0);

        /// <summary>
        /// Sets whether the player can skip the cutscene by pressing input.
        /// </summary>
        /// <param name="toggle">True to allow skipping, false to disable skipping</param>
        public static void SetCutsceneCanBeSkipped(bool toggle)
           => Function.Call(Hash.SET_CUTSCENE_CAN_BE_SKIPPED, toggle);

        /// <summary>
        /// Configures fade in/out behavior at the start and end of cutscenes.
        /// </summary>
        /// <param name="fadeOutAtStart">Fade to black when cutscene starts</param>
        /// <param name="fadeInAtStart">Fade from black when cutscene starts</param>
        /// <param name="fadeOutAtEnd">Fade to black when cutscene ends</param>
        /// <param name="fadeInAtEnd">Fade from black when cutscene ends</param>
        public static void SetCutsceneFadeValues(bool fadeOutAtStart = false, bool fadeInAtStart = false, bool fadeOutAtEnd = false, bool fadeInAtEnd = false)
           => Function.Call(Hash.SET_CUTSCENE_FADE_VALUES, fadeOutAtStart, fadeInAtStart, fadeOutAtEnd, fadeInAtEnd);

        /// <summary>
        /// Registers a game entity (ped, vehicle, object) to participate in a cutscene.
        /// The entity will be animated according to the cutscene's script.
        /// </summary>
        /// <param name="entity">The entity to register</param>
        /// <param name="cutsceneEntityName">The name this entity has in the cutscene script</param>
        /// <param name="cutsceneUsage">How the entity should be used (default: 2 = CU_CREATE_AND_ANIMATE_NEW_SCRIPT_ENTITY)</param>
        /// <param name="modelname">Model hash override (0 to use entity's current model)</param>
        /// <param name="cutsceneEntityOptionFlag">Additional options for entity behavior (see CutsceneEntityOptionFlag enum)</param>
        public static void RegisterEntityForCutscene(GTA.Entity entity, string cutsceneEntityName, int cutsceneUsage = 2, int modelname = 0, int cutsceneEntityOptionFlag = 0)
        {
            // Get entity handle, or 0 if entity doesn't exist
            int entityHandle = entity.Exists() ? entity.Handle : 0;
            Function.Call(Hash.REGISTER_ENTITY_FOR_CUTSCENE, entityHandle, cutsceneEntityName, cutsceneUsage, modelname, cutsceneEntityOptionFlag);
        }

        /// <summary>
        /// Registers a game entity by handle to participate in a cutscene.
        /// Overload for when you already have the entity handle as an integer.
        /// </summary>
        /// <param name="entity">The entity handle (integer)</param>
        /// <param name="cutsceneEntityName">The name this entity has in the cutscene script</param>
        /// <param name="cutsceneUsage">How the entity should be used (default: 2)</param>
        /// <param name="modelname">Model hash override (0 to use entity's current model)</param>
        /// <param name="cutsceneEntityOptionFlag">Additional options for entity behavior</param>
        public static void RegisterEntityForCutscene(int entity, string cutsceneEntityName, int cutsceneUsage = 2, int modelname = 0, int cutsceneEntityOptionFlag = 0)
           => Function.Call(Hash.REGISTER_ENTITY_FOR_CUTSCENE, entity, cutsceneEntityName, cutsceneUsage, modelname, cutsceneEntityOptionFlag);

        /// <summary>
        /// Sets the world position and heading where the cutscene will take place.
        /// All cutscene entities will be positioned relative to this origin.
        /// </summary>
        /// <param name="cutscenePos">World coordinates for the cutscene origin</param>
        /// <param name="cutsceneHeading">Heading/rotation in degrees</param>
        public static void SetCutsceneOrigin(Vector3 cutscenePos, float cutsceneHeading)
           => Function.Call(Hash.SET_CUTSCENE_ORIGIN, cutscenePos.X, cutscenePos.Y, cutscenePos.Z, cutsceneHeading, 0);

        /// <summary>
        /// Sets the origin position and rotation for all concatenated sections of a cutscene.
        /// Useful for multi-part cutscenes that need consistent positioning.
        /// </summary>
        /// <param name="cutsceneName">Name of the cutscene</param>
        /// <param name="position">World position for the cutscene origin</param>
        /// <param name="rotation">Rotation in degrees (pitch, roll, yaw)</param>
        public static void SetCutsceneOriginAndRotation(string cutsceneName, Vector3 position, Vector3 rotation)
        {
            // Get the number of concatenated sections in this cutscene
            int numOfConcats = GetNumOfConcats(cutsceneName);

            // Apply the position and rotation to each section
            for (int i = 0; i < numOfConcats; i++)
            {
                Function.Call(Hash.SET_CUTSCENE_ORIGIN_AND_ORIENTATION, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, i);
            }
        }

        /// <summary>
        /// Consolidated storage for all multiplayer character (MP) ped outfit variations.
        /// Maps MP identifier (e.g., "MP_1") to their clothing component data.
        /// Each ped has 12 components: head, beard, hair, torso, legs, hands, feet, teeth, accessories, tasks, decals, tops.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<int, RegisterEntityChunk.PedVariationData>> CutscenePedVariations = new Dictionary<string, Dictionary<int, RegisterEntityChunk.PedVariationData>>
        {
            { "MP_1", CreateDefaultVariationData() },
            { "MP_2", CreateDefaultVariationData() },
            { "MP_3", CreateDefaultVariationData() },
            { "MP_4", CreateDefaultVariationData() },
            { "MP_5", CreateDefaultVariationData() },
            { "MP_6", CreateDefaultVariationData() }
        };

        /// <summary>
        /// Creates a default variation data dictionary with all 12 clothing components initialized to (0, 0).
        /// Component IDs 0-11 represent different body parts and clothing slots.
        /// </summary>
        /// <returns>Dictionary with 12 component slots, all set to drawable 0, texture 0</returns>
        private static Dictionary<int, RegisterEntityChunk.PedVariationData> CreateDefaultVariationData()
        {
            var data = new Dictionary<int, RegisterEntityChunk.PedVariationData>();
            // Iterate through all 12 ped component slots
            for (int i = 0; i < 12; i++)
            {
                data[i] = new RegisterEntityChunk.PedVariationData(0, 0);
            }
            return data;
        }

        /// <summary>
        /// Retrieves and stores the current outfit of a ped for later use in cutscenes.
        /// This captures all 12 clothing components (drawable ID and texture variation) from the ped.
        /// Useful when you need to preserve a ped's appearance before a cutscene, then restore it after.
        /// </summary>
        /// <param name="mpId">Multiplayer character identifier (e.g., "MP_1", "MP_2", up to "MP_6")</param>
        /// <param name="ped">The ped whose outfit should be captured</param>
        public static void GetCutscenePedOutfit(string mpId, GTA.Ped ped)
        {
            // Validate that this is a known MP ID
            if (!CutscenePedVariations.ContainsKey(mpId))
                return;

            // Create new dictionary to store this ped's outfit data
            var variationData = new Dictionary<int, RegisterEntityChunk.PedVariationData>();

            // Loop through all 12 component slots and capture their current values
            for (int componentId = 0; componentId < 12; componentId++)
            {
                // Get the drawable ID (which clothing item) for this component
                int drawable = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, ped, componentId);

                // Get the texture variation (color/pattern) for this drawable
                int texture = Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, ped, componentId);

                // Store both values for this component
                variationData[componentId] = new RegisterEntityChunk.PedVariationData(drawable, texture);
            }

            // Save this outfit data to the global storage for later retrieval
            CutscenePedVariations[mpId] = variationData;
        }

        /// <summary>
        /// Applies a previously stored outfit to a ped.
        /// This restores all 12 clothing components that were captured with GetCutscenePedOutfit.
        /// Typically used after a cutscene to restore a character's original appearance.
        /// </summary>
        /// <param name="mpId">Multiplayer character identifier (e.g., "MP_1", "MP_2", up to "MP_6")</param>
        /// <param name="ped">The ped to apply the outfit to</param>
        public static void SetCutscenePedOutfit(string mpId, GTA.Ped ped)
        {
            // Validate that this MP ID exists in our storage
            if (!CutscenePedVariations.ContainsKey(mpId))
                return;

            // Apply each stored component to the ped
            foreach (var data in CutscenePedVariations[mpId])
            {
                int componentId = data.Key;           // Which component slot (0-11)
                int drawable = data.Value.Drawable;    // Which clothing item
                int texture = data.Value.Texture;      // Which texture/color variant

                // Apply this component to the ped (parameter 1 = palette, usually 1 for clothing)
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, componentId, drawable, texture, 1);
            }
        }

        /// <summary>
        /// Defines how an entity should be handled during cutscene playback.
        /// Controls whether the entity is animated, created, or deleted during the cutscene.
        /// </summary>
        public enum CutsceneUsage
        {
            /// <summary>Animate an existing entity in the game world</summary>
            CU_ANIMATE_EXISTING_SCRIPT_ENTITY,

            /// <summary>Animate an existing entity, then delete it when cutscene ends</summary>
            CU_ANIMATE_AND_DELETE_EXISTING_SCRIPT_ENTITY,

            /// <summary>Create a new entity for the cutscene and animate it</summary>
            CU_CREATE_AND_ANIMATE_NEW_SCRIPT_ENTITY,

            /// <summary>Don't animate the entity during the cutscene</summary>
            CU_DONT_ANIMATE_ENTITY
        }

        /// <summary>
        /// Optional flags for controlling entity behavior during cutscenes.
        /// These can be combined using bitwise OR operations.
        /// </summary>
        public enum CutsceneEntityOptionFlag
        {
            /// <summary>No special options</summary>
            CEO_NONE,

            /// <summary>Preserve facial blood and damage during cutscene</summary>
            CEO_PRESERVE_FACE_BLOOD_DAMAGE,

            /// <summary>Preserve body blood and damage during cutscene</summary>
            CEO_PRESERVE_BODY_BLOOD_DAMAGE,

            /// <summary>Remove body blood damage when cutscene starts</summary>
            CEO_REMOVE_BODY_BLOOD_DAMAGE = 4,

            /// <summary>Copy damage from original entity to cutscene model</summary>
            CEO_CLONE_DAMAGE_TO_CS_MODEL = 8,

            /// <summary>Reset the entity's physics capsule when cutscene ends</summary>
            CEO_RESET_CAPSULE_AT_END = 16,

            /// <summary>Make this entity the shadow focus during cutscene exit</summary>
            CEO_IS_CASCADE_SHADOW_FOCUS_ENTITY_DURING_EXIT = 32,

            /// <summary>Ignore the model name when registering entity</summary>
            CEO_IGNORE_MODEL_NAME = 64,

            /// <summary>Preserve hair scaling from original ped</summary>
            CEO_PRESERVE_HAIR_SCALE = 128,

            /// <summary>Set up hair scale instantly without transition</summary>
            CEO_INSTANT_HAIR_SCALE_SETUP = 256,

            /// <summary>Don't reset ped's physics capsule</summary>
            CEO_DONT_RESET_PED_CAPSULE = 512,

            /// <summary>Update this entity as a real door during cutscene</summary>
            CEO_UPDATE_AS_REAL_DOOR = 1024
        }

        /// <summary>
        /// Flags representing different sections of a cutscene.
        /// Used for controlling which parts of a concatenated cutscene should play.
        /// These are bit flags and can be combined.
        /// </summary>
        public enum CutsceneSection
        {
            CS_SECTION_1 = 1,
            CS_SECTION_2 = 2,
            CS_SECTION_3 = 4,
            CS_SECTION_4 = 8,
            CS_SECTION_5 = 16,
            CS_SECTION_6 = 32,
            CS_SECTION_7 = 64,
            CS_SECTION_8 = 128,
            CS_SECTION_9 = 256,
            CS_SECTION_10 = 512,
            CS_SECTION_11 = 1024,
            CS_SECTION_12 = 2048,
            CS_SECTION_13 = 4096,
            CS_SECTION_14 = 8192,
            CS_SECTION_15 = 16384,
            CS_SECTION_16 = 32768,
            CS_SECTION_17 = 65536,
            CS_SECTION_18 = 131072,
            CS_SECTION_19 = 262144,
            CS_SECTION_20 = 524288,
            CS_SECTION_21 = 1048576,
            CS_SECTION_22 = 2097152,
            CS_SECTION_23 = 4194304,
            CS_SECTION_24 = 8388608,
            CS_SECTION_25 = 16777216,
            CS_SECTION_26 = 33554432,
            CS_SECTION_27 = 67108864,
            CS_SECTION_28 = 134217728,
            CS_SECTION_29 = 268435456,
            CS_SECTION_30 = 536870912,
            CS_SECTION_31 = 1073741824
        }

        /// <summary>
        /// Flags that control how a cutscene is played back.
        /// These affect cutscene behavior like loading, skipping, and audio.
        /// </summary>
        public enum CutscenePlaybackFlags
        {
            /// <summary>Cutscene was requested from a UI widget</summary>
            CUTSCENE_REQUESTED_FROM_WIDGET = 1,

            /// <summary>Cutscene was requested directly from skip functionality</summary>
            CUTSCENE_REQUESTED_DIRECTLY_FROM_SKIP = 2,

            /// <summary>Cutscene was requested from Z-axis skip</summary>
            CUTSCENE_REQUESTED_FROM_Z_SKIP = 4,

            /// <summary>Cutscene is being played during a mission</summary>
            CUTSCENE_REQUESTED_IN_MISSION = 8,

            /// <summary>Force load audio events for this cutscene</summary>
            CUTSCENE_PLAYBACK_FORCE_LOAD_AUDIO_EVENT = 16
        }
    }
}
