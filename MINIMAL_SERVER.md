# Minimal AeroScape Server

## Status: ✅ COMPLETE

Server has been successfully gutted to minimal viable state with only:
- Login/Logout system
- Walking/Running  
- Woodcutting skill ONLY
- Player updating (appearance, equipment)
- Inventory system (for woodcutting logs)
- Item system (for logs/axe)
- Bronze axe (1351) auto-spawn on login

## What Was Disabled

### Services (commented out in Program.cs)
- Grand Exchange (not registered)
- Banking (PlayerBankService)
- Trading (TradingService)
- Shops (ShopService - kept for dependencies)
- Prayer (PrayerService - kept for dependencies) 
- Death/Combat (DeathService - kept for dependencies)
- Magic (MagicService)
- Clan Chat (ClanChatService, persistence, save service)
- Bounty Hunter (BountyHunterService - kept for dependencies)
- Construction (ConstructionService)
- Castle Wars (CastleWarsService)
- Commands (CommandService - re-enabled for testing)

### Packet Handlers (commented out in Program.cs)
- Player trading/follow options
- NPC interactions
- Combat-related packets
- Magic packets
- Friends/ignore lists
- Private messaging
- Clan packets
- Construction packets

### Object Interactions (disabled in ObjectInteractionService.cs)
- Mining skill
- Agility skill  
- Banking objects
- Prayer altars
- Magic altars
- Bounty Hunter entrance

## Building & Running

```bash
cd /home/aeroverra/.openclaw/workspace/projects/aeroscape-dotnet
export PATH=$PATH:/home/aeroverra/.dotnet
dotnet build
cd AeroScape.Server.App
dotnet run
```

Server runs on port 43594.

## Testing
- Server starts successfully ✅
- Players spawn with bronze axe if they don't have one ✅
- Only woodcutting, walking, and basic features remain active

## Next Steps
- Test client login/walking/woodcutting
- Add features back incrementally as needed