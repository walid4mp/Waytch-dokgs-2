# CHANGELOG

## 0.4.0 — 2026-08-08
### Stage 1 — Core compile fixes
- Renamed audio enum to `AudioBusKind` (resolves class/enum collision).
- Added `using UnityEngine;` in `Interfaces.cs`.
- Added public `IGameSettings.RaiseSettingsChanged()`.
- Consolidated `DayPhase` in Core; added `WorldClockDriver` MonoBehaviour.
- Moved `GameSettingsService` to `NeonCipher.TimeWeather`.
- Rewrote `AudioBus` (clip registry, DontDestroyOnLoad root).
- Rewrote `VehicleController` (`_rb` no-shadowing, drone branch, `SetKind()`).
- Deleted duplicate `Source/Saving/MonoBehaviourSafe.cs`.
- Cleaned both game asmdefs.

### Stage 2 — Gameplay systems
- New `IInputProvider` + `UnityInputReader` (reads new Input System directly) + `FakeInputProvider` test double.
- Rewrote `PlayerController` (walk/run/crouch/jump/coyote-time/interact).
- Rewrote `ThirdPersonCameraRig` (direct device reads, wall clipping, `SnapYawTo`).
- Rewrote `Hacking.cs`: complete `IHackingBus` (`ActiveMask`, `IsBusy`, events) + six original hackables.
- Fixed `PhoneController.IsOpen`, added `Toggle()`, extended `PhoneScreen`.
- Rewrote `PlayerActions.inputactions` with valid GUIDs (KBM/Gamepad/Touch) — JSON-validated.
- Cleaned Save system (`GameStateCollector`, `SaveMenuController`).
- Fixed `NpcRoutine` (Core using + safe clock access).

### Stage 3 — Playable glue
- NEW `PlayableBootstrap` — boots services + UI + world; Esc/Tab hotkeys.
- NEW `RuntimeUiBuilder` — full runtime UI: Splash, Loading (progress bar), offline Login, Main Menu (New Game/Continue/Settings/Language/Download/Exit), Settings (quality/volume/vibration/subtitles/language — all wired), HUD (HP/wanted/hack bar/time/objective/money/hints), Pause (Resume/Save/Settings/MainMenu/Exit), LinkDeck phone (9 apps).
- NEW `GameSceneComposer` — original Lumen Bay prototype city from primitives: road grid, lane markers, towers, apartments, warehouse, Police HQ, hospital, mall, fuel station, residential block, industrial zone, park + trees, beach, sea, port, bridge, tunnel, streetlights, hackable traffic lights, safehouse.
- Player rig (capsule body, cube head, limb cylinders), third-person camera, red car + motorbike with WheelColliders.
- NEW `VehicleInteractable` + `VehicleDriver` — E to enter/exit; camera retarget; crouch = brake.
- First mission "First Signal" (3 steps: plaza → port → safehouse) with waypoint beacons + triggers, money/XP rewards.

### Stage 4 — Scene, editor tooling, packaging
- NEW `Assets/Scenes/Main.unity` (+ `.meta` fixed GUID) as the launch scene.
- NEW `ProjectSettings/EditorBuildSettings.asset` — scene registered for builds.
- NEW `Assets/Editor/NeonCipherProjectSetup.cs` — `Neon Cipher → Open/Generate Main Scene`, auto build-settings registration.
- NEW `Assets/Editor/NeonCipherBuild.cs` — `Neon Cipher → Build → Windows64 / APK (Debug) / AAB (Release)`.
- NEW `NeonCipher.Editor.asmdef` (Editor-only).
- 44 fixed-GUID `.meta` files generated for all sources/assets so links resolve deterministically.
