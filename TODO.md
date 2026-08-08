# TODO — Neon Cipher

## Done (Stages 1–4)
- [x] Fix all compile errors in Core/TimeWeather/Audio/Vehicle/Player/Camera/Input/Hacking/Save/NPC/UI.
- [x] Valid PlayerActions.inputactions (KBM/Gamepad/Touch).
- [x] Runtime UI: Splash, Loading, Login, Main Menu, Settings, HUD, Pause, Phone — all buttons functional.
- [x] Runtime city composer (roads, buildings, districts, park, beach, port, bridge, tunnel, lights, traffic lights).
- [x] Playable character (walk/run/jump/crouch/interact) + third-person camera.
- [x] Drivable car + motorbike (enter/exit/drive/brake/reverse, camera retarget).
- [x] First playable mission with 3 steps, beacons, rewards.
- [x] Main.unity + EditorBuildSettings + editor build menu (Win64/APK/AAB).
- [x] Fixed-GUID .meta files for deterministic linking.
- [x] Docs: README / CHANGELOG / FIX_REPORT / TODO / ROADMAP.

## Stage 5 (next)
- [ ] Open in Unity 2022.3.20f1, confirm zero console errors (Editor pass).
- [ ] Play-test: full loop Splash → Login → New Game → mission complete.
- [ ] Build Windows64 exe; Android APK (debug) + AAB (release) locally.
- [ ] Wire CI secrets (UNITY_LICENSE etc.) and confirm green workflow artifacts.
- [ ] Touch joystick UI on Android (on-screen sticks bound to UnityInputReader).
- [ ] Hack prompt + Q-to-hack interaction hooked to nearest IHackable.
- [ ] NPC civilians walking waypoints; police patrol FSM placed in city.
- [ ] Day/night sun rotation driven by WorldClock; weather visual states.
- [ ] Replace primitive placeholder art with authored assets.
- [ ] Google Play: real keystore, privacy policy, IARC, data-safety.
