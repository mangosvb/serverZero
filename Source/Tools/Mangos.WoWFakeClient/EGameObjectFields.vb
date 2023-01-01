'
' Copyright (C) 2013-2023 getMaNGOS <https://getmangos.eu>
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

Public Enum EGameObjectFields
    OBJECT_FIELD_CREATED_BY = EObjectFields.OBJECT_END + &H0                      ' 0x006 - Size: 2 - Type: GUID - Flags: PUBLIC
    GAMEOBJECT_DISPLAYID = EObjectFields.OBJECT_END + &H2                         ' 0x008 - Size: 1 - Type: INT - Flags: PUBLIC
    GAMEOBJECT_FLAGS = EObjectFields.OBJECT_END + &H3                             ' 0x009 - Size: 1 - Type: INT - Flags: PUBLIC
    GAMEOBJECT_ROTATION = EObjectFields.OBJECT_END + &H4                          ' 0x00A - Size: 4 - Type: FLOAT - Flags: PUBLIC
    GAMEOBJECT_STATE = EObjectFields.OBJECT_END + &H8                             ' 0x00E - Size: 1 - Type: INT - Flags: PUBLIC
    GAMEOBJECT_POS_X = EObjectFields.OBJECT_END + &H9                             ' 0x010 - Size: 1 - Type: FLOAT - Flags: PUBLIC
    GAMEOBJECT_POS_Y = EObjectFields.OBJECT_END + &HA                             ' 0x011 - Size: 1 - Type: FLOAT - Flags: PUBLIC
    GAMEOBJECT_POS_Z = EObjectFields.OBJECT_END + &HB                             ' 0x012 - Size: 1 - Type: FLOAT - Flags: PUBLIC
    GAMEOBJECT_FACING = EObjectFields.OBJECT_END + &HC                            ' 0x013 - Size: 1 - Type: FLOAT - Flags: PUBLIC
    GAMEOBJECT_DYN_FLAGS = EObjectFields.OBJECT_END + &HD                         ' 0x014 - Size: 1 - Type: INT - Flags: DYNAMIC
    GAMEOBJECT_FACTION = EObjectFields.OBJECT_END + &HE                           ' 0x015 - Size: 1 - Type: INT - Flags: PUBLIC
    GAMEOBJECT_TYPE_ID = EObjectFields.OBJECT_END + &HF                          ' 0x016 - Size: 1 - Type: INT - Flags: PUBLIC
    GAMEOBJECT_LEVEL = EObjectFields.OBJECT_END + &H10                            ' 0x017 - Size: 1 - Type: INT - Flags: PUBLIC
    GAMEOBJECT_ARTKIT = EObjectFields.OBJECT_END + &H11
    GAMEOBJECT_ANIMPROGRESS = EObjectFields.OBJECT_END + &H12
    GAMEOBJECT_PADDING = EObjectFields.OBJECT_END + &H13
    GAMEOBJECT_END = EObjectFields.OBJECT_END + &H14                              ' 0x018
End Enum