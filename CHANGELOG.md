# Changelog

All notable changes to serginian.UI will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-02-16

### Initial Release

serginian.UI's first public release - a complete UI framework for Unity 6000.0+.

#### Added - Core Features

**Views & Windows**
- `UiView` base class with animated show/hide transitions
- `UiWindow` class with lifecycle events (OnShown, OnHide, OnDestroyed)
- Cursor management methods (`ShowCursor()`, `HideCursor()`)
- Support for `CanvasGroup`-based visibility and interactability

**View Animations**
- `ViewFadeAnimation` - fade in/out transitions
- `ViewSlideAnimation` - horizontal slide with fade
- ScriptableObject-based animation system for easy customization
- Support for custom view animations via inheritance

**Window Management**
- `UiMaster` singleton for centralized window registry
- Dynamic window loading via Unity Addressables
- Context-based window organization (`UI/{contextID}/{WindowTypeName}`)
- Methods: `GetWindow<T>()`, `GetOrCreateWindow<T>()`, `CreateWindow<T>()`

**Navigation System**
- `Coordinator` base class implementing Flow Coordinator pattern
- Optional back stack navigation with `EnableBackNavigation()`
- Navigation methods: `NavigateTo<T>()`, `NavigateBack()`
- Overlay system: `ShowOverlay<T>()`, `CloseOverlay<T>()`
- View registration system with optional setup callbacks

**Button System**
- `UiButton` component with pointer event handling
- Composable behaviour architecture via `ICustomButtonBehaviour`
- Click debouncing with `minClickInterval`
- Programmatic click support with `PerformClick()`
- Text and interactability management

**Built-in Button Behaviours**
- `ButtonColorBehaviour` - state-based color transitions
- `ButtonAnimationBehaviour` - ScriptableObject animation triggers
- `ButtonSelectableBehaviour` - selection state with `UiButtonGroup` support
- `ButtonSoundBehaviour` - audio feedback on interactions

**Button Animations**
- `ButtonPunchAnimation` - scale punch effects
- `UiButtonJumpUpAnimation` - vertical translation for selection
- Support for custom button animations via inheritance
- Separate animation types for clicks/hover and selection states

**Button Groups**
- `UiButtonGroup` ScriptableObject for mutual exclusion
- Perfect for tab bars and radio button groups
- Automatic selection management

#### Technical Details

- **Unity Version**: 6000.0+ (uses `Awaitable` async pattern)
- **Dependencies**:
  - DOTween (Demigiant) - animation engine
  - TextMeshPro - button text rendering
  - Unity Addressables - dynamic window loading
- **Architecture**: Code-first framework with inheritance-based extensibility
- **Async Support**: Full async/await integration for animations and navigation

#### Documentation

- Complete README with Quick Start guide
- API reference for all major components
- Code examples for common use cases
- Troubleshooting section for common issues

---

[1.0.0]: https://github.com/serginian/serginian.UI/releases/tag/v1.0.0
