using UnityEditor;
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// Gives panels and popups the <see cref="CanvasGroup"/> their animations fade. Shared by the default Inspector
    /// and the studio-look Inspector of <see cref="PanelViewBase"/>, so it works with or without Veneer.
    /// </summary>
    internal static class PanelCanvasGroupFix
    {
        /// <summary>
        /// RequireComponent only adds the CanvasGroup to panels added from now on: this gives it to older ones as soon as
        /// they are inspected - on the next editor tick, not while the Inspector is being built. Prefab assets are skipped.
        /// </summary>
        /// <param name="panels">The inspected panel components.</param>
        public static void EnsureOnNextTick(Object[] panels)
        {
            EditorApplication.delayCall += () =>
            {
                foreach (var panel in panels)
                {
                    if (panel == null)
                    {
                        continue;
                    }

                    var gameObject = ((Component)panel).gameObject;
                    if (!EditorUtility.IsPersistent(gameObject) && !gameObject.TryGetComponent<CanvasGroup>(out _))
                    {
                        Undo.AddComponent<CanvasGroup>(gameObject);
                    }
                }
            };
        }
    }
}
