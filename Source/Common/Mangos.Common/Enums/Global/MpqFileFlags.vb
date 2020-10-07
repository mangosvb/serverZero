Namespace Enums.Global
    Public Enum MpqFileFlags As Long
        MPQ_Changed = 1                     '&H00000001
        MPQ_Protected = 2                   '&H00000002
        MPQ_CompressedPK = 256              '&H00000100
        MPQ_CompressedMulti = 512           '&H00000200
        MPQ_Compressed = 65280              '&H0000FF00
        MPQ_Encrypted = 65536               '&H00010000
        MPQ_FixSeed = 131072                '&H00020000
        MPQ_SingleUnit = 16777216           '&H01000000
        MPQ_Unknown_02000000 = 33554432     '&H02000000 - The file is only 1 byte long and its name is a hash
        MPQ_FileHasMetadata = 67108864      '&H04000000 - Indicates the file has associted metadata.
        MPQ_Exists = 2147483648             '&H80000000
    End Enum
End NameSpace