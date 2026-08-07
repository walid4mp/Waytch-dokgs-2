# 🎮 Neon Cipher

> An **original** open-world action-adventure game inspired by modern urban-hacker
> gameplay. Built in Unity (URP, C#). Original IP, original mechanics, original city.

> ⚠️ This project is 100% original. It is **not** affiliated with, derived from,
> or based on Watch Dogs®, Watch Dogs 2®, Watch Dogs: Legion®, Ubisoft®, or any
> related trademark. All names, characters, factions, mechanics, story, and
> assets are created for this project.

| Field | Value |
|---|---|
| Engine | **Unity 2022.3 LTS + URP** |
| Language | **C# 9.0** |
| Platforms | Android 10+, Windows, macOS, Linux |
| Architecture | Clean Architecture + SOLID + Modular ECS-ish layout |
| Status | Scaffold (0.1.0) — playable vertical slice compiles |
| Genre | Open-world action-adventure / urban-hacker |
| License | MIT (assets are CC0 / royalty-free placeholders) |

---

## ✨ Features (vertical slice)

- ✅ Original story set in **Lumen Bay**, a fictional neon megacity
- ✅ Original protagonist **Kade Mercer** ("Cipher")
- ✅ Third-person controller (walk / run / jump / crouch / climb)
- ✅ Drivable vehicle system (cars, bikes, drones)
- ✅ NPC daily-routine AI (work → shop → home → sleep)
- ✅ Police AI (patrol → suspect → chase → arrest)
- ✅ Dynamic traffic system (cars, buses, bikes, traffic lights)
- ✅ Mission system (linear + side objectives + dynamic events)
- ✅ Inventory (weapons, gadgets, consumables, crafting mats)
- ✅ **LinkDeck** phone interface (in-game, original)
- ✅ **Original** hacking mechanics — all named distinctly:
  - 📷 *Eye Hack* — security cameras
  - 🔐 *Cypher Lock* — electronic locks & doors
  - 🚦 *Signal Override* — traffic lights & bridges
  - 🛸 *Swarm Hack* — public drones & streetlights
  - ⚡ *Grid Tap* — substations, alarms, vending machines
  - 🎛️ *Console Breach* — terminals, ATMs, billboards
- ✅ Save / Load (JSON + AES, slot-based)
- ✅ Day / night cycle (4 phases, 24 min real-time = 24 h in-game)
- ✅ Dynamic weather (clear, rain, fog, neon-storm, smog)
- ✅ Audio: music, SFX, ambient, vehicle — placeholder loop tracks
- ✅ Input Actions: touch (Android), gamepad, keyboard & mouse
- ✅ Localization: en, fr, ar, ja (JSON tables + runtime switcher)
- ✅ Settings: graphics quality, audio bus, controls, accessibility
- ✅ Offline mode (no network features required; optional Steam/Google
     Play hooks live behind `NEONCIPHER_NETWORKING` define)
- ✅ GitHub Actions: Unity build + test + Android release/debug
- ✅ Adaptive launcher icon + App Bundle (.aab)

---

## 🛠 Building

### Local — Unity Editor

1. Install **Unity Hub** + **Unity 2022.3.20f1 LTS**.
2. `Add project from disk` → point at this folder.
3. Open **Window → Asset Management → Localization Tables** and
   generate the string/asset tables (en, fr, ar, ja).
4. **File → Build Profiles → Android** → Switch Platform.
5. **File → Build And Run** for APK, or
6. **File → Build Settings → Player Settings → Publishing Settings → Build App Bundle**
   for AAB.

### CI — GitHub Actions (recommended for APK/AAB)

The workflow at `.github/workflows/unity-build.yml` produces:
- `NeonCipher-Android-Debug-Apk`
- `NeonCipher-Android-Release-Aab`
- `NeonCipher-StandaloneWindows64`
- Test report (Unity Edit Mode + Play Mode tests)

It uses `game-ci/unity-builder@v4`, an open-source GitHub Action that
activates Unity in CI via a Personal Access Token provided as the
secret `UNITY_LICENSE` (or uses a Pro/Plus seat activation).

Builds run on push to `main` and on tag `v*` (which uploads them as
GitHub Release assets).

---

## 📁 Folder layout

```
NeonCipher/
├── Assets/                 # Unity-managed: scenes, prefabs, materials, audio
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Materials/Textures/Models
│   ├── Audio/  (managed mirror of /Audio)
│   ├── UI/
│   ├── Settings/           # URP profiles
│   ├── Localization/       # Localization tables
│   └── Plugins/Android/    # gradle template, AndroidManifest fragment
├── Source/                 # Game C# source (Clean Architecture)
│   ├── Player/ Vehicle/ NPC/ Traffic/ Mission/ Inventory/ PhoneUI/
│   ├── Hacking/ Saving/ World/ UI/ Audio/ Input/ Localization/
│   └── Building/ Networking/
├── UI/                     # Layout XML / UI Toolkit UXML
├── Audio/                  # Music/SFX source + loop info
├── Networking/             # Optional Steam/Netcode server stubs
├── Documentation/
├── .github/workflows/
├── BuildScripts/           # Linux/macOS CI builders (called by Actions)
├── ProjectSettings/        # Unity-generated (kept in repo)
└── README.md
```

---

## 🔒 Security notes (read before contributing)

- **Never** commit Unity license files, `.env`, signing keys, or
  keystore passwords. `.gitignore` already excludes them.
- The Android keystore (`Configs/release.keystore.example`) is a
  template only; generate a real one with `keytool`.
- **Secrets** (`UNITY_LICENSE`, `ANDROID_KEYSTORE_BASE64`, etc.) must
  live in GitHub → Settings → Secrets and Variables → Actions.

---

## 🧪 Tests

Edit-mode tests under `Source/.../Tests/`. Play-mode tests live under
`Assets/Tests/PlayMode/`. Run them via `Window → General → Test Runner`
locally, or `Tests` job in CI.

---

## 📜 License

Code: **MIT** — see `LICENSE`.
Placeholder visuals / audio: **CC0 / royalty-free** — see
`Documentation/Compliance/ASSETS_LIST.md`. Replace with your own
art + sound before publishing.

---

## 🤝 Contributing

1. Branch off `main` (`feat/your-feature`).
2. Follow the [Architecture guide](Documentation/Architecture/ARCHITECTURE.md).
3. Add tests for any new system.
4. Open a PR — CI must pass.
