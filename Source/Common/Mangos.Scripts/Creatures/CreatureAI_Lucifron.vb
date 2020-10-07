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
    Public Class CreatureAI_Lucifron
        Inherits WS_Creatures_AI.BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const Impending_Doom_Cooldown As Integer = 20000
        Private Const Lucifrons_Curse_Cooldown As Integer = 20000
        Private Const Shadow_Shock_Cooldown As Integer = 6000

        Private Const Impending_Doom As Integer = 19702
        Private Const Lucifrons_Curse As Integer = 19703
        Private Const Shadow_Shock As Integer = 19460

        Public Phase As Integer = 0
        Public NextImpendingDoom As Integer = 0
        Public NextLucifronsCurse As Integer = 0
        Public NextShadowShock As Integer = 0
        Public NextWaypoint As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As WS_Creatures.CreatureObject)
            MyBase.New(Creature)
            Phase = 0
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnEnterCombat()
            If Phase > 1 Then Exit Sub
            MyBase.OnEnterCombat()
            aiCreature.Flying = False
            AllowedAttack = True
            Phase = 1
            'ReinitSpells()
        End Sub

        Public Overrides Sub OnLeaveCombat(Optional ByVal Reset As Boolean = True)
            MyBase.OnLeaveCombat(Reset)
            AllowedAttack = True
            Phase = 0
        End Sub

        Public Overrides Sub OnKill(ByRef Victim As WS_Base.BaseUnit)
            'Does he cast a dummy spell on target death?
        End Sub

        Public Overrides Sub OnDeath()
            'Does he do anything on his own death?
        End Sub

        Public Overrides Sub OnThink()
            If Phase < 1 Then Exit Sub

            If (Phase = 1) Then
                NextImpendingDoom -= AI_UPDATE
                NextLucifronsCurse -= AI_UPDATE
                NextShadowShock -= AI_UPDATE

                If NextImpendingDoom <= 0 Then
                    NextImpendingDoom = Impending_Doom_Cooldown
                    aiCreature.CastSpell(Impending_Doom, aiTarget) 'Impending DOOOOOM!
                End If

                If NextLucifronsCurse <= 0 Then
                    NextLucifronsCurse = Lucifrons_Curse_Cooldown
                    aiCreature.CastSpell(Lucifrons_Curse, aiTarget) 'Lucifrons Curse.
                End If

                If NextShadowShock <= 0 Then
                    NextShadowShock = Shadow_Shock_Cooldown
                    aiCreature.CastSpell(Shadow_Shock, aiTarget) 'Summon Player
                End If
            End If

            If NextWaypoint > 0 Then
                NextWaypoint -= AI_UPDATE
                If NextWaypoint <= 0 Then
                    On_Waypoint()
                End If
            End If
        End Sub

        Public Sub Cast_Lucirons_Curse()
            For i As Integer = 0 To 2
                Dim theTarget As WS_Base.BaseUnit = aiCreature
                If theTarget Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(Lucifrons_Curse, aiTarget)
                Catch Ex As Exception
                    aiCreature.SendChatMessage("Failed to cast Lucifron's Curse. This is bad. Please report to developers.", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
                End Try
            Next
        End Sub

        Public Sub Cast_Impending_Doom()
            For i As Integer = 1 To 2
                Dim theTarget As WS_Base.BaseUnit = aiCreature
                If theTarget Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(Impending_Doom, aiTarget)
                Catch Ex As Exception
                    aiCreature.SendChatMessage("Failed to cast IMPENDING DOOOOOM! Please report this to a developer.", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
                End Try
            Next
        End Sub

        Public Sub Cast_Shadow_Shock()
            For i As Integer = 2 To 2
                Dim theTarget As WS_Base.BaseUnit = aiCreature.GetRandomTarget
                If theTarget Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(Shadow_Shock, theTarget.positionX, theTarget.positionY, theTarget.positionZ)
                Catch Ex As Exception
                    aiCreature.SendChatMessage("Failed to cast Shadow Shock. Please report this to a developer.", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
                End Try
            Next
        End Sub

        Public Sub On_Waypoint()
            Select Case CurrentWaypoint
                Case 0
                    NextWaypoint = aiCreature.MoveTo(-0.0F, -0.0F, -0.0F, 0.0F, True) 'No Waypoint Coords! Will need to back track from MaNGOS!
                Case 1
                    NextWaypoint = 10000
                    'NextSummon = NextWaypoint
                    aiCreature.MoveTo(0.0F, -0.0F, -0.0F, 0.0F)
                Case 2
                    NextWaypoint = 23000
                Case 3
                    NextWaypoint = 10000
                    aiCreature.MoveTo(0.0F, -0.0F, -0.0F, 0.0F)
                Case 4, 6, 8, 10, 12
                    NextWaypoint = 23000
                Case 5
                    NextWaypoint = 10000
                    aiCreature.MoveTo(-0.0F, -0.0F, -0.0F, 0.0F)
                Case 7
                    NextWaypoint = 10000
                    aiCreature.MoveTo(-0.0F, -0.0F, -0.0F, 0.0F)
                Case 9
                    NextWaypoint = 10000
                    aiCreature.MoveTo(0.0F, -0.0F, -0.0F, 0.0F)
                Case 11
                    NextWaypoint = 10000
                    aiCreature.MoveTo(-0.0F, -0.0F, -0.0F, 0.0F)
            End Select
            CurrentWaypoint += 1
            If CurrentWaypoint > 12 Then CurrentWaypoint = 3
        End Sub
    End Class
End Namespace