using System.Collections.Generic;
using Veneer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniMVC
{
    /// <summary>
    /// Studio-look Inspector of every slider view: a Value Animation card (None or Trail; the trail's colour, delay, duration and ease
    /// show only while Trail is chosen) and a card with the fields the slider class adds.
    /// </summary>
    [CustomEditor(typeof(SliderViewBase), true)]
    [CanEditMultipleObjects]
    internal sealed class SliderViewStudioEditor : VeneerInspector<SliderViewBase>
    {
        private static readonly HashSet<string> BaseFields = new()
        {
            "valueAnimation", "trailColor", "trailDelay", "trailDuration", "trailEase"
        };

        private static readonly string[] AnimationIcons = { "minus-circle", "graph" };

        private static readonly string[] AnimationTooltips =
        {
            "None: the bar moves at once.",
            "Trail: a drop leaves a lighter bar behind that glides down to the fill (e.g. a health bar)."
        };

        protected override VeneerBrand Brand => UniMVCBrand.Value;

        protected override void Build(VeneerInspectorBuilder b)
        {
            SerializedProperty animation = serializedObject.FindProperty("valueAnimation");
            b.Header(ObjectNames.NicifyVariableName(target.GetType().Name), "Slider view", "sliders");
            b.StatePill(() =>
            {
                serializedObject.UpdateIfRequiredOrScript();
                if (animation.hasMultipleDifferentValues)
                {
                    return ("Mixed", "idle");
                }

                return animation.enumValueIndex == (int)SliderAnimation.Trail ? ("Trail", "ok") : ("Instant", "idle");
            });

            b.Card("Value animation", "graph", () =>
            {
                b.Enum("valueAnimation", AnimationIcons, AnimationTooltips, "Value animation");

                var trail = new VisualElement();
                b.Add(trail);
                using (b.Into(trail))
                {
                    b.Color("trailColor", "Trail color");
                    b.Slider("trailDelay", "Trail delay", "Instant", "Long", unit: "s");
                    b.Slider("trailDuration", "Trail duration", "Fast", "Slow", unit: "s");
                    b.Field("trailEase", "Trail ease");
                }

                b.VisibleWhen(trail, "valueAnimation", (int)SliderAnimation.Trail);
            });

            List<string> ownPaths = UniMVCLayout.OwnPaths(serializedObject, BaseFields);
            if (ownPaths.Count > 0)
            {
                b.Card("Settings", "settings", () => UniMVCLayout.AddFields(b, ownPaths));
            }
        }
    }
}
