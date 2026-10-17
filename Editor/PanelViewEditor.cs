#if !HAS_VENEER
using UnityEditor;
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// Fallback Inspector of every panel and popup while Veneer is not installed (the studio look replaces it
    /// otherwise): an Animation Settings header on top - the open and close animations (the popup ones on a
    /// <see cref="PopupViewBase"/>), their duration and ease - then the panel's own fields and its views (drawn by
    /// <see cref="ViewCollectionDrawer"/>).
    /// </summary>
    [CustomEditor(typeof(PanelViewBase), true)]
    [CanEditMultipleObjects]
    internal sealed class PanelViewEditor : Editor
    {
        // Drawn under Animation Settings instead of in the default list below it.
        private static readonly string[] Excluded =
        {
            "m_Script", "openAnimation", "closeAnimation", "popupOpenAnimation", "popupCloseAnimation",
            "animationDuration", "animationEase"
        };

        private void OnEnable() => PanelCanvasGroupFix.EnsureOnNextTick(targets);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            DrawAnimationSettings();
            EditorGUILayout.Space(6f);
            DrawPropertiesExcluding(serializedObject, Excluded);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAnimationSettings()
        {
            var isPopup = target is PopupViewBase;
            var open = serializedObject.FindProperty(isPopup ? "popupOpenAnimation" : "openAnimation");
            var close = serializedObject.FindProperty(isPopup ? "popupCloseAnimation" : "closeAnimation");

            EditorGUILayout.LabelField("Animation Settings", ViewCollectionDrawer.HeaderStyle, GUILayout.Height(24f));
            EditorGUILayout.PropertyField(open, new GUIContent("Open Animation", open.tooltip));
            EditorGUILayout.PropertyField(close, new GUIContent("Close Animation", close.tooltip));

            // Both None: nothing animates, so duration and ease don't apply.
            using (new EditorGUI.DisabledScope(open.enumValueIndex == 0 && close.enumValueIndex == 0 && !open.hasMultipleDifferentValues))
            {
                var duration = serializedObject.FindProperty("animationDuration");
                var ease = serializedObject.FindProperty("animationEase");
                EditorGUILayout.PropertyField(duration, new GUIContent("Duration", duration.tooltip));
                EditorGUILayout.PropertyField(ease, new GUIContent("Ease", ease.tooltip));
            }

#if !HAS_DOTWEEN
            EditorGUILayout.HelpBox("These animations need DOTween, which isn't in this project. Until it's installed, " +
                                    "the panel opens and closes at once; the settings are kept.", MessageType.Info);
#endif
        }
    }
}
#endif
