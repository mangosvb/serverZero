Namespace Enums.Player
    <Flags>
    Public Enum PlayerFlags As Integer
        PLAYER_FLAGS_GROUP_LEADER = &H1
        PLAYER_FLAGS_AFK = &H2
        PLAYER_FLAGS_DND = &H4
        PLAYER_FLAGS_GM = &H8                        'GM Prefix
        PLAYER_FLAGS_DEAD = &H10
        PLAYER_FLAGS_RESTING = &H20
        PLAYER_FLAGS_UNK7 = &H40                    'Admin Prefix?
        PLAYER_FLAGS_FFA_PVP = &H80
        PLAYER_FLAGS_CONTESTED_PVP = &H100
        PLAYER_FLAGS_IN_PVP = &H200
        PLAYER_FLAGS_HIDE_HELM = &H400
        PLAYER_FLAGS_HIDE_CLOAK = &H800
        PLAYER_FLAGS_PARTIAL_PLAY_TIME = &H1000
        PLAYER_FLAGS_IS_OUT_OF_BOUNDS = &H4000      'Out of Bounds
        PLAYER_FLAGS_UNK15 = &H8000                 'Dev Prefix?
        PLAYER_FLAGS_SANCTUARY = &H10000
        PLAYER_FLAGS_NO_PLAY_TIME = &H2000
        PLAYER_FLAGS_PVP_TIMER = &H40000
    End Enum
End NameSpace