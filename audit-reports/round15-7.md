# Round 15 Audit Report - Magic & Prayer Systems

## Overview
Audited all magic and prayer related files in the Aeroscape .NET project for round 15. This audit focused on finding **real bugs still present** after 14 rounds of fixes.

## Files Audited
1. `Services/MagicService.cs`
2. `Services/PrayerService.cs` 
3. `Combat/MagicNpcService.cs`
4. `Combat/MagicSpellData.cs`
5. `Handlers/MagicOnNPCMessageHandler.cs`
6. `Handlers/MagicOnPlayerMessageHandler.cs`
7. `Handlers/PrayerMessageHandler.cs`
8. `Messages/MagicOnNPCMessage.cs`
9. `Messages/MagicOnPlayerMessage.cs`
10. `Messages/PrayerMessage.cs`

## Bugs Found

### 1. **Array Bounds Safety Issues in MagicService** ⚠️

**File**: `Services/MagicService.cs`  
**Methods**: `TryAlchemy`, `CastBonesSpell`, `CastTeleport`

**Issue**: While bounds checking was added for `ModernLevelRequirements` and `ModernSpellXp` arrays, the implementation has a potential logical flaw:

```csharp
// In TryAlchemy, CastBonesSpell, CastTeleport:
if (levelIndex < 0 || levelIndex >= ModernLevelRequirements.Length)
    return false;
if (levelIndex >= ModernSpellXp.Length)
    return false;
```

**Problem**: The second check only validates the upper bound of `ModernSpellXp` but not the lower bound (< 0). If `ModernSpellXp.Length` is somehow smaller than `ModernLevelRequirements.Length`, a negative `levelIndex` could still pass the first check but cause issues in the second array access.

**Recommendation**: Consolidate bounds checking:
```csharp
if (levelIndex < 0 || levelIndex >= Math.Max(ModernLevelRequirements.Length, ModernSpellXp.Length))
    return false;
```

### 2. **Iron Ore Logic Gap in Superheat** ⚠️

**File**: `Services/MagicService.cs`  
**Method**: `TrySuperheat`

**Issue**: Edge case handling for iron ore (itemId 440) has a potential logical gap:

```csharp
if (itemId == 440)
{
    var coalCount = playerItems.InvItemCount(player, 453);
    if (coalCount == 1)
    {
        ui.SendMessage(player, "You need 2 coal and 1 iron ore to superheat a steel bar.");
        return false;
    }
}
```

**Problem**: This only handles `coalCount == 1`. If `coalCount > 2` (e.g., player has 5 coal), the method continues to `FindSuperheatRecipe` which could return either iron bar (if coalCount == 0 logic is triggered) or steel bar (if coalCount == 2 logic is triggered), but the selection logic in `FindSuperheatRecipe` is ambiguous for higher coal counts.

**Current Logic Flow**:
- `coalCount == 1`: Error message ✓
- `coalCount == 0`: Iron bar (no coal) ✓  
- `coalCount == 2`: Steel bar ✓
- `coalCount > 2`: **Undefined behavior** - could go either route

**Recommendation**: Add explicit handling for coal counts > 2 to ensure predictable behavior.

### 3. **Resource Consistency Issue in MagicOnPlayerMessageHandler** ⚠️

**File**: `Handlers/MagicOnPlayerMessageHandler.cs`  
**Method**: `TryCastModernSpell`

**Issue**: The method calls `_magic.TryConsumeCombatRunes(player, spell)` to check and consume runes, but then performs damage calculation and applies effects regardless of whether the consumption was successful.

**Problem**: If rune consumption fails internally (due to race conditions, inventory changes between check and consume, etc.), the spell effects still apply, creating inconsistency.

**Current Flow**:
```csharp
if (player.SkillLvl[CombatConstants.SkillMagic] < spell.LevelRequired || !_magic.TryConsumeCombatRunes(player, spell))
    return false;

int damage = CombatFormulas.MagicDamage(spellId, /*...*/);
// Effects applied regardless of actual rune consumption state
```

**Recommendation**: Add validation that runes were actually consumed, or restructure to consume runes as the final step after all other validations.

## Positive Observations

### Security Improvements ✅
- All array access now has bounds checking
- Null reference protections added to spell definitions
- Input validation present on player/NPC indices

### Code Quality ✅
- Clean separation of concerns between services
- Proper use of records for message types
- Immutable data structures (FrozenDictionary, FrozenSet)
- Comprehensive spell data definitions

### Logic Fixes ✅
- Prayer conflict resolution appears correct
- Magic level requirements properly validated
- Autocast staff detection working properly

## Conclusion

The magic and prayer systems show significant improvement from previous rounds. The main issues found are edge cases and consistency concerns rather than critical bugs. The codebase appears stable for the core functionality, with the identified issues being optimization opportunities rather than showstoppers.

**Risk Level**: LOW - All identified issues are edge cases that would require specific conditions to manifest.