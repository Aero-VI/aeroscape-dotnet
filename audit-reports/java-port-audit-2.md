# Java Port Audit Report 2
**Generated:** 2026-03-27  
**Scope:** GameEngine.cs + WalkQueue.cs + DeathService.cs + NPC.cs vs Java source

## Executive Summary
This audit compares the C# port of the AeroScape game engine against the original Java implementation. Overall, the port is functionally complete with good architectural improvements, but several key discrepancies exist that could affect gameplay mechanics and compatibility.

## Critical Issues

### 1. Engine.java vs GameEngine.cs

#### **MISSING: Global Minigame State Variables**
**Java has extensive global state that C# lacks:**
- `Engine.playersingame` → C# has `PlayersInGame` but different usage
- `Engine.constPlayers` → Missing entirely  
- `Engine.sR`, `Engine.pdT` (drop party timer) → Missing
- Massive static shop arrays (`shop2[]`, `shop3[]`, etc.) → Missing
- House system arrays (`houseX[]`, `houseY[]`, `houseOwners[]`) → Missing
- `Engine.itemsToDrop[]`, `Engine.itemsToDropN[]` → Missing

#### **MISSING: Drop Party System** 
Java has a complete drop party implementation with:
- `pdT` timer and countdown messages
- `partyItems[]` and `partyItemsN[]` arrays
- Complex drop positioning logic in `testPartyDrop()`
- Automatic item scattering at coordinates (3053, 3373)

**C# Impact:** Drop party events completely non-functional.

#### **MISSING: House System**
Java tracks player house ownership with:
- `houseOwners[]` string array
- `houseX[]`, `houseY[]` coordinate arrays  
- `hMoveX[]`, `hMoveY[]` movement arrays
- `saveHouses()` method for persistence

**C# Impact:** Player housing system completely absent.

#### **MAJOR: Shop System Differences**
Java has hardcoded shop inventories as static arrays:
- 16+ different shop arrays (`shop2`, `shop3`, `shop4`, etc.)
- Each with items, quantities, and prices
- Direct array access for shop operations

C# uses a service-based approach via `ShopService`, but the hardcoded shop data is missing.

#### **MISSING: Bounty Hunter Integration**
Java: `Engine.BH = new bountyHunter()` with active integration  
C#: No bounty hunter system references

### 2. Walking.java vs WalkQueue.cs

#### **MISSING: Interface Management**
Java `Walking.java` line 14: `p.frames.removeShownInterface(p);`  
Java lines 45-48: Multiple interface restoration calls:
```java
p.frames.removeShownInterface(p);
p.frames.restoreTabs(p);
p.frames.restoreInventory(p);  
p.frames.removeChatboxInterface(p);
```

C# has equivalent but different implementation using async writes.

#### **MISSING: Attack State Reset**  
Java clears more interaction flags:
```java
p.attackingPlayer = false;
p.attackingNPC = false;
```

C# clears these but also includes additional flags like `AttackPlayer = 0`, `AttackNPC = 0`.

#### **CORE DIFFERENCE: Running Detection**
Java: Running state comes from packet analysis  
C#: Running state comes from `WalkMessage.IsRunning` property

This suggests different message handling architectures.

### 3. Player.java vs DeathService.cs

#### **MISSING: Death Complexity in Player.java**
Player.java contains death logic directly in the `process()` method with:

```java
if (p.attackingPlayer) {
    Engine.playerCombat.attackPlayer(p);
}
if (p.attackingNPC) {
    Engine.playerNPCCombat.attackPlayer(p);  
}
```

**Key Missing Elements:**
- **Kill tracking arrays:** `killedBy[]` for tracking damage sources
- **Player killer detection:** `getPlayerKiller()` method with damage analysis
- **Gravestone coordinate tracking:** `gsX`, `gsY`, `gsH`, `gsItems`, `gsEquip` lists
- **Retribution prayer explosion damage** when prayer icon = 3
- **Clan Wars flag handling** for equipment drops
- **Assault mode integration** with different death rules
- **Bounty hunter area special handling**

#### **ARCHITECTURAL DIFFERENCE**
Java: Death logic embedded in Player class  
C#: Death logic extracted to dedicated DeathService (better architecture)

However, the C# version is missing several Java death mechanics entirely.

### 4. NPC.java vs NPC.cs

