# Neon Cipher — Asset & Compliance Manifest

A list of every placeholder asset shipped with the scaffold, its origin
license, and the replacement requirement before Google Play submission.

> All assets in this scaffold are either **generated procedurally**,
> **created by the project team**, or **CC0 / royalty-free placeholders**.
> No Watch Dogs®, Ubisoft®, or third-party copyrighted art is included.

## Audio (placeholders)

| ID | Source | License | Replace before launch? |
|---|---|---|---|
| `music_title_*`     | self-composed | CC0 | Yes (full OST) |
| `music_drive_*`     | self-composed | CC0 | Yes |
| `ambient_city_*`    | self-composed | CC0 | Yes |
| `ambient_rain_*`    | self-composed | CC0 | Yes |
| `sfx_*`             | self-recorded or generated | CC0 | Recommend OEM library |
| `voice_*`           | TBD | TBD | Yes |

All IDs must be loaded via `SfxId` static class — never by string.

## Models (placeholders)

| Class | Source | License | Replace before launch? |
|---|---|---|---|
| `Player_Cipher.glb` | Meshy / procedural | CC0 | Yes |
| `NPC_Civilian_*.glb` | Mixamo (CC0) → re-skinned | CC0 | Yes |
| `Vehicle_Car_*.glb` | Quaternius starter pack | CC0 | Yes |
| `Vehicle_Bike_*.glb` | self-modelled or paid pack | MIT / CC0 | Yes |
| `Drone_Public_*.glb` | self-modelled | CC0 | Yes |
| `City_LumenBay_*` | OpenStreetMap (ODbL) tile → custom mesh | ODbL → project asset | Yes |

## Textures

| Kind | Source | License | Replace before launch? |
|---|---|---|---|
| Building diffuse | procedural | CC0 | Optional |
| Skybox | self-painted | CC0 | Optional |
| UI sprites | self-drawn | CC0 | Optional |

## Fonts

`Inter-Regular.ttf`, `NotoSansArabic-Regular.ttf`, `NotoSansJP-Regular.otf`
(Google Fonts, OFL). Bundled under `/Assets/Plugins/Fonts/`.

## Code & middleware

| Library | License |
|---|---|
| Unity 2022.3 LTS / URP | Unity Companion Licence |
| `com.unity.inputsystem` | Unity Companion Licence |
| `com.unity.localization`   | Unity Companion Licence |
| NUnit 3 | MIT |

## Compliance checklist (before Google Play)

- [ ] Replace placeholders above.
- [ ] Generate a real Android keystore (`keytool -genkey ...`).
- [ ] Switch to a Content Rating questionnaire (IARC).
- [ ] Privacy policy URL hosted (no third party tracking; game is offline).
- [ ] Add an in-game data-safety card matching the actual code.
- [ ] Age rating = T for Teen (violence, mild language, online purchases **not** yet).
- [ ] Test on Android 10, 11, 12, 13, 14, 15 devices.

## Not-affiliated statement

This project is 100 % original. It is not endorsed by, sponsored by, or
affiliated with **Watch Dogs®, Watch Dogs 2®, Watch Dogs: Legion®** or
**Ubisoft Entertainment SA**. All trademarks belong to their owners and
appear here only for declarations of non-affiliation.
