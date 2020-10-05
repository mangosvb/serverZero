Namespace Globals
    Public Enum ECorpseFields
        CORPSE_FIELD_OWNER = EObjectFields.OBJECT_END + &H0                           ' 0x006 - Size: 2 - Type: GUID - Flags: PUBLIC
        CORPSE_FIELD_FACING = EObjectFields.OBJECT_END + &H2                          ' 0x008 - Size: 1 - Type: FLOAT - Flags: PUBLIC
        CORPSE_FIELD_POS_X = EObjectFields.OBJECT_END + &H3                           ' 0x009 - Size: 1 - Type: FLOAT - Flags: PUBLIC
        CORPSE_FIELD_POS_Y = EObjectFields.OBJECT_END + &H4                           ' 0x00A - Size: 1 - Type: FLOAT - Flags: PUBLIC
        CORPSE_FIELD_POS_Z = EObjectFields.OBJECT_END + &H5                           ' 0x00B - Size: 1 - Type: FLOAT - Flags: PUBLIC
        CORPSE_FIELD_DISPLAY_ID = EObjectFields.OBJECT_END + &H6                      ' 0x00C - Size: 1 - Type: INT - Flags: PUBLIC
        CORPSE_FIELD_ITEM = EObjectFields.OBJECT_END + &H7                            ' 0x00D - Size: 19 - Type: INT - Flags: PUBLIC
        CORPSE_FIELD_BYTES_1 = EObjectFields.OBJECT_END + &H1A                        ' 0x020 - Size: 1 - Type: BYTES - Flags: PUBLIC
        CORPSE_FIELD_BYTES_2 = EObjectFields.OBJECT_END + &H1B                        ' 0x021 - Size: 1 - Type: BYTES - Flags: PUBLIC
        CORPSE_FIELD_GUILD = EObjectFields.OBJECT_END + &H1C                          ' 0x022 - Size: 1 - Type: INT - Flags: PUBLIC
        CORPSE_FIELD_FLAGS = EObjectFields.OBJECT_END + &H1D                          ' 0x023 - Size: 1 - Type: INT - Flags: PUBLIC
        CORPSE_FIELD_DYNAMIC_FLAGS = EObjectFields.OBJECT_END + &H1E                  ' 0x024 - Size: 1 - Type: INT - Flags: DYNAMIC
        CORPSE_FIELD_PAD = EObjectFields.OBJECT_END + &H1F                            ' 0x025 - Size: 1 - Type: INT - Flags: NONE
        CORPSE_END = EObjectFields.OBJECT_END + &H20                                  ' 0x026
    End Enum
End NameSpace