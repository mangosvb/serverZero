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
    Public Module ChatEnum

        Public Enum ChatMsg As Integer
            CHAT_MSG_SAY = &H0
            CHAT_MSG_PARTY = &H1
            CHAT_MSG_RAID = &H2
            CHAT_MSG_GUILD = &H3
            CHAT_MSG_OFFICER = &H4
            CHAT_MSG_YELL = &H5
            CHAT_MSG_WHISPER = &H6
            CHAT_MSG_WHISPER_INFORM = &H7
            CHAT_MSG_EMOTE = &H8
            CHAT_MSG_TEXT_EMOTE = &H9
            CHAT_MSG_SYSTEM = &HA
            CHAT_MSG_MONSTER_SAY = &HB
            CHAT_MSG_MONSTER_YELL = &HC
            CHAT_MSG_MONSTER_EMOTE = &HD
            CHAT_MSG_CHANNEL = &HE
            CHAT_MSG_CHANNEL_JOIN = &HF
            CHAT_MSG_CHANNEL_LEAVE = &H10
            CHAT_MSG_CHANNEL_LIST = &H11
            CHAT_MSG_CHANNEL_NOTICE = &H12
            CHAT_MSG_CHANNEL_NOTICE_USER = &H13
            CHAT_MSG_AFK = &H14
            CHAT_MSG_DND = &H15
            CHAT_MSG_IGNORED = &H16
            CHAT_MSG_SKILL = &H17
            CHAT_MSG_LOOT = &H18
            CHAT_MSG_RAID_LEADER = &H57
            CHAT_MSG_RAID_WARNING = &H58
        End Enum

        <Flags()>
        Public Enum ChatFlag As Byte
            FLAGS_NONE = 0
            FLAGS_AFK = 1
            FLAGS_DND = 2
            FLAGS_GM = 3
        End Enum

    End Module
End NameSpace