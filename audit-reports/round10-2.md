# Round 10 Verification Audit Report

**Files Audited:** GameEngine.cs, WalkQueue.cs, DeathService.cs, NPC.cs  
**Focus:** Real bugs still present after 9 rounds of fixes  
**Date:** 2026-03-27

## Bugs Still Present

### 1. GameEngine.cs

**BUG: Thread Safety Violation in Player Array Access**
- **Location:** Lines 328-334, ProcessGlobalTimers()
- **Issue:** The player count loop accesses `Players` array without synchronization during concurrent modifications
- **Code:**
```csharp
for (int i = 1; i < Players.Length; i++)
{
    var p = Players[i];
    if (p != null && p.Online)
        count++;
}
```
- **Impact:** Can cause race conditions when players connect/disconnect during the count
- **Severity:** High - Runtime instability

**BUG: Memory Leak in Global Timer References**  
- **Location:** Lines 341-357, ProcessGlobalTimers()
- **Issue:** ZamorakP and SaradominP player references are checked but never cleared when players disconnect
- **Code:**
```csharp
var zamorakCarrier = ZamorakP > 0 && ZamorakP < Players.Length ? Players[ZamorakP] : null;
if (zamorakCarrier == null)
{
    ZamorakFlag = false;
    ZamorakP = 0; // Only clears the ID, not the reference leak
}
```
- **Impact:** Holds onto disconnected player references
- **Severity:** Medium - Memory waste

**BUG: Inconsistent Death State Logic**
- **Location:** Lines 479-485, ProcessPlayerTick()  
- **Issue:** Dragon claw multi-hit timer can execute on dead targets
- **Code:**
```csharp
if (p.ClawTimer > 0)
{
    // ... timer logic
    var target = Players[p.AttackPlayer];
    if (target != null && !target.IsDead) // Check happens too late
    {
        target.AppendHit(p.ThirdHit, 0);
    }
}
```
- **Impact:** Damage can be applied to players who died between the timer start and execution
- **Severity:** Medium - Game logic bug

### 2. WalkQueue.cs  

**BUG: Synchronous Frame Operations Block Game Thread**
- **Location:** Lines 130-134, Write() method
- **Issue:** `.GetAwaiter().GetResult()` blocks the game loop thread
- **Code:**
```csharp
private static void Write(Player player, Action<FrameWriter> build)
{
    // ...
    w.FlushToAsync(session.GetStream(), session.CancellationToken).GetAwaiter().GetResult();
}
```
- **Impact:** Network delays can stall the entire game tick, causing lag spikes
- **Severity:** High - Performance critical

**BUG: Walking Queue Bounds Not Validated** 
- **Location:** Lines 113-122, AddStepToWalkingQueue()
- **Issue:** WQueueWritePtr can exceed array bounds if WalkingQueueSize is incorrect
- **Code:**
```csharp
if (player.WQueueWritePtr >= player.WalkingQueueSize)
{
    return; // Returns without validation, ptr still invalid
}
player.WalkingQueueX[player.WQueueWritePtr] = x; // Potential bounds violation
```
- **Impact:** Array index out of bounds exception
- **Severity:** Medium - Crash potential

### 3. DeathService.cs

**BUG: Race Condition in Gravestone Creation**
- **Location:** Lines 213-217, MoveItemsToGravestone()
- **Issue:** Multiple death events could create duplicate gravestones at same location
- **Code:**
```csharp
if (!engine.LoadedObjects.Exists(o => o.ObjectId == 12719 && o.X == player.gsX && o.Y == player.gsY))
{
    engine.LoadedObjects.Add(new LoadedObject(12719, player.gsX, player.gsY, 0, 10));
}
```
- **Impact:** Check-then-act race condition if multiple players die simultaneously at same coords
- **Severity:** Medium - Duplicate objects

**BUG: Null Reference Potential in Owner Cleanup**
- **Location:** Lines 148-149, CleanupFamiliar()  
- **Issue:** follower.IsDead assignment without null check on follower property
- **Code:**
```csharp
if (player.follower != null)
{
    player.follower.IsDead = true; // follower could be nulled by another thread
}
```
- **Impact:** NullReferenceException if follower is cleared concurrently
- **Severity:** Low - Rare race condition

### 4. NPC.cs

**BUG: Combat State Persistence After Death**
- **Location:** Lines 222-230, AppendHit()
- **Issue:** AttackingPlayer is set to false but AttackPlayer ID is not cleared  
- **Code:**
```csharp
if (CurrentHP <= 0)
{
    CurrentHP = 0;
    AttackingPlayer = false;
    IsDead = true;
    // Missing: AttackPlayer = 0;
}
```
- **Impact:** Dead NPCs retain target references, affecting cleanup logic
- **Severity:** Medium - Memory/state leak

**BUG: Follow Counter Reset Logic Flaw**
- **Location:** Lines 257-267, AppendPlayerFollowing()
- **Issue:** FollowCounter resets to 0 when Owner exists, breaking follow timeout for summoned creatures
- **Code:**
```csharp
if (!player.AttackingNPC && FollowCounter < 4 && Owner is null)
{
    FollowCounter++; // Only increments if Owner is null
}
else
{
    FollowCounter = 0; // Always resets for owned NPCs
}
```
- **Impact:** Summoned familiars never timeout from following, causing permanent attachment
- **Severity:** Medium - Summoning system bug

## Summary

**Total Bugs Found:** 8  
**High Severity:** 2 (Thread safety, Blocking I/O)  
**Medium Severity:** 5 (Game logic, Race conditions, Memory issues)  
**Low Severity:** 1 (Rare null reference)

The most critical issues are the thread safety violations in GameEngine and the blocking I/O in WalkQueue, which can cause server instability and performance problems.