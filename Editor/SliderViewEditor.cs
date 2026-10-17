#if !HAS_VENEER
using UnityEditor;
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// Fallback Inspector of every slider view while Veneer is not installed (the studio look replaces it otherwise):
    /// a Value Animation header on top (the Trail's colour, delay, duration and ease only show while the animation is on),
    /// then the view's own fields.
    /// </summary>
    [CustomEditor(typeof(SliderViewBase), true)]
    [CanEditMultipleObjects]
    internal sealed class SliderViewEditor : Editor
    {
        private static readonly string[] Excluded =
        {
            "m_Script", "valueAnimation", "trailColor", "trailDelay", "trailDuration", "trailEase"
        };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            EditorGUILayout.LabelField("Animation Settings", ViewCollectionDrawer.HeaderStyle, GUILayout.Height(24f));
            var animation = serializedObject.FindProperty("valueAnimation");
            EditorGUILayout.PropertyField(animation, new GUIContent("Value Animation", animation.tooltip));
            if ((SliderAnimation)animation.enumValueIndex == SliderAnimation.Trail)
            {
                DrawField("trailColor", "Trail Color");
                DrawField("trailDelay", "Trail Delay");
                DrawField("trailDuration", "Trail Duration");
                DrawField("trailEase", "Trail Ease");
            }

            DrawPropertiesExcluding(serializedObject, Excluded);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawField(string field, string label)
        {
            var property = serializedObject.FindProperty(field);
            EditorGUILayout.PropertyField(property, new GUIContent(label, property.tooltip));
        }
    }
}
#endif
