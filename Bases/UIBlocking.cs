using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniMVC
{
    /// <summary>
    /// Whether a panel or popup that takes over from the game is open - one whose "Blocks Gameplay" is ticked,
    /// such as an inventory window or a pause menu - so other systems can stand by meanwhile (e.g. interaction
    /// stops detecting) without knowing any panel. <see cref="Changed"/> fires when the first such panel opens
    /// and when the last one closes.
    /// </summary>
    /// <remarks>
    /// A panel counts from the moment it starts opening to the moment it starts closing. Panels destroyed while
    /// open (e.g. with their scene) stop counting when a scene unloads.
    /// </remarks>
    public static class UIBlocking
    {
        private static readonly HashSet<PanelViewBase> Open = new();
        private static readonly List<PanelViewBase> Gone = new();
        private static bool _hooked;

        /// <summary>Whether any open panel or popup blocks gameplay.</summary>
        public static bool IsBlocking => Open.Count > 0;

        /// <summary>Raised with true when the first blocking panel opens, and with false when the last one closes.</summary>
        public static event Action<bool> Changed;

        internal static void Add(PanelViewBase panel)
        {
            Hook();
            if (Open.Add(panel) && Open.Count == 1)
            {
                Changed?.Invoke(true);
            }
        }

        internal static void Remove(PanelViewBase panel)
        {
            if (Open.Remove(panel) && Open.Count == 0)
            {
                Changed?.Invoke(false);
            }
        }

        private static void Hook()
        {
            if (!_hooked)
            {
                SceneManager.sceneUnloaded += OnSceneUnloaded;
                _hooked = true;
            }
        }

        // Panels destroyed with their scene never closed: they stop counting here.
        private static void OnSceneUnloaded(Scene scene)
        {
            Gone.Clear();
            foreach (var panel in Open)
            {
                if (panel == null)
                {
                    Gone.Add(panel);
                }
            }

            if (Gone.Count == 0)
            {
                return;
            }

            foreach (var panel in Gone)
            {
                Open.Remove(panel);
            }

            Gone.Clear();
            if (Open.Count == 0)
            {
                Changed?.Invoke(false);
            }
        }

        // Keeps the static state clean when "Enter Play Mode Options" skips the domain reload.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => Open.Clear();
    }
}
