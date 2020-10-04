Imports mangosVB.WorldServer

Namespace Creatures
    Public Class CreatureAI_Taragaman_the_Hungerer
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const NOVA_COOLDOWN As Integer = 4000
        Private Const UPPER_COOLDOWN As Integer = 12000

        Private Const NOVA_SPELL As Integer = 11970
        Private Const UPPER_SPELL As Integer = 18072

        Public NextWaypoint As Integer = 0
        Public NextNOVA As Integer = 0
        Public NextUPPER As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub
        Public Overrides Sub OnThink()

            NextNOVA -= AI_UPDATE
            NextUPPER -= AI_UPDATE

            If NextNOVA <= 0 Then
                NextNOVA = NOVA_COOLDOWN
                aiCreature.CastSpell(NOVA_SPELL, aiTarget) 'Fire Nova
            End If

            If NextUPPER <= 1 Then
                NextUPPER = UPPER_COOLDOWN
                aiCreature.CastSpell(UPPER_SPELL, aiTarget) 'Uppercut
            End If
        End Sub

        Public Sub CastNOVA()
            For i As Integer = 0 To 1
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(NOVA_SPELL, aiTarget)
            Next
        End Sub

        Public Sub CastUPPER()
            For i As Integer = 1 To 1
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(UPPER_SPELL, aiTarget)
            Next
        End Sub
    End Class
End Namespace