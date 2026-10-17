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
├── Bases/         ViewBase, PanelViewBase, PopupViewBase, ButtonViewBase..., ControllerBase
├── Editor/        Create menu, templates, Inspector tools, Setup/ (the DOTween check)
├── UIManager/     UIManager
├── Controllers/   your ControllerBase controllers
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
| `SliderViewBase` | `Slider` | `OnValueChanged(float)` · `SetValue(value, max)` · Value Animation (Trail) |
| `DropdownViewBase` | `TMP_Dropdown` | `OnValueChanged(int)` · `SetOptions(list, selected)` |
| `TextViewBase` | `TMP_Text` | `SetText(string)` |
| `ImageViewBase` | `Image` | `SetSprite(Sprite)` |

Every view is initialized exactly once, even while it is hidden. `OnInitialize` is where components are
cached and listeners are added. When you override it in a panel or popup, call `base.OnInitialize()`
first: that is where the panel initializes its child views. The `Set...` methods change a control without
calling its `OnValueChanged`.

## The UIManager and panels in the Inspector

The `UIManager`, every panel and every popup show their views in the Inspector under one header per kind
(**Panels**, **Popups**, **Buttons**, **Toggles**, **Sliders**, **Dropdowns**, **Texts**, **Images**), each with its
list below. `UIManager.Initialize()` initializes the views in its lists, and each panel then initializes
the views in its own lists, all the way down.

**Collect From Children** (under the lists) fills them from the hierarchy for you. The `UIManager` gets
the views that aren't inside any panel or popup (and its controllers), and each panel or popup gets the
views inside it. Run it on the `UIManager` and on each panel after changing the hierarchy, or edit the
lists by hand.

Editor code that builds UI into a scene (e.g. a system setting up a panel built with
[Easy UI](https://github.com/fatihgezerx/EasyUI)) does the same with `UISetup`: `EnsureUIManager(canvas)`,
`EnsureController<T>(ui)` and `ListViews(ui, root)`, which lists every view under `root` in the panel it sits
in (or the `UIManager`) without touching views listed before, and drops entries of destroyed views - all
recorded for undo.

## In the editor

With the optional **Veneer** installed (`Assets/Scripts/Veneer`; the setup script in `Editor/Setup/` sets
`HAS_VENEER` while it is there), every UniMVC component gets the studio look: an indigo header card with the
UniMVC logo or a fitting icon and a state pill. Without the kit nothing breaks: Unity's plain Inspectors with the
**Animation Settings** and view-list headers described here take over, and every editor feature works the same.

- **Panels and popups**: tab **Panel** (the front) has an *Animation* card (open and close animation, a Duration slider and
  an Ease; the last two hide while both animations are None, and a hint tells when DOTween is missing) and a *Behaviour* card
  with two chips (*Blocks gameplay*, *Rebuild layout on show*). The fields the panel class adds (e.g. the grid size of an
  inventory panel) are in an automatic **Other** card under the tabs. Tab **Views** has the **Collect from children** button
  and one list per kind of view. The pill shows *Open* / *Hidden* in Play Mode. The `CanvasGroup` is still added automatically.
- **Sliders**: a *Value animation* card (None or Trail; the trail colour, delay, duration and ease show only for Trail); the
  fields the slider class adds are in the automatic **Other** card below it.
- **UIManager**: tab **Views** (Collect from children and the lists) and tab **Controllers**; the pill counts the listed
  views and controllers.
- **Buttons, toggles, dropdowns, texts, images and controllers** get the header card (icon and subtitle by kind) and their
  own fields in the automatic **Other** card, because one editor serves every class you write.

Every Inspector above is drawn from a **default layout** that UniMVC ships (`PanelViewDefaultLayout`, `PopupViewDefaultLayout`,
`SliderViewDefaultLayout`, `UIManagerDefaultLayout`, `ControllerDefaultLayout`, `ViewDefaultLayouts`). In
`Tools > Veneer > Inspector Designer` you can **Customize** one (a copy you may rearrange, retitle or extend with tabs and
cards of your own) and **Reset** it to the system default. UniMVC's own items (state pills, the Collect button, the panel
Duration and Ease rows) can be moved or hidden there, not changed. A customized layout for a base type (e.g. `ViewBase`) is
used by every view without a layout of its own.

Selecting several objects, Undo, prefab overrides and the Create menu work as before. The tabs remember their selection for
the editor session.

## Animations

Every panel and popup has an **Animation Settings** header at the top of its Inspector: an **Open
Animation**, a **Close Animation**, a **Duration** (0.1 - 1 seconds) and an **Ease**. Every animation
also fades; **Fade In** / **Fade Out** only fade.

| | Open | Close |
|---|---|---|
| Panels | Fade In, Zoom Out (110% → 100%), Slide From Left / Right / Top / Bottom, Unfold | Fade Out, Zoom In (100% → 110%), Slide To Left / Right / Top / Bottom, Fold |
| Popups | Fade In, Pop In (from nothing), Drop In (from above), Rotate In (from -15° and 80%), Unfold, Slide Up, Flip In (from 90° around Y) | Fade Out, Pop Out, Drop Out (falls away), Rotate Out, Fold, Slide Down, Flip Out |

Slides move by the view's own width or height, so a full-screen panel slides in from off screen. Unfold
opens sideways from a thin line first, then up and down; Fold does it backwards. Try the **OutBack** ease
with Pop In and **OutBounce** with Drop In.

- The animations run on DOTween, on unscaled time, so a pause menu animates too - never in `Update` or a
  coroutine.
- The fade runs on the panel's own `CanvasGroup`, which panels require (added with the script, and to
  older panels as soon as they are inspected). While an animation plays, that `CanvasGroup` is not
  interactable: nothing can be clicked on a panel that is still arriving or already leaving.
