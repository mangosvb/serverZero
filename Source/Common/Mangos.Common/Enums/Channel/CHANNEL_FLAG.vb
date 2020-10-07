Namespace Enums.Channel
    <Flags>
    Public Enum CHANNEL_FLAG As Byte
        'General                  0x18 = 0x10 | 0x08
        'Trade                    0x3C = 0x20 | 0x10 | 0x08 | 0x04
        'LocalDefence             0x18 = 0x10 | 0x08
        'GuildRecruitment         0x38 = 0x20 | 0x10 | 0x08
        'LookingForGroup          0x50 = 0x40 | 0x10

        CHANNEL_FLAG_NONE = &H0
        CHANNEL_FLAG_CUSTOM = &H1
        CHANNEL_FLAG_UNK1 = &H2
        CHANNEL_FLAG_TRADE = &H4
        CHANNEL_FLAG_NOT_LFG = &H8
        CHANNEL_FLAG_GENERAL = &H10
        CHANNEL_FLAG_CITY = &H20
        CHANNEL_FLAG_LFG = &H40
    End Enum
End NameSpace