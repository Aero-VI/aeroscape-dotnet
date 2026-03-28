# Missing Skills Implementation Plan

## Priority 1: Combat Skills (Core Mechanics)
These are handled within the combat system but need XP gain verification:

### 1. Attack (Skill ID: 0)
- Already handled in combat system
- Verify XP gain on successful hits
- Check weapon attack bonuses

### 2. Strength (Skill ID: 2) 
- Already handled in combat system
- Verify XP gain based on combat style
- Check strength bonus calculations

### 3. Defence (Skill ID: 1)
- Already handled in combat system
- Verify XP gain when defensive style selected
- Check armor requirements

### 4. Hitpoints/Constitution (Skill ID: 3)
- Already handled in combat system
- Verify XP gain (1/3 of combat XP)
- Check HP restoration methods

### 5. Ranged (Skill ID: 4)
- Needs implementation
- Java: Check arrow/bolt consumption
- Java: Ranged weapon attack speeds
- Java: Ranged XP calculations

## Priority 2: Popular Missing Skills

### 6. Agility (Skill ID: 16) ⭐
**Java Implementation Found:**
- Obstacles: Rope swing (2282), Log (2294), Net (20211), Wall slide (2302)
- XP values: 250 (log), 400 (net), 200 (wall)
- Animations: 844 (log), 3063 (net), 756 (wall)
- Timers for each obstacle

**C# Implementation Needed:**
- Create AgilitySkill.cs
- Add obstacle interactions in ObjectInteractionService
- Implement course tracking
- Add run energy restoration bonus

### 7. Thieving (Skill ID: 17) ⭐
**Java Implementation Found:**
- NPCs: Man (1,9), Farmer (2234), Hero (21), Paladin (20)
- Levels required: 1, 40, 65, 83
- Animation: 881 (pickpocket)
- Rewards: Coins based on NPC level

**C# Implementation Needed:**
- Create ThievingSkill.cs
- Add pickpocket option in NPCOption2Handler
- Implement stun/failure mechanics
- Add stall thieving

### 8. Slayer (Skill ID: 18) ⭐
**Java Implementation Found:**
- Master: Duradel (NPC 1599)
- Task assignment dialogue
- Dragon dungeon teleport

**C# Implementation Needed:**
- Create SlayerSkill.cs
- Add task assignment system
- Track kills per task
- Slayer-only monsters

## Priority 3: Other Skills

### 9. Farming (Skill ID: 19)
**Java References Found:**
- Seeds: 5096 (Marigold), 5283 (Apple), 5100 (Limpwurt), 5288 (Papaya)
- Patches mentioned in dialogues

**C# Implementation Needed:**
- Create FarmingSkill.cs
- Patch state management
- Growth timers
- Harvesting system

### 10. Hunter (Skill ID: 21)
**Java Implementation:**
- Net: 11259 (imp catching net)
- Implings mentioned

**C# Implementation Needed:**
- Create HunterSkill.cs
- Trap placement system
- Creature spawning
- Barehanded catching

### 11. Summoning (Skill ID: 23)
**Java Implementation:**
- Pikkupstix (NPC 6970)
- Pouches mentioned
- Cape ID: 12169/12171

**C# Implementation Needed:**
- Create SummoningSkill.cs
- Familiar system
- Pouch creation
- Special moves

### 12. Dungeoneering (Skill ID: 24)
- No Java implementation found
- Skip for now (complex system)

## Implementation Order:
1. ✅ Verify combat skills XP (Attack, Strength, Defence, HP, Ranged)
2. ⭐ Implement Agility (has full Java code)
3. ⭐ Implement Thieving (has full Java code)
4. ⭐ Implement Slayer (partial Java code)
5. Implement Farming (partial Java code)
6. Implement Hunter (minimal Java code)
7. Implement Summoning (minimal Java code)
8. Skip Dungeoneering (no Java code)