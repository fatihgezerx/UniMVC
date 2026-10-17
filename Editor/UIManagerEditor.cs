#if !HAS_VENEER
using UnityEditor;
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// Fallback Inspector of <see cref="UIManager"/> while Veneer is not installed (the studio look replaces it
    /// otherwise): its controllers under a Controllers header, then its views under one header per kind (drawn by
    /// <see cref="ViewCollectionDrawer"/>).
    /// </summary>
    [CustomEditor(typeof(UIManager))]
    internal sealed class UIManagerEditor : Editor
    {
        private SerializedProperty _controllers;
        private SerializedProperty _views;

        private void OnEnable()
        {
            _controllers = serializedObject.FindProperty("controllers");
            _views = serializedObject.FindProperty("views");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Controllers", ViewCollectionDrawer.HeaderStyle, GUILayout.Height(24f));
            EditorGUILayout.PropertyField(_controllers, new GUIContent("Controllers", _controllers.tooltip), true);
            EditorGUILayout.Space(6f);
            EditorGUILayout.PropertyField(_views, GUIContent.none, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
