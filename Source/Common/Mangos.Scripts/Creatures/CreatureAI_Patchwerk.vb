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
    Public Class CreatureAI_Patchwerk
        Inherits WS_Creatures_AI.BossAI

        Private Const AI_UPDATE As Integer = 1000
        Private Const BERSERK_COOLDOWN As Integer = 420000 'Heavy enrage, cuts through raid like butter.
        Private Const FRENZY_COOLDOWN As Integer = 150000 '"soft" enrage, pops at 5%, no need to be recasted.
        'Private Const SUMMONPLAYER_COOLDOWN As Integer = 6000 'This might be an unoffical spell! Recheck at https://github.com/mangoszero/scripts/blob/master/scripts/eastern_kingdoms/naxxramas/boss_patchwerk.cpp
        ' Private Const HATEFUL_STRIKE_COOLDOWN As Integer = 1000 'This is by far the biggest group breaker. Won't have this working for a VERY long time.

        Private Const BERSERK_SPELL As Integer = 26662
        Private Const FRENZY_SPELL As Integer = 28131
        'Private Const SUMMONPLAYER_SPELL As Integer = 20477
        'Private Const HATEFUL_STRIKE As Integer = 28308 - See cooldown for more information.


        Public Phase As Integer = 0
        Public NextWaypoint As Integer = 0
        Public NextBerserk As Integer = 0
        Public NextFrenzy As Integer = 0
        'Public NextSummon As Integer = 0
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
            aiCreature.SendChatMessage("Patchwerk want to play!", ChatEnum.ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
            aiCreature.SendPlaySound(8909, True)
        End Sub

        Public Overrides Sub OnLeaveCombat(Optional ByVal Reset As Boolean = True)
            MyBase.OnLeaveCombat(Reset)
            AllowedAttack = True
            Phase = 0
            aiCreature.SendChatMessage("LEAVING COMBAT!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        End Sub

        Public Overrides Sub OnKill(ByRef Victim As BaseUnit)
            aiCreature.SendChatMessage("No more play?", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
            aiCreature.SendPlaySound(8912, True)
        End Sub

        Public Overrides Sub OnDeath()
            aiCreature.SendChatMessage("What happened to.. Patch..", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
            aiCreature.SendPlaySound(8911, True)
        End Sub

        Public Overrides Sub OnThink()
            If Phase < 1 Then Exit Sub

            If (Phase = 1) Then
                NextBerserk -= AI_UPDATE
                NextFrenzy -= AI_UPDATE
                'NextSummon -= AI_UPDATE

                If NextBerserk <= 0 Then
                    NextBerserk = BERSERK_COOLDOWN
                    aiCreature.CastSpellOnSelf(BERSERK_SPELL) 'Berserk
                End If

                If NextFrenzy <= 1 Then
                    NextFrenzy = FRENZY_COOLDOWN
                    aiCreature.CastSpellOnSelf(FRENZY_SPELL) 'Frenzy
                End If

                'If NextSummon <= 0 Then
                '    NextSummon = SUMMONPLAYER_COOLDOWN
                '    aiCreature.CastSpell(SUMMONPLAYER_SPELL, aiTarget) 'Summon Player
                'End If
            End If

            If NextWaypoint > 0 Then
                NextWaypoint -= AI_UPDATE
                If NextWaypoint <= 0 Then
                    On_Waypoint()
                End If
            End If
        End Sub

        Public Overrides Sub OnHealthChange(Percent As Integer)
            MyBase.OnHealthChange(Percent)
            If Percent <= 5 Then
                aiCreature.CastSpellOnSelf(FRENZY_SPELL)
            End If
        End Sub

        Public Sub CastBerserk()
            For i As Integer = 0 To 1
                Dim Self As BaseUnit = aiCreature
                If Self Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpellOnSelf(BERSERK_SPELL)
                Catch Ex As Exception
                    'Log.WriteLine(LogType.WARNING, "BERSERK FAILED TO CAST ON PATCHWERK!")
                    aiCreature.SendChatMessage("BERSERK FAILED TO CAST ON PATCHWERK! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
                End Try
            Next
        End Sub
        'Public Sub CastSummonPlayer()
        '    For i As Integer = 0 To 3
        '        Dim theTarget As BaseUnit = aiCreature.GetRandomTarget
        '        If theTarget Is Nothing Then Exit Sub
        '        Try
        '            aiCreature.CastSpell(SUMMONPLAYER_SPELL, theTarget.positionX, theTarget.positionY, theTarget.positionZ)
        '        Catch Ex As Exception
        '            'Log.WriteLine(LogType.WARNING, "SUMMON FAILED TO CAST ON TARGET!")
        '			aiCreature.SendChatMessage("SUMMON FAILED TO CAST ON TARGET! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        '       End Try
        '   Next
        ' End Sub

        Public Sub On_Waypoint() 'Waypoints will definitely need some adjustments, but these should hold for now.
            Select Case CurrentWaypoint
                Case 0
                    NextWaypoint = aiCreature.MoveTo(3261.996582, 3228.5979, 294.063354, 2.53919, True) 'Coordinates do not work, but they are accurate for when we figure out how to move Patchwerk.
                Case 1
                    NextWaypoint = 10000
                    'NextSummon = NextWaypoint
                    aiCreature.MoveTo(316.822021, 3149.243652, 294.063354, 2.437088)
                Case 2
                    NextWaypoint = 23000
                Case 3
                    NextWaypoint = 10000
                    aiCreature.MoveTo(3130.012695, 3141.432861, 294.063354, 3.364644)
                Case 4, 6, 8, 10, 12
                    NextWaypoint = 23000
                Case 5
                    NextWaypoint = 10000
                    aiCreature.MoveTo(3162.650635, 3152.284912, 294.063354, 5.952532)
                Case 7
                    NextWaypoint = 10000
                    aiCreature.MoveTo(3248.241211, 3226.086426, 294.063354, 5.571616)
                Case 9
                    NextWaypoint = 10000
                    aiCreature.MoveTo(3313.826904, 3231.999512, 294.063354, 6.231347)
            End Select
            CurrentWaypoint += 1
            If CurrentWaypoint > 11 Then CurrentWaypoint = 3
        End Sub
    End Class
End Namespace