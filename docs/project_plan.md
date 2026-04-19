# HellFarm Project Plan

This plan turns the current project analysis into deliverable stories. The intent is to make each story small enough for a developer or AI agent to complete independently, with clear acceptance criteria and validation steps.

## Current State

HellFarm has a working prototype loop:

- Player moves around a tilemap arena.
- Enemies spawn and chase the player.
- Weapons attack automatically.
- Enemies drop experience vials.
- Experience causes level-ups.
- Level-ups show upgrade choices.
- Victory and defeat screens exist.

The highest priority is making the level-up and restart loop trustworthy. After that, the project should move into spawn/combat tuning, player-facing polish, content expansion, and repository hygiene.

## Milestone 1: Trustworthy Level-Up Loop

Goal: Every offered upgrade works, level-up UI is robust, and a restarted run begins cleanly.

### Story 1.1: Implement Rake Damage Upgrade

As a player, when I choose "Rake Damage", my rake attacks should deal more damage.

Scope:

- Update the sword/rake ability controller to respond to the `rake_damage` upgrade.
- Apply a 15 percent damage increase per upgrade quantity, matching the existing resource description.
- Keep existing `sword_rate` behavior intact.

Acceptance criteria:

- Selecting `rake_damage` increases rake hitbox damage.
- Multiple selections stack predictably.
- Damage increase is visible in floating damage text.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Controllers/SwordAbilityController.cs`
- `resources/upgrades/rake_damage.tres`

### Story 1.2: Implement Sickle Damage Upgrade

As a player, when I choose "Sickle Damage", my sickle attacks should deal more damage.

Scope:

- Update the sickle ability controller to listen for ability upgrade events.
- Apply a 10 percent damage increase per upgrade quantity, matching the existing resource description.
- Ensure the upgrade only appears after the player has unlocked the sickle.

Acceptance criteria:

- `sickle_damage` does not appear before `sickle` is selected.
- Selecting `sickle_damage` increases sickle hitbox damage.
- Multiple selections stack predictably.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Controllers/SickleAbilityController.cs`
- `resources/upgrades/sickle_damage.tres`

### Story 1.3: Make Upgrade Choice Generation Null-Safe

As a player, I should never see a broken or empty upgrade card.

Scope:

- Change upgrade choice generation so it returns only valid upgrades.
- Do not instantiate cards for null upgrades.
- Handle the case where zero upgrades are available.
- Remove unused random selection logic in `OnLevelUp` if it remains unnecessary.

Acceptance criteria:

- If two or more valid upgrades exist, two cards are shown.
- If one valid upgrade exists, one card is shown.
- If no valid upgrades exist, the game resumes without crashing or opening an empty screen.
- No null reference can occur in `AbilityUpgradeCard.SetAbilityUpgrade`.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Managers/UpgradeManager.cs`
- `Code/UI/UpgradeScreen.cs`
- `Code/UI/AbilityUpgradeCard.cs`

### Story 1.4: Reset Run State On Restart

As a player, restarting after victory or defeat should begin a fresh run.

Scope:

- Add an explicit reset method to the autoloaded game state.
- Clear player upgrades when a new run starts or when restart is pressed.
- Ensure old upgrades do not leak into the next scene.

Acceptance criteria:

- After selecting upgrades and restarting, `PlayerUpgrades` is empty at the start of the new run.
- The player starts with only the default rake ability.
- Sickle is not present unless selected in the current run.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Events/GameState.cs`
- `Code/UI/EndScreen.cs`
- `Code/Scenes/Main.cs`

### Story 1.5: Remove Gameplay Debug Output

As a developer, normal gameplay should not spam the Godot output panel.

Scope:

- Remove or gate debug prints from player damage, experience collection, and weapon timer updates.
- Keep useful diagnostics only if they are behind a clear debug flag.

Acceptance criteria:

