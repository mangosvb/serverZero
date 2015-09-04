Imports MangosVB.WorldServer

Namespace Scripts
    Public Class CreatureAI_Searing_Blade_Cultist
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const COA_COOLDOWN As Integer = 15000

        Private Const COA_SPELL As Integer = 18266

        Public NextWaypoint As Integer = 0
        Public NextCOA As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnThink()

            NextCOA -= AI_UPDATE

            If NextCOA <= 0 Then
                NextCOA = COA_COOLDOWN
                aiCreature.CastSpell(COA_SPELL, aiTarget) 'Curse of Agony
            End If
        End Sub

        Public Sub CastCOA()
            For i As Integer = 0 To 3
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(COA_SPELL, aiTarget)
            Next
        End Sub
    End Class
End Namespace