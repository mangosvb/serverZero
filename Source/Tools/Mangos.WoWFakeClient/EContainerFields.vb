Public Enum EContainerFields
    CONTAINER_FIELD_NUM_SLOTS = EItemFields.ITEM_END + &H0                        ' 0x02A - Size: 1 - Type: INT - Flags: PUBLIC
    CONTAINER_ALIGN_PAD = EItemFields.ITEM_END + &H1                              ' 0x02B - Size: 1 - Type: BYTES - Flags: NONE
    CONTAINER_FIELD_SLOT_1 = EItemFields.ITEM_END + &H2                           ' 0x02C - Size: 72 - Type: GUID - Flags: PUBLIC
    CONTAINER_END = EItemFields.ITEM_END + &H3A                                   ' 0x074
End Enum