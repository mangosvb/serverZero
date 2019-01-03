'
' Copyright (C) 2013-2019 getMaNGOS <https://getmangos.eu>
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

Public Module CharacterEnum

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

    <Flags()>
    Public Enum CharacterFlagState
        CHARACTER_FLAG_NONE = &H0
        CHARACTER_FLAG_UNK1 = &H1
        CHARACTER_FLAG_UNK2 = &H2
        CHARACTER_FLAG_LOCKED_FOR_TRANSFER = &H4                    'Character Locked for Paid Character Transfer
        CHARACTER_FLAG_UNK4 = &H8
        CHARACTER_FLAG_UNK5 = &H10
        CHARACTER_FLAG_UNK6 = &H20
        CHARACTER_FLAG_UNK7 = &H40
        CHARACTER_FLAG_UNK8 = &H80
        CHARACTER_FLAG_UNK9 = &H100
        CHARACTER_FLAG_UNK10 = &H200
        CHARACTER_FLAG_HIDE_HELM = &H400
        CHARACTER_FLAG_HIDE_CLOAK = &H800
        CHARACTER_FLAG_UNK13 = &H1000
        CHARACTER_FLAG_GHOST = &H2000                               'Player is ghost in char selection screen
        CHARACTER_FLAG_RENAME = &H4000                              'On login player will be asked to change name
        CHARACTER_FLAG_UNK16 = &H8000
        CHARACTER_FLAG_UNK17 = &H10000
        CHARACTER_FLAG_UNK18 = &H20000
        CHARACTER_FLAG_UNK19 = &H40000
        CHARACTER_FLAG_UNK20 = &H80000
        CHARACTER_FLAG_UNK21 = &H100000
        CHARACTER_FLAG_UNK22 = &H200000
        CHARACTER_FLAG_UNK23 = &H400000
        CHARACTER_FLAG_UNK24 = &H800000
        CHARACTER_FLAG_LOCKED_BY_BILLING = &H1000000
        CHARACTER_FLAG_DECLINED = &H2000000
        CHARACTER_FLAG_UNK27 = &H4000000
        CHARACTER_FLAG_UNK28 = &H8000000
        CHARACTER_FLAG_UNK29 = &H10000000
        CHARACTER_FLAG_UNK30 = &H20000000
        CHARACTER_FLAG_UNK31 = &H40000000
        CHARACTER_FLAG_UNK32 = &H80000000
    End Enum

End Module
