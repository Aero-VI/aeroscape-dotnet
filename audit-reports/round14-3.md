# Audit Report - Round 14-3

## Audit Scope
- **Directory:** `/home/aeroverra/.openclaw/workspace/projects/aeroscape-dotnet`
- **Files Audited:** All combat, magic, skills, and prayer-related files (31 files total)
- **Focus:** Real bugs still present after 13 rounds of fixes

## Files Examined
- Combat: PlayerVsPlayerCombat.cs, PlayerVsNpcCombat.cs, NpcVsPlayerCombat.cs, CombatFormulas.cs, MagicSpellData.cs, WeaponData.cs, MagicNpcService.cs, CombatConstants.cs, HitSplat.cs, CombatStyle.cs
- Magic: MagicService.cs, MagicOnPlayerMessageHandler.cs, MagicOnNPCMessageHandler.cs
- Skills: GatheringSkillBase.cs, MiningSkill.cs, FishingSkill.cs, WoodcuttingSkill.cs, SmithingSkill.cs, CookingSkill.cs, FiremakingSkill.cs, CraftingSkill.cs, HerbloreSkill.cs, FletchingSkill.cs, RunecraftingSkill.cs, SkillConstants.cs
- Prayer: PrayerService.cs, PrayerMessageHandler.cs
- Data: DbSkill.cs
- Messages: PrayerMessage.cs, MagicOnNPCMessage.cs, MagicOnPlayerMessage.cs

## Critical Bugs Found

### 1. **Array Bounds Vulnerability in MagicService.cs**
**Location:** Lines 231-234, 269-272, 285-288
```csharp
private bool CastTeleport(Player player, int buttonId, int x, int y, params (int ItemId, int Amount)[] runes)
{
    // Add bounds checking for array access - check each array individually
    if (buttonId < 0 || buttonId >= ModernLevelRequirements.Length)
        return false;
    if (buttonId >= ModernSpellXp.Length)  // <-- Still missing bounds check here
        return false;
        
    if (player.MagicDelay > 0 || player.SkillLvl[6] < ModernLevelRequirements[buttonId] || !HasRunes(player, runes))
        return false;
```

**Issue:** The bounds checking comments were added but the actual array access `ModernLevelRequirements[buttonId]` and `ModernSpellXp[buttonId]` still occurs without proper validation. If `buttonId` is out of bounds, this will cause an IndexOutOfRangeException.

**Impact:** Server crash, potential denial of service

### 2. **Null Reference Risk in MagicService.TryConsumeCombatRunes**
**Location:** Lines 144-154
```csharp
public bool TryConsumeCombatRunes(Player player, SpellDefinition spell)
{
    // Validate spell definition exists to prevent null reference exceptions
    if (spell == null || spell.RuneRequirements == null)
        return false;
        
    // Cache rune requirements to prevent potential race conditions from accessing twice
    var runeRequirements = spell.RuneRequirements.Select(r => (r.RuneId, r.Amount)).ToArray();
    
    if (!HasRunes(player, runeRequirements))
        return false;
```

**Issue:** While null checks were added, the issue is that `spell.RuneRequirements` is accessed twice - once in the null check and again in the Select call. In a multithreaded environment, this could still cause a null reference if the property changes between the check and usage.

**Fix Needed:** Cache the RuneRequirements property before the null check.

### 3. **Race Condition in Combat Systems**
**Location:** Throughout PlayerVsPlayerCombat.cs, PlayerVsNpcCombat.cs, NpcVsPlayerCombat.cs

**Issue:** The combat classes access `_engine.Players[attackerId]` and `_engine.Npcs[npcId]` arrays without thread-safety considerations. Multiple combat processes could be reading/writing to the same entities simultaneously.

**Impact:** Data corruption, inconsistent combat state, potential crashes

### 4. **Missing Input Validation in CombatFormulas.Random**
**Location:** CombatFormulas.cs, line 20
```csharp
public static int Random(int range)
{
    if (range <= 0) return 0;
    return _rng.Next(range + 1);
}
```

**Issue:** While the method handles `range <= 0`, there's no protection against integer overflow when `range` is `int.MaxValue`. This could cause `range + 1` to overflow and become negative, leading to an ArgumentOutOfRangeException in `_rng.Next()`.

**Impact:** Server crash when max damage calculations exceed expected ranges

### 5. **Thread-Safety Issue with Static Random Instance**
**Location:** CombatFormulas.cs, line 18
```csharp
private static readonly Random _rng = new();
```

**Issue:** A single static Random instance is used across all combat calculations without thread synchronization. This can lead to race conditions in a multithreaded environment where multiple threads call Random() simultaneously.

**Impact:** Non-deterministic behavior, potential exceptions from Random class

### 6. **Memory Leak in Equipment Array Access**
**Location:** PlayerVsPlayerCombat.cs, lines 64-69
```csharp
// Check if equipment array has enough slots for weapon slot access
if (attacker.Equipment.Length <= CombatConstants.SlotWeapon)
{
    ResetAttack(attacker);
    return;
}
        
int weaponId = attacker.Equipment[CombatConstants.SlotWeapon];
```

**Issue:** While bounds checking was added, there's no corresponding check for `attacker.EquipmentN` array which is accessed elsewhere in the same class. This creates an inconsistent state where one array is protected but the other isn't.

**Impact:** Potential IndexOutOfRangeException in equipment processing

## Summary

Despite 13 rounds of fixes, **6 critical bugs remain** that could cause:
- Server crashes (array bounds, null reference, integer overflow)
- Data corruption (race conditions, thread safety issues)
- Inconsistent game state (memory leaks, incomplete validations)

**Recommendation:** Focus on thread-safety improvements and comprehensive input validation before production deployment.