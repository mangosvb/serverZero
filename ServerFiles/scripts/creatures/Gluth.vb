Imports System
Imports System.Threading
Imports mangosVB.WorldServer

Namespace Scripts
    Public Class CreatureAI
        Inherits BossAI
        'TODO: Implement proper zombie chow summons. Fix decimate. Fix him going underground. Fix mortal wound a debuff instead of dispellable buff. Fix terrifying roar.
        'Reference: https://github.com/mangoszero/scripts/blob/master/scripts/eastern_kingdoms/naxxramas/boss_gluth.cpp
        Private Const AI_UPDATE As Integer = 1000
        Private Const Mortal_Wound_CD As Integer = 10000
        Private Const Decimate_CD As Integer = 110000
        Private Const Frenzy_CD As Integer = 25000
        Private Const Terrifying_Roar_CD As Integer = 15000
        Private Const Zombie_Chow_CD As Integer = 22000



        Private Const Spell_Decimate As Integer = 28374 'This may not work, not sure if the spell ID is pre-TBC or not.
        Private Const Spell_Frenzy As Integer = 28131
        Private Const Spell_Mortal_Wound As Integer = 25646
        Private Const Spell_Terrifying_Roar As Integer = 29685
        Private Const NPC_Zombie_Chow As Integer = 16360
        'private Const Spell_Summon_Zombie_Chow As Integer = 28216 - Seems to be removed from DBC..
        'Private Const Spell_Call_All_Zombie_Chow As Integer = 29681 - Seems to be removed from DBC..
        'Private Const spell_zombie_chow_search As Integer = 28235 - Seems to be removed from DBC..

        Public NextMortalWound As Integer = 0
        Public NextDecimate As Integer = 0
        Public NextFrenzy As Integer = 0
        Public NextRoar As Integer = 0
        Public NextWaypoint As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub
		
        Public Overrides Sub OnThink()
            NextDecimate -= AI_UPDATE
            NextFrenzy -= AI_UPDATE
            NextMortalWound -= AI_UPDATE
            NextRoar -= AI_UPDATE

            If NextDecimate <= 0 Then
                NextDecimate = Decimate_CD
                aiCreature.CastSpell(Spell_Decimate, aiTarget) 'Earthborer Acid
            End If
			
            If NextFrenzy <= 1 Then
                NextFrenzy = Frenzy_CD
                aiCreature.CastSpellOnSelf(Spell_Frenzy)
            End If
			
            If NextMortalWound <= 2 Then
                NextMortalWound = Mortal_Wound_CD
                aiCreature.CastSpell(Spell_Mortal_Wound, aiTarget)
            End If
			
            If NextRoar <= 3 Then
                NextRoar = Terrifying_Roar_CD
                aiCreature.CastSpell(Spell_Terrifying_Roar, aiTarget)
            End If
        End Sub

        Public Sub CastDecimate()
            For i As Integer = 0 To 3
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(Spell_Decimate, aiTarget)
                Catch ex As Exception
                    aiCreature.SendChatMessage("I have failed to cast decimate. Whoever made this script is bad. Please report this to the developers.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            Next
        End Sub

        Public Sub CastFrenzy()
            For i As Integer = 1 To 3
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpellOnSelf(Spell_Frenzy)
                Catch ex As Exception
                    aiCreature.SendChatMessage("I have failed to cast Frenzy. Whoever made this script did a poor job, please report this to the developers.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            Next
        End Sub

        Public Sub CastMortalWound()
            For i As Integer = 2 To 3
                Dim target As BaseUnit = aiCreature
                If target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(Spell_Mortal_Wound, aiTarget)
                Catch ex As Exception
                    aiCreature.SendChatMessage("I have failed to cast Mortal Wound. Whoever made this script did a poor job, please report this to the developers.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            Next
        End Sub
		
        Public Sub CastTerrifyingRoar()
            For i As Integer = 3 To 3
                Dim target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(Spell_Terrifying_Roar, aiTarget)
                Catch ex As Exception
                    aiCreature.SendChatMessage("I have failed to cast terrifying roar. Whoever made this script did a poor job, please report this to the developers.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            Next
        End Sub

        Public Overrides Sub OnDeath()
            MyBase.OnDeath()
            aiCreature.SendChatMessage("I have successfully been slain. Good job!", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
        End Sub

        Public Sub SpawnZombieChow(ByVal Count As Integer)
            For i As Integer = 1 To Count
                If Zombie_Chow_CD <= 0 Then
                    aiCreature.SpawnCreature(16360, 3267.9F, -3172.1F, 297.42F)
                End If
            Next
        End Sub
        ' Gluth falls under Naxxramas without this. Not perfect but much better than before. Please note this was tested on a server with no vmaps/maps and that may be why he falls under Naxxramas.
        Public Overrides Sub OnLeaveCombat(Optional Reset As Boolean = True)
            MyBase.OnLeaveCombat(Reset)
            aiCreature.MoveTo(3304.269, -3136.414, 296.7151, 0.8140599)
        End Sub
    End Class
End Namespace