- Normal play does not print health, XP, or timer spam.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Actors/Player.cs`
- `Code/Managers/ExperienceManager.cs`
- `Code/Controllers/SwordAbilityController.cs`

## Milestone 2: Runtime Stability And Core Feel

Goal: Fix likely runtime hazards and make common moment-to-moment interactions feel intentional.

### Story 2.1: Fix Player Death Animation Flow

As a player, dying should show a valid death response without animation errors.

Scope:

- Add a `dead` animation to the player scene or change the code to play an existing animation.
- Ensure the defeat screen still appears.
- Decide whether the player body remains visible, disappears, or freezes.

Acceptance criteria:

- Player death does not attempt to play a missing animation.
- Player can no longer move after death.
- Defeat screen appears consistently.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Actors/Player.cs`
- `scenes/game_objects/player/player.tscn`

### Story 2.2: Fix Ability Node Lifetime Management

As a developer, instantiated ability scenes should be owned by the Godot scene tree until they free themselves.

Scope:

- Remove `using var` from instantiated Godot nodes.
- Let ability animations or timers call `QueueFree`.
- Check both rake and sickle ability lifetimes.

Acceptance criteria:

- Rake swings still appear and disappear correctly.
- Sickle instances still appear and disappear correctly.
- No premature disposal of active scene nodes.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Controllers/SwordAbilityController.cs`
- `Code/Controllers/SickleAbilityController.cs`
- `Code/Abilities/SwordAbility.cs`
- `Code/Abilities/SickleAbility.cs`

### Story 2.3: Restore Experience Vial Pickup Polish

As a player, collecting XP should feel responsive and satisfying.

Scope:

- Restore or replace the commented pickup tween in `ExperienceVial`.
- Optionally add a short magnet pull to the player before collection.
- Ensure collection still emits exactly one XP event.

Acceptance criteria:

- XP vials visibly move or animate when collected.
- A vial cannot be collected multiple times.
- XP count still increments correctly.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/GameOjects/ExperienceVial.cs`
- `scenes/game_objects/experience_vial/experience_vial.tscn`

### Story 2.4: Preserve Overflow Experience On Level-Up

As a player, extra XP gained beyond the current level target should carry into the next level.

Scope:

- Change level-up logic so excess XP is retained instead of reset to zero.
- Handle the possibility that a large XP gain causes multiple level-ups.

Acceptance criteria:

- If current XP is 1/2 and the player gains 3 XP, the next level starts with overflow XP.
- Large XP gains can trigger multiple level-ups without losing XP.
- Experience bar displays correct progress after level-up.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Managers/ExperienceManager.cs`
- `Code/UI/ExperienceBar.cs`

## Milestone 3: Combat And Spawn Tuning

Goal: Make the arena survival loop more readable, fair, and replayable.

### Story 3.1: Tune Enemy Spawn Progression

As a player, enemy pressure should ramp up clearly over the run.

Scope:

- Review the current 5-minute arena timing.
- Tune spawn rate changes across difficulty levels.
- Confirm when pig enemies enter the spawn table.
- Consider a maximum active enemy count to prevent runaway density.

Acceptance criteria:

- Early game gives the player time to learn.
- Mid-game increases pressure.
- Late game feels intense but playable.
- Spawn rate never becomes negative or unreasonably low.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Managers/EnemyManager.cs`
- `Code/Managers/ArenaTimeManager.cs`
- `scenes/manager/enemy_manager.tscn`
- `scenes/manager/arena_time_manager.tscn`

### Story 3.2: Add Enemy Wave Variety

As a player, the run should not feel like the same spawn pattern for five minutes.

Scope:

- Add at least one new enemy behavior or wave pattern.
- Possible options: fast weak enemy, slow tank enemy, burst wave, directional herd, or elite variant.
- Keep new content compatible with the existing component system.

Acceptance criteria:

