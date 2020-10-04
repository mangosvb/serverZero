Imports MangosVB.WorldServer

Namespace Creatures
    Public Class CreatureAI_Earthborer
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const ACID_COOLDOWN As Integer = 10000
        Private Const ACID_SPELL As Integer = 18070

        Public NextWaypoint As Integer = 0
        Public NextAcid As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnThink()
            NextAcid -= AI_UPDATE

            If NextAcid <= 0 Then
                NextAcid = ACID_COOLDOWN
                aiCreature.CastSpell(ACID_SPELL, aiTarget) 'Earthborer Acid
            End If
        End Sub

        Public Sub CastAcid()
            For i As Integer = 0 To 3
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(ACID_SPELL, aiTarget)
            Next
        End Sub
    End Class
End Namespace