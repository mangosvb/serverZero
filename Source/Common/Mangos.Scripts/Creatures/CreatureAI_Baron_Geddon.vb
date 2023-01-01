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

Imports Mangos.Common.Enums.Chat
Imports Mangos.Common.Enums.Misc
Imports Mangos.World.AI
Imports Mangos.World.Objects

'AI TODO: Implement a workaround (Or fix, fixes work too!) for Armageddon.
Namespace Creatures
    Public Class CreatureAI_Baron_Geddon
        Inherits WS_Creatures_AI.BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const Inferno_CD As Integer = 45000
        Private Const Ignite_CD As Integer = 30000
        Private Const Living_Bomb_CD As Integer = 35000

        Private Const Spell_Inferno As Integer = 19695
        Private Const Spell_Ignite As Integer = 19659 'Drains a random targets mana.
        Private Const Spell_Living_Bomb As Integer = 20475
        Private Const Spell_Armageddon As Integer = 20478 'Cast at 2% to make self invincible, this spell won't work so we'll make a workaround.

        Public NextWaypoint As Integer = 0
        Public CurrentWaypoint As Integer = 0
        Public NextInferno As Integer = 0
        Public NextIgnite As Integer = 0
        Public NextLivingBomb As Integer = 0

        Public Sub New(ByRef Creature As WS_Creatures.CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnThink()

            NextInferno -= AI_UPDATE
            NextIgnite -= AI_UPDATE
            NextLivingBomb -= AI_UPDATE

            If NextInferno <= 0 Then
                NextInferno = Inferno_CD
                aiCreature.CastSpell(Spell_Inferno, aiTarget)
            End If

            If NextIgnite <= 0 Then
                NextIgnite = Ignite_CD
                aiCreature.CastSpell(Spell_Ignite, aiCreature.GetRandomTarget)
            End If

            If NextLivingBomb <= 0 Then
                NextLivingBomb = Living_Bomb_CD
                aiCreature.CastSpell(Spell_Living_Bomb, aiCreature.GetRandomTarget)
            End If
        End Sub

        Public Sub CastInferno()
            For i As Integer = 0 To 3
                Dim Target As WS_Base.BaseUnit = aiTarget
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(Spell_Inferno, aiTarget) 'This spell should be mitigated with fire resistance and nothing more.
            Next
        End Sub

        Public Sub CastIgnite()
            For i As Integer = 1 To 3
                Dim target As WS_Base.BaseUnit = aiCreature.GetRandomTarget
                If target Is Nothing Then Exit Sub
                aiCreature.CastSpell(Spell_Ignite, aiCreature.GetRandomTarget) 'This spell drains 400 mana per second and MUST be dispelled immediately or your healers will wipe the group.
            Next
        End Sub

        Public Sub CastLivingBomb()
            For i As Integer = 2 To 3
                Dim target As WS_Base.BaseUnit = aiCreature.GetRandomTarget
                If target Is Nothing Then Exit Sub
                aiCreature.CastSpell(Spell_Living_Bomb, aiCreature.GetRandomTarget) 'The traditional way of getting away of this is to run where the dead trash is from your group so they don't die, but we may need to fix AoE implementations for this.
            Next
        End Sub

        Public Overrides Sub OnHealthChange(Percent As Integer)
            MyBase.OnHealthChange(Percent)
            If Percent <= 2 Then
                Try
                    aiCreature.CastSpellOnSelf(Spell_Armageddon) 'I think during this time he's supposed to have a kamakazie of sorts.
                Catch ex As Exception
                    aiCreature.SendChatMessage("I have failed to become invincible at 2% or less HP. this is a problem.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            End If
        End Sub
    End Class
End Namespace
