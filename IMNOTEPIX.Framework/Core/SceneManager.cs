using System;
using System.Collections.Generic;
using IMNOTEPIX.Framework.Interfaces;

namespace IMNOTEPIX.Framework.Core
{
    /// <summary>
    /// Manages a single active IScene at a time.
    ///
    /// Scenes are registered as factories (Func&lt;IScene&gt;) rather than
    /// pre-constructed instances, so no scene allocates resources until
    /// it is actually started.
    ///
    /// Usage:
    ///   SceneManager.Register("BombPlant", () => new BombPlantScene());
    ///   SceneManager.StartScene("BombPlant");
    ///   // each tick:
    ///   SceneManager.Update();
    /// </summary>
    public static class SceneManager
    {
        private static readonly Dictionary<string, Func<IScene>> _factories
            = new Dictionary<string, Func<IScene>>(StringComparer.OrdinalIgnoreCase);

        private static IScene _currentScene;
        private static string _currentSceneName;

        // ─────────────────────────────────────────────────────────────────────
        // Registration
        // ─────────────────────────────────────────────────────────────────────

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

            _factories[name] = factory;
        }

        /// <summary>Removes a scene factory registration.</summary>
        public static void Unregister(string name)
        {
            if (name != null)
                _factories.Remove(name);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Control
        // ─────────────────────────────────────────────────────────────────────

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
            if (!_factories.TryGetValue(name, out var factory))
            {
                throw new KeyNotFoundException(
                    $"No scene registered with name '{name}'. " +
                    $"Call SceneManager.Register() first.");
            }

            if (_currentScene != null)
            {
                if (!forceReplace) return false;
                StopCurrentScene();
            }

            _currentScene = factory();
            _currentSceneName = name;
            _currentScene.Start();
            return true;
        }

        /// <summary>
        /// Updates the active scene. Call this once per OnTick.
        /// Automatically stops and disposes the scene when it reports IsFinished.
        /// </summary>
        public static void Update()
        {
            if (_currentScene == null) return;

            _currentScene.Update();

            if (_currentScene.IsFinished)
                FinishCurrentScene();
        }

        /// <summary>
        /// Stops and disposes the current scene immediately.
        /// Safe to call when no scene is running.
        /// </summary>
        public static void StopCurrentScene()
        {
            if (_currentScene == null) return;

            _currentScene.Stop();
            (_currentScene as IDisposable)?.Dispose();
            _currentScene = null;
            _currentSceneName = null;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Queries
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>True if a scene is currently active.</summary>
        public static bool IsSceneRunning => _currentScene != null;

        /// <summary>
        /// The name of the currently running scene, or null if none is active.
        /// </summary>
        public static string CurrentSceneName => _currentSceneName;

        /// <summary>
        /// True if the scene with the given name is the one currently running.
        /// </summary>
        public static bool IsScene(string name)
            => string.Equals(_currentSceneName, name, StringComparison.OrdinalIgnoreCase);

        /// <summary>True if a factory is registered under the given name.</summary>
        public static bool HasScene(string name)
            => name != null && _factories.ContainsKey(name);

        // ─────────────────────────────────────────────────────────────────────
        // Private
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called by Update() when the scene reports IsFinished.
        /// Calls Stop() then Dispose() (if implemented) before clearing state.
        /// </summary>
        private static void FinishCurrentScene()
        {
            _currentScene.Stop();
            (_currentScene as IDisposable)?.Dispose();
            _currentScene = null;
            _currentSceneName = null;
        }
    }
}