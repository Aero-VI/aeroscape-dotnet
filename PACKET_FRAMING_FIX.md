# Packet Framing Fix - Opcode 129 and Others

## Root Cause
The packet parser was misaligned because several opcodes had incorrect size definitions in Protocol_508.json:

1. **Opcode 129 (Prayer)**: Was set to -3 (read all available bytes), should be 2 bytes
2. **Opcode 155 (BountyHunter)**: Was set to -3, should be 2 bytes  
3. **Opcode 214 (ItemOnNPC)**: Was set to -3, should be 8 bytes

The -3 size caused the parser to consume all available bytes, which often included the next packet's opcode. Since 0x81 (129) appears frequently in packet payloads, the parser would misinterpret payload data as opcode 129, creating an endless loop.

## Fix Applied
Updated `/home/aeroverra/.openclaw/workspace/projects/aeroscape-dotnet/AeroScape.Server.Network/Protocol/Protocol_508.json`:

```json
// Changed from:
{ "opcode": 129, "size": -3, "name": "Unknown129" }
// To:
{ "opcode": 129, "size": 2, "name": "Prayer" }

// Changed from:
{ "opcode": 155, "size": -3, "name": "Unknown155" }
// To:
{ "opcode": 155, "size": 2, "name": "BountyHunter" }

// Changed from:
{ "opcode": 214, "size": -3, "name": "Unknown214" }
// To:
{ "opcode": 214, "size": 8, "name": "ItemOnNPC" }
```

## Evidence
- Raw TCP showed walk packets (0x31/49, 0x77/119) arriving correctly
- Parser kept reading opcode 129 due to misalignment
- Java source (Packets.java) also had 129 set to -3, but the C# decoders expected fixed sizes
- PrayerDecoder expects 2 bytes, BountyHunterDecoder expects 2 bytes, ItemOnNPCDecoder expects 8 bytes

## Testing Required
1. Build the project with updated Protocol_508.json
2. Run the server
3. Click to walk - should now see opcodes 49/119/138 instead of endless 129
4. Test prayer buttons to ensure opcode 129 still works correctly

## Note on Opcode 214
There's a registration conflict where opcode 214 is registered for both ActionButtonsDecoder and ItemOnNPCDecoder in PacketRouter.cs. The Java code has 214 commented out from ActionButtons, suggesting ItemOnNPC is the correct handler. This conflict should be resolved by removing 214 from the ActionButtonsDecoder registration.