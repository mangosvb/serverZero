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
    Public Class CreatureAI_Searing_Blade_Cultist
        Inherits WS_Creatures_AI.BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const COA_COOLDOWN As Integer = 15000

        Private Const COA_SPELL As Integer = 18266

        Public NextWaypoint As Integer = 0
        Public NextCOA As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As WS_Creatures.CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnThink()

            NextCOA -= AI_UPDATE

            If NextCOA <= 0 Then
                NextCOA = COA_COOLDOWN
                aiCreature.CastSpell(COA_SPELL, aiTarget) 'Curse of Agony
            End If
        End Sub

        Public Sub CastCOA()
            For i As Integer = 0 To 3
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(COA_SPELL, aiTarget)
            Next
        End Sub
    End Class
End Namespace