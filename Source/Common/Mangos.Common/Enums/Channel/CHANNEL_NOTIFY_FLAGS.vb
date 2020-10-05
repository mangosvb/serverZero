Namespace Enums.Channel
    Public Enum CHANNEL_NOTIFY_FLAGS
        CHANNEL_JOINED = 0                      ' %s joined channel.
        CHANNEL_LEFT = 1                        ' %s left channel.
        CHANNEL_YOU_JOINED = 2                  ' Joined Channel: [%s]
        CHANNEL_YOU_LEFT = 3                    ' Left Channel: [%s]
        CHANNEL_WRONG_PASS = 4                  ' Wrong password for %s.
        CHANNEL_NOT_ON = 5                      ' Not on channel %s.
        CHANNEL_NOT_MODERATOR = 6               ' Not a moderator of %s.
        CHANNEL_SET_PASSWORD = 7                ' [%s] Password changed by %s.
        CHANNEL_CHANGE_OWNER = 8                ' [%s] Owner changed to %s.
        CHANNEL_NOT_ON_FOR_NAME = 9             ' [%s] Player %s was not found.
        CHANNEL_NOT_OWNER = &HA                 ' [%s] You are not the channel owner.
        CHANNEL_WHO_OWNER = &HB                 ' [%s] Channel owner is %s.
        CHANNEL_MODE_CHANGE = &HC               '
        CHANNEL_ENABLE_ANNOUNCE = &HD           ' [%s] Channel announcements enabled by %s.
        CHANNEL_DISABLE_ANNOUNCE = &HE          ' [%s] Channel announcements disabled by %s.
        CHANNEL_MODERATED = &HF                 ' [%s] Channel moderation enabled by %s.
        CHANNEL_UNMODERATED = &H10              ' [%s] Channel moderation disabled by %s.
        CHANNEL_YOUCANTSPEAK = &H11             ' [%s] You do not have permission to speak.
        CHANNEL_KICKED = &H12                   ' [%s] Player %s kicked by %s.
        CHANNEL_YOU_ARE_BANNED = &H13           ' [%s] You are banned from that channel.
        CHANNEL_BANNED = &H14                   ' [%s] Player %s banned by %s.
        CHANNEL_UNBANNED = &H15                 ' [%s] Player %s unbanned by %s.
        CHANNEL_NOT_BANNED = &H16               ' [%s] Player %s is not banned.
        CHANNEL_ALREADY_ON = &H17               ' [%s] Player %s is already on the channel.
        CHANNEL_INVITED = &H18                  ' %s has invited you to join the channel '%s'
        CHANNEL_INVITED_WRONG_FACTION = &H19    ' Target is in the wrong alliance for %s.
        CHANNEL_WRONG_FACTION = &H1A            ' Wrong alliance for %s.
        CHANNEL_INVALID_NAME = &H1B             ' Invalid channel name
        CHANNEL_NOT_MODERATED = &H1C            ' %s is not moderated
        CHANNEL_PLAYER_INVITED = &H1D           ' [%s] You invited %s to join the channel
        CHANNEL_PLAYER_INVITE_BANNED = &H1E     ' [%s] %s has been banned.
        CHANNEL_THROTTLED = &H1F                ' [%s] The number of messages that can be sent to this channel is limited, please wait to send another message.
        CHANNEL_NOT_IN_AREA = &H20              ' [%s] You are not in the correct area for this channel.
        CHANNEL_NOT_IN_LFG = &H21               ' [%s] You must be queued in looking for group before joining this channel.
    End Enum
End NameSpace