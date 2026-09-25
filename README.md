# UniMVC

A small, clean view layer for Unity's uGUI: typed view bases, one `UIManager` to initialize and find
them, and script templates in the Create menu.

## Overview

Every piece of UI is a **view**: a MonoBehaviour deriving from one of UniMVC's bases (`PanelViewBase`,
`ButtonViewBase`, `DropdownViewBase`...). A single `UIManager` on the Canvas initializes the views listed
in it once, each panel initializes the views listed inside it, and any view can reach any other by its
type. There are no scene-wide lookups, no string names and no singletons.

UniMVC contains only its bases, the `UIManager` and its editor tools. Your game's views live next to them
in your MVC folder, one subfolder per kind:

```
MVC/
├── Bases/         ViewBase, PanelViewBase, PopupViewBase, ButtonViewBase...
├── Editor/        Create menu, templates, Inspector tools
├── UIManager/     UIManager
├── Panels/        your PanelViewBase views
├── Popups/        your PopupViewBase views
├── Buttons/       your ButtonViewBase views
└── ...
```

`UIManager/` holds an Assembly Definition Reference, so the `UIManager` compiles together with the bases
(which use it) while your own views compile on their own.

Other packages follow the same layout: when both are installed, e.g. the Inventory System drops its
`InventoryPanel` into `MVC/Panels/` and its buttons into `MVC/Buttons/`, and the Localization System
drops its `LanguageDropdown` into `MVC/Dropdowns/`. Once copied, they are your own code to edit.

## Views

| Base | Wraps | Override |
|---|---|---|
| `ViewBase` | - | `OnInitialize()` · `Show()` / `Hide()` |
| `PanelViewBase` | a screen or window | `OnShown()` / `OnHidden()` · `Toggle()` · its child views |
| `PopupViewBase` | a dialog or popup | Same as `PanelViewBase`, listed under its own header |
| `ButtonViewBase` | `Button` | `OnClick()` |
| `ToggleViewBase` | `Toggle` | `OnValueChanged(bool)` · `SetIsOn(bool)` |
| `SliderViewBase` | `Slider` | `OnValueChanged(float)` · `SetValue(value, max)` |
| `DropdownViewBase` | `TMP_Dropdown` | `OnValueChanged(int)` · `SetOptions(list, selected)` |
| `TextViewBase` | `TMP_Text` | `SetText(string)` |

Every view is initialized exactly once, even while it is hidden. `OnInitialize` is where components are
cached and listeners are added. When you override it in a panel or popup, call `base.OnInitialize()`
first: that is where the panel initializes its child views. The `Set...` methods change a control without
calling its `OnValueChanged`.

## The UIManager and panels in the Inspector

The `UIManager`, every panel and every popup show their views in the Inspector under one header per kind
(**Panels**, **Popups**, **Buttons**, **Toggles**, **Sliders**, **Dropdowns**, **Texts**), each with its
list below. `UIManager.Initialize()` initializes the views in its lists, and each panel then initializes
the views in its own lists, all the way down.

**Collect From Children** (under the lists) fills them from the hierarchy for you. The `UIManager` gets
the views that aren't inside any panel or popup, and each panel or popup gets the views inside it. Run it
on the `UIManager` and on each panel after changing the hierarchy, or edit the lists by hand.

## Setup

### Requirements

- Unity 2021.3 LTS or newer
- uGUI and TextMeshPro (`com.unity.ugui`), included by default

Importing UniMVC never breaks your project. A small setup script checks for these, leaves UniMVC out of
compilation while one is missing, and offers to install it (**Tools > UniMVC > Check Dependencies**).

### Installation

Copy the repository's contents into `Assets/Scripts/MVC/`. Your own `Panels/`, `Buttons/`... folders
then sit next to `Bases/`. Systems that use UniMVC (e.g. InventorySystem, LocalizationSystem) can also
download it there for you, from their setup dialog. Either way you get the same files, all visible and
editable in `Assets/Scripts/MVC/`.

## Quick Start

**1. Add a `UIManager` to your Canvas** (or to a parent of every canvas).

**2. Create views** with **Create > Scripting > MVC > Panel View / Popup View / Button View / ...** in the
folder they belong to, add them to the matching UI objects, then press **Collect From Children** on the
`UIManager` and on each panel.

**3. Initialize it once** from your own bootstrap code, after the systems your views use:

```csharp
[SerializeField] private UIManager uiManager;

private void Awake() => uiManager.Initialize();
```

**4. Use them:**

```csharp
using UniMVC;

public class SettingsButton : ButtonViewBase
{
    protected override void OnClick() => UI.Get<SettingsPanel>().Show();
}

public class SettingsPanel : PanelViewBase
{
    protected override void OnShown() => Time.timeScale = 0f;
    protected override void OnHidden() => Time.timeScale = 1f;
}
```

`UI` is the `UIManager` that initialized the view. From outside the UI, use `uiManager.Get<T>()`.
Views created at runtime join with `uiManager.Register(view)`.

## License

[MIT License](LICENSE)
