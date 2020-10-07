Namespace Enums.Social
    Public Enum FriendResult As Byte
        FRIEND_DB_ERROR = &H0
        FRIEND_LIST_FULL = &H1
        FRIEND_ONLINE = &H2
        FRIEND_OFFLINE = &H3
        FRIEND_NOT_FOUND = &H4
        FRIEND_REMOVED = &H5
        FRIEND_ADDED_ONLINE = &H6
        FRIEND_ADDED_OFFLINE = &H7
        FRIEND_ALREADY = &H8
        FRIEND_SELF = &H9
        FRIEND_ENEMY = &HA
        FRIEND_IGNORE_FULL = &HB
        FRIEND_IGNORE_SELF = &HC
        FRIEND_IGNORE_NOT_FOUND = &HD
        FRIEND_IGNORE_ALREADY = &HE
        FRIEND_IGNORE_ADDED = &HF
        FRIEND_IGNORE_REMOVED = &H10
    End Enum
End NameSpace