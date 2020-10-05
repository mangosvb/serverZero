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

Imports Mangos.Common.Enums
Imports Mangos.World.AI
Imports Mangos.World.Objects

Namespace Creatures
    Public Class CreatureAI_Onyxia
        Inherits WS_Creatures_AI.BossAI

        Private Const AI_UPDATE As Integer = 1000
        Private Const BREATH_COOLDOWN As Integer = 11000
        Private Const WING_BUFFET_COOLDOWN As Integer = 15000
        Private Const CLEAVE_COOLDOWN As Integer = 3000
        Private Const KNOCK_COOLDOWN As Integer = 22000
        Private Const ROAR_COOLDOWN As Integer = 18000
        Private Const FIREBALL_COOLDOWN As Integer = 5000

        Private Const BREATH_SPELL As Integer = 18435
        Private Const WING_BUFFET_SPELL As Integer = 18500
        Private Const CLEAVE_SPELL As Integer = 19983
        Private Const TAIL_SWEEP_SPELL As Integer = 15847
        Private Const KNOCK_SPELL As Integer = 19633
        Private Const ROAR_SPELL As Integer = 18431
        Private Const FIREBALL_SPELL As Integer = 18392

        Private Const WHELP_CREATURE As Integer = 11262

        Public Phase As Integer = 0
        Public NextWaypoint As Integer = 0
        Public NextBreathe As Integer = 0
        Public NextWingBuffet As Integer = 0
        Public NextCleave As Integer = 0
        Public NextFireball As Integer = 0
        Public KnockTimer As Integer = 0
        Public RoarTimer As Integer = 0
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
            ReinitSpells()
            aiCreature.SendChatMessage("How fortuitous, usually I must leave my lair to feed!", ChatEnum.ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        End Sub

        Public Overrides Sub OnLeaveCombat(Optional ByVal Reset As Boolean = True)
            MyBase.OnLeaveCombat(Reset)
            AllowedAttack = True
            Phase = 0
            aiCreature.SendChatMessage("LEAVING COMBAT!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        End Sub

        Public Overrides Sub OnKill(ByRef Victim As BaseUnit)
            'TODO: Yell
            'TODO: Send sound (Die mortal?)!
            aiCreature.SendChatMessage("Die mortal, $N!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL, Victim.GUID)
        End Sub

        Public Overrides Sub OnHealthChange(ByVal Percent As Integer)
            aiCreature.SendChatMessage("My health: " & Percent & "%! (" & Phase & ")", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
            If Phase = 1 Then
                If Percent <= 65 Then
                    Phase = 2
                    Go_FlyPhase()
                End If
            ElseIf Phase = 2 Then
                If Percent <= 40 Then
                    Phase = 3
                    Go_LastPhase()
                End If
            End If
        End Sub

        Public Overrides Sub OnDeath()
            'TODO: Yell
            aiCreature.SendChatMessage("I shouldn't have died yet! Feed my kids plx!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        End Sub

        Public Overrides Sub OnThink()
            If Phase < 1 Then Exit Sub

            If (Phase = 1 OrElse Phase = 3) Then
                NextBreathe -= AI_UPDATE
                NextCleave -= AI_UPDATE
                NextWingBuffet -= AI_UPDATE

                If NextBreathe <= 0 Then
                    NextBreathe = BREATH_COOLDOWN
                    aiCreature.CastSpell(BREATH_SPELL, aiTarget) 'Flame breathe
                End If
                If NextWingBuffet <= 0 Then
                    NextWingBuffet = WING_BUFFET_COOLDOWN
                    aiCreature.CastSpell(WING_BUFFET_SPELL, aiTarget) 'Wing buffet
                End If
                If NextCleave <= 0 Then
                    NextCleave = CLEAVE_COOLDOWN
                    aiCreature.CastSpell(CLEAVE_SPELL, aiTarget) 'Cleave
                    'aiCreature.CastSpell(TAIL_SWEEP_SPELL, aiTarget) 'Tail Sweep
                End If

                If Phase = 3 Then
                    KnockTimer -= AI_UPDATE
                    RoarTimer -= AI_UPDATE

                    If KnockTimer <= 0 Then
                        KnockTimer = KNOCK_COOLDOWN
                        aiCreature.CastSpell(KNOCK_SPELL, aiTarget) 'Knock Away
                        aiHateTable(aiTarget) *= 0.75F
                    End If
                    If RoarTimer <= 0 Then
                        RoarTimer = ROAR_COOLDOWN
                        aiCreature.CastSpell(ROAR_SPELL, aiTarget) 'Bellowing Roar
                    End If
                End If

            ElseIf Phase = 2 Then
                NextFireball -= AI_UPDATE

                If NextFireball <= 0 Then
                    NextFireball = FIREBALL_COOLDOWN
                    CastFireball()
                End If
            End If

            If NextWaypoint > 0 Then
                NextWaypoint -= AI_UPDATE
                If NextWaypoint <= 0 Then
                    On_Waypoint()
                End If
            End If
        End Sub

        Public Sub CastFireball()
            For i As Integer = 0 To 3
                Dim theTarget As BaseUnit = aiCreature.GetRandomTarget
                If theTarget Is Nothing Then Exit Sub

                aiCreature.CastSpell(FIREBALL_SPELL, theTarget.positionX, theTarget.positionY, theTarget.positionZ)
            Next
        End Sub

        Public Sub Go_FlyPhase()
            'TODO: Get up into the air
            'TODO: Start sending random fireballs (only to ranged dps?)
            'TODO: Deep breathe
            'TODO: Do emote
            'TODO: Whelps
            'TODO: Movement in the air
            aiCreature.SendChatMessage("Phase 2!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
            CurrentWaypoint = 0
            On_Waypoint()
            'DONE: Reset hate table
            AllowedAttack = False
            aiTarget = Nothing
            ResetThreatTable()
            aiCreature.SendTargetUpdate(0)
        End Sub

        Public Sub Go_LastPhase()
            'TODO: Land again
            'TODO: Do emote
            aiCreature.SendChatMessage("Phase 3!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
            NextWaypoint = 0
            aiCreature.Flying = False
            'TODO: Do these after landing
            ReinitSpells()
            'DONE: Reset hate table
            AllowedAttack = True
            aiTarget = Nothing
            ResetThreatTable()
            aiCreature.SendTargetUpdate(0)
        End Sub

        Public Sub ReinitSpells()
            NextBreathe = BREATH_COOLDOWN
            NextWingBuffet = WING_BUFFET_COOLDOWN
            NextCleave = CLEAVE_COOLDOWN
            KnockTimer = KNOCK_COOLDOWN
            RoarTimer = ROAR_COOLDOWN
        End Sub

        Public Sub On_Waypoint()
            Select Case CurrentWaypoint
                Case 0
                    NextWaypoint = aiCreature.MoveTo(-75.945F, -219.245F, -83.375F, 0.004947F, True)
                Case 1
                    'TODO: Set Flying
                    aiCreature.Flying = True
                    NextWaypoint = 10000
                    NextFireball = NextWaypoint
                    aiCreature.MoveTo(42.621F, -217.195F, -66.056F, 3.014011F)
                Case 2
                    NextWaypoint = 23000
                    'TODO: How many whelps per side?
                    SpawnWhelpsLeft(15)
                    SpawnWhelpsRight(15)
                Case 3
                    NextWaypoint = 10000
                    aiCreature.MoveTo(12.27F, -254.694F, -67.997F, 2.395585F)
                Case 4, 6, 8, 10, 12
                    NextWaypoint = 23000
                    'TODO: How many whelps per side?
                    SpawnWhelpsLeft(10)
                    SpawnWhelpsRight(10)
                Case 5
                    NextWaypoint = 10000
                    aiCreature.MoveTo(-79.02F, -252.374F, -68.965F, 0.885179F)
                Case 7
                    NextWaypoint = 10000
                    aiCreature.MoveTo(-80.257F, -174.24F, -69.293F, 5.695741F)
                Case 9
                    NextWaypoint = 10000
                    aiCreature.MoveTo(27.875F, -178.547F, -66.041F, 3.908957F)
                Case 11
                    NextWaypoint = 10000
                    aiCreature.MoveTo(-4.868F, -217.171F, -86.71F, 3.14159F)
            End Select
            CurrentWaypoint += 1
            If CurrentWaypoint > 12 Then CurrentWaypoint = 3
        End Sub

        Public Sub SpawnWhelpsLeft(ByVal Count As Integer)
            For i As Integer = 1 To Count
                aiCreature.SpawnCreature(WHELP_CREATURE, -30.812F, -166.395F, -89.0F)
            Next
        End Sub

        Public Sub SpawnWhelpsRight(ByVal Count As Integer)
            For i As Integer = 1 To Count
                aiCreature.SpawnCreature(WHELP_CREATURE, -30.233F, -264.158F, 89.896F)
            Next
        End Sub

    End Class
End Namespace