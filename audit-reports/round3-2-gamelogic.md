## Bugs

`AeroScape.Server.Core/Items/PlayerItemsService.cs:32-72,77-118,161-172` — noted items are still treated as non-stackable in inventory add/delete/count logic, while Java treats `stackable || noted` the same; this breaks noted inventory behavior and cascades into bank/trade paths — Java ref: `legacy-java/server508/src/main/java/DavidScape/players/items/PlayerItems.java:121-171,185-227`

`AeroScape.Server.Core/Items/PlayerBankService.cs:99-125` — withdraw-as-note is only applied inside the stackable-item branch, so noteable non-stackable bank items are withdrawn unnoted; Java applies `withdrawNote && canBeNoted(itemId)` in the non-stackable branch — Java ref: `legacy-java/server508/src/main/java/DavidScape/players/items/PlayerBank.java:82-100`

`AeroScape.Server.Core/Movement/WalkQueue.cs:21-28` — walking packets are discarded outright while frozen; Java still resets/builds the walking queue and only prevents movement until the freeze ends, so queued movement survives the freeze — Java ref: `legacy-java/server508/src/main/java/DavidScape/io/packets/Walking.java:25-57`

`AeroScape.Server.Core/Services/DeathService.cs:88-91` — Retribution is still incomplete: C# only plays gfx on the dying player, but Java damages nearby players and shows gfx on each affected target — Java ref: `legacy-java/server508/src/main/java/DavidScape/players/Player.java:4455-4464`

`AeroScape.Server.Core/Services/DeathService.cs:189-206` — wilderness death drops are still wrong: Java also drops bones (`526`) and creates public ground items (`owner=""`), but C# drops no bones and tags all items with the victim username, temporarily locking loot to the dead player — Java ref: `legacy-java/server508/src/main/java/DavidScape/players/Player.java:4469-4473,4935-4969`

`AeroScape.Server.Core/Services/DeathService.cs:215-240` — non-wilderness gravestone death still deletes inventory/equipment without preserving them anywhere and never spawns the gravestone object; Java copies items into `gsItems/gsEquip`, sets the timer, and creates object `12719`, so the current C# path loses items permanently — Java ref: `legacy-java/server508/src/main/java/DavidScape/players/Player.java:4482-4516`

`AeroScape.Server.Core/Combat/PlayerVsNpcCombat.cs:350-355` — killing Elvarg during Dragon Slayer step 3 still only advances the quest stage; Java also sets `HeadTimer = 8`, gives item `11279`, and sends the completion message — Java ref: `legacy-java/server508/src/main/java/DavidScape/players/combat/PlayerNPCCombat.java:81-87`
