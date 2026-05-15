# PathShift — Project Guide

A Unity tower‑defense game. This file is the entry point for any developer (human or AI) joining the project.

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
- [ ] **Phase 1 — Domain isolation**
  - Tighten interfaces; remove Unity deps where reasonable.
  - Replace linear `Enemy.GetBehavior<T>()` with dictionary lookup.
  - Add unit‑test project for Domain.
- [ ] **Phase 2 — Bootstrap split**
  - Break `GameLifetimeScope` into per‑module `Installer`s.
  - Split `GameBootstrapper` into `MouseInputRouter` + `GameLoopStarter`.
  - Introduce `IGameStateMachine` (Playing/Paused/Won/Lost), `ISceneLoader`, `IInputService`.
- [ ] **Phase 3 — Tower & Placement**
  - Split `TowerAttackSystem` (registry vs. tick loop).
  - Unify placement under a Command pattern.
  - Put factories behind interfaces.
- [ ] **Phase 4 — Presentation/View split**
  - Extract pure logic from `UnityHealth`, `UnityMovement`, etc.
  - Animators behind `IAnimatorController`.
- [ ] **Phase 5 — Docs & contributing guide**

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
