# Neon Cipher — Game Design Document

> 100 % original IP. Not affiliated with Watch Dogs®, Watch Dogs 2®,
> Watch Dogs: Legion® or Ubisoft Entertainment SA.

## Pitch

Imagine a freelance network auditor hired by ordinary citizens of
**Lumen Bay** — a fictional neon megacity in the year 2187 — to expose
corporate corruption **without raising a weapon**. You are **Kade "Cipher"
Mercer**, a system-literate protagonist with a smartphone-tablet called
the **LinkDeck**, walking, driving, climbing and infiltrating.

## Mechanics (original)

* **Eye Hack** — subvert CCTV cameras and drone patrols for safe passage.
* **Cypher Lock** — silently defeat electronic locks on doors and vaults.
* **Signal Override** — pause / reroute traffic lights and bridge gates.
* **Swarm Hack** — commandeer public utility drones for recon.
* **Grid Tap** — temporarily silence alarms, dim a substation, or short
  a vending machine.
* **Console Breach** — break into terminals and ATMs for payout data.

These correspond to six `HackType` enum entries; see `Hacking.cs`.

## Pillars

1. **No lethal combat.** Heavy use of stealth, infiltration, puzzle hacking.
2. **Offline-first.** No mandatory login, no always-online DRM.
3. **Touch + controller parity.** Same actions, same UI flow.
4. **Short play sessions.** Mission arcs of 5-15 minutes.

## Story beats (placeholder)

| Act | Hook |
|---|---|
| I — "The Bypass" | First paid contract: reroute traffic at a warehouse while evading the Civic Guard. |
| II — "Red Door" | Cypher Lock a corporate vault; discover leaked records; expose fraud. |
| III — "Neon Storm" | A mega-corporation tries to hijack the city's grid; storm-conduit final mission. |

## Voice / tone

* Voice: cynical, witty. Think media-savvy activist, not soldier.
* Music: synthwave palette, *no* licensed samples in scaffold.
* Art direction: cyan/magenta/yellow accents on cool steel-blue base.

## Controls matrix

| Action | Touch | Gamepad | KBM |
|---|---|---|---|
| Move | virtual joystick | LS | WASD |
| Look | two-finger drag | RS | mouse delta |
| Jump | on-screen button | A / Cross | Space |
| Run  | double-touch forward | LB / L1 | Shift |
| Crouch | hold button | B / Circle | C |
| Hack | hold button | RT | Q (hold) |
| Interact | on-screen button | X / Square | E |
| Phone | tap icon | Back | Tab |
| Pause | tap icon | Start | Esc |

## Mission design budget

* 12 main missions (avg 7 steps) across three acts.
* 6 side gigs, dynamically generated from pollution/crime events.
* 3 contract factions: **Citizens**, **Watchdogs Collective**, **GridFront**.

## NPC daily schedule

| Slot | Time | Behaviour |
|---|---|---|
| Wake | 06:00-07:00 | leaves home, walks to station |
| Commute | 07:00-08:30 | bus + walk |
| Work | 09:00-12:30 | desk animation |
| Lunch | 12:30-13:30 | walks to food stall |
| Work (cont.) | 13:30-17:00 | desk animation |
| Shop | 17:00-18:00 | walks to mall |
| Home | 18:00-21:30 | interior animation |
| Sleep | 22:00-06:00 | stays inside, low poly idle |
