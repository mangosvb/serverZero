'
' Copyright (C) 2013-2021 getMaNGOS <https://getmangos.eu>
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

Namespace Enums.Character
    Public Enum CharResponse As Byte
        CHAR_LIST_FAILED = &H2C
        CHAR_CREATE_SUCCESS = &H2E
        CHAR_CREATE_ERROR = &H2F
        CHAR_CREATE_FAILED = &H30
        CHAR_CREATE_NAME_IN_USE = &H31
        CHAR_CREATE_DISABLED = &H32
        CHAR_CREATE_PVP_TEAMS_VIOLATION = &H33
        CHAR_CREATE_SERVER_LIMIT = &H34
        CHAR_CREATE_ACCOUNT_LIMIT = &H35
        CHAR_DELETE_SUCCESS = &H39
        CHAR_DELETE_FAILED = &H3A
        CHAR_LOGIN_NO_WORLD = &H3D
        CHAR_LOGIN_FAILED = &H40
        CHAR_NAME_INVALID_CHARACTER = &H46
    End Enum
End Namespace