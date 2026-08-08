# Neon Cipher

Original open-world hacking-action prototype built with **Unity 2022.3 LTS + URP** and clean, modular C# (Clean Architecture + SOLID). 100% original IP — no copyrighted characters, names, worlds, or assets.

## Current Status — Playable Prototype (v0.4.0)

The project now opens directly into a working scene. On Play you get:

- **Boot flow**: Splash → Loading → offline Login → Main Menu → In-Game
- **Runtime-built UI**: Settings (graphics / volumes / vibration / subtitles / language EN-FR-AR-JA), HUD (time, objective, money, hack bar, hints), Pause menu (Resume / Save / Settings / Exit), LinkDeck phone (9 apps)
- **Playable character**: walk / run / jump / crouch / interact (E) with a third-person camera (mouse, gamepad, touch)
- **Drivable vehicles**: red car and motorbike — press E to enter/exit, drive with the same movement keys, brake with C, camera retargets automatically
- **Original Lumen Bay prototype city** built at runtime: road grid, towers, apartments, warehouse, Police HQ, hospital, mall, fuel station, residential block, industrial zone, park with trees, beach, sea, port, bridge, tunnel, streetlights, hackable traffic lights, safehouse
- **First mission "First Signal"**: 3 steps (plaza → port → safehouse) with glowing waypoint beacons and money/XP rewards
- **Original hacking system**: Eye Hack, Cypher Lock, Signal Override, Swarm Hack, Grid Tap, Console Breach — with a progress bar on the HUD
- **Save system**: JSON + AES-CBC slots; Pause → Save writes slot 1

## How to open & play

1. Clone the repo.
2. Open with **Unity 2022.3 LTS** (2022.3.20f1 recommended — see `ProjectSettings/ProjectVersion.txt`).
3. Open `Assets/Scenes/Main.unity` (menu: `Neon Cipher → Open Main Scene`).
4. Press **Play**.

### Controls
| Action | Keyboard | Gamepad |
|---|---|---|
| Move | WASD | Left stick |
| Look | Mouse | Right stick |
| Run | Left Shift | X / □ |
| Jump | Space | A / ✕ |
| Crouch / Brake | C | B / ◯ |
| Interact / Enter vehicle | E | Y / △ |
| Hack | Q | RT |
| Phone | Tab | View / Select |
| Pause | Esc | Start |

### Build
Menu: `Neon Cipher → Build → Windows64 / Android APK (Debug) / Android AAB (Release)`.
Outputs land in `Builds/`. CI workflow `.github/workflows/unity-build.yml` produces the same artifacts on push when Unity secrets are configured (`UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD`, and the `ANDROID_*` keystore secrets).

## Docs
- [`README.md`](README.md) — this file
- [`CHANGELOG.md`](CHANGELOG.md) — version history
- [`FIX_REPORT.md`](FIX_REPORT.md) — every fix, file by file
- [`TODO.md`](TODO.md) — current task list
- [`ROADMAP.md`](ROADMAP.md) — milestones
- [`Documentation/GameDesign/GDD.md`](Documentation/GameDesign/GDD.md)
- [`Documentation/Architecture/ARCHITECTURE.md`](Documentation/Architecture/ARCHITECTURE.md)

## Layout
- `Source/` — modular C#: Core, Player, Vehicle, NPC, Traffic, Mission, Inventory, PhoneUI, Hacking, Saving, World, UI, Input, Audio, Networking, Camera.
- `Assets/` — scenes, editor tools, streaming localization, Android plugins, tests.
- `ProjectSettings/`, `Packages/` — Unity configuration.
- `.github/workflows/` — CI/CD.

## Note on art
All visuals are runtime-generated **placeholder primitives** — intentionally replaceable. Nothing is copied from any existing game. Swap in authored assets per district when ready (see ROADMAP, Milestone D).
