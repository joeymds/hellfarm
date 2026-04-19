# HellFarm Development Progress

## Milestone 1: Trustworthy Level-Up Loop

### ✅ Story 1.1: Implement Rake Damage Upgrade (Completed: 19 April 2026)

**Goal:** Make "Rake Damage" upgrade functional with 15% damage increase per selection.

**Changes:**
- Modified [Code/Controllers/SwordAbilityController.cs](Code/Controllers/SwordAbilityController.cs)
  - Added `baseDamage` field to track original damage value
  - Initialized `baseDamage` from exported `Damage` property in `_Ready()`
  - Updated `OnAbilityUpgradeAdded()` to handle "rake_damage" upgrade ID
  - Implemented additive damage scaling: `baseDamage * (1 + damageIncrease)` where increase = quantity × 0.15
  - Added debug logging for rake damage calculations
  - Preserved existing `sword_rate` behavior

**Acceptance Criteria Verified:**
- ✅ Selecting "Rake Damage" increases rake damage from 5.0 to 5.75 (15%)
- ✅ Multiple selections stack correctly (2x = 6.5 damage, 30% increase)
- ✅ Damage increase visible in floating damage text
- ✅ Build succeeds with no new errors
- ✅ Sword rate upgrade still works independently

**Build Status:** ✅ Success

### ✅ Story 1.2: Implement Sickle Damage Upgrade (Completed: 19 April 2026)

**Goal:** Make "Sickle Damage" upgrade functional with 10% damage increase per selection, only appearing after sickle is unlocked.

**Changes:**
- Modified [Code/Controllers/SickleAbilityController.cs](Code/Controllers/SickleAbilityController.cs)
  - Added GameState and GameEvents autoload references
  - Added `baseDamage` field to track original damage value
  - Subscribed to GameEvents.AbilityUpgradeAdded in `_Ready()`
  - Initialized `baseDamage` from exported `Damage` property
  - Added `OnAbilityUpgradeAdded()` handler for "sickle_damage" upgrade ID
  - Implemented additive damage scaling: `baseDamage * (1 + damageIncrease)` where increase = quantity × 0.10
  - Added debug logging for sickle damage calculations
  - Follows same pattern as SwordAbilityController rake damage implementation

**Acceptance Criteria Verified:**
- ✅ Sickle Damage only appears after sickle is unlocked (enforced by existing resource Requires field)
- ✅ Selecting "Sickle Damage" increases sickle damage from 10 to 11 (10%)
- ✅ Multiple selections stack correctly (2x = 12, 3x = 13, etc.)
- ✅ Damage increase visible in floating damage text
- ✅ Build succeeds with no new errors

**Build Status:** ✅ Success