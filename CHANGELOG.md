# Changelog

## [Unreleased]

### Editor
- New studio-look Inspectors (Veneer) for panels and popups (tabs Panel / Settings / Views), slider views, the
  `UIManager` (tabs Views / Controllers), and an automatic header-card Inspector for buttons, toggles, dropdowns, texts, images
  and controllers. All keep multi-object editing, Undo and prefab overrides; the view lists are the kit's list editor and the
  **Collect from children** action is now a UI-free helper (`ViewCollectionTools`, also records prefab-instance overrides).
- The studio Inspectors live in their own gated assembly `UniMVC.Editor.Studio` (needs `HAS_VENEER`). `UniMVC.Editor`
  keeps the Create menu, templates, `UISetup` and the IMGUI `ViewCollectionDrawer`; without the kit the previous panel, slider and
  `UIManager` Inspectors remain as the fallback (`#if !HAS_VENEER`). The panel `CanvasGroup` fix is shared (`PanelCanvasGroupFix`).
- `DOTweenGuard` also keeps `HAS_VENEER` in step with the optional kit (set while `Veneer.Editor` exists; when
  UniMVC is deleted it is set to what is still installed). It never downloads or asks.
- Brand: indigo accent, `UniMVCBrand` and `Editor/Brand/UniMVCBrand.uss`.
