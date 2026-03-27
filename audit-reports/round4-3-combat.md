# Round 4 Combat/Skills/Magic Audit Report

## Overview
Comprehensive comparison of C# implementations against Java reference files:
- Combat: PlayerVsPlayerCombat.cs vs PlayerCombat.java
- Combat: PlayerVsNpcCombat.cs vs PlayerNPCCombat.java  
- Combat: NpcVsPlayerCombat.cs vs NPCPlayerCombat.java
- Magic: MagicService.cs vs Magic.java
- Magic: MagicNpcService.cs vs MagicNPC.java
- Skills: MiningSkill.cs vs Mining.java

## Bugs

### Combat System Bugs

**PlayerVsPlayerCombat.cs:61 — Missing equipment bounds check — PlayerCombat.java:45**
The C# code retrieves weapon ID without validating the Equipment array bounds:
```csharp
int weaponId = attacker.Equipment[CombatConstants.SlotWeapon];
```
Java uses raw array access but the slot constant (3) should be validated against Equipment.Length.

**PlayerVsPlayerCombat.cs:225 — Incorrect ranged damage formula — PlayerCombat.java:171**
C# uses melee max hit for ranged damage calculation:
```csharp
int maxHit = CombatFormulas.MaxMeleeHit(
    attacker.SkillLvl[CombatConstants.SkillStrength],
    attacker.EquipmentBonus[CombatConstants.BonusStrength]);
```
Java correctly uses ranged level: `hitDamage = p.skillLvl[4] / 4` (skill[4] = ranged).

**PlayerVsNpcCombat.cs:85 — Missing magic autocasting reset condition — PlayerNPCCombat.java:85**
C# only checks staff existence but doesn't verify the specific autocast-compatible staffs. Java has explicit `magicNPC.autoCasting` checks.

**PlayerVsNpcCombat.cs:178 — Wrong ranged damage calculation for NPCs — PlayerNPCCombat.java:195**
C# incorrectly uses melee seed for ranged XP calculation:
```csharp
int xpSeedHit = rangeLevel < 15 ? 1 : rangeLevel / 4;
```
Should match Java's `hitDamage = p.skillLvl[4] / 4` before applying random.

**NpcVsPlayerCombat.cs:47 — Incorrect proximity check bounds — NPCPlayerCombat.java:20**
C# uses `-2 to 2` range but Java uses `-3 to 3`:
```csharp
if (offsetX < -2 || offsetX > 2 || offsetY < -2 || offsetY > 2)
```
Java: `!(offsetX > -3 && offsetX < 3) || !(offsetY > -3 && offsetY < 3)`

**NpcVsPlayerCombat.cs:95 — Missing crystal bow max hit calculation — PlayerVsPlayerCombat.cs:240**
Crystal bow damage should be calculated but C# uses undefined `maxHit`. Java uses `Misc.random(30)` specifically for crystal bow.

### Magic System Bugs

**MagicService.cs:89 — Missing spell ID validation in TryConsumeCombatRunes — Magic.java:530**
No validation that spell definition exists before consuming runes. Could lead to null reference exceptions.

**MagicService.cs:215 — Superheat recipe validation gap — Magic.java:895**
The iron+coal steel bar recipe check is incomplete. Java has explicit validation: `if ((itemID == 440) && hasReq(p, 453, 2))` but C# may incorrectly allow steel production with insufficient coal.

**MagicNpcService (file not found) — Missing staff rune replacement logic — MagicNPC.java:389**
No C# equivalent found for `checkStaff(ReqItems r)` method that removes rune requirements based on equipped elemental staff. This is critical for autocast functionality.

### Skills System Bugs

**MiningSkill.cs:102 — Incorrect XP formula scaling — Mining.java:71**
C# divides by 9 instead of 3:
```csharp
double xp = (_currentRock.BaseXp * miningLevel) / 9.0;
```
Java: `giveMiningXP((getXpForOre(rockid) * p.skillLvl[14]) / 3)` then `p.addSkillXP(xp / 3, 14)`, so final divisor should be 3, not 9.

**MiningSkill.cs:78 — Missing level requirement check for pickaxe — Mining.java:124**
C# `FindBestPickaxe()` checks mining level against pickaxe requirements but doesn't validate that the player actually meets the equipment requirements for the specific pickaxe.

### Critical Missing Features

**Combat - Missing special attack configurations — PlayerCombat.java:150-400**
C# has `WeaponData.PlayerVsPlayerSpecialAttacks` but significant special attack configurations are missing, particularly:
- Anger weapons (7806, 7807, 7808, 7809) with specific 60-69 damage ranges
- Godsword special attack energy costs (Bandos requires exactly 100%, not >=100%)
- Dragon claw multi-hit calculations have different variable names in C# vs Java

**Magic - Missing ancient magics AoE effects — Magic.java:570**
Java ancient spells have area-of-effect damage that hits nearby players within a 3x3 radius around the target. No C# equivalent found for this critical PvP mechanic.

**Prayer - Missing protection prayer damage reduction — Prayer.java:25**
Java has a complex `Hitter` counter system for protection prayers that reduces damage periodically. C# `ApplyPrayerProtection` in combat files implements this but may have different timing.

## Summary

Found **10 critical bugs** that would cause incorrect damage calculations, missing gameplay mechanics, and potential crashes. The most severe issues are in ranged combat formulas, magic autocast validation, and missing AoE effects for ancient spells.