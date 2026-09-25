using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace UniMVC.Setup
{
    /// <summary>
    /// Keeps UniMVC compiling whether or not DOTween is in the project. Panels and popups animate with
    /// DOTween, which is optional: this sets the <c>HAS_DOTWEEN</c> scripting define symbol while
    /// <c>DOTween.dll</c> is in the project (in Assets, or as a package) and clears it when it isn't, and
    /// UniMVC compiles its animation code only under that symbol - without it, panels open and close at
    /// once. DOTween comes from the Asset Store, not the Package Manager, so it can't be installed from
    /// here: when it's missing, a dialog says so and offers its Asset Store page.
    /// </summary>
    /// <remarks>
    /// This assembly references nothing, so it always compiles. It updates the symbol whenever scripts
    /// reload or DOTween appears or disappears, and clears it when UniMVC itself is deleted, so a leftover
    /// symbol never makes a later copy of UniMVC compile against a missing DOTween.
    /// </remarks>
    [InitializeOnLoad]
    internal sealed class DOTweenGuard : AssetPostprocessor, IActiveBuildTargetChanged
    {
        private const string Define = "HAS_DOTWEEN";
        private const string DOTweenFile = "DOTween.dll";
        private const string SetupAsmdefFile = "UniMVC.Setup.asmdef";
        private const string DeclinedKey = "UniMVC.Setup.DOTweenDeclined";
        private const string AssetStoreUrl = "https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676";

        static DOTweenGuard()
        {
            EditorApplication.delayCall += () => Refresh(true);
        }

        public int callbackOrder => 0;

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget) => Refresh(false);

        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            // UniMVC itself is being deleted: nothing will keep the symbol up to date any more, so clear it
            // now. Also forget an earlier "Not now", so a fresh copy tells again.
            if (ContainsFile(deleted, SetupAsmdefFile))
            {
                SessionState.EraseBool(DeclinedKey);
                SetDefine(false);
                return;
            }

            if (ContainsFile(imported, SetupAsmdefFile))
            {
                SessionState.EraseBool(DeclinedKey);
            }

            // DOTween appeared or disappeared: update the symbol right away, during this import, so the
            // compilation that follows already uses it.
            if (ContainsFile(imported, DOTweenFile) || ContainsFile(deleted, DOTweenFile) || ContainsFile(moved, DOTweenFile))
            {
                Refresh(false);
            }
        }

        /// <summary>Sets or clears the symbol; tells about a missing DOTween when <paramref name="prompt"/>.</summary>
        private static void Refresh(bool prompt)
        {
            // UniMVC was deleted, but this code is still loaded: Unity keeps the old scripts while the
            // project has compile errors. The symbol was cleared when it was deleted, so leave it alone.
            if (!AssetExists(SetupAsmdefFile))
            {
                return;
            }

            var present = AssetExists(DOTweenFile);
            SetDefine(present);

            if (present || !prompt || Application.isBatchMode || SessionState.GetBool(DeclinedKey, false))
            {
                return;
            }

            const string message =
                "UniMVC's panel and popup animations use DOTween (free, from the Asset Store), which isn't in this project.\n\n" +
                "Until it's installed, panels and popups open and close at once - everything else works, and their " +
                "animation settings are kept.";

            if (EditorUtility.DisplayDialog("UniMVC", message, "Get DOTween", "Not now"))
            {
                Application.OpenURL(AssetStoreUrl);
            }

            SessionState.SetBool(DeclinedKey, true);
        }

        // Whether a file with exactly this name is in Assets or Packages.
        private static bool AssetExists(string fileName)
        {
            foreach (var guid in AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(fileName)))
            {
                if (Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid)) == fileName)
                {
                    return true;
                }
            }

            return false;
        }

        private static void SetDefine(bool present)
        {
            var target = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            PlayerSettings.GetScriptingDefineSymbols(target, out var current);

            var defines = new List<string>(current);
            if (present == defines.Contains(Define))
            {
                return;
            }

            if (present)
            {
                defines.Add(Define);
            }
            else
            {
                defines.Remove(Define);
            }

            PlayerSettings.SetScriptingDefineSymbols(target, defines.ToArray());
        }

        private static bool ContainsFile(string[] paths, string fileName)
        {
            foreach (var path in paths)
            {
                if (Path.GetFileName(path) == fileName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
