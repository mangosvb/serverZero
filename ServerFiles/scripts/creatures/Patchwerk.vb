Imports System
Imports System.Threading
Imports WorldServer

Namespace Scripts
    Public Class CreatureAI
        Inherits BossAI

        Private Const AI_UPDATE As Integer = 1000
        Private Const BERSERK_COOLDOWN As Integer = 20000
        Private Const FRENZY_COOLDOWN As Integer = 15000
        'Private Const SUMMONPLAYER_COOLDOWN As Integer = 6000 'This might be an unoffical spell! Recheck at https://github.com/mangoszero/scripts/blob/master/scripts/eastern_kingdoms/naxxramas/boss_patchwerk.cpp

        Private Const BERSERK_SPELL As Integer = 26662
        Private Const FRENZY_SPELL As Integer = 28131
        'Private Const SUMMONPLAYER_SPELL As Integer = 20477


        Public Phase As Integer = 0
        Public NextWaypoint As Integer = 0
        Public NextBerserk As Integer = 0
        Public NextFrenzy As Integer = 0
        'Public NextSummon As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
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

            aiCreature.SendChatMessage("Patchwerk want to play!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        End Sub

        Public Overrides Sub OnLeaveCombat(Optional ByVal Reset As Boolean = True)
            MyBase.OnLeaveCombat(Reset)

            AllowedAttack = True
            Phase = 0

            aiCreature.SendChatMessage("LEAVING COMBAT!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        End Sub

        Public Overrides Sub OnKill(ByRef Victim As BaseUnit)
            'TODO: Yell
            'TODO: Send sound (No more play?)!
            aiCreature.SendChatMessage("No more play?", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        End Sub

        Public Overrides Sub OnDeath()
            'TODO: Yell
            aiCreature.SendChatMessage("What happened to.. Patch.. aaaahrr..", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        End Sub

        Public Overrides Sub OnThink()
            If Phase < 1 Then Exit Sub

            If (Phase = 1) Then
                NextBerserk -= AI_UPDATE
                NextFrenzy -= AI_UPDATE
                'NextSummon -= AI_UPDATE

                If NextBerserk <= 0 Then
                    NextBerserk = BERSERK_COOLDOWN
                    aiCreature.CastSpell(BERSERK_SPELL, aiTarget) 'Berserk
                End If
                If NextFrenzy <= 1 Then
                    NextFrenzy = FRENZY_COOLDOWN
                    aiCreature.CastSpell(FRENZY_SPELL, aiTarget) 'Frenzy
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
        Public Sub CastFrenzy()
            For i As Integer = 0 To 1
                Dim Self As BaseUnit = aiCreature
                If Self Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpellOnSelf(FRENZY_SPELL)
                Catch Ex As Exception
                    'Log.WriteLine(BaseWriter.LogType.WARNING, "FRENZY FAILED TO CAST ON PATCHWERK!")
					aiCreature.SendChatMessage("FRENZY FAILED TO CAST ON PATCHWERK! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
                End Try
            Next
        End Sub

        Public Sub CastBerserk()
            For i As Integer = 1 To 1
                Dim Self As BaseUnit = aiCreature
                If Self Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpellOnSelf(BERSERK_SPELL)
                Catch Ex As Exception
                    'Log.WriteLine(BaseWriter.LogType.WARNING, "BERSERK FAILED TO CAST ON PATCHWERK!")
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
        '            'Log.WriteLine(BaseWriter.LogType.WARNING, "SUMMON FAILED TO CAST ON TARGET!")
		'			aiCreature.SendChatMessage("SUMMON FAILED TO CAST ON TARGET! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
        '       End Try
        '   Next
        ' End Sub

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