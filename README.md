# serginian.UI

**The Swiss Army Knife for Unity UI Development**

serginian.UI is a comprehensive, production-ready UI framework that provides everything you need to build polished, scalable interfaces for Unity games of any size. Like a Swiss Army knife, it combines essential tools—animated views, intelligent button systems, window management, and navigation patterns into one cohesive, well-architected solution.

## Philosophy

Stop reinventing the wheel with every project. serginian.UI handles the architectural heavy lifting so you can focus on building your game's unique interface. Built on proven design patterns (Flow Coordinator, composable behaviours, ScriptableObject-based assets), the framework is both **opinionated** in structure and **flexible** in implementation—extend and customize every component to match your exact needs.

## What You Get

- **🎨 Animated Views & Windows** — Smooth transitions powered by DOTween with ScriptableObject animation assets
- **🎯 Modular Button System** — Composable behaviours for colors, sounds, animations, and selection states
- **📦 Window Management** — Dynamic loading via Addressables with context-based organization
- **🧭 Navigation Coordinator** — Flow-based screen management with back stack support
- **🔧 Fully Extensible** — Every component designed for inheritance and customization

## Who Is This For?

serginian.UI is built **for programmers**. You'll need solid knowledge of **C#** and **Unity** to work with this framework effectively. If you're comfortable with:
- Object-oriented programming and inheritance
- Unity's component system and ScriptableObjects
- Async/await patterns
- DOTween animation library

...then you're ready to build professional UI with serginian.UI.

> **Note:** This is not a visual UI builder or drag-and-drop tool. It's a code-first framework that gives you architectural structure and reusable components to accelerate development.

---

## Table of Contents

