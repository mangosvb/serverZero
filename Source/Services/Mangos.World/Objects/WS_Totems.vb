﻿'
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

Imports Mangos.Common
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Spell
Imports Mangos.World.Player
Imports Mangos.World.Server
Imports Mangos.World.Spells

Namespace Objects

    Public Class WS_Totems

        Public Class TotemObject
            Inherits WS_Creatures.CreatureObject

            Public Caster As WS_Base.BaseUnit = Nothing
            Public Duration As Integer = 0

            Private Type As TotemType = TotemType.TOTEM_PASSIVE

            Public Sub New(ByVal Entry As Integer, ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single, ByVal Orientation As Single, ByVal Map As Integer, Optional ByVal Duration_ As Integer = 0)
                MyBase.New(Entry, PosX, PosY, PosZ, Orientation, Map, Duration_)
                If aiScript IsNot Nothing Then aiScript.Dispose()
                aiScript = Nothing
                Duration = Duration_
            End Sub

            Public Sub InitSpell(ByVal SpellID As Integer)
                ApplySpell(SpellID)
            End Sub

            Public Sub Update()

                For i As Integer = 0 To _Global_Constants.MAX_AURA_EFFECTs - 1
                    If Not ActiveSpells(i) Is Nothing Then
                        If ActiveSpells(i).SpellDuration = _Global_Constants.SPELL_DURATION_INFINITE Then ActiveSpells(i).SpellDuration = Duration

                        'DONE: Count aura duration
                        If ActiveSpells(i).SpellDuration <> _Global_Constants.SPELL_DURATION_INFINITE Then
                            ActiveSpells(i).SpellDuration -= WS_TimerBasedEvents.TSpellManager.UPDATE_TIMER

                            'DONE: Cast aura (check if: there is aura; aura is periodic; time for next activation)
                            For j As Byte = 0 To 2
                                If ActiveSpells(i) IsNot Nothing AndAlso ActiveSpells(i).Aura(j) IsNot Nothing AndAlso
                                   ActiveSpells(i).Aura_Info(j).Amplitude <> 0 AndAlso
                                   ((Duration - ActiveSpells(i).SpellDuration) Mod ActiveSpells(i).Aura_Info(j).Amplitude) = 0 Then
                                    ActiveSpells(i).Aura(j).Invoke(Me, ActiveSpells(i).SpellCaster, ActiveSpells(i).Aura_Info(j), ActiveSpells(i).SpellID, ActiveSpells(i).StackCount + 1, AuraAction.AURA_UPDATE)
                                End If
                            Next

                            'DONE: Remove finished aura
                            If Not ActiveSpells(i) Is Nothing AndAlso ActiveSpells(i).SpellDuration <= 0 AndAlso ActiveSpells(i).SpellDuration <> _Global_Constants.SPELL_DURATION_INFINITE Then RemoveAura(i, ActiveSpells(i).SpellCaster, True)
                        End If

                        For j As Byte = 0 To 2
                            If ActiveSpells(i) IsNot Nothing AndAlso ActiveSpells(i).Aura_Info(j) IsNot Nothing Then
                                If ActiveSpells(i).Aura_Info(j).ID = SpellEffects_Names.SPELL_EFFECT_APPLY_AREA_AURA Then
                                    Dim Targets As New List(Of WS_Base.BaseUnit)
                                    If TypeOf Caster Is WS_PlayerData.CharacterObject Then
                                        Targets = _WS_Spells.GetPartyMembersAtPoint(CType(Caster, WS_PlayerData.CharacterObject), ActiveSpells(i).Aura_Info(j).GetRadius, positionX, positionY, positionZ)
                                    Else
                                        Targets = _WS_Spells.GetFriendAroundMe(Me, ActiveSpells(i).Aura_Info(j).GetRadius)
                                    End If

                                    For Each Unit As WS_Base.BaseUnit In Targets
                                        If Unit.HaveAura(ActiveSpells(i).SpellID) = False Then
                                            _WS_Spells.ApplyAura(Unit, Me, ActiveSpells(i).Aura_Info(j), ActiveSpells(i).SpellID)
                                        End If
                                    Next
                                End If
                            End If
                        Next

                    End If
                Next

            End Sub
        End Class
    End Class
End Namespace