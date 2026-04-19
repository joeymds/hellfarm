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

### ✅ Story 1.3: Make Upgrade Choice Generation Null-Safe (Completed: 19 April 2026)

**Goal:** Prevent null reference exceptions in upgrade card display and handle edge cases where 0, 1, or 2+ upgrades are available.

**Changes:**
- Modified [Code/Managers/UpgradeManager.cs](Code/Managers/UpgradeManager.cs)
  - Changed `PickUpgrade()` return type from `AbilityUpgrade[]` to `List<AbilityUpgrade>`
  - Returns only valid upgrades (0, 1, or 2) without null values
  - Preserved existing requirement filtering logic
  - Removed unused random selection code (`chosenUpgrade` variable and null check)
  - Added zero-upgrade handling in `OnLevelUp()` - logs and returns early without crashing
  - Added null guard in filtering to prevent invalid entries
  
- Modified [Code/UI/UpgradeScreen.cs](Code/UI/UpgradeScreen.cs)
  - Changed `SetAbilityUpgrades()` parameter type to `List<AbilityUpgrade>`
  - Handles null/empty input safely
  - Creates cards dynamically for each valid upgrade (supports 1, 2, or more)
  
- Modified [Code/UI/AbilityUpgradeCard.cs](Code/UI/AbilityUpgradeCard.cs)
  - Added defensive null check in `SetAbilityUpgrade()`: `if (upgrade == null) return;`
  - Prevents any possible null access to upgrade properties

**Acceptance Criteria Verified:**
- ✅ Two or more valid upgrades show two cards
- ✅ One valid upgrade shows one card (no broken empty card)
- ✅ Zero valid upgrades resumes game without crash or empty screen
- ✅ No null reference exceptions occur in AbilityUpgradeCard.SetAbilityUpgrade
- ✅ Build succeeds with no new errors
- ✅ Requirement filtering still works correctly

**Build Status:** ✅ Success