Namespace Globals
    Public Enum EDynamicObjectFields
        DYNAMICOBJECT_CASTER = EObjectFields.OBJECT_END + &H0                         ' 0x006 - Size: 2 - Type: GUID - Flags: PUBLIC
        DYNAMICOBJECT_BYTES = EObjectFields.OBJECT_END + &H2                          ' 0x008 - Size: 1 - Type: BYTES - Flags: PUBLIC
        DYNAMICOBJECT_SPELLID = EObjectFields.OBJECT_END + &H3                        ' 0x009 - Size: 1 - Type: INT - Flags: PUBLIC
        DYNAMICOBJECT_RADIUS = EObjectFields.OBJECT_END + &H4                         ' 0x00A - Size: 1 - Type: FLOAT - Flags: PUBLIC
        DYNAMICOBJECT_POS_X = EObjectFields.OBJECT_END + &H5                          ' 0x00B - Size: 1 - Type: FLOAT - Flags: PUBLIC
        DYNAMICOBJECT_POS_Y = EObjectFields.OBJECT_END + &H6                          ' 0x00C - Size: 1 - Type: FLOAT - Flags: PUBLIC
        DYNAMICOBJECT_POS_Z = EObjectFields.OBJECT_END + &H7                          ' 0x00D - Size: 1 - Type: FLOAT - Flags: PUBLIC
        DYNAMICOBJECT_FACING = EObjectFields.OBJECT_END + &H8                         ' 0x00E - Size: 1 - Type: FLOAT - Flags: PUBLIC
        DYNAMICOBJECT_PAD = EObjectFields.OBJECT_END + &H9                            ' 0x00F - Size: 1 - Type: BYTES - Flags: PUBLIC
        DYNAMICOBJECT_END = EObjectFields.OBJECT_END + &HA                            ' 0x010
    End Enum
End NameSpace