using System.Collections.Generic;
using Veneer;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UniMVC
{
    /// <summary>Layout pieces shared by the studio-look Inspectors of UniMVC.</summary>
    internal static class UniMVCLayout
    {
        // Serialized fields of ViewCollection, in the order the old Inspector listed them, with the label of each list.
        private static readonly (string Field, string Label)[] ViewKinds =
        {
            ("panels", "Panel views"),
            ("popups", "Popup views"),
            ("buttons", "Button views"),
            ("toggles", "Toggle views"),
            ("sliders", "Slider views"),
            ("dropdowns", "Dropdown views"),
            ("texts", "Text views"),
            ("images", "Image views")
        };

        /// <summary>
        /// The visible top-level fields of <paramref name="serializedObject"/> that are not in <paramref name="skip"/> - the fields a
        /// subclass adds to its base type. <c>m_Script</c> and <c>[HideInInspector]</c> fields are never included.
        /// </summary>
        public static List<string> OwnPaths(SerializedObject serializedObject, HashSet<string> skip)
        {
            var paths = new List<string>(8);
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enter = true;
            while (iterator.NextVisible(enter))
            {
                enter = false;
                if (iterator.propertyPath != "m_Script" && !skip.Contains(iterator.propertyPath))
                {
                    paths.Add(iterator.propertyPath);
                }
            }

            return paths;
        }

        /// <summary>Adds one field row per path to the current container; arrays become lists.</summary>
        public static void AddFields(VeneerInspectorBuilder b, List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                SerializedProperty property = b.SerializedObject.FindProperty(paths[i]);
                if (property != null && property.isArray && property.propertyType != SerializedPropertyType.String)
                {
                    b.List(paths[i]);
                }
                else
                {
                    b.Field(paths[i]);
                }
            }
        }

        /// <summary>Adds a card with one list per kind of view of the <see cref="ViewCollection"/> at <paramref name="collectionPath"/>.</summary>
        public static void AddViewLists(VeneerInspectorBuilder b, string collectionPath)
        {
            b.Card("Views by kind", "layers", () =>
            {
                for (int i = 0; i < ViewKinds.Length; i++)
                {
                    b.List(collectionPath + "." + ViewKinds[i].Field, ViewKinds[i].Label);
                }
            });
        }

        /// <summary>
        /// Adds the "Collect from children" button. It runs <see cref="ViewCollectionTools.Collect"/> and binds <paramref name="rebind"/> again
        /// (the page that holds the lists), because the arrays were changed outside the bound lists.
        /// </summary>
        public static Button AddCollectButton(VeneerInspectorBuilder b, VisualElement rebind, bool primary)
        {
            SerializedObject serializedObject = b.SerializedObject;
            return b.Button("Collect from children", "refresh", () =>
            {
                ViewCollectionTools.Collect(serializedObject.targetObjects);
                serializedObject.Update();
                rebind.Bind(serializedObject);
            }, ViewCollectionTools.Tooltip, primary);
        }
    }
}
