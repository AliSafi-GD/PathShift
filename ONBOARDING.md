# PathShift — Onboarding

Read this once and you should be able to find your way around the codebase. Pair it with [CLAUDE.md](CLAUDE.md), which has the conventions and refactor roadmap.

---

## 1. Run it

1. Open the project in **Unity** (current LTS).
2. Open `Assets/Scenes/SampleScene.unity`.
3. Press Play. Drag a card from the bottom bar onto a walkable tile to place a tower. Click an existing tower to sell or upgrade it.

If the scene's `[GameLifetimeScope]` GameObject shows a missing script anywhere, drag the right component back into the empty slot.

---

## 2. Architecture in 60 seconds

Four layers under `Assets/_project/Scripts/`. Higher layers depend on lower; never the reverse.

```
UI            uGUI / UI Toolkit controllers           (Currency, Stats, Cards, Tower actions)
Presentation  MonoBehaviour adapters + animators      (UnityHealth, UnityMovement, …)
Core          Services, factories, registries         (Grid, Pathfinding, Wave, Tower, …)
Domain        Plain C# entities + interfaces          (Enemy, Health, IHealth, IMovable, …)
```

**Wiring:** [VContainer](https://github.com/hadashiA/VContainer). The DI root is [`GameLifetimeScope`](Assets/_project/Scripts/Core/Bootstrap/GameLifetimeScope.cs), which delegates to one installer per concern in [`Bootstrap/Modules/`](Assets/_project/Scripts/Core/Bootstrap/Modules/). To see what a system registers, read its module.

**Communication channels** (pick the right one — see [CLAUDE.md](CLAUDE.md#wiring-rules)):
- **DI injection** — stable collaborators
- **`IEventBus`** — cross-cutting notifications (wave completed, enemy killed)
- **C# events on interfaces** — tight 1-to-N view subscriptions (`IWallet.BalanceChanged`, `IHealth.OnDied`)

---

## 3. Three end-to-end flows

Read these once. Each one threads through most of the architecture.

### Flow A — Player places a tower

1. Player drags a card → [`TowerCardView.OnEndDrag`](Assets/_project/Scripts/UI/Cards/TowerCardView.cs) selects the card and asks the committer to commit.
2. [`PlacementCommitter.TryCommitAtMouse`](Assets/_project/Scripts/Core/Tower/PlacementCommitter.cs) reads the mouse, raycasts to the world, then calls the placement service.
3. [`TowerPlacementService.TryPlaceTower`](Assets/_project/Scripts/Core/Tower/TowerPlacementService.cs) runs `Preview()` first (cell walkable? wallet affords? path not blocked?). If everything passes, it mutates: `grid.SetWalkable(false)` → `pathService.Recalculate()` → `wallet.TrySpend()` → `ITowerFactory.Create()` → `IPlacedTowerRegistry.Register()`.
4. Result comes back as a [`PlacementResult`](Assets/_project/Scripts/Core/Tower/PlacementResult.cs). The committer clears the card selection and refreshes the main path visualization.
5. From the next frame, [`TowerAttackSystem.Update`](Assets/_project/Scripts/Core/Tower/TowerAttackSystem.cs) iterates `IPlacedTowerRegistry.All` and ticks every tower.

Click an existing tower → [`MouseInputRouter`](Assets/_project/Scripts/Core/Bootstrap/MouseInputRouter.cs) routes to [`TowerActionsController.HandleWorldClick`](Assets/_project/Scripts/UI/Tower/TowerActionsController.cs) when no card is selected. Sell and upgrade go through [`TowerActionsService`](Assets/_project/Scripts/Core/Tower/TowerActionsService.cs).

### Flow B — A wave starts and an enemy walks

1. [`GameLoopStarter.Start`](Assets/_project/Scripts/Core/Bootstrap/GameLoopStarter.cs) runs once after DI is fully resolved. It shows the initial path and calls `waveService.Start()`.
2. `WaveService` reads [`LevelConfig`](Assets/_project/Scripts/Core/Wave/LevelConfig.cs) → iterates each [`WaveConfig`](Assets/_project/Scripts/Core/Wave/WaveConfig.cs) → for every `WaveSpawnGroup`, calls `IEnemySpawner.Spawn(enemyConfig)` `count` times with `interval` seconds between spawns.
3. [`EnemySpawner.Spawn`](Assets/_project/Scripts/Core/Spawner/IEnemySpawner.cs):
   - `IEnemyFactory.CreateEnemy(spawnPos, config)` instantiates the prefab and attaches `UnityMovement`, `UnityHealth`, `UnityMeleeAttacker`, `UnityAttackable`. Returns a plain-C# [`Enemy`](Assets/_project/Scripts/Domain/Entities/Enemy.cs) entity that wraps them.
   - `enemy.Movement.SetPath(currentPath)` then `enemy.Movement.Move()` kicks off [`UnityMovement`](Assets/_project/Scripts/Presentation/View/UnityMovement.cs)'s DOTween walk through cells.
   - `OnFinishedMove` → enemy attacks `MainTower.Attackable`, then damages itself by `MaxHealth` so it dies and despawns.
   - `health.OnDied` cleanup unsubscribes, removes from `EnemyContainer`, optionally awards a kill reward to the wallet, and triggers the death animation before destroying the GameObject.
4. When the last wave finishes and the alive count hits zero, `WaveService.OnAllWavesCompleted` fires → [`GameOverController`](Assets/_project/Scripts/Core/Bootstrap/GameOverController.cs) calls `stateMachine.Win()`.

### Flow C — A tower fires and an enemy takes damage

1. [`TowerAttackSystem.Update`](Assets/_project/Scripts/Core/Tower/TowerAttackSystem.cs) iterates `IPlacedTowerRegistry.All` and calls `Tower.Tick(dt, aliveEnemies)`.
2. [`Tower.Tick`](Assets/_project/Scripts/Core/Tower/Tower.cs) decrements its cooldown; when ready, asks `ITargetingPolicy` for a target and calls `weapon.Fire(position, target)`.
3. Cannon path: [`CannonWeapon.Fire`](Assets/_project/Scripts/Core/Tower/ITargetingPolicy.cs) → [`ProjectileFactory.Create`](Assets/_project/Scripts/Core/Tower/ProjectileFactory.cs) → [`ProjectileView.Update`](Assets/_project/Scripts/Core/Tower/ProjectileView.cs) homes the projectile and on hit calls `targetAttackable.ReceiveDamage(damage)`.
4. Mortar path: [`MortarWeapon.Fire`](Assets/_project/Scripts/Core/Tower/ITargetingPolicy.cs) → [`MortarProjectileFactory.Create`](Assets/_project/Scripts/Core/Tower/MortarProjectileFactory.cs) → arcs via DOTween → on explode, damages every enemy inside `splashRadius`.
5. `IAttackable.ReceiveDamage` is implemented by [`UnityAttackable`](Assets/_project/Scripts/Presentation/View/UnityAttackable.cs), which forwards to [`UnityHealth.TakeDamage`](Assets/_project/Scripts/Presentation/View/UnityHealth.cs) — itself a thin adapter over [`Domain.Combat.Health`](Assets/_project/Scripts/Domain/Combat/Health.cs).
6. When `Health.CurrentHealth` hits zero, it fires `OnDied`. `EnemySpawner`'s subscription does the cleanup described in Flow B.

---

## 4. How to add a new tower

1. **Create a `TowerConfig` SO**: `Assets/_project/Configs/...` → right-click → *Configs / Tower Config*. Set `damage`, `fireRate`, `range`, `weaponKind` (Cannon or Mortar). For mortar, fill `splashRadius`, `arcHeight`, `mortarTravelTime`. Drag a 3D model into `viewPrefab` (the actual tower mesh). Optionally drop a lower-LOD model into `previewPrefab` (shown while dragging).
2. **Create a `TowerCardData` SO**: right-click → *Configs / Tower Card*. Point it at the `TowerConfig`, set `cost`, optionally `sellRefundRate`, and add upgrade tiers under `upgradeSteps` (each step is `TowerConfig` + `Cost`).
3. **Add the card to the active `DeckConfig`** in the scene (`Configs/Decks/...`). It will appear in the card bar next play.

No code changes required. Both the placement preview and the attack path read everything from `TowerConfig` and `TowerCardData`.

---

## 5. How to add a new enemy

1. **Create an `EnemyConfig` SO**: right-click → *Configs / Enemy Config*. Set `maxHealth`, `moveSpeed`, `damage`, `attackInterval`, `killReward`. Drop your enemy prefab (with an `EnemyView` script at the root) into `prefab`.
2. **Add the enemy to a wave**: open any `WaveConfig` (under the active `LevelConfig`) and add a `WaveSpawnGroup` pointing at your new `EnemyConfig` with `count` and `interval`.

Again, no code. `EnemyFactory` auto-adds `UnityMovement`, `UnityHealth`, `UnityMeleeAttacker`, `UnityAttackable`, and the three animators to the prefab at spawn time.

---

## 6. Where things live

See the folder map in [CLAUDE.md § Layered architecture](CLAUDE.md#layered-architecture). Quick index for the most-touched files:

| Want to change… | Open |
|---|---|
| What gets DI-registered | [`Bootstrap/Modules/`](Assets/_project/Scripts/Core/Bootstrap/Modules/) (one file per concern) |
| Win/lose/pause behavior | [`Core/State/GameStateMachine.cs`](Assets/_project/Scripts/Core/State/GameStateMachine.cs) |
| The wave loop | [`Core/Wave/`](Assets/_project/Scripts/Core/Wave/) |
| Placement validation | [`Core/Tower/TowerPlacementService.cs`](Assets/_project/Scripts/Core/Tower/TowerPlacementService.cs) |
| Sell/upgrade rules | [`Core/Tower/TowerActionsService.cs`](Assets/_project/Scripts/Core/Tower/TowerActionsService.cs) |
| Targeting strategy | [`Core/Tower/ITargetingPolicy.cs`](Assets/_project/Scripts/Core/Tower/ITargetingPolicy.cs) |
| Pathfinding | [`Core/Pathfinding/`](Assets/_project/Scripts/Core/Pathfinding/) |
| Wallet rules | [`Core/Economy/Wallet.cs`](Assets/_project/Scripts/Core/Economy/Wallet.cs) |

---

## 7. Conventions you must follow

The full list is in [CLAUDE.md § Naming conventions](CLAUDE.md#naming-conventions) and [§ Wiring rules](CLAUDE.md#wiring-rules). Short version:

- Don't call `FindObjectOfType` / `GameObject.Find` / new up a singleton. Register in a module.
- Plain C# services use constructor injection; MonoBehaviours use `[Inject]` method injection.
- Consumers depend on `IFoo`, not `Foo`. Register `As<IFoo>()`.
- Namespaces mirror folders.

---

## 8. Open debt (read before refactoring)

Tracked in [CLAUDE.md § Refactor roadmap](CLAUDE.md#refactor-roadmap-in-progress). Highlights:

- `IAttackable.ReceiveDamage` is a pass-through to `IHealth.TakeDamage`. The abstraction earns its keep only if shields/armor get added — otherwise it's worth collapsing.
- `Core/Map/MapInstaller.cs` lives in the `Domain/Map/` folder. Pre-existing folder/namespace mismatch; left alone to keep change surface small.
- `MouseInputRouter` still `new`s `TestInput` directly. An `IInputService` seam is worth introducing when a second input source appears.
- No unit tests yet. `Health`, `Wallet`, `GameStateMachine` are all plain C# and would be the easy first wins.
