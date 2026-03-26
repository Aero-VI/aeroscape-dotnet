# DavidScape 508 — Complete Feature List

> **Source:** `legacy-java/server508` codebase analysis  
> **Protocol:** RuneScape 508  
> **Engine Tick:** 600ms game cycle

---

## Table of Contents

1. [Networking & Engine](#1-networking--engine)
2. [Skills](#2-skills)
3. [Combat](#3-combat)
4. [Magic System](#4-magic-system)
5. [Prayer System](#5-prayer-system)
6. [Minigames & Activities](#6-minigames--activities)
7. [Player Interaction](#7-player-interaction)
8. [Item Systems](#8-item-systems)
9. [NPC Systems](#9-npc-systems)
10. [World & Object Interactions](#10-world--object-interactions)
11. [Commands](#11-commands)
12. [Quests](#12-quests)
13. [Miscellaneous Systems](#13-miscellaneous-systems)

---

## 1. Networking & Engine

### Protocol & Login
- **RS2 508 protocol** with full packet handler (256 opcodes defined)
- **HD and LD client support** — detects client type on login and adjusts interfaces (window pane 746 for HD, 548 for LD)
- **Login handshake** with server/client session key exchange, RSA-style login encryption parsing
- **Update server keys** sent on connection type 15
- **Character save/load** via text-based flat-file system (`mainsave/`, `backup/`)
- **Automatic backup system** — saves on login if total level > 34; auto-restores from backup on detected stat reset
- **Account verification** — new accounts receive a random verification code they must enter via `::verifycode`
- **IP ban / IP mute** support with file-based ban lists (`bannedhosts/`, `bannedchars/`, `ipmute/`)
- **Connection flood protection** (`Protect.java`) — max connections per host, username pattern detection, auto-kick/ban
- **Highscores system** — top 30 players tracked by total level and total XP, saved/loaded from `highscores.txt`

### Engine Core
- **600ms game tick** engine cycle processing all players and NPCs
- **Up to 2000 concurrent players** (configurable `maxPlayers`)
- **Player update masks:** appearance, animation, GFX, hit splats (2 hits), forced chat, face-to, chat text
- **NPC update masks:** animation, GFX, hit splats (2 hits), forced text, face coordinates, face entity
- **Map region system** with dynamic region loading and map data from packed `.dat` files
- **Ground item system** — items appear to dropper first, then globally after 60 ticks, despawn after 240 ticks
- **Object spawning system** — global and local object creation/removal
- **Projectile system** — supports magic/range projectiles with configurable speed, height, and lock-on target
- **NPC spawn system** from config files (`npcspawn.cfg`, `npclist.cfg`) with configurable stats, emotes, and walk ranges
- **NPC drop system** from config file (`npcdrops.cfg`) with random drop chances, min/max amounts
- **Periodic character auto-save** every ~10 seconds via `PlayerSave` thread
- **Idle timeout** — disconnects after 5 idle ticks
- **Profanity filter** on public chat (fuck, damn, shit, etc. replaced with asterisks)
- **Chat encryption/decryption** (Huffman-style RuneScape chat encoding)

---

## 2. Skills

The server supports **25 skill slots** (indices 0–24). All skills can reach level 120+ (max XP 510,000,000). Skillcapes are awarded at level 120 with automatic trimming when the player has multiple 120s.

### Attack (Skill 0)
- Melee accuracy skill; affects hit chance in PvP and PvE
- Trained through melee combat
- Skillcape from Ajjat (NPC 4288)

### Defence (Skill 1)
- Reduces incoming damage; required for equipping armour tiers
- Trained through melee combat (defensive style)
- Skillcape from Melee Tutor (NPC 7950)

### Strength (Skill 2)
- Determines melee max hit
- Trained through melee combat (aggressive style)
- Skillcape from Sloane (NPC 4297)

### Hitpoints (Skill 3)
- Health pool; starts at level 10 (1154 XP)
- Trained passively through all combat
- Skillcape from Surgeon General Tafani (NPC 961)

### Ranged (Skill 4)
- Ranged combat skill
- Supports shortbows, longbows, crossbows, darts, knives, thrown axes, javelins
- Ammo types: Bronze through Rune arrows, bolt racks
- Equipment tiers: Leather, studded, green/blue/red/black d'hide, Karil's, 3rd age
- Skillcape from Armour Salesman (NPC 682)
- Supplies from Range Tutor (NPC 1861) — bows + "go fletch arrows"

### Prayer (Skill 5)
- **27 prayers** supported with full drain rate system
- Bone burying: Regular, burnt, bat, wolf, monkey, big, shaikahan, jogre, burnt jogre, baby dragon, dragon, zogre, fayrg, raurg, ourg, dagannoth, wyvern bones
- Each bone type has a unique XP value scaled by prayer level
- Altar restoration at monasteries, GWD altars, and various objects
- Skillcape from Brother Jered (NPC 802)

### Magic (Skill 6)
- Full Modern, Ancient, and Lunar spellbooks (see [Magic System](#4-magic-system))
- Autocasting support via Slayer's staff / powered staves
- Skillcape from Robe Store Owner (NPC 1658)

### Cooking (Skill 7)
- **Cookable fish:** Shrimps (Lv1), Trout (Lv15), Bass (Lv30), Lobster (Lv50), Shark (Lv79), Manta Ray (Lv90)
- Supports cooking on **ranges** (object 58124) and **fires** (object 28173)
- Burn chance mechanic based on cooking level
- Cook 1, Cook 5, Cook All dialogue options
- Skillcape from Head Chef (NPC 847)

### Woodcutting (Skill 8)
- **Tree types:** Normal, Oak, Willow, Maple, Yew, Magic, Teak, Mahogany, Achey
- **Axe tiers:** Bronze through Dragon (+ special axes)
- Axe-in-inventory detection; animation varies by axe type
- Speed calculation based on tree type, axe tier, and WC level
- Max logs per tree varies by tree type
- Skillcape from Woodcutting Tutor (NPC 4906) — also buys logs for 8 coins each

### Fletching (Skill 9)
- **Knife + Log crafting:** Normal logs → arrows (Lv1), Oak (Lv15), Willow (Lv30), Maple (Lv45), Yew (Lv65), Magic (Lv75)
- Produces variable amounts of arrows per log (random 1–10)
- Continuous fletching with auto-repeat
- Skillcape from Hickton (NPC 575)

### Fishing (Skill 10)
- **Fishing methods:**
  - Net fishing (small net, NPC 316) — Shrimps (Lv1)
  - Bait fishing (rod, NPC 316) — Trout (Lv15)
  - Big net fishing (NPC 313) — Bass (Lv30)
  - Cage fishing (lobster pot, NPC 312) — Lobsters (Lv40)
  - Harpoon fishing (NPC 312) — Sharks (Lv75)
  - Harpoon fishing (NPC 313) — Manta Rays (Lv90)
- Tool checks: Small fishing net, fishing rod, lobster pot, harpoon
- Skillcape from Master Fisher (NPC 308) — provides fishing supplies

### Firemaking (Skill 11)
- **Log types:** Normal (Lv1), Oak (Lv15), Willow (Lv30), Maple (Lv45), Yew (Lv60), Magic (Lv75)
- Tinderbox (item 590) + logs = fire object (2732)
- Fire despawns after 50 ticks, leaves ashes (item 592)
- **Coloured fires** via firelighters: Red, Green, Blue, Purple, White
  - Each creates a unique fire object (11404–11406, 20000–20001)
- Skillcape from Ignatius Vulcan (NPC 4946)

### Crafting (Skill 12)
- **Gem cutting** with chisel (item 1755):
  - Sapphire (Lv1), Emerald (Lv30), Ruby (Lv50), Diamond (Lv60), Dragonstone (Lv75), Onyx (Lv85)
- **Godsword assembly:** Blade + Hilt → Armadyl/Bandos/Saradomin/Zamorak Godsword
- **Tanning** via NPC 804: Green, Blue, Red, Black dragonhide → leather
- Crafting shop (NPC 4900) sells chisel, uncut gems, moulds
- Skillcape from Master Crafter (NPC 805)

### Mining (Skill 14)
- **Ore types:** Copper, Tin, Iron, Silver, Gold, Coal, Mithril, Adamantite, Runite, Limestone, Elemental
- **Pickaxe tiers:** Bronze through Dragon
- Speed calculation based on ore type, pickaxe tier, and mining level
- Correct pickaxe level requirement enforcement
- Skillcape from Dwarf (NPC 3295) — provides pickaxe

### Smithing (Skill 13)
- **Smelting** (ore on furnace, object 56332):
  - Tin/Copper → Bronze, Iron → Iron, Coal combos → Steel/Mithril/Adamant/Rune, Gold
  - Smelt 1, Smelt 5, Smelt All options
- **Smithing at anvil** (bar on anvil, object 54540):
  - Full smithing interface (interface 300) for all 6 metal tiers (Bronze–Rune)
  - Items: Dagger, Axe, Mace, Med helm, Bolts, Sword, Dart tips, Nails, Arrow tips, Scimitar, Crossbow limbs, Longsword, Throwing knives, Full helm, Square shield, Warhammer, Battleaxe, Chainbody, Kiteshield, Claws, 2H sword, Plateskirt, Platelegs, Platebody, Pickaxe
  - Bar requirement display (colour-coded green/red)
  - Level requirement display per item
- Skillcape from Thurgo (NPC 604)

### Herblore (Skill 15)
- **Herb cleaning:** Grimy Guam → Clean Guam, Grimy Ranarr → Clean Ranarr
- NPC Kaqemeex (455) accepts cleaned herbs, provides grimy herbs based on level
- Skillcape from Kaqemeex (NPC 455)

### Agility (Skill 16)
- **Barbarian Agility Course** fully implemented:
  - Rope swing (object 2282) at [2551, 3554]
  - Log balance (object 2294) at [2551, 3546]
  - Obstacle net (object 20211) at [2539, 3546]
  - Wall slide (object 2302) at [2536, 3547]
  - Wall jumps (object 1948) at three locations
  - Balancing ledge (object 3205)
- **Underground agility obstacles** (object 25161) — wall shortcut
- Custom emotes during agility (walk emote replaced, running disabled during obstacles)
- Skillcape from Cap'n Izzy No-Beard (NPC 437)

### Thieving (Skill 17)
- **Pickpocketing NPCs:**
  - Man/Woman (Lv1) — 1–2 coins
  - Farmer (Lv40) — 1–4 coins
  - Hero (Lv65) — 1–6 coins
  - Paladin (Lv83) — 1–10 coins
- Pickpocket animation (881)
- Skillcape from Martin Thwait (NPC 2270)

### Slayer (Skill 18)
- **Slayer master:** Duradel (NPC 1599)
- **Task types:** Dragons, Guards, Giants, Ghosts, Heroes
- Random task amount (1–50)
- Slayer gem (item 4155) to check current task
- **Slayer cave** teleport via Duradel
- **Slayer shop** (shop 8): Slayer gem, facemask, earmuffs, slayer helmet, etc.
- Skillcape from Duradel

### Farming (Skill 19)
- **Flower patches:** Marigold seeds, Limpwurt seeds
- **Tree patches:** Apple tree seeds, Papaya tree seeds
- Planting via seed + farming patch (object 34573)
- Growth timer (30 ticks), then harvest for XP + produce
- Master Gardener (NPC 3299) provides seeds based on level
- Skillcape from Master Gardener

### Runecrafting (Skill 20)
- **All standard altars:** Air (Lv1), Mind (Lv2), Water (Lv5), Earth (Lv9), Fire (Lv14), Body (Lv20), Cosmic (Lv27), Chaos (Lv35), Nature (Lv44), Law (Lv54), Death (Lv65), Blood/Soul (Lv78)
- **Rune multiplier** based on level: `floor((level - reqLevel) / 20) + 1`
- Rune Essence (item 1436) required
- **Runecrafting teleport interface** (interface 45) with altar teleports
- Essence mining rock (object 16687) spawned in multiple locations
- Aubury (NPC 553) teleports to rune essence mine
- Skillcape from Aubury

### Hunter (Skill 21)
- **Impling catching** with butterfly net (item 11259):
  - Baby impling (Lv1) → jar
  - Young impling (Lv40) → jar
  - Magpie impling (Lv60) → jar
  - Ninja impling (Lv70) → jar
  - Dragon impling (Lv83) → jar
- **Impling jar looting** — random reward tables per tier (ores, runes, coins, logs)
- Skillcape from Hunting Expert (NPC 5113)

### Construction (Skill 22)
- **Player-Owned Houses (POH):**
  - Enter via house portal (object 15482) or `::house` command
  - Visit others via `::enter [name]` or `::goinhouse [name]`
  - House locking (`::lock` / `::unlock`)
  - Building Mode toggle (via house options interface 398)
  - Guest expelling
- **Room system** — 5 room slots (Room 0–4) with directional building:
  - **Parlour** — 200 coins, chairs, fireplace
  - **Garden** — 250 coins, portal, plants, trees
  - **Kitchen** — 350 coins, range, shelves
  - **Bedroom** — 450 coins, bed, dresser, wardrobe
  - **Chapel** — 600 coins, altar, pews, icon
  - **Throne Room** — 1500 coins, throne, guards
  - **Study** — bookshelves
- **Room deletion** via `::deleteroom [0-4]`
- **House decoration styles:** Stone (1585, 500gp), Dark Stone (1588, 1000gp, Lv50), White Stone (13116, 2500gp, Lv80)
- **Furniture building** in Garden:
  - Portal, tree space, big/small plant spaces with 3 tiers each
  - Plant types: Fern, bush, tall plant, dock leaf, thistle, reeds, etc.
- **Construction interface** (402) for room creation
- **Height-based instancing** — each player's house on a unique height level
- Skillcape from Estate Agent (NPC 4247)

### Summoning (Skill 23)
- **Familiar summoning** from pouches:
  - Spirit Wolf (Lv1), Dreadfowl (Lv4), Spirit Spider (Lv10), Thorny Snail (Lv13)
  - Spirit Tz-Kih (Lv22), Bronze→Rune Minotaurs (Lv36–86)
  - Fire/Moss/Ice Titans (Lv79), Lava Titan (Lv83), Swamp Titan (Lv85)
  - Geyser Titan (Lv89), Abyssal Titan (Lv93), Iron Titan (Lv95), Steel Titan (Lv99)
- **Familiar tab** (interface 663) with Call and Dismiss options
- Familiars follow player, teleport to player if too far
- **Pet system:** Baby dragons (4 colours), pet monkey — require Lv99/75 Summoning
  - Drop pet item to summon, pick up NPC to retrieve
- **Summoning shop** (NPC Pikkupstix, 6970) — all pouches
- **White Knight** familiar from special item (4447)
- Skillcape from Pikkupstix

---

## 3. Combat

### Melee Combat (PvP)
- **Max hit formula:** Based on strength level + equipment strength bonus
- **Attack styles:** Accurate, Aggressive, Defensive, Controlled (4 styles)
- **Weapon-specific attack interfaces** for every weapon class:
  - Whip (93), Maul/Hammers (76), Flails/Maces (88), Crossbows (79), Bows (77)
  - Staves (90), Darts/Knives/Thrown (91), Daggers (89), Pickaxes (83)
  - Axes/Battleaxes (75), Halberds (84), Spears (85), Claws (78)
  - 2H Swords/Godswords (81), Default swords (82)
- **Auto-retaliate** toggle
- **Skull system** — 180-tick skull timer when attacking first in wilderness
- **Wilderness level** combat range enforcement
- **Freeze mechanic** — prevents movement for configurable duration

### Special Attacks
- **Dragon Battleaxe** (item 1377): Strength boost + "Aarrrgggghhhh!" forced chat, 100% energy
- **Dragon Claws** (item 3101): Multi-hit special (4 hits with decreasing damage)
- **Dragonfire Shield** (item 11283): Ranged dragonfire attack, 10-tick cooldown, up to 50 damage
- **Godsword specials** supported for AGS, BGS, SGS, ZGS
- **Special attack bar** — regenerates 1% per 2 ticks, displayed via config 300/301
- Special attack toggle on all weapon interfaces that support it

### Ranged Combat
- **Bow types:** Shortbows, longbows, crossbows, dark bow, crystal bow, Karil's crossbow
- **Max hit formula:** Range level × equipment range bonus multiplier
- **Projectile rendering** with GFX

### NPC Combat (PvE)
- **NPC aggression** — NPCs follow and attack players
- **Dragon fire mechanic** — dragons alternate between melee and fire attacks
  - Anti-dragon shield / DFS reduces fire damage
- **NPC prayer protection** — Protect from Melee reduces NPC hits (partial, with occasional bypass)
- **Barrows brothers** — 6 brothers tracked via boolean array, chest reward system
- **GWD kill count** — Saradomin, Zamorak, Bandos, Armadyl KC tracking (20 KC to enter boss lairs)
- **Slayer task tracking** — kills decrement task counter
- **NPC drops** loaded from external config with chance/amount ranges

### Death System
- **PvP death (wilderness):** All items dropped to ground for killer
- **PvE death:** Items stored in gravestone (object 12719)
  - Gravestone lasts 200 ticks (~2 minutes)
  - Items recoverable by clicking gravestone
  - Items drop to ground as ground items after gravestone expires
- **Death animation** (7197), prayer Retribution explosion on death
- **Stats fully restored** on respawn
- **Duel/minigame death** — teleport to appropriate location, no item loss

---

## 4. Magic System

### Modern Spellbook (Interface 192)

#### Combat Spells (on Players and NPCs)
| Tier | Wind | Water | Earth | Fire |
|------|------|-------|-------|------|
| Strike | Lv1, max 2 | Lv5, max 4 | Lv9, max 6 | Lv13, max 8 |
| Bolt | Lv17, max 9 | Lv23, max 10 | Lv29, max 11 | Lv35, max 12 |
| Blast | Lv41, max 13 | Lv47, max 14 | Lv53, max 15 | Lv59, max 16 |
| Wave | Lv62, max 17 | Lv65, max 18 | Lv70, max 19 | Lv75, max 20 |

- Full projectile GFX and animations for all 16 combat spells
- Rune requirements enforced (mind/chaos/death/blood + elemental)

#### Teleport Spells
- Varrock Teleport (Lv25) — 3 air, 1 fire, 1 law
- Lumbridge Teleport (Lv31) — 3 air, 1 earth, 1 law
- Falador Teleport (Lv37) — 3 air, 1 water, 2 law
- Camelot Teleport (Lv45) — 5 air, 1 law (to 2662, 3305 — actually Ardougne-ish)
- Ardougne Teleport — 2 water, 2 law
- Watchtower Teleport — 2 earth, 2 law
- Trollheim Teleport — 2 fire, 2 law
- Ape Atoll Teleport — 2 fire, 2 law, 2 water, 1 banana

#### Utility Spells
- **Low Alchemy** (Lv20) — 3 fire, 1 nature → 2 coins
- **High Alchemy** (Lv55) — 5 fire, 1 nature → 2 coins
- **Superheat Item** (Lv43) — 4 fire, 1 nature + ore → bar
  - Supports: Bronze, Iron, Silver, Steel, Gold, Mithril, Adamant, Rune
  - Enforces smithing level requirements
- **Bones to Peaches** (Lv60) — 4 earth, 4 water, 2 nature, bones → peaches
- **Charge** (Lv80) — 3 air, 3 fire, 3 blood → 1.3x arena spell power

#### Enchantment Spells (Levels 1–6)
- **Level 1** (Lv7): Sapphire ring→Ring of recoil, sapphire bracelet, necklace, amulet
- **Level 2** (Lv27): Emerald items (ring→Ring of dueling, bracelet, necklace, amulet)
- **Level 3** (Lv49): Ruby items (ring→Ring of forging, bracelet, necklace, amulet)
- **Level 4** (Lv57): Diamond items (ring→Ring of life, bracelet, necklace, amulet)
- **Level 5** (Lv68): Dragonstone items (ring→Ring of wealth, bracelet, amulet)
- **Level 6** (Lv87): Onyx items (ring→Ring of stone, bracelet, necklace, amulet of fury)

### Ancient Magicks (Interface 193)

All ancient spells supported on both players and NPCs:

| Element | Rush | Burst | Blitz | Barrage |
|---------|------|-------|-------|---------|
| **Ice** | Lv58, max 18, freeze 5t | Lv70, max 22, freeze 10t | Lv82, max 26, freeze 15t | Lv94, max 30, freeze 33t |
| **Blood** | Lv56, max 17, heal ¼ | Lv68, max 21, heal ½ | Lv80, max 25, heal ½ | Lv92, max 29, heal ¼ |
| **Shadow** | Lv52, max 16, stat drain | Lv64, max 20, stat drain | Lv76, max 24, stat drain | Lv88, max 28, stat drain |
| **Smoke** | Lv50, max 15, poison | Lv62, max 20, poison | Lv74, max 23, poison | Lv86, max 27, poison |

- **Multi-target barrage spells** — hit all players within 3-tile radius of target
- **Ice spells** apply freeze delay
- **Blood spells** heal caster
- **Smoke spells** apply poison hit type

### Lunar Spellbook (Interface 430)
- Lunar magic action button handling present
- Vengeance spell (`lastVeng` tracking, "Taste Vengeance!" mechanic)

### Autocasting
- **Staff autocasting** (interface 319) — select any modern spell for auto-cast
- Supported for all Strike through Wave spells (16 spells)
- `castAuto` / `modernSpell` tracking per player

---

## 5. Prayer System

### All 27 Prayers
| # | Prayer | Level | Drain |
|---|--------|-------|-------|
| 0 | Thick Skin | 1 | 3 |
| 1 | Burst of Strength | 4 | 4 |
| 2 | Clarity of Thought | 7 | 5 |
| 3 | Sharp Eye | 8 | 6 |
| 4 | Mystic Will | 9 | 7 |
| 5 | Rock Skin | 10 | 8 |
| 6 | Superhuman Strength | 13 | 9 |
| 7 | Improved Reflexes | 16 | 10 |
| 8 | Rapid Restore | 19 | 6 |
| 9 | Rapid Heal | 22 | 7 |
| 10 | Protect Item | 25 | 6 |
| 11 | Hawk Eye | 26 | 12 |
| 12 | Mystic Lore | 27 | 13 |
| 13 | Steel Skin | 28 | 14 |
| 14 | Ultimate Strength | 31 | 15 |
| 15 | Incredible Reflexes | 34 | 16 |
| 16 | Protect from Magic | 37 | 17 |
| 17 | Protect from Ranged | 40 | 18 |
| 18 | Protect from Melee | 43 | 19 |
| 19 | Eagle Eye | 44 | 20 |
| 20 | Mystic Might | 45 | 21 |
| 21 | Retribution | 46 | 22 |
| 22 | Redemption | 49 | 23 |
| 23 | Smite | 52 | 24 |
| 24 | Summoning protection | 35 | 15 |
| 25 | Chivalry | 60 | 26 |
| 26 | Piety | 70 | 28 |

- **Overhead icons:** Protect Melee (0), Protect Range (1), Protect Magic (2), Retribution (3), Redemption (4), Smite (5), Summoning (7)
- **Prayer switching** — mutual exclusion logic for all conflicting prayers
- **Prayer drain** — drains per game tick, depletes prayer points
- **Retribution on death** — AoE damage (10 + random 15) to nearby players

---

## 6. Minigames & Activities

### Castle Wars
- **Two teams:** Saradomin (cape 4041) and Zamorak (cape 4042)
- **Waiting rooms** with team balancing enforcement
- **Flag capture mechanics:**
  - Take enemy flag (items 4037 Saradomin, 4039 Zamorak) — equips as weapon
  - Score by returning to own flag stand
  - Flag status tracking (Safe/Taken)
- **Castle Wars items:** Barricades (4053), explosive potions (4045), ropes (954), toolkits (4051), rocks (4043), bandages (4049), pickaxes
- **Scoreboard** (object 4484) showing Saradomin vs Zamorak scores
- **Barriers and doors** — team-specific access
- **Multi-floor castles** — stairs between floors
- **Game timer** — timed rounds, auto-end with rewards (300 coins to winners)
- **Equipment restrictions** — no capes or hats allowed (extensive item check)
- **Death in CW** — respawn at team spawn, flag returned

### Fight Pits (TzHaar)
- **Waiting room** (area 2394–2404, 5169–5175) with countdown timer
- **Free-for-all PvP** arena (area 2370–2426, 5128–5167)
- **Minimum 2 players** to start
- **Last man standing wins** — receives 3rd Age armour piece + announcement
- **Player count overlay** during game
- **120-tick timer** between rounds

### Barbarian Assault
- **5 waves** of increasing difficulty
- **4 NPC types per wave:** Healers, Rangers, Fighters, Runners
  - Wave scaling: 2/4/6/8/10 NPCs per type
  - HP scaling: 25/40/65/80/100
  - Max hit scaling: 5/9/13/15/18
- **3 players minimum** to start
- **Wave progression** — complete all types to advance
- **Kill tracking** — remaining NPCs announced
- **Height-based instancing** for multiple concurrent games
- **Rune supply spring** in arena
- **Reward system** — reward points redeemable at NPC 5029 for BA armour + coins
- **Rules:** Runners/Healers only killable with magic

### Bounty Hunter
- **Bounty Hunter crater** area (3085–3185, 3662–3765)
- **Automatic opponent matching** — finds players in area without opponents
- **Opponent tracking** — username display on interface 653
- **Leave/death handling** — opponent notified, re-matched

### Clan Wars
- **Clan challenge system** — right-click "Challenge" on clan leader
- **Height-instanced battle arena** (3263–3329, 3713–3841)
- **Barrier walls** during countdown (spawned objects), removed when timer ends
- **Team tracking** — clan member count per side
- **Win condition** — enemy clan reaches 0 members in field
- **Victory/defeat interfaces** (650/651)
- **Lobby area** with portal entry

### Dueling
- **Challenge system** — right-click "Duel" option
- **3-second countdown** with forced chat ("3", "2", "1", "Fight!!!")
- **Teleport to arena** (3362–3391, 3228–3241)
- **Victory message** broadcast to all players
- **Return to original position** after duel

### Barrows
- **6 brothers** tracked individually (boolean array)
- **Chest rewards** (object 10284) — requires all 6 killed:
  - Barrows armour sets, rune items, bolt racks, runes
  - Random 1–3 items from weighted loot table
- **Teleport to barrows** area (3552, 9693)

### God Wars Dungeon
- **4 faction lairs:** Saradomin, Zamorak, Bandos, Armadyl
- **Kill count system** — 20 KC required per faction to enter boss room
- **KC tracking** — `::kc` command to check
- **GWD altars** — restore prayer for each faction
- **Ice bridge** crossing, rope entry
- **Height level 2** instancing
- **Boss room doors** — KC deducted on entry

### Party Room
- **Deposit items** into party chest (object 26193, shop 17)
- **Party drop lever** (object 26194) — costs 5000 coins, 300-tick countdown
- **Admin restriction** — admins cannot deposit
- **Teleport:** `::party`

### Slayer Cave
- **Teleport** via Duradel NPC option
- **Slayer-specific monsters** inside
- **4-tick loading screen** on entry

---

## 7. Player Interaction

### Trading
- **Full 2-screen trade system** (`PTrade.java`):
  - Screen 1 (interface 335): Add/remove items, see partner's offer
  - Screen 2 (interface 334): Confirmation with item list summary
- **Trade options:** Offer 1/5/10/All/X items
- **Remove options:** Remove 1 from trade
- **Both players must accept** each screen
- **Decline** at any point returns all items
- **Free slot display** — shows partner's available inventory space
- **Anti-scam:** Admin accounts (rights > 1) cannot trade
- **Trade request** system with `:tradereq:` message format

### Following
- **Player following** — walk towards target, face target
- **Distance cap** — stops following if > 12 tiles away

### Friends & Ignores
- **Friends list** — up to 200 friends
- **Ignore list** — up to 100 ignores
- **Online status** — friends see when you log in/out
- **Private messaging** — send/receive with rights icon display

### Clan Chat System
- **Full clan chat** implementation:
  - Create/name/rename clan channels
  - Join/leave other player's channels
  - Talk with `/` prefix
  - **Rank system** — 6 ranks + owner
  - **Kick players** from clan
  - **LootShare** toggle with chance-based loot distribution
  - **Join requirements** — configurable minimum rank
  - **Clan member list** — shows online members with ranks
  - **Auto-save** clan data every 60 seconds

---

## 8. Item Systems

### Inventory
- 28 inventory slots
- **Stackable items** handled correctly (runes, coins, arrows, etc.)
- **Noted items** — note detection via description prefix "swap"
- **Item switching/rotating** in inventory

### Banking
- **1000 bank slots**
- **9 bank tabs** + main tab with:
  - Create tabs by dragging items to tab icons
  - Collapse tabs back to main
  - Insert mode (rearrange items within/between tabs)
  - Swap mode (exchange positions)
- **Deposit options:** 1, 5, 10, Last-X, All, X (custom amount)
- **Withdraw options:** 1, 5, 10, Last-X, All, All-but-one, X
- **Withdraw as note** toggle
- **Bank booth objects** (2213, 2672, 4483, 25808, 26972, 28089) and banker NPCs (494, 495)

### Shops
- **18+ shops** with full buy/sell mechanics:
  1. General Store — sell any item, items destock over time
  2. Supplies Shop — tools, weapons, fishing/hunting equipment
  3. Ranged Shop — d'hide armour, studded, trimmed sets
  4. Madrith's Shop — high-tier items (DFS, Bandos, whip, godswords)
  5. Horvik's Armour — all metal armour tiers
  6. Thessalia's Clothes — capes, amulets, decorative items
  7. Crafting Shop — chisel, gems, moulds, leather
  8. Slayer Shop — slayer equipment
  9. Rare Items — all party hats and holiday items
  10. Barrows Shop — all barrows sets
  11. Summoning Shop — all pouches and pets
  12. Construction Shop — furniture items
  13. Staff Shop — staff-exclusive items
  14. Member Shop — member-exclusive cosmetics
  16. Costume Shop — costumes
  18. Magic Shop — mystic, infinity, god staves, robes
- **Dynamic stock** — restocks every 10 seconds
- **Value display** — shows buy/sell prices
- **Price calculation** from item definition shop values

### Equipment
- **14 equipment slots:** Head, Cape, Amulet, Weapon, Chest, Shield, Legs, Hands, Feet, Ring, Arrows
- **Name-based slot detection** for all item types
- **Level requirements** enforced: Attack, Defence, Strength, Magic, Ranged, Runecrafting
- **Two-handed weapon** detection — automatically unequips shield
- **Full/half body/mask detection** — hides appropriate appearance layers
- **Equipment bonuses** — 12 bonus types (stab/slash/crush/magic/range attack + defence, strength, prayer)
- **Equipment stats interface** (667) with full bonus display
- **Skillcape requirements** — must have Lv120 in matching skill
- **Dragon Slayer quest gate** — rune platebody and dragon chain require quest completion

### Food & Potions
- **Food healing amounts:**
  - Shrimps (3), Trout (7), Bass (13), Lobster (12), Swordfish (16), Shark (20), Manta ray (22)
  - Sea turtle (16), Monkfish (16), Tuna (8), Salmon (9), Cake (1), Peaches (20)
  - Birthday cake (136 + GFX!)
  - Bandages (8, no animation)
- **Eat delay** — 3-tick cooldown, adds 2 to combat delay
- **Potions (4-dose systems):**
  - **Super Restore** (3024→3030) — restores all combat stats by 26% + 8
  - **Saradomin Brew** (6685→6691) — heals 15% HP + 2, boosts defence, lowers attack/range/magic
  - **Super Strength** (2440→161) — boosts strength by 15% + 5
  - **Super Attack** (2436→149) — boosts attack by 15% + 5
  - **Super Defence** (2442→167) — boosts defence by 15% + 5
  - **Ranging Potion** (2444→173) — boosts ranged by 10% + 4
  - **Strength Potion** (113→119) — boosts strength by 10% + 3
  - **Attack Potion** (2428→125) — boosts attack by 10% + 3
  - **Defence Potion** (2432→137) — boosts defence by 10% + 3
  - **Combat Potion** (2430→131) — boosts attack/strength/defence/range/magic by 30% + 10
  - All potions leave empty vials (229)
- **Drink delay** — 3-tick cooldown

### Ground Items
- Items visible only to dropper for first 60 ticks
- Then visible to all players
- Despawn after 240 ticks total
- Untradable items only visible to dropper
- Skillcapes don't appear on ground (anti-loot)

---

## 9. NPC Systems

### NPC Behaviour
- **Random walking** within defined movement ranges
- **Player following** with pathfinding (8-directional movement)
- **Aggro system** — NPCs follow and attack players who engage them
- **Respawn system** — configurable respawn delay per NPC type
- **NPC lists** loaded from config with: name, combat level, max HP, max hit, attack type, weakness, emotes, attack delay

### Summoned NPCs
- Separate from regular NPCs
- Cannot be attacked by other players (unless NPC is already fighting them)
- Follow owner, teleport to owner if too far (> 15 tiles)
- Height level syncing with owner

### NPC Dialogue System
- **Chatbox interfaces** (241, 458) for NPC conversations
- **Multi-option dialogues** (458 — 3 options)
- **NPC head models** displayed in dialogue
- **Animated NPC expressions** (various emote IDs)
- **Skill cape dialogues** — master NPCs offer capes at Lv120
- **Quest dialogues** — Dragon Slayer quest NPCs with branching conversations

### Notable NPCs
- **Bankers** (13, 14, 15, 494, 495, 2270, 2619) — bank access
- **Shop NPCs** — 15+ shop keepers
- **Skill tutors** — one per skill, provide supplies and skillcapes
- **Quest NPCs** — Guildmaster (198), Oziach (747), Oracle (746), Duke (741), Klarense (744)
- **Hairdresser** (598) — male/female hair/beard customization interfaces
- **Estate Agent** (4247) — construction services
- **Hans** (0) — welcome NPC

---

## 10. World & Object Interactions

### Object Interactions (Option 1)
- **Banks:** Booth objects (2213, 2672, 4483, 25808, 26972)
- **Trees:** 30+ tree object IDs for Woodcutting
- **Rocks:** 25+ rock object IDs for Mining
- **Altars:** All 12 Runecrafting altars + prayer altars
- **Doors/gates:** Extensive coordinate-based door handling for buildings
- **Stairs:** Multiple staircase objects with height level transitions
- **Ladders:** Up/down transitions
- **Agility obstacles:** Rope swings, log balances, nets, walls
- **GWD objects:** Rope entry, faction doors (KC-gated), ice bridge, prayer altars
- **Castle Wars objects:** Flags, barriers, supply tables, stairs, scoring stands
- **Barbarian Assault:** 5 wave entrance objects
- **Portals:** PK portal, Clan Wars portal, Fight Pits portal, Bounty Hunter
- **Wilderness ditch:** Jump mechanic with animation
- **Gravestone** (12719) — click to recover items
- **Barrows chest** (10284) — claim rewards
- **Party room lever** (26194) and chest (26193)
- **Ancient altar** (6552) — switch to/from Ancient Magicks
- **Lunar altar** (17010) — switch to/from Lunar spellbook
- **Farming patches** — plant seeds, harvest produce
- **Furnace** (56332) — smelting
- **Anvil** (54540) — smithing
- **Cooking range** (58124, 28173) — cooking
- **Cannon** base placement system (object 7/8)

### Item on Object
- **Ore on furnace** — smelting dialogue (all 8 ore types)
- **Bar on anvil** — opens smithing interface (all 6 metal types)
- **Raw fish on range/fire** — cooking dialogue
- **Seeds on farming patch** — planting
- **Cannon parts** — assembly

### Item on Item
- **Chisel + uncut gems** — gem cutting (6 gem types)
- **Godsword blade + hilt** — 4 godsword assemblies
- **Knife + logs** — fletching (6 log types)
- **Tinderbox + logs** — firemaking (6 log types)
- **Firelighters + logs** — coloured log creation (5 colours)
- **Tinderbox + coloured logs** — coloured fire lighting

### Item on NPC
- Debug item-on-NPC handler (displays item/NPC/interface info)

---

## 11. Commands

### Player Commands (Rights ≥ 0)
| Command | Description |
|---------|-------------|
| `::home` | Teleport to Lumbridge (3222, 3219) |
| `::wildy` / `::pvp` | Teleport to wilderness |
| `::gwd` | Teleport to God Wars Dungeon |
| `::house` | Teleport to own POH |
| `::enter [name]` | Enter another player's house |
| `::lock` / `::unlock` | Lock/unlock house |
| `::party` | Teleport to party room |
| `::assault` | Teleport to Barbarian Assault |
| `::cw` | Teleport to Castle Wars |
| `::kc` | Check GWD kill counts |
| `::fixgwd` | Fix GWD underground height glitch |
| `::players` | List online players with combat levels |
| `::commands` | Show command list |
| `::help` | Show help information |
| `::changepass [new]` | Change password |
| `::yell [msg]` | Global server message (with rank titles) |
| `::char` | Open character design screen |
| `::male` / `::female` | Quick gender change |
| `::afk` / `::back` | AFK status toggle with animations |
| `::smoke` | Smoking emote |
| `::whereis [name]` | Find player's location |
| `::savebackup` | Manual backup save |
| `::reportbug [msg]` | Submit bug report |
| `::reportabuse [msg]` | Submit abuse report |
| `::joinchat [name]` | Join clan chat |
| `::leavechat` | Leave clan chat |
| `::c [msg]` | Clan chat message |
| `::newname [name]` | Rename own clan |
| `::newroom [0-4]` | Build construction room |
| `::deleteroom [0-4]` | Delete construction room |
| `::verifycode [code]` | Verify new account |
| `::setskills` | Set skill display |
| `::goinhouse [name]` | Enter player's POH |

### Moderator Commands (Rights ≥ 1)
| Command | Description |
|---------|-------------|
| `::jail [name]` | Jail a player |
| `::mute [name]` | Mute a player |
| `::unmute [name]` | Unmute a player |
| `::ban [name]` | Ban a player |
| `::unban [name]` | Unban a player |
| `::kick [name]` | Kick a player |
| `::staff` | Teleport to staff zone |
| `::god2` | God mode (fly emotes, infinite run) |
| `::godoff` | Disable god mode |
| `::private` | Teleport to private area |

### Administrator Commands (Rights = 2)
| Command | Description |
|---------|-------------|
| `::tele [x] [y] [h]` | Teleport to coordinates |
| `::teleto [name]` | Teleport to player |
| `::teletome [name]` | Teleport player to you |
| `::item [id] [amt]` | Spawn item |
| `::bank` | Open bank anywhere |
| `::npc [id]` | Spawn NPC |
| `::object [id]` | Spawn object |
| `::master` | Set all skills to 136 |
| `::slave` | Set all skills to 98 |
| `::setskill [id] [lvl] [xp]` | Set specific skill |
| `::pnpc [id]` | Transform into NPC |
| `::unpc` | Revert from NPC |
| `::god` | God mode |
| `::emote [id]` | Perform animation |
| `::gfx [id]` | Display graphic |
| `::si [id]` | Show interface |
| `::coords` | Show current coordinates |
| `::empty` | Clear inventory |
| `::kill [name]` | Instant-kill player |
| `::rs` | Restore special attack |
| `::ancients` / `::modern` / `::lunar` | Switch spellbook |
| `::fullkc` | Set all GWD KC to 200 |
| `::givemember [name]` | Grant membership |
| `::removemember [name]` | Remove membership |
| `::ipban [name]` | IP ban player |
| `::ipmute [name]` | IP mute player |
| `::clangame` | Start server clan wars event |
| `::bh` | Teleport to Bounty Hunter |
| `::rebuildnpclist` | Rebuild NPC list |
| `::restorestats` | Restore all stats |
| `::restoreenergy` | Restore run energy |
| `::logout` | Force logout |

---

## 12. Quests

### Dragon Slayer
- **Full quest implementation** with 6 stages (0–5):
  1. **Start:** Talk to Guildmaster (NPC 198) — sends to Oziach
  2. **Oziach** (NPC 747) in Edgeville — assigns dragon kill task
  3. **Guildmaster** explains requirements: map, ship, anti-dragon shield
  4. **Gathering phase:**
     - **Oracle** (NPC 746) on Ice Mountain — gives map (item 1538)
     - **Duke of Lumbridge** (NPC 741) — gives anti-dragon shield (item 1540)
     - **Klarense** (NPC 744) at Port Sarim — sails to Crandor with map
  5. **Boat cutscene:** Multi-stage animated sequence (sailing → thunder → dragon attack → crash)
  6. **Kill Elvarg** in cave (2833, 9656)
  7. **Return to Guildmaster** — quest complete
- **Rewards:** 2 Quest Points, ability to wear Rune platebody & Dragon chainbody, 180,650 Attack XP, 180,650 Strength XP
- **Quest journal** in quest tab with stage-appropriate text
- **Quest complete interface** (277) with rewards display
- **3-option dialogues** for quest progression

### Quest Cape
- Requires all quests completed (QuestPoints ≥ 2)
- Wise Old Man (NPC 2253) awards Quest Cape (9813) + Hood (9814)

---

## 13. Miscellaneous Systems

### Character Customization
- **Full character design screen** (interface 771):
  - Gender selection (Male/Female)
  - Hair styles (16 male, 22 female options)
  - Facial hair (15 male options)
  - Hair colour (24+ colours)
  - Body randomization
- **Hairdresser NPC** (598) — separate male (596) and female (592) interfaces
- **Clothing shop** (NPC Thessalia, 548) — torso/arms/legs customization (interface 591)

### Emotes
- **44 emotes** on emote tab (interface 464):
  - Standard: Yes, No, Bow, Angry, Think, Wave, Shrug, Cheer, Beckon, Laugh, Joy Jump, Yawn, Dance, Jig, Spin, Headbang, Cry, Blow Kiss, Panic, Raspberry, Clap, Salute
  - Special: Goblin Bow, Goblin Salute, Glass Box, Climb Rope, Lean, Glass Wall, Head Slap, Stomp, Flap, Idea, Zombie Walk, Zombie Dance, Zombie Hand, Scared, Rabbit Hop
  - Holiday: Snowman Dance, Air Guitar, Safety First, Explore
- **Skillcape emotes** — unique animation + GFX per cape (50+ skillcapes supported)
- **God cape emotes** — Saradomin/Zamorak/Guthix capes with force chat + GFX

### Appearance
- **Player-as-NPC** transformation (`::pnpc`)
- **Gnomecopter** (item 12842) — special fly emotes, no armour allowed, "ZOOM" force chat
- **Gender-specific appearance** with full look array (7 slots: hair, beard, torso, arms, bracelets, legs, shoes)
- **5 colour slots** (hair, torso, legs, feet, skin)

### Wilderness
- **Wilderness level** calculation: `(absY - 3520) + 1`
- **Combat level range** enforcement based on wilderness level
- **Attack option** changes to "Attack" in wilderness/duel/pits, "Walk Here" elsewhere
- **Skull mechanic** — 180-tick skull for initiating combat

### Run Energy
- **100 max energy**, drains while running
- **Regenerates** 1 point per 4 ticks while not running
- **Toggle** via settings tab or minimap orb
- **Gnomecopter** gives infinite run energy

### Stat Restoration
- **Natural stat restore** every 75 ticks — boosted stats decrease, lowered stats increase by 1
- **Potion stat decay** follows same system

### Random Events
- **Anti-bot system** — random skill selection challenge
  - Interface 134 displays all skills, player must click named skill
  - Wrong answer = disconnect
  - Correct answer = 5 coins reward
- Triggers randomly during object interactions (~1/150 chance)

### Jailing System
- **Jail location** (2604, 3105)
- **Jail timer** with periodic forced chat: "I have been jailed for breaking the rules."
- **Command restriction** — jailed players can only `::yell`
- **Movement/action restriction** while jailed

### Membership System
- **Member flag** — unlocks member shop (NPC 1835) and member area
- **Staff can grant/remove** membership
- **Member-exclusive items** — trimmed armour, god dragonhide, etc.

### Amulet of Glory Teleports
- **Operate item** or **click item** — 3-option dialogue:
  - Fight Pits (2399, 5178)
  - Castle Wars (2442, 3090)
  - Port Sarim (3048, 3203)

### DFS Special Attack
- **Dragonfire Shield** (11283) operate — ranged fire attack
- 50 max damage with projectile GFX
- 10-tick cooldown

### Skill Teleports
- **Clicking any skill** in the skills tab teleports to that skill's training area
- All 24 skills mapped to specific training locations

### Cannon System
- **Cannon base** (item 6) placement
- **Assembly** via item-on-object
- **One cannon per player** restriction

### Destroy Item Confirmation
- **Untradable items** show destroy confirmation interface (94) instead of dropping
- "Clicking yes will permanently delete this item"

### Login/Logout Broadcasts
- **Login message:** `[username] has logged in.` (red colour)
- **Logout message:** `[username] has logged out.` (red colour)
- **Welcome messages** on login with update notes

### Update Notes System
- **Multi-page update notes** shown on first interactions after login (interface 132)
- Tracks `Update` counter per player to show sequential update pages

### Highscores Board
- **In-game object** (3192) shows top 30 players
- **Displays** on interface 156 with total levels

### Location Tracking
- **Player location** tracked as human-readable string (e.g., "Varrock", "Wilderness", "His House")
- Used by `::whereis` command

---

*This document was auto-generated from source code analysis of the DavidScape 508 RSPS legacy Java codebase.*
