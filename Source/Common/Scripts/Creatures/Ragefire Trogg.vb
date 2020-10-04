Imports mangosVB.WorldServer

Namespace Creatures
    Public Class CreatureAI_Ragefire_Trogg
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const STRIKE_COOLDOWN As Integer = 4000

        Private Const STRIKE_SPELL As Integer = 11976

        Public NextWaypoint As Integer = 0
        Public NextStrike As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnThink()

            NextStrike -= AI_UPDATE

            If NextStrike <= 0 Then
                NextStrike = STRIKE_COOLDOWN
                aiCreature.CastSpell(STRIKE_SPELL, aiTarget) 'Strike
            End If
        End Sub

        Public Sub CastStrike()
            For i As Integer = 0 To 3
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(STRIKE_SPELL, aiTarget)
            Next
        End Sub
    End Class
End Namespace