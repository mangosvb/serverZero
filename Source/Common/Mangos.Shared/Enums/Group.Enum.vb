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

Public Module GroupEnum

    <Flags()>
    Public Enum GroupType As Byte
        PARTY = 0
        RAID = 1
    End Enum

    <Flags()>
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

    Public Enum GroupDungeonDifficulty As Byte
        DIFFICULTY_NORMAL = 0
        DIFFICULTY_HEROIC = 1
    End Enum

    Public Enum GroupLootMethod As Byte
        LOOT_FREE_FOR_ALL = 0
        LOOT_ROUND_ROBIN = 1
        LOOT_MASTER = 2
        LOOT_GROUP = 3
        LOOT_NEED_BEFORE_GREED = 4
    End Enum

    Public Enum GroupLootThreshold As Byte
        Uncommon = 2
        Rare = 3
        Epic = 4
    End Enum

    Public Enum PartyCommand As Byte
        PARTY_OP_INVITE = 0
        PARTY_OP_LEAVE = 2
    End Enum

    Public Enum PartyCommandResult As Byte
        INVITE_OK = 0                   'You have invited [name] to join your group.
        INVITE_NOT_FOUND = 1            'Cannot find [name].
        INVITE_NOT_IN_YOUR_PARTY = 2    '[name] is not in your party.
        INVITE_NOT_IN_YOUR_INSTANCE = 3 '[name] is not in your instance.
        INVITE_PARTY_FULL = 4           'Your party is full.
        INVITE_ALREADY_IN_GROUP = 5     '[name] is already in group.
        INVITE_NOT_IN_PARTY = 6         'You aren't in party.
        INVITE_NOT_LEADER = 7           'You are not the party leader.
        INVITE_NOT_SAME_SIDE = 8        'gms - Target is not part of your alliance.
        INVITE_IGNORED = 9              '[name] is ignoring you.
        INVITE_RESTRICTED = 13
    End Enum

    Private Enum PromoteToMain As Byte
        MainTank = 0
        MainAssist = 1
    End Enum

End Module
