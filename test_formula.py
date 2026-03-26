# Java: yCalc + (xCalc << 1786653352)
# In Java, shift operators on int mask the shift amount with 0x1F (31).
# 1786653352 & 31 = 8.
# So the shift is effectively xCalc << 8.
# Let's test the values to make sure xCalc << 8 is producing mapIds that match what's in 1.dat.

map_ids = set()
import struct
with open("legacy-java/server508/data/mapdata/1.dat", "rb") as f:
    data = f.read()

offset = 0
while offset + 24 <= len(data):
    i = struct.unpack(">i", data[offset:offset+4])[0]
    offset += 4
    if i == 0: break
    mapId = struct.unpack(">i", data[offset:offset+4])[0]
    offset += 20
    map_ids.add(mapId)

print(f"Loaded {len(map_ids)} map IDs")
print(f"Sample map IDs: {list(map_ids)[:10]}")

# Default Lumbridge spawn: 3222, 3219
# p.mapRegionX = (p.teleportToX >> 3); // 3222 >> 3 = 402
# p.mapRegionY = (p.teleportToY >> 3); // 3219 >> 3 = 402
# Wait, Frames.java says:
# p.mapRegionX = (p.absX >> 3) - 6; // 3222 >> 3 = 402, 402 - 6 = 396?
# No, let's look at Player.java absX/absY or mapRegionX/Y

