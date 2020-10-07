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
    Public Class CreatureAI_Earthborer
        Inherits WS_Creatures_AI.BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const ACID_COOLDOWN As Integer = 10000
        Private Const ACID_SPELL As Integer = 18070

        Public NextWaypoint As Integer = 0
        Public NextAcid As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As WS_Creatures.CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnThink()
            NextAcid -= AI_UPDATE

            If NextAcid <= 0 Then
                NextAcid = ACID_COOLDOWN
                aiCreature.CastSpell(ACID_SPELL, aiTarget) 'Earthborer Acid
            End If
        End Sub

        Public Sub CastAcid()
            For i As Integer = 0 To 3
                Dim Target As WS_Base.BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(ACID_SPELL, aiTarget)
            Next
        End Sub
    End Class
End Namespace