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

Namespace Globals
    Public Module Functions
        Public Function GuidIsCreature(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_UNIT Then Return True
            Return False
        End Function

        Public Function GuidIsPet(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_PET Then Return True
            Return False
        End Function

        Public Function GuidIsItem(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_ITEM Then Return True
            Return False
        End Function

        Public Function GuidIsGameObject(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_GAMEOBJECT Then Return True
            Return False
        End Function

        Public Function GuidIsDnyamicObject(ByVal guid As ULong) As Boolean
            If GuidHigh2(GUID) = GUID_DYNAMICOBJECT Then Return True
            Return False
        End Function

        Public Function GuidIsTransport(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_TRANSPORT Then Return True
            Return False
        End Function

        Public Function GuidIsMoTransport(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_MO_TRANSPORT Then Return True
            Return False
        End Function

        Public Function GuidIsCorpse(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_CORPSE Then Return True
            Return False
        End Function

        Public Function GuidIsPlayer(ByVal guid As ULong) As Boolean
            If GuidHigh2(guid) = GUID_PLAYER Then Return True
            Return False
        End Function

        Public Function GuidHigh2(ByVal guid As ULong) As ULong
            Return (guid And GUID_MASK_HIGH)
        End Function

        Public Function GuidHigh(ByVal guid As ULong) As UInteger
            Return (guid And GUID_MASK_HIGH) >> 32UL
        End Function

        Public Function GuidLow(ByVal guid As ULong) As UInteger
            Return (guid And GUID_MASK_LOW)
        End Function

        Public Function GetShapeshiftModel(ByVal form As ShapeshiftForm, ByVal race As Races, ByVal model As Integer) As Integer
            Select Case form
                Case ShapeshiftForm.FORM_CAT
                    If race = Races.RACE_NIGHT_ELF Then Return 892
                    If race = Races.RACE_TAUREN Then Return 8571
                Case ShapeshiftForm.FORM_BEAR, ShapeshiftForm.FORM_DIREBEAR
                    If race = Races.RACE_NIGHT_ELF Then Return 2281
                    If race = Races.RACE_TAUREN Then Return 2289
                Case ShapeshiftForm.FORM_MOONKIN
                    If race = Races.RACE_NIGHT_ELF Then Return 15374
                    If race = Races.RACE_TAUREN Then Return 15375
                Case ShapeshiftForm.FORM_TRAVEL
                    Return 632
                Case ShapeshiftForm.FORM_AQUA
                    Return 2428
                Case ShapeshiftForm.FORM_FLIGHT
                    If race = Races.RACE_NIGHT_ELF Then Return 20857
                    If race = Races.RACE_TAUREN Then Return 20872
                Case ShapeshiftForm.FORM_SWIFT
                    If race = Races.RACE_NIGHT_ELF Then Return 21243
                    If race = Races.RACE_TAUREN Then Return 21244
                Case ShapeshiftForm.FORM_GHOUL
                    If race = Races.RACE_NIGHT_ELF Then Return 10045 Else Return model
                Case ShapeshiftForm.FORM_CREATUREBEAR
                    Return 902
                Case ShapeshiftForm.FORM_GHOSTWOLF
                    Return 4613
                Case ShapeshiftForm.FORM_SPIRITOFREDEMPTION
                    Return 12824
                Case Else
                    Return model
                    'Case ShapeshiftForm.FORM_CREATURECAT
                    'Case ShapeshiftForm.FORM_AMBIENT
                    'Case ShapeshiftForm.FORM_SHADOW
            End Select
        End Function

        Public Function GetShapeshiftManaType(ByVal form As ShapeshiftForm, ByVal manaType As ManaTypes) As ManaTypes
            Select Case form
                Case ShapeshiftForm.FORM_CAT, ShapeshiftForm.FORM_STEALTH
                    Return ManaTypes.TYPE_ENERGY
                Case ShapeshiftForm.FORM_AQUA, ShapeshiftForm.FORM_TRAVEL, ShapeshiftForm.FORM_MOONKIN,
                 ShapeshiftForm.FORM_MOONKIN, ShapeshiftForm.FORM_MOONKIN, ShapeshiftForm.FORM_SPIRITOFREDEMPTION, ShapeshiftForm.FORM_FLIGHT, ShapeshiftForm.FORM_SWIFT
                    Return ManaTypes.TYPE_MANA
                Case ShapeshiftForm.FORM_BEAR, ShapeshiftForm.FORM_DIREBEAR
                    Return ManaTypes.TYPE_RAGE
                Case Else
                    Return manaType
            End Select
        End Function
    End Module
End Namespace
