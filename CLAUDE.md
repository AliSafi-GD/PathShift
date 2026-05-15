# PathShift — Project Guide

A Unity tower‑defense game. This file is the entry point for any developer (human or AI) joining the project.

> **New here?** Read [ONBOARDING.md](ONBOARDING.md) first — it walks through three end‑to‑end flows and shows how to add a tower or enemy without writing code. This file is the deeper reference for conventions and the refactor roadmap.

## Stack

- **Unity** (current LTS) — runtime
- **VContainer** — DI container (primary wiring)
- **DOTween** — tweening / juice
- **UI Toolkit + uGUI** — HUD and overlays
- **Input System (new)** — input handling

## Layered architecture

The code lives under `Assets/_project/Scripts/` and is split into four layers. Higher layers can depend on lower; never the other way around.

```
UI            (uGUI / UI Toolkit controllers)
Presentation  (MonoBehaviour views + adapters implementing Domain interfaces)
Core          (services, factories, registries — mostly plain C#)
Domain        (entities + interfaces, no Unity dependency where avoidable)
```

### Folder map

| Path | Purpose |
|---|---|
| `Domain/Entities/` | Plain C# entities (e.g. `Enemy`) — composition of behaviors |
| `Domain/Interfaces/` | Behavior contracts: `IHealth`, `IMovable`, `IAttacker`, `IAttackable`, `IBehavior` |
| `Domain/Map/` | Map abstractions: `IMapProvider`, `IMapFactory`, `IMapView` |
| `Core/Bootstrap/` | `GameLifetimeScope` (DI root) + `GameBootstrapper` (entry MonoBehaviour) |
| `Core/Cards/` | Card data, deck config, selection service |
| `Core/Economy/` | Wallet, currency, costs |
| `Core/Enemy/` | Enemy factory, container, view, config |
| `Core/Events/` | `GameEventBus`, event base types |
| `Core/Grid/` | Grid data, cell view, grid service, `GridPosition` |
| `Core/Pathfinding/` | A* implementation + `PathService` |
| `Core/Spawner/` | Enemy spawner interface |
| `Core/Stats/` | Lives, score, game stats |
| `Core/Tower/` | Tower model, placement, targeting, projectiles, registry |
| `Core/Wave/` | Wave + level config, `WaveService` |
| `Presentation/` | Presenters + `View/` (Unity adapters for Domain interfaces) |
| `UI/` | uGUI HUDs (Currency, Stats, Cards, Tower actions) |
| `MapEditor/` | Editor + runtime grid gizmos |

## Wiring rules

1. **DI first.** Register every service in a `LifetimeScope`/Installer. Never call `FindObjectOfType`, `GameObject.Find`, or new up a singleton inside game code.
2. **Constructor injection for plain C#.** `[Inject]` method only for `MonoBehaviour`s.
3. **Interface > concrete.** Register `As<IFoo>()`; consumers depend on `IFoo`, not `Foo`.
4. **Three communication channels — use the right one:**
   - **DI** for stable collaborator references (services calling services).
   - **`IEventBus` (GameEventBus)** for cross‑cutting notifications (e.g. `WaveCompleted`, `EnemyKilled`).
   - **C# events on interfaces** (e.g. `IWallet.BalanceChanged`) for tight 1‑to‑N view subscriptions.

## Naming conventions

- **Namespaces** mirror folders: `_project.Scripts.<Layer>.<Module>`.
- **Interfaces** prefixed `I` (`IGrid`, `ITowerPlacementService`).
- **MonoBehaviour views** end in `View` (`TowerView`, `EnemyView`).
- **Services** end in `Service` (`PathService`, `WaveService`).
- **Factories** end in `Factory`.
- **Config ScriptableObjects** end in `Config` (`TowerConfig`, `WaveConfig`).
- **No abbreviations in type names.** Prefer `Position` over `Pos`, `Manager` over `Mgr`.

## Refactor roadmap (in progress)

Status of the cleanup the team agreed on:

- [x] **Phase 0 — Hygiene** *(done)*
  - Removed dead `ServiceLocator` and `GridGizmos2`.
  - Fixed `Entitties` → `Entities` and `GridPostion` → `GridPosition` typos.
  - Stripped empty `RegisterBuildCallback` and noise comments from `GameLifetimeScope`.
