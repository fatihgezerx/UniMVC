using System.IO;
using UnityEditor;
using UnityEditor.Compilation;

namespace UniMVC
{
    /// <summary>
    /// <c>Assets/Create/Scripting/MVC/...</c>: creates a new view (or controller) script from the matching
    /// template, in the folder selected in the Project window - e.g. a Button View in <c>MVC/Buttons</c>.
    /// </summary>
    internal static class MVCScriptMenu
    {
        private const string Menu = "Assets/Create/Scripting/MVC/";
        private const int Priority = 80;

        [MenuItem(Menu + "Panel View", false, Priority)]
        private static void CreatePanelView() => Create("PanelView", "NewPanelView.cs");

        [MenuItem(Menu + "Popup View", false, Priority + 1)]
        private static void CreatePopupView() => Create("PopupView", "NewPopupView.cs");

        [MenuItem(Menu + "Button View", false, Priority + 2)]
        private static void CreateButtonView() => Create("ButtonView", "NewButtonView.cs");

        [MenuItem(Menu + "Toggle View", false, Priority + 3)]
        private static void CreateToggleView() => Create("ToggleView", "NewToggleView.cs");

        [MenuItem(Menu + "Slider View", false, Priority + 4)]
        private static void CreateSliderView() => Create("SliderView", "NewSliderView.cs");

        [MenuItem(Menu + "Dropdown View", false, Priority + 5)]
        private static void CreateDropdownView() => Create("DropdownView", "NewDropdownView.cs");

        [MenuItem(Menu + "Text View", false, Priority + 6)]
        private static void CreateTextView() => Create("TextView", "NewTextView.cs");

        [MenuItem(Menu + "Image View", false, Priority + 7)]
        private static void CreateImageView() => Create("ImageView", "NewImageView.cs");

        [MenuItem(Menu + "Controller", false, Priority + 20)]
        private static void CreateController() => Create("Controller", "NewController.cs");

        // Templates sit next to this assembly's asmdef, wherever UniMVC was installed.
        private static void Create(string template, string fileName)
        {
            var asmdef = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName("UniMVC.Editor");
            var path = Path.GetDirectoryName(asmdef)!.Replace('\\', '/') + "/Templates/" + template + "Template.cs.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, fileName);
        }
    }
}
