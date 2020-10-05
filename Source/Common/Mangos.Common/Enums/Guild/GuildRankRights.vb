'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Namespace Enums.Guild
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

    'Default Guild Ranks
    'TODO: Set the ranks during guild creation
End Namespace