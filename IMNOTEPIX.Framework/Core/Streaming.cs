using GTA.Native;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.fCore
{
    /// <summary>
    /// Wrappers for GTA streaming natives: anim dicts, PTFX assets, and map cull boxes.
    /// </summary>
    public class Streaming
    {
        /// <summary>Returns <c>true</c> when <paramref name="animDict"/> is fully loaded.</summary>
        public static bool HasAnimDictLoaded(string animDict)
            => Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, animDict);

        /// <summary>
        /// Blocks until <paramref name="dict"/> is loaded, then returns it.
        /// Optionally registers the dict in <paramref name="animDictList"/> for
        /// bulk cleanup via <see cref="RemoveAnimDicts"/>.
        /// </summary>
        public static string RequestAnimDict(string dict, List<string> animDictList = null)
        {
            if (string.IsNullOrEmpty(dict))
                return dict;

            Function.Call(Hash.REQUEST_ANIM_DICT, dict);
            while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, dict))
                GTA.Script.Wait(0);

            animDictList?.Add(dict);

            return dict;
        }

        /// <summary>Unloads all dicts previously registered in <paramref name="animDictList"/>.</summary>
        public static void RemoveAnimDicts(List<string> animDictList)
        {
            foreach (var dict in animDictList)
                RemoveAnimDict(dict);
        }

        /// <summary>Unloads a single anim dict by name.</summary>
        public static void RemoveAnimDict(string dict)
        {
            if (!string.IsNullOrEmpty(dict))
                Function.Call(Hash.REMOVE_ANIM_DICT, dict);
        }

        /// <summary>
        /// Blocks until the named PTFX asset is loaded, then returns the name.
        /// Use when the asset must be ready before the next line executes.
        /// </summary>
        public static string RequestNamedPTFXAsset(string fxName)
        {
            Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, fxName);
            while (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, fxName))
                GTA.Script.Wait(0);

            return fxName;
        }

        /// <summary>
        /// Fires the load request without waiting for completion, then returns the name.
        /// Use when you can tolerate the asset not being ready immediately.
        /// </summary>
        public static string LoadNamedPTFXAsset(string fxName)
        {
            Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, fxName);
            if (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, fxName))
                GTA.Script.Wait(0);

            return fxName;
        }

        /// <summary>Unloads the named PTFX asset.</summary>
        public static void RemoveNamedPTFXAsset(string fxName)
            => Function.Call(Hash.REMOVE_NAMED_PTFX_ASSET, fxName);

        /// <summary>Unloads the currently active PTFX asset set via <c>USE_PARTICLE_FX_ASSET</c>.</summary>
        public static void RemovePTFXAsset()
            => Function.Call(Hash.REMOVE_PTFX_ASSET);

        /// <summary>Sets the active particle FX asset for subsequent particle calls.</summary>
        public static void UseParticleFXAsset(string fxAsset)
            => Function.Call(Hash.USE_PARTICLE_FX_ASSET, fxAsset);

        /// <summary>
        /// Enables or disables a named map data cull box (e.g. "prologue", "Prologue_Main").
        /// </summary>
        public static void SetMapDataCullboxEnabled(string name, bool enabled)
            => Function.Call(Hash.SET_MAPDATACULLBOX_ENABLED, name, enabled);

        public static void SetModelAsNoLongerNeeded(int model)
            => Function.Call(Hash.SET_​MODEL_​AS_​NO_​LONGER_​NEEDED, model);

        public static void RequestModel(int model)
            => Function.Call(Hash.REQUEST_​MODEL, model);

        public static bool HasModelLoaded(int model)
            => Function.Call<bool>(Hash.HAS_​MODEL_​LOADED, model);

        public static bool IsIplActive(string iplName)
            => Function.Call<bool>(Hash.IS_​IPL_​ACTIVE, iplName);

        public static void RequestIpl(string iplName)
           => Function.Call(Hash.REQUEST_IPL, iplName);

        public static void RemoveIpl(string iplName)
           => Function.Call(Hash.REMOVE_IPL, iplName);

        public static void IplGroupSwapStart(string iplName1, string iplName2)
            => Function.Call(Hash.IPL_​GROUP_​SWAP_​START, iplName1, iplName2);

        public static void IplGroupSwapCancel()
            => Function.Call(Hash.IPL_​GROUP_​SWAP_​CANCEL);

        public static bool IplGroupSwapIsReady()
            => Function.Call<bool>(Hash.IPL_​GROUP_​SWAP_​IS_​READY);

        public static void IplGroupSwapFinish()
            => Function.Call(Hash.IPL_​GROUP_​SWAP_​FINISH);

        public static bool IplGroupSwapIsActive()
            => Function.Call<bool>(Hash.IPL_​GROUP_​SWAP_​IS_​ACTIVE);

        public static void SetInteriorActive(int interiorID, bool toggle)
            => Function.Call(Hash.SET_​INTERIOR_​ACTIVE, interiorID, toggle);

        public static void LoadAllObjectsNow()
            => Function.Call(Hash.LOAD_​ALL_​OBJECTS_​NOW);
    }
}
