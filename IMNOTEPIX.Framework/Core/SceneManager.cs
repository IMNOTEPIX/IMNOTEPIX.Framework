using System;
using System.Collections.Generic;
using IMNOTEPIX.Framework.fInterfaces;

namespace IMNOTEPIX.Framework.fCore
{
    /// <summary>
    /// Manages a single active IScene at a time.
    ///
    /// Scenes are registered as factories (Func&lt;IScene&gt;) rather than
    /// pre-constructed instances, so no scene allocates resources until
    /// it is actually started.
    /// </summary>
    ///
    /// Usage:
    ///   SceneManager.Register("BombPlant", () => new BombPlantScene());
    ///   SceneManager.StartScene("BombPlant");
    ///   // each tick:
    ///   SceneManager.Update();
    public class SceneManager
    {
        private static readonly Dictionary<string, Func<IScene>> factories = new Dictionary<string, Func<IScene>>(StringComparer.OrdinalIgnoreCase);

        private static IScene currentScene;
        private static string currentSceneName;

        /// <summary>
        /// Registers a scene factory under the given name.
        /// The factory is called each time the scene is started, so a fresh
        /// instance is created for every run.
        /// Replaces any existing registration with the same name.
        /// </summary>
        public static void Register(string name, Func<IScene> factory)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Scene name cannot be null or empty.", nameof(name));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            factories[name] = factory;
        }

        /// <summary>Removes a scene factory registration.</summary>
        public static void Unregister(string name)
        {
            if (name != null)
                factories.Remove(name);
        }

        /// <summary>
        /// Starts the named scene.
        ///
        /// If a scene is already running and <paramref name="forceReplace"/> is
        /// false (default), the call is ignored and this method returns false.
        /// If <paramref name="forceReplace"/> is true, the current scene is
        /// stopped and disposed before the new one starts.
        ///
        /// Returns true if the scene was started, false if it was blocked.
        /// </summary>
        public static bool StartScene(string name, bool forceReplace = false)
        {
            if (!factories.TryGetValue(name, out var factory))
                throw new KeyNotFoundException($"No scene registered with name '{name}'. " + $"Call SceneManager.Register() first.");

            if (currentScene != null)
            {
                if (!forceReplace)
                    return false;
                StopCurrentScene();
            }

            currentScene = factory();
            currentSceneName = name;
            currentScene.Start();
            return true;
        }

        /// <summary>
        /// Updates the active scene. <b>Call this once per OnTick.</b>
        /// Automatically stops and disposes the scene when it reports IsFinished.
        /// </summary>
        public static void Update()
        {
            if (currentScene != null)
                currentScene.Update();

            if (currentScene != null && currentScene.IsFinished)
                FinishCurrentScene();
        }

        /// <summary>
        /// Stops and disposes the current scene immediately.
        /// Safe to call when no scene is running.
        /// </summary>
        public static void StopCurrentScene()
        {
            if (currentScene == null)
                return;

            currentScene.Stop();
            (currentScene as IDisposable)?.Dispose();
            currentScene = null;
            currentSceneName = null;
        }

        /// <summary>True if a scene is currently active.</summary>
        public static bool IsSceneRunning => currentScene != null;

        /// <summary>
        /// The name of the currently running scene, or null if none is active.
        /// </summary>
        public static string CurrentSceneName => currentSceneName;

        /// <summary>
        /// True if the scene with the given name is the one currently running.
        /// </summary>
        public static bool IsScene(string name)
            => string.Equals(currentSceneName, name, StringComparison.OrdinalIgnoreCase);

        /// <summary>True if a factory is registered under the given name.</summary>
        public static bool HasScene(string name)
            => name != null && factories.ContainsKey(name);

        /// <summary>
        /// Called by Update() when the scene reports IsFinished.
        /// Calls Stop() then Dispose() (if implemented) before clearing state.
        /// </summary>
        private static void FinishCurrentScene()
        {
            currentScene.Stop();
            (currentScene as IDisposable)?.Dispose();
            currentScene = null;
            currentSceneName = null;
        }
    }
}
