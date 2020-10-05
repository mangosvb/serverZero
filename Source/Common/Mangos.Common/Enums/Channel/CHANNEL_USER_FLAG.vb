Namespace Enums.Channel
    <Flags>
    Public Enum CHANNEL_USER_FLAG As Byte
        CHANNEL_FLAG_NONE = &H0
        CHANNEL_FLAG_OWNER = &H1
        CHANNEL_FLAG_MODERATOR = &H2
        CHANNEL_FLAG_MUTED = &H4
        CHANNEL_FLAG_CUSTOM = &H10
    End Enum
End NameSpace