#### **MISSING: NPC Drop System**
Java NPC.java has `npcDied()` method (lines 40-90) that:
- Reads from `npcdrops.cfg` 
- Parses complex drop format: `npcId=item,min-max,chance:outOf;item2,min-max,chance:outOf`
- Handles multiple items per NPC with probability calculations
- Creates ground items directly

C# delegates this to `DeathService.DropNpcLoot()` but the implementation is different and may not handle the same drop format complexity.

#### **MISSING: Combat Integration**
Java NPC calls `Engine.npcPlayerCombat.attackPlayer(this)` directly in `process()`  
C# delegates combat to separate service classes but the integration points differ.

#### **MISSING: NPC Special Abilities**
Java NPC has:
- `doingRet` (retribution prayer)  
- Retribution explosion damage to nearby players
- `isPen` flag for Assault minigame integration
- Special handling for familiar teleportation

#### **DIFFERENT: Follow Logic**
Java `appendPlayerFollowing()` has more complex distance and height checks:
```java
if (n.absX > pX + 15 || n.absY > pY + 15 || n.absX < pX - 15 || n.absY < pY - 15 || 
    n.heightLevel < p.heightLevel || n.heightLevel > p.heightLevel)
```

C# simplified this logic and may behave differently at height boundaries.

## Medium Priority Issues

### Player Processing Differences  
- Java Player.java has 5000+ lines with extensive minigame integration
- C# extracts much functionality to services (better design) but loses some integration
- Java has hardcoded shop arrays per player, C# uses centralized shop service

### Timer Naming Inconsistencies
- Java: `homeTeleDelay` → C#: `HomeTeleDelay` (casing)
- Java: `followPlayer` → C#: `FollowPlayer` 
- Java: `isDead` → C#: `IsDead`

### Missing Static Helper Methods
- Java: `Engine.getIdFromName()`, `Engine.getPlayerCount()`
- C#: Implemented as instance methods with slightly different logic

## Positive Improvements in C# Port

### **Better Architecture**
1. **Service separation:** DeathService, WalkQueue, ShopService extracted from monolithic classes
2. **Dependency injection:** Better testability and modularity  
3. **Type safety:** Proper nullable handling and type checking
4. **Async network operations:** Non-blocking packet sending
5. **Thread safety:** Proper locking mechanisms added

### **Code Quality**
1. **Consistent naming:** PascalCase C# conventions
2. **Better error handling:** Try-catch blocks and validation
3. **Memory management:** Proper disposal patterns
4. **Documentation:** XML comments and clearer structure

## Compatibility Risk Assessment

### **HIGH RISK:**
- Drop party system completely missing
- House system absent  
- Different NPC drop parsing may break existing drop tables
- Retribution prayer damage missing
- Kill tracking for bounty hunter missing

### **MEDIUM RISK:**
- Shop system architecture change may affect item pricing/availability
- NPC follow behavior differences at height boundaries
- Death location handling differences for special areas

### **LOW RISK:**
- Naming convention differences
- Code organization improvements
- Better error handling

## Recommendations

### **Phase 1: Critical Functionality**
1. **Implement drop party system** - Add timer, item arrays, and drop logic
2. **Add house system foundation** - Owner tracking and coordinate arrays  
3. **Fix NPC retribution prayer** - Add explosion damage logic
4. **Implement kill tracking** - Add killedBy arrays and getPlayerKiller logic

### **Phase 2: Compatibility**  
1. **Add shop data** - Port hardcoded shop arrays or create equivalent data files
2. **Enhance NPC drops** - Ensure cfg file format 100% compatibility
3. **Add bounty hunter hooks** - Prepare integration points

### **Phase 3: Polish**
1. **Create data migration tools** - Convert Java player files to C# format
2. **Add configuration validation** - Ensure cfg files load correctly
3. **Performance testing** - Verify the service-based architecture performs equivalently

## Conclusion

The C# port demonstrates excellent software engineering with improved architecture, type safety, and maintainability. However, it's missing several core gameplay systems (drop parties, housing, retribution prayer) and has subtle behavioral differences that could affect compatibility with existing player data and content.

The missing functionality appears to be more about incomplete feature porting rather than architectural problems. The service-based design is superior to the Java monolith and should be maintained while adding the missing features.

**Estimated completion:** 85% functionally equivalent, 15% missing critical features.