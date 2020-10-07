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

Imports Mangos.World.AI
Imports Mangos.World.Objects

Namespace Creatures
    Public Class CreatureAI_Ragefire_Shaman
        Inherits WS_Creatures_AI.BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const HEAL_COOLDOWN As Integer = 8000
        Private Const BOLT_COOLDOWN As Integer = 3000

        Private Const HEAL_SPELL As Integer = 11986
        Private Const BOLT_SPELL As Integer = 9532

        Public NextWaypoint As Integer = 0
        Public NextHeal As Integer = 0
        Public NextBolt As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As WS_Creatures.CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub
        Public Overrides Sub OnThink()

            NextHeal -= AI_UPDATE
            NextBolt -= AI_UPDATE

            If NextHeal <= 0 Then
                NextHeal = HEAL_COOLDOWN
                aiCreature.CastSpell(HEAL_SPELL, aiTarget) 'HEALING WAVE
            End If

            If NextBolt <= 1 Then
                NextBolt = BOLT_COOLDOWN
                aiCreature.CastSpell(BOLT_SPELL, aiTarget) 'LIGHTNING BOLT
            End If
        End Sub

        Public Sub CastHeal()
            For i As Integer = 0 To 1
                Dim Target As WS_Base.BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(HEAL_SPELL, aiTarget)
            Next
        End Sub

        Public Sub CastBolt()
            For i As Integer = 1 To 1
                Dim Target As WS_Base.BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(BOLT_SPELL, aiTarget)
            Next
        End Sub
    End Class
End Namespace