Imports System
Imports System.Threading
Imports MangosVB.WorldServer

Namespace Scripts
    Public Class CreatureAI
        Inherits BossAI

        Private Const AI_UPDATE As Integer = 1000
        Private Const EARTHQUAKE_COOLDOWN As Integer = 10000
        Private Const MAGMASPLASH_COOLDOWN As Integer = 5000
		Private Const PYROBLAST_COOLDOWN As Integer = 20000
		'Private Const SUMMONPLAYER_COOLDOWN As Integer = 20000

        Private Const EARTHQUAKE_SPELL As Integer = 19798
        Private Const MAGMASPLASH_SPELL As Integer = 28131
		Private Const PYROBLAST_SPELL As Integer = 20228
        'Private Const SUMMONPLAYER_SPELL As Integer = 20477

        Public Phase As Integer = 0
        Public NextWaypoint As Integer = 0
        Public NextEarthQuake As Integer = 0
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
        End Sub

        Public Overrides Sub OnLeaveCombat(Optional ByVal Reset As Boolean = True)
            MyBase.OnLeaveCombat(Reset)
            AllowedAttack = True
            Phase = 0
        End Sub

        Public Overrides Sub OnKill(ByRef Victim As BaseUnit)
            'This is only here for if something is needed when a target is killed, Golemagg doesn't have a yell.
        End Sub

        Public Overrides Sub OnDeath()
            'Does Golemagg give loot on death or cast a dummy spell? 
        End Sub

        Public Overrides Sub OnThink()
            If Phase < 1 Then Exit Sub

            If (Phase = 1) Then
                NextEarthQuake -= AI_UPDATE
                NextMagmaSplash -= AI_UPDATE
				NextPyroBlast -= AI_UPDATE
                'NextSummon -= AI_UPDATE

                If NextEarthQuake <= 0 Then
                    NextEarthQuake = EarthQuake_COOLDOWN
                    aiCreature.CastSpell(EarthQuake_SPELL, aiTarget) 'EarthQuake
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
        End Sub

        Public Sub CastEarthQuake()
            For i As Integer = 0 To 2
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(EarthQuake_SPELL, aiTarget)
                Catch Ex As Exception
					aiCreature.SendChatMessage("Earthquake FAILED TO CAST ON MY TARGET! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
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
					aiCreature.SendChatMessage("MAGMASPLASH FAILED TO CAST ON TARGET! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
                End Try
            Next
        End Sub

        Public Sub CastPyroBlast()
            For i As Integer = 2 To 2
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(PYROBLAST_SPELL, aiTarget)
                Catch Ex As Exception
                    aiCreature.SendChatMessage("PYROBLAST FAILED TO CAST ON TARGET! Please report this to the DEV'S!", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
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
    End Class
End Namespace