- At least one new enemy or wave pattern appears during the run.
- The new content has distinct speed, health, drop chance, or behavior.
- The player can visually distinguish the new threat.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Actors/`
- `Code/Managers/EnemyManager.cs`
- `scenes/game_objects/`

### Story 3.3: Add Basic Combat Telemetry For Tuning

As a developer, I need simple run metrics to tune the game.

Scope:

- Track basic run stats such as elapsed time, level reached, enemies killed, and upgrades selected.
- Keep the data in memory for now.
- Display it on the end screen or log it behind a debug flag.

Acceptance criteria:

- Victory and defeat can show at least time survived and level reached.
- Enemy kill count is tracked.
- No noisy logging during normal gameplay.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Events/GameState.cs`
- `Code/Components/DeathComponent.cs`
- `Code/UI/EndScreen.cs`
- `Code/Managers/ArenaTimeManager.cs`

## Milestone 4: Player-Facing UX

Goal: Make the game more understandable and more polished without changing the core loop.

### Story 4.1: Add Title Or Start Screen

As a player, I should enter the game through a simple start screen instead of immediately spawning into the arena.

Scope:

- Add a lightweight title screen with Start and Quit.
- Keep it fast and simple.
- Avoid adding complex settings until needed.

Acceptance criteria:

- Launching the game shows a title screen.
- Start begins a fresh run.
- Quit exits the game.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `scenes/ui/`
- `Code/UI/`
- `project.godot`

### Story 4.2: Improve Upgrade Screen Presentation

As a player, level-up choices should feel clickable and understandable.

Scope:

- Add visual states for hover, focus, and selection.
- Add upgrade icons if available.
- Make card text wrap and fit reliably.
- Support keyboard/controller focus if practical.

Acceptance criteria:

