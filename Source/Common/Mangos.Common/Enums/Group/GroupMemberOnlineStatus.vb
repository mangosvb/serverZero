Namespace Enums.Group
    <Flags>
    Public Enum GroupMemberOnlineStatus
        MEMBER_STATUS_OFFLINE = &H0
        MEMBER_STATUS_ONLINE = &H1
        MEMBER_STATUS_PVP = &H2
        MEMBER_STATUS_DEAD = &H4            ' dead (health=0)
        MEMBER_STATUS_GHOST = &H8           ' ghost (health=1)
        MEMBER_STATUS_PVP_FFA = &H10        ' pvp ffa
        MEMBER_STATUS_UNK3 = &H20           ' unknown
        MEMBER_STATUS_AFK = &H40            ' afk flag
        MEMBER_STATUS_DND = &H80            ' dnd flag
    End Enum
End NameSpace