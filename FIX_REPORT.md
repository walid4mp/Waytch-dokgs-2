# FIX_REPORT — Neon Cipher

## 2026-08-08 — Stages 1–4: compile fixes + playable prototype foundation

### Root problems identified in prior source
| # | File | Issue | Impact |
|---|------|-------|--------|
| 1 | `Source/Core/Interfaces.cs` | Used `UnityEngine.Vector3` without `using UnityEngine;` | CS0246 (build broken) |
| 2 | `Source/Core/Interfaces.cs` | Enum `AudioBus` collided with class `AudioBus` | CS0102 |
| 3 | `Source/Core/Interfaces.cs` | UI raised `SettingsChanged` event externally | CS0070 |
| 4 | `Source/Core/Interfaces.cs` + `WorldClock.cs` | `DayPhase` declared twice | CS0101 |
| 5 | `Source/TimeWeather/WorldClock.cs` | No MonoBehaviour driver — clock never ticked | Runtime dead |
| 6 | `Source/TimeWeather/GameSettingsService.cs` | Wrong namespace (`Localization` instead of `TimeWeather`) | CS0246 in callers |
| 7 | `Source/Audio/AudioBus.cs` | Referenced undefined `AudioBusKind` | CS0246 |
| 8 | `Source/Vehicle/VehicleController.cs` | Field `rigidbody` shadowed inherited member; no drone logic | CS0108 + physics |
| 9 | `Source/Saving/MonoBehaviourSafe.cs` | Duplicate `SaveMenuController`; missing base class | CS0101 + CS0246 |
| 10 | `Source/NeonCipher.Core.asmdef` | Referenced invalid package GUID | asmdef load error |
| 11 | `Source/UI/HudController.cs` / `MainMenuController.cs` | Missing `using NeonCipher.Core;` etc. | CS0246 |
| 12 | `Source/Player/PlayerActions.inputactions` | Corrupted placeholder GUIDs (`"…-xxx"`) | Input System import failure |
| 13 | `Source/Player/PlayerController.cs` | Depended on generated `PlayerActions` class that may not exist | CS0246 |
| 14 | `Source/Hacking/Hacking.cs` | `HackingBus` did not implement `ActiveMask` from its interface | CS0535 |
| 15 | `Source/PhoneUI/PhoneController.cs` | Broken `IsOpen` boolean expression | Logic error |
| 16 | Repo root | No `.unity` scene, no `EditorBuildSettings.asset`, no `.meta` files for `Source/` | Project unopenable / scripts unlinked |
| 17 | `Source/World/WorldStreaming.cs` | References scenes that do not exist | Runtime load errors (left unused; superseded by composer) |

### Fixes applied
- **Interfaces.cs** — added `using UnityEngine;`, renamed audio enum to `AudioBusKind`, added `IGameSettings.RaiseSettingsChanged()`, single `DayPhase` source.
- **WorldClock.cs** — uses Core `DayPhase`; added `WorldClockDriver` MonoBehaviour that ticks the clock at runtime.
- **GameSettingsService.cs** — moved to `NeonCipher.TimeWeather`; guarded disk I/O.
- **AudioBus.cs** — rewritten around `AudioBusKind`, clip registry, DontDestroyOnLoad root.
- **VehicleController.cs** — `_rb` rename (no shadowing), `SpeedKmh` via `Rigidbody.velocity`, drone flight branch, public `SetKind()`.
- **MonoBehaviourSafe.cs** — deleted (duplicate referencing a non-existent base).
- **PlayerController.cs** — reads input via `UnityInputReader` (low-level Input System), adds crouch toggle, coyote-time jump, camera-relative movement, interact via `IInteractable` interface.
- **InputProvider.cs** — new `IInputProvider` + `UnityInputReader` + `FakeInputProvider` (test double).
- **ThirdPersonCameraRig.cs** — removed reference to non-existent helper; reads mouse/gamepad/touch directly; wall clipping via linecast.
- **PlayerActions.inputactions** — rewritten with valid unique GUIDs; KBM/Gamepad/Touch schemes (JSON validated).
- **Hacking.cs** — full `IHackingBus` implementation (`ActiveMask`, `IsBusy`, events), six original hackable device stubs.
- **PhoneController.cs** — fixed `IsOpen`, added `Toggle()`, extended `PhoneScreen` enum (Map/Messages/Contacts/Camera/Missions/Inventory).
- **HudController.cs / MainMenuController.cs** — added missing usings; `RaiseSettingsChanged()` pattern; runtime `Bind()` API.
- **GameStateCollector.cs / SaveMenuController.cs** — cleaned, guarded `TryGet` calls, single canonical `SaveMenuController : MonoBehaviour`.
- **NpcRoutine.cs** — added `using NeonCipher.Core;`, guarded clock access.
- **GameBootstrap.cs** — no longer loads a non-existent scene; registers services and attaches `WorldClockDriver`.
- **asmdefs** — `NeonCipher.Core` (no refs), `NeonCipher.Game` (refs Core), `NeonCipher.Editor` (Editor-only).
- **NEW** `PlayableBootstrap.cs` — single scene entry: Splash → Loading → Login → Main Menu → In-Game flow; Esc pause, Tab phone.
- **NEW** `RuntimeUiBuilder.cs` — builds Splash/Loading/Login/MainMenu/Settings/HUD/Pause/Phone entirely at runtime with UGUI (no prefabs needed). All buttons wired to real actions.
- **NEW** `GameSceneComposer.cs` — builds the ORIGINAL Lumen Bay prototype city (roads, buildings, police HQ, hospital, mall, fuel station, residential, industrial, park, beach, sea, port, bridge, tunnel, streetlights, hackable traffic lights, safehouse), player rig, car + bike with wheel colliders, and first mission with triggers.
- **NEW** `VehicleInteractable.cs` + `VehicleDriver` — press E to enter/exit car/bike; camera retargets to vehicle; brake on Crouch key.
- **NEW** `Assets/Scenes/Main.unity` (+ fixed-guid `.meta`) with `[Playable Bootstrap]` object; `EditorBuildSettings.asset` registers it as the launch scene.
- **NEW** `Assets/Editor/NeonCipherProjectSetup.cs` — menu: `Neon Cipher → Open/Generate Main Scene`; auto-registers scene in build settings.
- **NEW** `Assets/Editor/NeonCipherBuild.cs` — menu: `Neon Cipher → Build → Windows64 / Android APK (Debug) / Android AAB (Release)`.
- **NEW** 44 fixed-GUID `.meta` files so every script/asset link resolves deterministically in Unity.

### Verification performed here
- JSON validation of `PlayerActions.inputactions` — OK.
- Static review of all interfaces/impl pairing — OK.
- Remaining verification (Unity Editor compile + Play-mode smoke test) must run inside Unity 2022.3 LTS; this environment has no Unity editor.

### Known notes
- `Source/` folder sits outside `Assets/` intentionally (kept from the original layout); Unity compiles it via the asmdefs with the generated `.meta` files. If Unity flags it, move `Source/` under `Assets/` — paths in asmdefs are namespace-based, nothing else changes.
- Vehicle driving uses WheelColliders for the car/bike; a drone controller exists in code and can be spawned later.