- Cards visibly respond to hover or focus.
- Cards remain readable at the project viewport size.
- Upgrade choice can be made reliably with mouse.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/UI/UpgradeScreen.cs`
- `Code/UI/AbilityUpgradeCard.cs`
- `scenes/ui/ability_upgrade_card.tscn`
- `scenes/ui/upgrade_screen.tscn`

### Story 4.3: Add Pause Screen

As a player, I should be able to pause and resume the game.

Scope:

- Add a pause input action.
- Add a pause overlay with Resume, Restart, and Quit.
- Ensure it does not conflict with level-up or end screens.

Acceptance criteria:

- Pressing pause during gameplay pauses the tree.
- Resume returns to gameplay.
- Pause cannot override upgrade selection or end screen behavior.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `project.godot`
- `Code/UI/`
- `scenes/ui/`

## Milestone 5: Content Expansion

Goal: Add enough decisions and variety to support repeated play.

### Story 5.1: Add More Upgrade Types

As a player, I want level-up choices that support different builds.

Scope:

- Add several new upgrades using the existing resource pattern.
- Good candidates: move speed, max health, pickup radius, weapon range, weapon size, weapon cooldown, healing, or extra projectiles.
- Implement each effect in the relevant controller or component.

Acceptance criteria:

- At least four new upgrades exist.
- Every new upgrade has a mechanical effect.
- Upgrade descriptions match actual behavior.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `resources/upgrades/`
- `Code/Actors/Player.cs`
- `Code/Controllers/`
- `Code/Components/`
- `Code/Managers/UpgradeManager.cs`

### Story 5.2: Add Another Weapon

As a player, I want at least one more unlockable weapon path.

Scope:

- Add a new ability scene, controller, and upgrade resource.
- Follow the existing `Ability` resource and controller pattern.
- Add at least one supporting upgrade for the weapon.

Acceptance criteria:

- New weapon can be selected from the upgrade screen.
- New weapon appears in the player ability list after selection.
- New weapon damages enemies.
- At least one upgrade modifies the new weapon.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `Code/Abilities/`
- `Code/Controllers/`
- `resources/upgrades/`
- `scenes/ability/`

## Milestone 6: Repository Hygiene And Project Maintenance

Goal: Make the repository clean, intentional, and easier to continue.

### Story 6.1: Resolve Godot 4.6 Migration State

As a developer, I need the project version and generated files to be intentional.

Scope:

- Decide whether the project is now officially Godot 4.6 and .NET 8.
- If yes, commit the project file, import metadata, and UID files together.
- If no, revert the migration changes intentionally.

Acceptance criteria:

- `HellFarm.csproj` targets the intended Godot SDK and .NET version.
- `project.godot` feature version matches the intended Godot version.
- `.import` file changes are either committed intentionally or reverted intentionally.
- `dotnet build HellFarm.csproj` succeeds.

Suggested files:

- `HellFarm.csproj`
- `project.godot`
- `*.import`
- `*.uid`

### Story 6.2: Remove Or Ignore Backup Project Files

As a developer, I do not want stale backup files cluttering the repository.

Scope:

- Review `HellFarm.csproj.old*` files.
- Delete them if they are not needed.
- If backup files may be generated again, add a narrow ignore rule.

Acceptance criteria:

- Unneeded `HellFarm.csproj.old*` files are removed from the working tree.
- `.gitignore` prevents accidental backup project files if appropriate.
- No required project file is removed.

Suggested files:

- `HellFarm.csproj.old`
- `HellFarm.csproj.old.1`
- `HellFarm.csproj.old.2`
- `HellFarm.csproj.old.3`
- `.gitignore`

### Story 6.3: Add A Basic README

As a developer or player, I need a quick explanation of how to run and understand the project.

Scope:

- Add a README with project overview, controls, Godot version, build instructions, and current status.
- Include known limitations and useful development commands.

Acceptance criteria:

- README explains how to open the project in Godot.
- README lists controls.
- README names the intended Godot and .NET versions.
- README links to this project plan.

Suggested files:

- `README.md`
- `docs/project_plan.md`

### Story 6.4: Add Manual Test Checklist

As a developer, I need a repeatable checklist for validating gameplay changes.

Scope:

- Create a manual QA checklist for the core run loop.
- Include upgrade selection, restart, victory, defeat, XP collection, and enemy spawning.
- Keep it short enough to run often.

Acceptance criteria:

- Checklist exists in `docs/`.
- Checklist covers the core loop and highest risk regressions.
- Checklist includes expected outcomes.

Suggested files:

- `docs/manual_test_checklist.md`

## Recommended Delivery Order

1. Story 1.1: Implement Rake Damage Upgrade
2. Story 1.2: Implement Sickle Damage Upgrade
3. Story 1.3: Make Upgrade Choice Generation Null-Safe
4. Story 1.4: Reset Run State On Restart
5. Story 1.5: Remove Gameplay Debug Output
6. Story 2.1: Fix Player Death Animation Flow
7. Story 2.2: Fix Ability Node Lifetime Management
8. Story 2.3: Restore Experience Vial Pickup Polish
9. Story 2.4: Preserve Overflow Experience On Level-Up
10. Story 6.1: Resolve Godot 4.6 Migration State
11. Story 6.2: Remove Or Ignore Backup Project Files
12. Story 6.3: Add A Basic README
13. Story 6.4: Add Manual Test Checklist
14. Story 3.1: Tune Enemy Spawn Progression
15. Story 3.2: Add Enemy Wave Variety
16. Story 3.3: Add Basic Combat Telemetry For Tuning
17. Story 4.1: Add Title Or Start Screen
18. Story 4.2: Improve Upgrade Screen Presentation
19. Story 4.3: Add Pause Screen
20. Story 5.1: Add More Upgrade Types
21. Story 5.2: Add Another Weapon

## Definition Of Done For Each Story

- The story acceptance criteria are met.
- The project builds with `dotnet build HellFarm.csproj`.
- A short manual playtest is performed in Godot when available.
- No unrelated files are changed.
- Any generated Godot metadata changes are intentional.
- The final response or PR notes include what changed, how it was validated, and any remaining risks.
