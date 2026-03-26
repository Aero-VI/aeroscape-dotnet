import struct

with open("legacy-java/server508/data/mapdata/1.dat", "rb") as f:
    data = f.read()

offset = 0
count = 0
while offset < len(data):
    if offset + 4 > len(data): break
    i = struct.unpack(">i", data[offset:offset+4])[0]
    offset += 4
    if i == 0:
        print("Got 0, breaking")
        break
    if offset + 20 > len(data): break
    mapId = struct.unpack(">i", data[offset:offset+4])[0]
    offset += 4
    keys = struct.unpack(">iiii", data[offset:offset+16])
    offset += 16
    if count < 5:
        print(f"i={i}, mapId={mapId}, keys={keys}")
    count += 1
print(f"Total loaded: {count}")
