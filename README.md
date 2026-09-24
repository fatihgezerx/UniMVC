# UniMVC

A small, clean view layer for Unity's uGUI: typed view bases, one `UIManager` to initialize and find
them, and script templates in the Create menu.

## Overview

Every piece of UI is a **view**: a MonoBehaviour deriving from one of UniMVC's bases (`PanelViewBase`,
`ButtonViewBase`, `DropdownViewBase`...). A single `UIManager` on the Canvas initializes every view below
it once, and lets any view reach any other by its type. There are no scene-wide lookups, no string names
and no singletons.

UniMVC contains only these bases. Your game's views live next to them in your MVC folder, one subfolder
per kind:

```
MVC/
├── _Bases/        UniMVC itself
├── Panels/        your PanelViewBase views
├── Buttons/       your ButtonViewBase views
├── Dropdowns/     ...
└── ...
```

Other packages follow the same layout: when both are installed, e.g. the Inventory System drops its
`InventoryPanel` into `MVC/Panels/` and its buttons into `MVC/Buttons/`, and the Localization System
drops its `LanguageDropdown` into `MVC/Dropdowns/`. Once copied, they are your own code to edit.

## Views

| Base | Wraps | Override |
|---|---|---|
| `ViewBase` | - | `OnInitialize()` · `Show()` / `Hide()` |
| `PanelViewBase` | a screen or window | `OnShown()` / `OnHidden()` · `Toggle()` |
| `ButtonViewBase` | `Button` | `OnClick()` |
| `ToggleViewBase` | `Toggle` | `OnValueChanged(bool)` · `SetIsOn(bool)` |
| `SliderViewBase` | `Slider` | `OnValueChanged(float)` · `SetValue(value, max)` |
| `DropdownViewBase` | `TMP_Dropdown` | `OnValueChanged(int)` · `SetOptions(list, selected)` |
| `TextViewBase` | `TMP_Text` | `SetText(string)` |

Every view is initialized exactly once, even while it is hidden. `OnInitialize` is where components are
cached and listeners are added. The `Set...` methods change a control without calling its
`OnValueChanged`.

## Setup

### Requirements

- Unity 2021.3 LTS or newer
- uGUI and TextMeshPro (`com.unity.ugui`), included by default

Importing UniMVC never breaks your project. A small setup script checks for these, leaves UniMVC out of
compilation while one is missing, and offers to install it (**Tools > UniMVC > Check Dependencies**).

### Installation

Copy the repository into your project as your MVC folder, e.g. `Assets/Scripts/MVC/`. Your own
`Panels/`, `Buttons/`... folders then sit next to `_Bases/`.

It can also be added through the Package Manager (`+ > Add package from git URL`,
`https://github.com/fatihgezerx/UniMVC.git`). Your views then go to `Assets/Scripts/MVC/`.

## Quick Start

**1. Add a `UIManager` to your Canvas** (or to a parent of every canvas).

**2. Initialize it once** from your own bootstrap code, after the systems your views use:

```csharp
[SerializeField] private UIManager uiManager;

private void Awake() => uiManager.Initialize();
```

**3. Create views** with **Create > Scripting > MVC > Panel View / Button View / ...** in the folder
they belong to, and add them to the matching UI objects:

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
