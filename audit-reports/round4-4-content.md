# Content Systems Audit - Round 4

Comparing C# content systems against Java implementations for:
- Shops
- Commands  
- NPC dialogues
- Object interactions
- Clan chat
- Construction
- Save/Load
- DI wiring

## Bugs

**ShopService.cs:40** — Shop 10 pricing inconsistency — C# uses `Fill(27, 2000)` for all prices, Java has mixed prices ending with `20, 750, 400` for last 3 items — Java ref: ShopHandler.java:42

**ShopService.cs:41** — Shop 13 pricing array length mismatch — C# has 27 prices ending at `1, 1, 10`, Java has 34 prices ending at `1, 1, 10, 5000, 5000, 5000, 5000, 5000, 5000, 5000` — Java ref: ShopHandler.java:46

**CommandService.cs:missing** — Missing "enter" command — Java allows entering other players' houses with `::enter <player>`, C# has no equivalent — Java ref: Commands.java:114-150

**Program.cs:missing** — Missing DI registration for ButtonMessage handler — ButtonMessage.cs exists but no corresponding handler or registration in Program.cs — No handler class found

**ClanChatService.cs:67** — Typo in kick message — "You've been kick from the chat." should be "You've been kicked from the chat." — Grammar error
