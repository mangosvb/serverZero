'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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
Namespace Enums
    Public Module GuildEnum

        Public Enum GuildRankRights
            GR_RIGHT_EMPTY = &H40
            GR_RIGHT_GCHATLISTEN = &H41
            GR_RIGHT_GCHATSPEAK = &H42
            GR_RIGHT_OFFCHATLISTEN = &H44
            GR_RIGHT_OFFCHATSPEAK = &H48
            GR_RIGHT_PROMOTE = &HC0
            GR_RIGHT_DEMOTE = &H140
            GR_RIGHT_INVITE = &H50
            GR_RIGHT_REMOVE = &H60
            GR_RIGHT_SETMOTD = &H1040
            GR_RIGHT_EPNOTE = &H2040
            GR_RIGHT_VIEWOFFNOTE = &H4040
            GR_RIGHT_EOFFNOTE = &H8040
            GR_RIGHT_ALL = &HF1FF
        End Enum

        Public Enum GuildEvent As Byte
            PROMOTION = 0           'uint8(2), string(name), string(rankName)
            DEMOTION = 1            'uint8(2), string(name), string(rankName)
            MOTD = 2                'uint8(1), string(text)                                             'Guild message of the day: <text>
            JOINED = 3              'uint8(1), string(name)                                             '<name> has joined the guild.
            LEFT = 4                'uint8(1), string(name)                                             '<name> has left the guild.
            REMOVED = 5             '??
            LEADER_IS = 6           'uint8(1), string(name                                              '<name> is the leader of your guild.
            LEADER_CHANGED = 7      'uint8(2), string(oldLeaderName), string(newLeaderName) 
            DISBANDED = 8           'uint8(0)                                                           'Your guild has been disbanded.
            TABARDCHANGE = 9        '??
            SIGNED_ON = 12
            SIGNED_OFF = 13
        End Enum

        'Default Guild Ranks
        'TODO: Set the ranks during guild creation
        Public Enum GuildDefaultRanks As Byte
            GR_GUILDMASTER = 0
            GR_OFFICER = 1
            GR_VETERAN = 2
            GR_MEMBER = 3
            GR_INITIATE = 4
        End Enum

        Public Enum GuildCommand As Byte
            GUILD_CREATE_S = &H0
            GUILD_INVITE_S = &H1
            GUILD_QUIT_S = &H2
            GUILD_FOUNDER_S = &HC
        End Enum

        Public Enum GuildError As Byte
            GUILD_PLAYER_NO_MORE_IN_GUILD = &H0
            GUILD_INTERNAL = &H1
            GUILD_ALREADY_IN_GUILD = &H2
            ALREADY_IN_GUILD = &H3
            INVITED_TO_GUILD = &H4
            ALREADY_INVITED_TO_GUILD = &H5
            GUILD_NAME_INVALID = &H6
            GUILD_NAME_EXISTS = &H7
            GUILD_LEADER_LEAVE = &H8
            GUILD_PERMISSIONS = &H8
            GUILD_PLAYER_NOT_IN_GUILD = &H9
            GUILD_PLAYER_NOT_IN_GUILD_S = &HA
            GUILD_PLAYER_NOT_FOUND = &HB
            GUILD_NOT_ALLIED = &HC
        End Enum

        Public Enum PetitionSignError As Integer
            PETITIONSIGN_OK = 0                     ':Closes the window
            PETITIONSIGN_ALREADY_SIGNED = 1         'You have already signed that guild charter
            PETITIONSIGN_ALREADY_IN_GUILD = 2       'You are already in a guild
            PETITIONSIGN_CANT_SIGN_OWN = 3          'You can's sign own guild charter
            PETITIONSIGN_NOT_SERVER = 4             'That player is not from your server
        End Enum

        Public Enum PetitionTurnInError As Integer
            PETITIONTURNIN_OK = 0                   ':Closes the window
            PETITIONTURNIN_ALREADY_IN_GUILD = 2     'You are already in a guild
            PETITIONTURNIN_NEED_MORE_SIGNATURES = 4 'You need more signatures
        End Enum

    End Module
End NameSpace