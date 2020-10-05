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

Public Module PetEnum

    Public Enum PetType As Byte
        SUMMON_PET = 0
        HUNTER_PET = 1
        GUARDIAN_PET = 2
        MINI_PET = 3
    End Enum

    Public Enum PetSaveType As Integer
        PET_SAVE_DELETED = -1
        PET_SAVE_CURRENT = 0
        PET_SAVE_IN_STABLE_1 = 1
        PET_SAVE_IN_STABLE_2 = 2
        PET_SAVE_NO_SLOT = 3
    End Enum

    Public Enum HappinessState As Byte
        UNHAPPY = 1
        CONTENT = 2
        HAPPY = 3
    End Enum

    Public Enum LoyaltyState As Byte
        REBELLIOUS = 1
        UNRULY = 2
        SUBMISSIVE = 3
        DEPENDABLE = 4
        FAITHFUL = 5
        BEST_FRIEND = 6
    End Enum

    Public Enum PetSpellState As Byte
        SPELL_UNCHANGED = 0
        SPELL_CHANGED = 1
        SPELL_NEW = 2
        SPELL_REMOVED = 3
    End Enum

    Public Enum ActionFeedback As Byte
        FEEDBACK_NONE = 0
        FEEDBACK_PET_DEAD = 1
        FEEDBACK_NO_TARGET = 2
        FEEDBACK_CANT_ATT = 3
    End Enum

    Public Enum PetTalk As Byte
        PET_TALK_SPECIAL_SPELL = 0
        PET_TALK_ATTACK = 1
    End Enum

End Module
