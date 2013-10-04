Imports System
Imports System.Threading
Imports WorldServer

Namespace Scripts
    Public Class CreatureAI
        Inherits BossAI

        Private Const AI_UPDATE As Integer = 1000
        Private Const EARTHQUACKE_COOLDOWN As Integer = 10000
        Private Const MAGMASPLASH_COOLDOWN As Integer = 5000
		Private Const PYROBLAST_COOLDOWN As Integer = 20000
		'Private Const SUMMONPLAYER_COOLDOWN As Integer = 20000

        Private Const EARTHQUACKE_SPELL As Integer = 19798
        Private Const MAGMASPLASH_SPELL As Integer = 28131
		Private Const PYROBLAST_SPELL As Integer = 20228
        'Private Const SUMMONPLAYER_SPELL As Integer = 20477


        Public Phase As Integer = 0
        Public NextWaypoint As Integer = 0
        Public NextEarthQuacke As Integer = 0
		Public NextMagmaSplash As Integer = 0
        Public NextPyroBlast As Integer = 0
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
                NextEarthQuacke -= AI_UPDATE
                NextMagmaSplash -= AI_UPDATE
				NextPyroBlast -= AI_UPDATE
                'NextSummon -= AI_UPDATE

                If NextEarthQuacke <= 0 Then
                    NextEarthQuacke = EarthQuacke_COOLDOWN
                    aiCreature.CastSpell(EarthQuacke_SPELL, aiTarget) 'EarthQuacke
                End If
                If NextMagmaSplash <= 1 Then
                    NextMagmaSplash = MagmaSplash_COOLDOWN
                    aiCreature.CastSpell(MagmaSplash_SPELL, aiTarget) 'MagmaSplash
                End If
				If NextPyroBlast <= 2 Then
                    NextPyroBlast = PyroBlast_COOLDOWN
                    aiCreature.CastSpell(PyroBlast_SPELL, aiTarget) 'PyroBlast
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
        Public Sub CastEarthQuacke()
            For i As Integer = 0 To 2
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(EarthQuacke_SPELL, aiTarget)
                Catch Ex As Exception
                    'Log.WriteLine(BaseWriter.LogType.WARNING, "FRENZY FAILED TO CAST ON PATCHWERK!")
					aiCreature.SendChatMessage("EarthQuacke FAILED TO CAST ON EARTHQUACKE! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
                End Try
            Next
        End Sub

        Public Sub CastMagmaSplash()
            For i As Integer = 1 To 2
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(MagmaSplash_SPELL, aiTarget)
                Catch Ex As Exception
                    'Log.WriteLine(BaseWriter.LogType.WARNING, "BERSERK FAILED TO CAST ON PATCHWERK!")
					aiCreature.SendChatMessage("MAGMASPLASH FAILED TO CAST ON TARGET! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
                End Try
            Next
        End Sub
		Public Sub CastPryoBlast()
            For i As Integer = 2 To 2
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(PyroBlast_SPELL, aiTarget)
                Catch Ex As Exception
                    'Log.WriteLine(BaseWriter.LogType.WARNING, "SUMMON FAILED TO CAST ON TARGET!")
					aiCreature.SendChatMessage("SUMMON FAILED TO CAST ON TARGET! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
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