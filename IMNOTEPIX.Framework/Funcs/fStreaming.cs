using GTA;
using GTA.Native;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.Funcs
{
    /// <summary>
    /// Wrappers for GTA streaming natives: anim dicts, PTFX assets, and map cull boxes.
    /// </summary>
    public static class fStreaming
    {
        // ── Anim dicts ────────────────────────────────────────────────────────

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

            animDictList?.Add(dict);

            while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, dict))
            {
                Function.Call(Hash.REQUEST_ANIM_DICT, dict);
                Script.Yield();
            }

            return dict;
        }

        /// <summary>Unloads all dicts previously registered in <paramref name="animDictList"/>.</summary>
        public static void RemoveAnimDicts(List<string> animDictList)
        {
            if (animDictList == null || animDictList.Count == 0)
                return;

            foreach (var dict in animDictList)
                Function.Call(Hash.REMOVE_ANIM_DICT, dict);
        }

        /// <summary>Unloads a single anim dict by name.</summary>
        public static void RemoveAnimDict(string dict)
        {
            if (!string.IsNullOrEmpty(dict))
                Function.Call(Hash.REMOVE_ANIM_DICT, dict);
        }

        // ── PTFX assets ───────────────────────────────────────────────────────

        /// <summary>
        /// Blocks until the named PTFX asset is loaded, then returns the name.
        /// Use when the asset must be ready before the next line executes.
        /// </summary>
        public static string RequestNamedPTFXAsset(string fxName)
        {
            while (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, fxName))
            {
                Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, fxName);
                Script.Yield();
            }

            return fxName;
        }

        /// <summary>
        /// Fires the load request without waiting for completion, then returns the name.
        /// Use when you can tolerate the asset not being ready immediately.
        /// </summary>
        public static string LoadNamedPTFXAsset(string fxName)
        {
            if (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, fxName))
                Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, fxName);

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

        // ── Map streaming ─────────────────────────────────────────────────────

        /// <summary>
        /// Enables or disables a named map data cull box (e.g. "prologue", "Prologue_Main").
        /// </summary>
        public static void SetMapDataCullboxEnabled(string name, bool enabled)
            => Function.Call(Hash.SET_MAPDATACULLBOX_ENABLED, name, enabled);
    }
}