- `OnShown()` runs when a panel starts opening and `OnHidden()` when it starts closing; the object is
  deactivated once the close animation ends. `IsOpen` is false from the moment it starts closing (unlike
  `IsVisible`), and `Toggle()` follows it: showing a closing panel turns it straight back.
- Whether a panel starts open is simply whether its object is active in the scene.

DOTween is optional. A small setup script (`Editor/Setup/`, with no dependencies of its own) sets the
`HAS_DOTWEEN` scripting define symbol while `DOTween.dll` is in the project and clears it when it isn't;
the animation code compiles only under that symbol. When DOTween is missing, a dialog says so (once per
editor session) and offers its Asset Store page. The animation settings are kept either way, so
installing DOTween later brings the animations back as they were set up.

### Slider animation

A slider view has an **Animation Settings** header too, with a **Value Animation**: *None* (the bar moves at once) or
**Trail**. With a Trail, a value set from code that **drops** (`SetValue`) does not drag the bar down: the bar falls at
once, while a lighter bar behind it (the *Trail Color*) stays where the value was, waits the *Trail Delay*, then glides
down to the bar in *Trail Duration* seconds with the *Trail Ease*. A value that rises carries the trail up with it, a
second blow during the wait restarts the glide from where the trail still is, and a new maximum (`SetValue(value, max)`
with another max) starts the trail over. It is a health bar's "how much did I lose" effect, but it works on any slider
(stamina, experience, a progress bar).

The slider makes the trail bar itself (a copy of the fill's image, in the same Fill Area, just behind the fill; nothing
to wire), and it follows the slider's direction. Like the panels' animations it runs on DOTween on unscaled time, with no
`Update`, and without DOTween the trail simply follows the bar. Leave it off for a slider the player drags: only
values set from code animate.

## Blocking gameplay

Tick **Blocks Gameplay** on a panel or popup the player works in - an inventory window, a pause menu, a
dialog - and, while it is open, `UIBlocking.IsBlocking` is true. `UIBlocking.Changed` fires with `true` when
the first such panel opens and with `false` when the last one closes, so any system can stand by meanwhile
without knowing a single panel (InteractionSystem, for one, stops detecting). A panel counts from the moment
it starts opening until it starts closing, or until it is switched off or destroyed.

```csharp
UIBlocking.Changed += blocking => playerCamera.enabled = !blocking;
```

It is off by default, so HUD panels, tooltips and prompts never block. A panel can start with it on by
overriding `BlocksGameplayByDefault` (InventorySystem's window does).

## Layout

Unity sizes a parent before its children. When layout components are nested, for example a text with
its own Content Size Fitter inside a popup with a Horizontal Layout Group and a Content Size Fitter,
content set while the panel was hidden (a new label) makes the panel open at the old size. It only fits
from the next open on.

Tick **Rebuild Layout On Show** on such a panel or popup: every time it is shown, before its animation,
it sizes its layout again, innermost first, so it opens at the right size on the first frame. The objects
to size are collected once, when the panel is initialized. If content changes while the panel is open
(e.g. a label after a language change), call `panel.RebuildLayout()` yourself. It does nothing while the
panel is hidden. Leave the option off on panels without nested layout components, since they don't need
it.

## Controllers

Views only show what they are told; **controllers** tell them. A controller (`ControllerBase`, created with
**Create > Scripting > MVC > Controller**) listens to your game - events, callbacks - and passes what
happens on to the views through `UI.Get<T>()`. Put controllers on the `UIManager`'s object and list them
under its **Controllers** header. That object is always active, so a controller keeps listening while
the panels it updates are closed. `UIManager.Initialize()` initializes every controller once, after every
view.

```csharp
public class HealthController : ControllerBase
{
    protected override void OnInitialize() => player.HealthChanged += OnHealthChanged;
    private void OnDestroy() => player.HealthChanged -= OnHealthChanged;

    private void OnHealthChanged(int health) => UI.Get<HudPanel>().SetHealth(health);
}
```

## Setup

### Requirements

- Unity 2021.3 LTS or newer
- uGUI and TextMeshPro (`com.unity.ugui`), included by default.
- Optional: [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)
  (free, from the Asset Store), for [panel and popup animations](#animations). Without it UniMVC compiles
  and works the same; panels and popups just open and close at once.

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

**3. That's it for setup:** the `UIManager` initializes itself in `Start`, and its `Start` is the last
one of the scene (`DefaultExecutionOrder(32000)`). Every `Awake` and every other `Start` has run by then,
so the systems your views use are ready first, wherever you initialize them. Each scene's `UIManager`
sets itself up when that scene loads, so multi-scene games need nothing extra.

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
