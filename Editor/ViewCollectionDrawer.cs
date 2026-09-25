using UnityEditor;
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// Draws a <see cref="ViewCollection"/> - on a <see cref="UIManager"/> and on every panel and popup - as
    /// one large header per kind of view with its list below, and a "Collect From Children" button that
    /// fills every list from the hierarchy.
    /// </summary>
    [CustomPropertyDrawer(typeof(ViewCollection))]
    internal sealed class ViewCollectionDrawer : PropertyDrawer
    {
        private const float HeaderHeight = 24f;
        private const float GroupSpacing = 6f;
        private const float ButtonHeight = 24f;

        private static readonly (string Field, string Header, string Label)[] Groups =
        {
            ("panels", "Panels", "Panel Views"),
            ("popups", "Popups", "Popup Views"),
            ("buttons", "Buttons", "Button Views"),
            ("toggles", "Toggles", "Toggle Views"),
            ("sliders", "Sliders", "Slider Views"),
            ("dropdowns", "Dropdowns", "Dropdown Views"),
            ("texts", "Texts", "Text Views")
        };

        private static GUIStyle _headerStyle;

        private static GUIStyle HeaderStyle => _headerStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.LowerLeft
        };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 0f;
            foreach (var group in Groups)
            {
                height += HeaderHeight + EditorGUI.GetPropertyHeight(property.FindPropertyRelative(group.Field), true) + GroupSpacing;
            }

            return height + ButtonHeight + GroupSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var y = position.y;
            foreach (var group in Groups)
            {
                var list = property.FindPropertyRelative(group.Field);

                EditorGUI.LabelField(new Rect(position.x, y, position.width, HeaderHeight), group.Header, HeaderStyle);
                y += HeaderHeight;

                var listHeight = EditorGUI.GetPropertyHeight(list, true);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, listHeight), list, new GUIContent(group.Label), true);
                y += listHeight + GroupSpacing;
            }

            var buttonRect = new Rect(position.x, y + GroupSpacing, position.width, ButtonHeight);
            if (GUI.Button(buttonRect, new GUIContent("Collect From Children",
                    "Refills every list with the views below this object that aren't inside another panel or popup.")))
            {
                Collect(property);
            }
        }

        private void Collect(SerializedProperty property)
        {
            foreach (var target in property.serializedObject.targetObjects)
            {
                if (target is not Component component || fieldInfo.GetValue(target) is not ViewCollection collection)
                {
                    continue;
                }

                Undo.RecordObject(target, "Collect Views");
                collection.CollectFrom(component.transform, component as PanelViewBase);
                EditorUtility.SetDirty(target);
            }

            property.serializedObject.Update();
        }
    }
}
