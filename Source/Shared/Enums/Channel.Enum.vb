'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Public Module ChannelEnum

    <Flags()>
    Public Enum ChatChannelsFlags
        FLAG_NONE = &H0
        FLAG_INITIAL = &H1              ' General, Trade, LocalDefense, LFG
        FLAG_ZONE_DEP = &H2             ' General, Trade, LocalDefense, GuildRecruitment
        FLAG_GLOBAL = &H4               ' WorldDefense
        FLAG_TRADE = &H8                ' Trade
        FLAG_CITY_ONLY = &H10           ' Trade, GuildRecruitment
        FLAG_CITY_ONLY2 = &H20          ' Trade, GuildRecruitment
        FLAG_DEFENSE = &H10000          ' LocalDefense, WorldDefense
        FLAG_GUILD_REQ = &H20000        ' GuildRecruitment
        FLAG_LFG = &H40000              ' LookingForGroup
    End Enum

    <Flags()>
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

    <Flags()>
    Public Enum CHANNEL_USER_FLAG As Byte
        CHANNEL_FLAG_NONE = &H0
        CHANNEL_FLAG_OWNER = &H1
        CHANNEL_FLAG_MODERATOR = &H2
        CHANNEL_FLAG_MUTED = &H4
        CHANNEL_FLAG_CUSTOM = &H10
    End Enum

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

End Module
