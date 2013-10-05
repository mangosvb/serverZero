Imports System
Imports System.Threading
Imports MangosVB.WorldServer

Namespace Scripts
    Public Class CreatureAI
        Inherits BossAI

        Private Const AI_UPDATE As Integer = 1000
        Private Const HEAL_COOLDOWN As Integer = 8000
		Private Const BOLT_COOLDOWN As Integer = 3000

        Private Const HEAL_SPELL As Integer = 11986
		Private Const BOLT_SPELL As Integer = 9532

        Public NextWaypoint As Integer = 0
        Public NextHeal As Integer = 0
		Public NextBolt As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)

            AllowedMove = False
            Creature.Flying = False

            Creature.VisibleDistance = 700
        End Sub
        Public Overrides Sub OnThink()

            NextHeal -= AI_UPDATE
            NextBolt -= AI_UPDATE

            If NextHeal <= 0 Then
                NextHeal = HEAL_COOLDOWN
                aiCreature.CastSpell(HEAL_SPELL, aiTarget) 'HEALING WAVE
            End If

            If NextBolt <= 1 Then
                NextBolt = BOLT_COOLDOWN
                aiCreature.CastSpell(BOLT_SPELL, aiTarget) 'LIGHTNING BOLT
            End If
        End Sub

        Public Sub CastHeal()
            For i As Integer = 0 To 1
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub

                aiCreature.CastSpell(HEAL_SPELL, aiTarget)
            Next
		End Sub
		
		Public Sub CastBolt()
            For i As Integer = 1 To 1
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub

                aiCreature.CastSpell(BOLT_SPELL, aiTarget)
            Next
        End Sub
    End Class
End Namespace