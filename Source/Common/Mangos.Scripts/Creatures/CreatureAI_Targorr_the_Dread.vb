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

Imports Mangos.Common.Enums.Chat
Imports Mangos.Common.Enums.Misc
Imports Mangos.World.AI
Imports Mangos.World.Objects

Namespace Creatures
    Public Class CreatureAI_Targorr_the_Dread
        Inherits WS_Creatures_AI.BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const ThrashCD As Integer = 7000
        Private Const FrenzyCD As Integer = 90000 'This should never be reused.

        Private Const Spell_Frenzy As Integer = 8599
        Private Const Spell_Thrash As Integer = 3391

        Public NextThrash As Integer = 0
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

            NextThrash -= AI_UPDATE

            If NextThrash <= 0 Then
                NextThrash = ThrashCD
                aiCreature.CastSpellOnSelf(Spell_Thrash) 'Should be cast on self. Correct me if wrong.
            End If
        End Sub

        Public Sub CastThrash()
            For i As Integer = 0 To 0
                Dim Target As WS_Base.BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpellOnSelf(Spell_Thrash)
                Catch ex As Exception
                    aiCreature.SendChatMessage("AI was unable to cast Thrash on himself. Please report this to a developer.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            Next
        End Sub

        Public Overrides Sub OnHealthChange(Percent As Integer)
            MyBase.OnHealthChange(Percent)
            If Percent <= 40 Then
                Try
                    aiCreature.CastSpellOnSelf(Spell_Frenzy)
                Catch ex As Exception
                    aiCreature.SendChatMessage("AI was unable to cast Frenzy on himself. Please report this to a developer.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            End If
        End Sub
    End Class
End Namespace