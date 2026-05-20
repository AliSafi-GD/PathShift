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
- [ ] **Phase 6 — Grid system rewrite** *(in progress)*
  - New folder `Core/GridSystem/` built side‑by‑side with old `Core/Grid/`. Old code stays until migration is done, then deleted.
  - Design principles agreed with owner:
    - **3D logical positions** (`X`, `Y=height`, `Z`) — store rich, let algorithms project to what they need.
    - **Grid is pure logic** — no `cellSize`, no `origin`, no `WorldPosition`. View concerns moved out entirely.
    - **Three separated concerns:** Cell (what exists), Role (design‑time purpose), Occupant (runtime content). Movement rules live with the mover, not the cell.
    - **`CellRole` is polymorphic** (`NormalRole`/`SpawnRole`/`CoreRole`) so designers can add roles without touching existing code. Enum was rejected because roles will carry behavior.
    - **No `State` enum** — `IsEmpty` is derived from `Occupant == null`. Walkability is computed by pathfinders consulting `IOccupant.BlocksGround`/`BlocksAir`.
    - **`IGrid` stays minimal** — `GetCell`, `IsInside`, `AllCells`, `CellsByRole<T>`. No `GetNeighbors` — pathfinders compute neighbors themselves.
  - Files created:
    - `GridPosition.cs` — 3D struct, `Y` is height.
    - `IOccupant.cs` — `BlocksGround`, `BlocksAir`.
    - `Roles/CellRole.cs` + `NormalRole`/`SpawnRole`/`CoreRole`.
    - `GridCell.cs` — `Position`, `Role`, `Occupant`, `TryPlace`, `Clear`.
    - `IGrid.cs` + `GridManager.cs` — dictionary‑backed.
    - `Data/GridData.cs` — SO with `List<GridPosition>` per role.
  - **Not done yet:** migrate `MapInstaller`, `TowerPlacementService`, `AStarPathfinder`, `PathService` to the new grid; make `Tower`/`MainTower` implement `IOccupant`; delete dead code in old `Core/Grid/` (`GridDataAsset`, `GridFactory`, `CellView`, `CellViewRegistry`, `UnityGameGridsPresenter`, `GridPresenter`).
  - **Known gotcha in old code (do not propagate):** old `GridPosition.Y` actually means world Z. New code: `Y` = height (matches Unity).

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

---

## How to advise on this codebase (SOLID + Clean Code mindset)

This project is being shaped iteratively with the owner. When discussing design, refactors, or new code, follow these principles. They are non‑negotiable defaults — only deviate when the owner explicitly asks for a shortcut.

### SOLID, applied concretely

- **S — Single Responsibility.** A class/struct has one reason to change. If a `GridCell` knows about pathfinding rules ("IsWalkableForGround"), that's a leak — push the rule to the pathfinder/mover. Cell exposes facts about itself; consumers apply their own logic.
- **O — Open/Closed.** New cell roles, new enemy types, new occupants must be addable **without modifying existing code**. If a new feature forces editing a `switch`/`if` chain in 3 places, the abstraction is wrong.
- **L — Liskov.** Subclasses must be drop‑in replacements. If `SpawnRole` throws on `AllowsOccupant`, while `NormalRole` returns `false` cleanly, the contract is broken. Prefer total functions over thrown exceptions for "this isn't allowed."
- **I — Interface Segregation.** Don't bloat interfaces with methods consumers don't need. `IGrid` shouldn't carry `GetNeighbors` if only A* uses it — let A* derive neighbors itself from `GetCell + IsInside`.
- **D — Dependency Inversion.** Already enforced by VContainer. High‑level (Tower placement) depends on `IGrid`, not `GridService`. Never `new` a service. Never `FindObjectOfType`.

### Clean Code rules that bite hardest here

1. **Names must reveal intent.** `GridPosition.Y` meaning world‑Z is a bug waiting to happen. If a 3D grid uses `Y` for height, name it that way. No abbreviations in type names.
2. **No leaking abstractions.** A domain object never exposes terms from a layer it doesn't own. `GridCell` doesn't say "ground" or "air"; an `IOccupant` says "BlocksGround/BlocksAir" because those are its own physical properties.
3. **Derive, don't store.** If a value can be computed cheaply from primary data, expose it as a getter, don't duplicate it as a field. `WorldPosition` derived from `GridPosition + layout` beats storing both.
4. **Type‑based dispatch is a smell.** `if (occupant is Tower)` or `switch (cell.Role)` repeated across files means polymorphism is missing. Replace with virtual methods on the type itself.
5. **Enum vs polymorphic type — pick by behavior, not by count.** Enum when it's a pure label and all logic lives outside (e.g., `CellState { Walkable, Blocked }`). Polymorphic class when each variant carries its own behavior or data (e.g., `CellRole` — Normal/Spawn/Core each react differently to placement). Don't pick based on "we only have 3 variants."
6. **State is often derived from data.** Before adding a `State` enum, ask: can this be computed from existing fields? `IsEmpty => Occupant == null` is better than a parallel `State` field that can desync.
7. **Store rich, consume narrow.** Domain data should be the most general form (e.g., 3D positions even if gameplay is 2D today). Algorithms project to what they need. Don't lose information at the data layer for short‑term convenience.
8. **Total over partial.** Prefer `TryPlace(occupant) → bool` over `Place(occupant)` that throws. Result types (`PlacementResult`) beat `bool + out` triplets for non‑trivial failures.

### How to discuss design with the owner

The owner prefers a **dialogue, not a monologue**. When asked about structure:

- **Push back when the user's idea improves the design.** If they spot a leak or redundancy you missed, acknowledge it directly — don't defend the prior shape. The user's intuition has been right several times during refactors.
- **Push back when they propose something worse, with reasoning.** Don't rubber‑stamp. Give the trade‑off, name the anti‑pattern if there is one (e.g., "Type‑based dispatch — replace with polymorphism").
- **Offer the senior framing.** Show what changes today, what changes when designer adds a new idea tomorrow, and which option keeps the door open.
- **Prefer the smallest abstraction that earns its keep.** Apply the house rule above. Don't introduce `IFoo<T>` because "we might need it." Introduce it when the third concrete consumer appears.
- **Default answer style:** brief tables for comparisons, code snippets for proposed shapes, a final "می‌خوای X یا Y?" so the owner picks the next step. Persian is the working language for design chat; code/identifiers stay English.

### When refactoring existing code

- **Find dead code before redesigning.** A file that "exists" doesn't mean it's wired. Check DI registration and call sites before assuming it's load‑bearing. (`GridFactory`, `CellView`, `GridDataAsset`, `GridPresenter`, `UnityGameGridsPresenter` were all dead in `Core/Grid/` despite looking active.)
- **Separate "what is" from "what's on it" from "what can pass."** This codebase repeatedly conflates these. Cell role (design), occupant (runtime content), and movement rules (consumer concern) are three distinct layers — keep them so.
- **Move view concerns out of domain.** `CellView`, world conversion, cell size — all of these belong in a `Presentation/` layer. Domain stays pure C# wherever possible.
