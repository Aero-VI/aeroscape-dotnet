# AUDIT ROUND 14 REPORT

## Files Audited
- GameEngine.cs
- WalkQueue.cs  
- DeathService.cs
- NPC.cs

## Bugs Found

### 1. GameEngine.cs - Potential Race Condition in Player Count Updates
**Line ~381-391**
```csharp
int count = 0;
lock (_playersLock) // Use dedicated lock object for thread safety
{
    for (int i = 1; i < Players.Length; i++)
    {
        var p = Players[i];
        if (p != null && p.Online)
            count++;
    }
}
PlayersInGame = count;
```
**Bug:** The `PlayersInGame` assignment happens outside the lock, creating a window where other threads could see an inconsistent state. While this specific case may not cause crashes, it violates thread safety principles.

**Fix:** Move the assignment inside the lock block.

### 2. WalkQueue.cs - Static Logger Reference Issue
**Line ~14 and ~126**
```csharp
private readonly ILogger<WalkQueue>? _logger;
// ...
_logger?.LogWarning(ex, "Network write failed for player {PlayerId}", player.PlayerId);
```
**Bug:** The `Write` method is static but tries to access the instance field `_logger`. This will not compile or will throw runtime errors if accessed from static context.

**Fix:** Either make the `Write` method non-static and call it as an instance method, or pass the logger as a parameter.

### 3. DeathService.cs - Potential Memory Leak in LoadedObjects
**Line ~220-231**
```csharp
lock (engine.LoadedObjects)
{
    var gravestoneExists = engine.LoadedObjects.Exists(o => 
        o.ObjectId == 12719 && o.X == player.gsX && o.Y == player.gsY);
        
    if (!gravestoneExists)
    {
        engine.LoadedObjects.Add(new LoadedObject(12719, player.gsX, player.gsY, 0, 10));
    }
}
```
**Bug:** Gravestones are added to LoadedObjects but there's no cleanup mechanism visible. Over time, this could lead to memory accumulation as gravestones are never removed from the collection.

**Impact:** Memory leak that could cause server degradation over extended gameplay.

### 4. NPC.cs - Logic Error in Follow Counter Management
**Line ~244-260**
```csharp
if (!player.AttackingNPC && FollowCounter < 4)
{
    FollowCounter++;
}
else if (player.AttackingNPC)
{
    // Reset counter when player is actively attacking
    // For owned/summoned NPCs, don't increment counter to prevent abandonment
    if (IsSummoned || Owner != null)
    {
        // Owned NPCs maintain following until explicitly dismissed
        FollowCounter = Math.Max(0, FollowCounter - 1);
    }
    else
    {
        // Wild NPCs reset counter when player attacks
        FollowCounter = 0;
    }
}
```
**Bug:** The condition structure creates a logical gap where `FollowCounter >= 4` AND `!player.AttackingNPC` results in no action being taken. This could leave NPCs in a stuck state where they neither increment the counter nor reset it.

**Impact:** NPCs could get stuck in an undefined behavior state, potentially leading to gameplay issues.

## Summary
4 real bugs still present after 13 rounds of fixes. Focus areas:
1. Thread safety consistency 
2. Static/instance method access conflicts
3. Memory leak prevention
4. State machine completeness