- [Requirements](#requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Core Concepts](#core-concepts)
  - [Views & Windows](#views--windows)
  - [View Animations](#view-animations)
  - [Window Management (UiMaster)](#uimaster--window-management)
  - [Screen Flow Navigation (Coordinator)](#coordinator--screen-flow-navigation)
- [Buttons](#buttons)
  - [UiButton](#uibutton)
  - [Built-in Button Behaviours](#built-in-button-behaviours)
  - [Button Animations](#button-animations-scriptableobjects)
  - [Custom Behaviours](#creating-custom-button-behaviours)

## Requirements

- **Unity** 6000.0+ (uses `Awaitable`)
- **DOTween** (Demigiant) — used for all tween-based animations
- **TextMeshPro** — used for button text
- **Addressables** — used by `UiMaster` for dynamic window loading

## Installation

Add the package to your Unity project via the Unity Package Manager using a Git URL:

1. Open **Window → Package Manager**
2. Click **+** → **Add package from git URL…**
3. Enter your repository URL, e.g.:
   ```
   https://github.com/serginian/serginian.UI.git
   ```

    Or add it directly to your `Packages/manifest.json`:

    ``` json 
    { 
        "dependencies": 
        { 
            "com.serginian.ui": "https://github.com/serginian/serginian.UI.git" 
        } 
    }
    ```

    > Make sure DOTween, TextMeshPro, and Addressables are already installed in your project.

## Quick Start

Get up and running with serginian.UI in 5 steps:

### 1. Set Up UiMaster
Create a Canvas and add a `UiMaster` component. Set the `contextID` field (e.g., `"MainMenu"`).

### 2. Create Your First Window
1. Create a new GameObject under your Canvas
2. Add a `CanvasGroup` component
3. Create a script inheriting from `UiWindow`:
   ```csharp
   public class MainMenuWindow : UiWindow
   {
       protected override void Awake()
       {
           base.Awake();
           // Your initialization
       }
   }
   ```
4. Attach this script to your GameObject

### 3. Add Window Animation
1. Right-click in Project → **Create → serginian → UI → Animation → Window Fade In-Out**
2. Assign the created animation asset to your window's `animationAsset` field

### 4. Set Up Addressables (Optional)
If using `UiMaster` for dynamic loading:
1. Make your window prefab Addressable
2. Set its address to: `UI/{contextID}/{WindowTypeName}` (e.g., `UI/MainMenu/MainMenuWindow`)

### 5. Create Interactive Buttons
1. Create a GameObject with a `RectTransform`
2. Add `UiButton` component
3. In Inspector, click **Add Behaviour** and add:
   - `ButtonColorBehaviour` (for visual feedback)
   - `ButtonAnimationBehaviour` (optional, for animations)
4. Wire up your button's `onClick` event

**Example complete setup:**
```csharp
public class MainMenuWindow : UiWindow
{
    [SerializeField] private UiButton playButton;
    
    protected override void Awake()
    {
        base.Awake();
        playButton.OnClick += OnPlayClicked;
    }
    
    private void OnPlayClicked()
    {
        Debug.Log("Play button clicked!");
    }
}
```

---

## Core Concepts


## Views & Windows

### UiView

`UiView` is the abstract base class for all UI panels. It requires a `CanvasGroup` component and manages visibility, interactability, and animated transitions.

#### Methods

| Method | Description |
|---|---|
| `ShowAsync()` | Shows the view with animation (awaitable) |
| `CloseAsync()` | Hides the view with animation (awaitable) |
| `Show()` | Fire-and-forget show |
| `Close()` | Fire-and-forget close |
| `ShowWithoutAnimation()` | Instantly visible |
| `CloseWithoutAnimation()` | Instantly hidden |

#### Setup

1. Add a `CanvasGroup` component to your UI panel GameObject
2. Create a script inheriting from `UiView`
3. Assign a **UiViewAnimation** ScriptableObject to the `animationAsset` field

#### Example

```csharp
public class InventoryPanel : UiView
{
    protected override void Awake()
    {
        base.Awake();
        // Panel starts hidden
    }
    
    public async void ToggleInventory()
    {
        if (IsVisible)
            await CloseAsync();
        else
            await ShowAsync();
    }
}
```

### UiWindow

Extends `UiView` with lifecycle events for more control over window state changes.

#### Lifecycle Events

```csharp
public event UnityAction<UiView> OnDestroyed;
public event UnityAction<UiView> OnShown;
public event UnityAction<UiView> OnHide;
``` 

These events are triggered automatically:
- `OnShown` — after `ShowAsync()` completes
- `OnHide` — after `CloseAsync()` completes  
- `OnDestroyed` — when the GameObject is destroyed

#### Basic Usage

``` csharp 
public class SettingsWindow : UiWindow 
{ 
    protected override void Awake() 
    { 
        base.Awake();
        
        // Subscribe to lifecycle events
        OnShown += HandleShown;
        OnHide += HandleHidden;
    }
    
    private void HandleShown(UiView view)
    {
        Debug.Log("Settings window is now visible");
    }
    
    private void HandleHidden(UiView view)
    {
        Debug.Log("Settings window is now hidden");
    }
}
```

#### Cursor Management

`UiWindow` provides protected methods for managing cursor visibility and lock state:

| Method | Description |
|---|---|
| `ShowCursor()` | Makes cursor visible and unlocks it (sets `CursorLockMode.None`) |
| `HideCursor()` | Hides cursor and locks it to screen center (sets `CursorLockMode.Locked`) |

**Example usage:**
```csharp
public class PauseMenuWindow : UiWindow
{
    public override async Awaitable ShowAsync()
    {
        await base.ShowAsync();
        ShowCursor(); // Show cursor when pause menu opens
    }

    public override async Awaitable CloseAsync()
    {
        await base.CloseAsync();
        HideCursor(); // Hide cursor when returning to gameplay
    }
}
```

---

## View Animations

All view animations are **ScriptableObject assets** — create them via the Asset menu and assign them to views in the Inspector.

### ViewFadeAnimation

Fades the `CanvasGroup` alpha in/out.

**Create:** *Right-click → Create → serginian → UI → Animation → Window Fade In-Out*

| Parameter | Description |
|---|---|
| `showDuration` | Fade-in duration (seconds) |
| `showEase` | DOTween ease for fade-in |
| `hideDuration` | Fade-out duration (seconds) |
| `hideEase` | DOTween ease for fade-out |

### ViewSlideAnimation

Slides the view horizontally while fading (extends `ViewFadeAnimation`).

**Create:** *Right-click → Create → serginian → UI → Animation → Window Slide In-Out*

| Parameter | Description |
|---|---|
| `slideDuration` | Slide duration (seconds) |
| `slideEase` | DOTween ease for slide |
| `direction` | `LeftToRight`, `RightToLeft`, or `None` |

### Custom View Animations

Subclass `UiViewAnimation` to create your own:
```csharp 
[CreateAssetMenu(menuName = "serginian/UI/Animation/My Animation")] 
public class MyViewAnimation : UiViewAnimation 
{ 
    public override async Awaitable ShowAsync(UiView view) 
    { 
        /* ... */ 
    } 

    public override async Awaitable HideAsync(UiView view) 
    { 
        /* ... */ 
    } 

    public override void Show(UiView view) 
    { 
        /* ... */ 
    } 

    public override void Close(UiView view) 
    { 
        /* ... */
    }
} 
``` 

---

## UiMaster — Window Management

`UiMaster` is a scene-level `MonoBehaviour` that acts as a registry and factory for `UiWindow` instances. It uses **Addressables** to load window prefabs at runtime, enabling dynamic UI loading and memory management.

### Setup

1. Add a `UiMaster` component to a root Canvas or empty GameObject in your scene
2. Set the `contextID` field (e.g., `"MainMenu"`, `"Gameplay"`, `"Settings"`)
3. Make your `UiWindow` prefabs Addressable with addresses matching the pattern:
   ```
   UI/{contextID}/{WindowTypeName}
   ```

**Example:**
- Window class: `SettingsWindow`
- Context ID: `"MainMenu"`  
- Addressable path: `UI/MainMenu/SettingsWindow`

### API

```csharp 
// Set the active UI context (usually done when changing scenes/modes)
UiMaster.SetContext("MainMenu");

// Get an already-instantiated window (returns null if not instantiated)
var settings = UiMaster.GetWindow<SettingsWindow>();

// Get or create (loads from Addressables if needed)
var settings = await UiMaster.GetOrCreateWindow<SettingsWindow>();

// Create a new window in a specific context 
var settings = await UiMaster.CreateWindow<SettingsWindow>("MainMenu");
```

### Practical Example

```csharp
public class GameManager : MonoBehaviour
{
    async void Start()
    {
        // Set context for main menu
        UiMaster.SetContext("MainMenu");
        
        // Load and show main menu
        var mainMenu = await UiMaster.GetOrCreateWindow<MainMenuWindow>();
        await mainMenu.ShowAsync();
    }
    
    public async void StartGame()
    {
        // Switch to gameplay context
        UiMaster.SetContext("Gameplay");
        
        // Load HUD
        var hud = await UiMaster.GetOrCreateWindow<GameHUD>();
        await hud.ShowAsync();
    }
}
``` 

---

## Coordinator — Screen Flow Navigation

`Coordinator` implements the **Flow Coordinator** pattern for managing screen-to-screen navigation with an optional back stack. Think of it as a navigation controller for your UI.

### Key Concepts

- **Navigation**: Transition from one view to another, optionally adding to back stack
- **Overlays**: Show views on top of current view without navigation (e.g., popups, tooltips)
- **Back Stack**: Maintains navigation history when enabled

### Basic Usage

1. Create a class inheriting from `Coordinator`
2. Register your views in `Run()` or `Start()` method
3. Use navigation methods to control flow

```csharp 
public class MainMenuCoordinator : Coordinator 
{ 
    [SerializeField] private HomeScreen homeScreen;
    [SerializeField] private SettingsScreen settingsScreen;
    [SerializeField] private CreditsScreen creditsScreen;

    public override void Run()
    {
        // Enable back navigation (adds views to stack)
        EnableBackNavigation();

        // Register all screens in this flow
        Register<HomeScreen>(homeScreen, t => {
            t.OnShowSettingsClicked += OpenSettings;
            t.OnShowCreditsClicked += ShowCreditsOverlay;
        });
        Register<SettingsScreen>(settingsScreen, t => {
            t.OnClose += GoBack;
        });
        Register<CreditsScreen>(creditsScreen);

        // Start at home screen
        _ = NavigateTo<HomeScreen>();
    }

    public void OpenSettings()
    {
        // Navigate to settings (adds to back stack)
        _ = NavigateTo<SettingsScreen>();
    }
    
    public void ShowCreditsOverlay()
    {
        // Show credits as overlay (doesn't affect back stack)
        _ = ShowOverlay<CreditsScreen>();
    }

    public void GoBack()
    {
        // Return to previous screen in stack
        _ = NavigateBack();
    }
}
```

### Navigation vs Overlays

| Feature | `NavigateTo<T>()` | `ShowOverlay<T>()` |
|---|---|---|
| Hides current view | ✅ Yes | ❌ No |
| Adds to back stack | ✅ Yes (if enabled) | ❌ No |
| Use case | Screen transitions | Popups, tooltips, modals | 

### API Reference

| Method | Description |
|---|---|
| `Register<T>(window, setup?)` | Register a view for navigation |
| `Unregister<T>()` | Remove a registered view |
| `EnableBackNavigation()` | Enable the back stack |
| `DisableBackNavigation()` | Disable the back stack |
| `NavigateTo<T>(waitForClose)` | Navigate to a registered view |
| `NavigateBack(waitForClose)` | Pop the stack and go to previous view |
| `ShowOverlay<T>()` | Show a view as an overlay (no navigation) |
| `CloseOverlay<T>()` | Close an overlay view |
| `SetCurrent<T>()` | Set current view without showing/hiding |

### Best Practices

- **Use overlays for temporary UI**: Popups, tooltips, and modals should use `ShowOverlay<T>()` instead of navigation
- **Register all views upfront**: Call `Register<T>()` for all views in `Run()` before any navigation
- **Enable back navigation selectively**: Only enable if your flow needs it (e.g., settings menus with sub-pages)
- **Async/await navigation**: Always await navigation calls when you need to perform actions after transition completes

---

## Buttons

### UiButton

`UiButton` is a `MonoBehaviour` you attach to any GameObject with a `RectTransform`. It handles pointer click, enter, and exit events and delegates to a list of **composable behaviours**.

**Inspector fields:**

| Field | Description |
|---|---|
| `isInteractable` | Whether the button accepts input |
| `minClickInterval` | Debounce interval between clicks (seconds) |
| `targetGraphic` | Graphics affected by colour changes (auto-detected if empty) |
| `targetText` | Optional TMP text component |
| `onClick` | UnityEvent fired on click |

**Code API:**
```csharp 
button.OnClick += () => Debug.Log("Clicked!"); 
button.SetText("Play"); 
button.SetInteractable(false); 
button.PerformClick(); // programmatic click

// Access a specific behaviour
var color = button.GetBehaviour<ButtonColorBehaviour>();
color.SetColors(Color.red, Color.yellow, Color.gray);
``` 

### Adding Behaviours

Behaviours are modular components that define how buttons respond to interactions. They are added in the Inspector via the custom editor.

**How to add:**
1. Select a GameObject with `UiButton` component
2. In Inspector, click **Add Behaviour** button
3. Choose from the dropdown menu
4. Configure behaviour parameters

Each behaviour implements `ICustomButtonBehaviour` and can be combined with others for complex interactions.

> **Tip**: Combine `ButtonColorBehaviour` with `ButtonAnimationBehaviour` for both visual and animated feedback!

---

## Built-in Button Behaviours

### ButtonAnimationBehaviour

Triggers `UiButtonAnimation` ScriptableObjects on hover, leave, and click events.

| Field | Description |
|---|---|
| `clickAnimation` | Animation played on click |
| `hoverAnimation` | Animation played on pointer enter |
| `leaveAnimation` | Animation played on pointer exit |

**When to use**: Add punch effects, scale animations, or custom visual feedback to button interactions.

### ButtonColorBehaviour

Tweens the colour of all target graphics based on button state.

| Field | Description |
|---|---|
| `defaultColor` | Normal state colour |
| `hoverColor` | Hover state colour |
| `inactiveColor` | Disabled state colour |
| `colorChangeDuration` | Transition duration (seconds) |

You can update colours at runtime:
```csharp 
var colorBehaviour = button.GetBehaviour();
colorBehaviour.SetColors(Color.red, Color.yellow, Color.gray);
``` 

### ButtonSelectableBehaviour

Allows a button to maintain a selected/deselected state. Works with `UiButtonGroup` for mutual exclusion (e.g., tab bars, radio buttons).

| Field | Description |
|---|---|
| `buttonGroup` | The `UiButtonGroup` asset this button belongs to |
| `selectOnClick` | Auto-select when clicked |
| `selectOnStart` | Start in selected state |
| `disableWhenSelected` | Disable interaction while selected |
| `selectionColor` | Colour override when selected |

> **Requires** `ButtonColorBehaviour` on the same button.

**Example:**
```csharp 
var selectable = button.GetBehaviour<ButtonSelectableBehaviour>(); 
selectable.OnSelected += () => Debug.Log("Selected!");
selectable.OnDeselected += () => Debug.Log("Deselected!");
selectable.SetSelected(true);
```

**Common use cases:**
- Tab navigation (only one tab selected at a time)
- Radio button groups
- Toggle states with visual feedback 

### ButtonSoundBehaviour

Plays audio clips on hover, leave, and click.

| Field | Description |
|---|---|
| `clickSound` | AudioClip for click |
| `hoverSound` | AudioClip for hover |
| `leaveSound` | AudioClip for pointer exit |
| `audioSource` | Optional source; falls back to `AudioSource.PlayClipAtPoint` at camera position |

---

## Button Animations (ScriptableObjects)

### ButtonPunchAnimation

A scale-punch effect on hover and click.

**Create:** *Right-click → Create → serginian → UI → Animation → Button Punch*

| Parameter | Description |
|---|---|
| `clickScale` / `hoverScale` / `defaultScale` | Scale factors |
| `clickDuration` / `hoverDuration` | Animation durations |
| `clickEase` / `hoverEase` | DOTween easing |

### UiButtonJumpUpAnimation

Translates the button vertically when selected/deselected.

**Create:** *Right-click → Create → serginian → UI → Animation → Button Jump Up*

| Parameter | Description |
|---|---|
| `jumpHeight` | Pixels to jump |
| `jumpDuration` / `returnDuration` | Durations |
| `jumpEase` / `returnEase` | DOTween easing |

### Custom Button Animations

Subclass `UiButtonAnimation` (for click/hover/leave) or `UiButtonSelectAnimation` (for select/deselect):

```csharp 
[CreateAssetMenu(menuName = "serginian/UI/Animation/My Button Anim")] 
public class MyButtonAnim : UiButtonAnimation 
{ 
    public override async Awaitable Click(UiButton button) 
    { 
        /* ... */ 
    } 

    public override Awaitable Enter(UiButton button) 
    {
        /* ... */ 
        return default; 
    } 

    public override Awaitable Leave(UiButton button) 
    { 
        /* ... */ 
        return default; 
    } 
}
``` 

### UiButtonGroup

A `ScriptableObject` that ensures only one button is selected at a time — perfect for tab bars and radio-style groups.

**Create:** *Right-click → Create → serginian → UI → Button Group*

Assign the same asset to the `buttonGroup` field of each `ButtonSelectableBehaviour` in the group.

### Creating Custom Button Behaviours

Implement `ICustomButtonBehaviour` and mark your class as `[Serializable]`:

```csharp
[Serializable]
public class MyCustomBehaviour : ICustomButtonBehaviour
{
    public bool ExecuteWhenDisabled => false;

    public void Initialize(UiButton button) { }
    public void OnEnabled(UiButton button) { }
    public void OnDisabled(UiButton button) { }
    public async Awaitable OnHover(UiButton button) { }
    public async Awaitable OnLeave(UiButton button) { }
    public async Awaitable OnClick(UiButton button) { }
}
```

The behaviour will automatically appear in the **Add Behaviour** dropdown in the `UiButton` Inspector.

---

## Troubleshooting

### Common Issues

**Window doesn't animate**
- Ensure `CanvasGroup` component is attached
- Check that `animationAsset` field is assigned
- Verify DOTween is imported and initialized

**UiMaster can't find window**
- Verify Addressable path matches pattern: `UI/{contextID}/{WindowTypeName}`
- Check that context ID is set correctly
- Ensure window prefab is marked as Addressable

**Button doesn't respond to clicks**
- Check `isInteractable` is set to `true`
- Verify `EventSystem` exists in scene
- Ensure button has `RectTransform` component

**Back navigation not working**
- Call `EnableBackNavigation()` in Coordinator's `Run()` method
- Use `NavigateTo<T>()` instead of manual show/hide
- Check that views are registered before navigation

---

## Credits

serginian.UI is built with:
- **DOTween** by Demigiant (http://dotween.demigiant.com/)
- **Unity** 6000.0+