- [x] **Phase 1 — Domain isolation** *(in progress)*
  - Removed `Enemy.GetBehavior<T>()` and `MainTower.GetBehavior<T>()`. Both entities now expose named properties (`Movement`, `Health`, `Attackable`, `Attacker`, `View`).
  - Extended `IMovable` with `CurrentCell` and `SetPath` so callers no longer reach into concrete `UnityMovement`.
  - Cleaned unused usings in touched files.
  - **Open question / known debt:** `IAttackable.ReceiveDamage` is currently a pass‑through to `IHealth.TakeDamage` (see `UnityAttackable`). Keeping the abstraction in case future shields/armor want to intercept damage before health. Revisit if no second consumer appears.
  - **Not done this phase:** Unit‑test project for Domain.
- [x] **Phase 2 — Bootstrap split** *(done)*
  - `GameLifetimeScope.Configure()` reduced from ~120 lines to 7 calls. Each concern lives in its own static class under `Core/Bootstrap/Modules/` (`MapModule`, `EnemyModule`, `TowerModule`, `EconomyModule`, `CardsModule`, `UIModule`, `GameModule`).
  - `GameBootstrapper` split into `MouseInputRouter` (MonoBehaviour, click routing) and `GameLoopStarter` (plain C# `IStartable + IDisposable`, starts/stops the wave loop and shows the initial path).
  - `IGameStateMachine` + `GameStateMachine` (Playing/Paused/Won/Lost) owns `Time.timeScale` transitions; `GameOverController` is now a thin listener.
  - `ISceneLoader` + `SceneLoader` wraps `SceneManager.LoadScene` for testability.
  - **Open question / known debt:** `IInputService` not introduced — `MouseInputRouter` still `new`s `TestInput` directly. Revisit when a second input source appears.
  - **Folder/namespace mismatch:** `Core/Map/MapInstaller.cs` lives in `Domain/Map/` folder. Pre-existing inconsistency, left alone to keep blast radius small.
- [x] **Phase 3 — Tower & Placement** *(done)*
  - `TowerAttackSystem` no longer owns a tower list; it ticks `IPlacedTowerRegistry.All` each frame. `TowerPlacementService` and `TowerActionsService` lost their `attackSystem` dependency.
  - Factory interfaces introduced: `ITowerFactory`, `IEnemyFactory`, `IMortarProjectileFactory` (`IProjectileFactory` already existed). Concrete `MonoBehaviour` factories now register `.As<IFoo>()` and consumers depend on the interface.
  - **Diverged from the original plan:** rejected a full Command pattern (no Undo/queueing need today). Instead introduced `PlacementResult` and `TowerActionResult` value types to replace the `bool + out cell + out failure` triplet. Same readability win without ceremony.
  - Removed dead `EnemyFactory.Setup(Transform)` method.
- [x] **Phase 4 — Presentation/View split** *(minimal pass)*
  - `Domain/Combat/Health.cs` introduced as the pure C# health model (implements `IHealth`). `UnityHealth` is now a thin scene adapter that holds the `[SerializeField] maxHealth` and forwards every call to an inner `Health`.
  - **Diverged from the original plan:** did not extract logic from `UnityMovement` (DOTween is bound to `transform`; the seam would be artificial) or `UnityMeleeAttacker` (tiny). Did not introduce `IAnimatorController` — animator scripts are already thin tween wrappers and an interface would be ceremony with no second consumer.
  - **Rule of thumb:** extract only when the pure logic earns its own unit test or shields the rest of the codebase from a heavy dependency. `Health` cleared that bar; the others didn't.
- [x] **Phase 5 — Docs & contributing guide** *(done)*
  - [ONBOARDING.md](ONBOARDING.md) at the project root: how to run the project, an in‑60‑seconds architecture summary, three end‑to‑end flows with file links (placement / wave / attack), and step‑by‑step recipes for adding a tower or an enemy.
  - This file (CLAUDE.md) now points to ONBOARDING.md from the top and stays as the conventions + roadmap reference.

## Design patterns used / planned

- **Installer** — DI module per system (planned in Phase 2).
- **Command** — placement and tower actions (planned in Phase 3).
- **State Machine** — game state, enemy state (planned in Phase 2/3).
- **Strategy** — targeting (`ITargetingPolicy` exists), movement, projectile flight.
- **Object Pool** — enemies and projectiles.
- **Factory + interface** — every spawnable.
- **Event Bus** — cross‑cutting notifications.

## What NOT to introduce

These were considered and rejected as over‑engineering for this project's size:

- Full Repository pattern over in‑memory game state.
- Strict MVC/MVP/MVVM layering on top of Unity's component model.
- CQRS / Mediator beyond the existing `IEventBus`.
- Premature generic abstractions (`IService<T>`, etc.) — wait for a third concrete use case.

## House rule

Every new pattern must answer in one sentence: *which specific pain in this codebase does it remove?* If it can't, don't add it.
