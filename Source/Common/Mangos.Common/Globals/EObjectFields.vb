Namespace Globals
    Public Enum EObjectFields
        OBJECT_FIELD_GUID = &H0                                                       ' 0x000 - Size: 2 - Type: GUID - Flags: PUBLIC
        OBJECT_FIELD_TYPE = &H2                                                       ' 0x002 - Size: 1 - Type: INT - Flags: PUBLIC
        OBJECT_FIELD_ENTRY = &H3                                                      ' 0x003 - Size: 1 - Type: INT - Flags: PUBLIC
        OBJECT_FIELD_SCALE_X = &H4                                                    ' 0x004 - Size: 1 - Type: FLOAT - Flags: PUBLIC
        OBJECT_FIELD_PADDING = &H5                                                    ' 0x005 - Size: 1 - Type: INT - Flags: NONE
        OBJECT_END = &H6
    End Enum
End NameSpace