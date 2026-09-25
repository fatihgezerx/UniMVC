using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// The one entry point of the UI: put it on the Canvas and call <see cref="Initialize"/> once from your
    /// own bootstrap code. It initializes the views listed under its headers in the Inspector (panels,
    /// popups, buttons...) - each panel then initializes the views listed inside it - and lets any code,
    /// and every view through <see cref="ViewBase.UI"/>, reach a view by its type:
    /// <code>
    /// uiManager.Get&lt;SettingsPanel&gt;().Show();
    /// </code>
    /// </summary>
    /// <remarks>
    /// "Collect From Children" in the Inspector fills the lists: the UIManager gets the views that aren't
    /// inside any panel, and each panel the views inside it. Views are looked up by their exact type; with
    /// several of one type, the first registered is returned. Views created at runtime can join with
    /// <see cref="Register"/>.
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class UIManager : MonoBehaviour
    {
        [Tooltip("The views outside any panel, by kind. Panels initialize the views inside them.")]
        [SerializeField] private ViewCollection views = new();

        private readonly Dictionary<Type, ViewBase> _registry = new();

        /// <summary>The views listed on this UIManager, by kind.</summary>
        public ViewCollection Views => views;

        public bool IsInitialized { get; private set; }

        /// <summary>Registers and initializes every listed view, and through the panels every view inside them. Later calls do nothing.</summary>
        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;
            views.Initialize(this);
        }

        /// <summary>Makes <paramref name="view"/> reachable through <see cref="Get{T}"/> and initializes it.</summary>
        public void Register(ViewBase view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            _registry.TryAdd(view.GetType(), view);
            view.Initialize(this);
        }

        public void Unregister(ViewBase view)
        {
            if (view != null && _registry.TryGetValue(view.GetType(), out var registered) && registered == view)
            {
                _registry.Remove(view.GetType());
            }
        }

        /// <summary>The view of type <typeparamref name="T"/>. Throws if there is none.</summary>
        public T Get<T>() where T : ViewBase
        {
            if (TryGet<T>(out var view))
            {
                return view;
            }

            throw new InvalidOperationException($"[UIManager] No {typeof(T).Name} registered under '{name}'.");
        }

        public bool TryGet<T>(out T view) where T : ViewBase
        {
            if (_registry.TryGetValue(typeof(T), out var found))
            {
                view = (T)found;
                return true;
            }

            view = null;
            return false;
        }
    }
}
