# AUDIT ROUND 4 - Protocol Layer Bugs

## Bugs

**ActionButtonsDecoder.cs:267-278 — Wrong packet structure for ActionButtons** — Java ref: ActionButtons.java:44-47

Java reads two separate words (`readUnsignedWord()` for interfaceId, then `readUnsignedWord()` for buttonId), but C# reads a packed DWord and extracts fields. This assumes the packet has a packed structure that doesn't match the Java implementation.

**EquipItemDecoder.cs:295 — Extracts interfaceId from packed DWord but Java ignores it** — Java ref: Equipment.java:60-61

Java reads `readDWord_v2()` as junk and ignores it, then reads `readUnsignedWordBigEndian()` for itemId. C# extracts interfaceId from the DWord (packed >> 16) which Java doesn't do.

**ObjectOption2Decoder.cs:447 — Incorrect comment about Java reference** — Java ref: ObjectOption2.java:entire file

Comment claims "The actual objectId is read later in the handler after distance check (Java ObjectOption2.java:56)" but the referenced Java file appears to handle player interactions, not object interactions. The packet structure may be correct but the comment is misleading.
