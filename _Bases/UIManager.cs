using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// The one entry point of the UI: put it on the Canvas (or a parent of every canvas) and call
    /// <see cref="Initialize"/> once from your own bootstrap code. It initializes every
    /// <see cref="ViewBase"/> below it - hidden ones included - and lets any code, and every view through
    /// <see cref="ViewBase.UI"/>, reach a view by its type:
    /// <code>
    /// uiManager.Get&lt;SettingsPanel&gt;().Show();
    /// </code>
    /// </summary>
    /// <remarks>
    /// Views are looked up by their exact type; with several of one type, the first found is returned.
    /// Views created at runtime can join with <see cref="Register"/>.
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class UIManager : MonoBehaviour
    {
        private readonly Dictionary<Type, ViewBase> _views = new();

        public bool IsInitialized { get; private set; }

        /// <summary>Registers and initializes every view below this object. Later calls do nothing.</summary>
        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;
            foreach (var view in GetComponentsInChildren<ViewBase>(true))
            {
                Register(view);
            }
        }

        /// <summary>Makes <paramref name="view"/> reachable through <see cref="Get{T}"/> and initializes it.</summary>
        public void Register(ViewBase view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            _views.TryAdd(view.GetType(), view);
            view.Initialize(this);
        }

        public void Unregister(ViewBase view)
        {
            if (view != null && _views.TryGetValue(view.GetType(), out var registered) && registered == view)
            {
                _views.Remove(view.GetType());
            }
        }

        /// <summary>The view of type <typeparamref name="T"/>. Throws if there is none.</summary>
        public T Get<T>() where T : ViewBase
        {
            if (TryGet<T>(out var view))
            {
                return view;
            }

            throw new InvalidOperationException($"[UIManager] No {typeof(T).Name} under '{name}'.");
        }

        public bool TryGet<T>(out T view) where T : ViewBase
        {
            if (_views.TryGetValue(typeof(T), out var found))
            {
                view = (T)found;
                return true;
            }

            view = null;
            return false;
        }
    }
}
