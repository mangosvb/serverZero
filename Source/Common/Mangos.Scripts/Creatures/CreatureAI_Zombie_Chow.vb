
Imports Mangos.World.AI
Imports Mangos.World.Objects

'Basically, this AI is kitable and if the AI hits Gluth, it heals her for 5% of her HP (50,000 in this case.). Since we can't really do it that way, it has a set waypoint.
Namespace Creatures
    Public Class CreatureAI_Zombie_Chow
        Inherits WS_Creatures_AI.BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const Infected_Wound_CD As Integer = 15000

        Private Const NPC_Gluth As Integer = 15932
        Private Const Spell_Infected_Wound As Integer = 29306 'The target takes 100 extra physical damage. This ability stacks.


        Public NextInfectedWound As Integer = 0
        Public NextWaypoint As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As WS_Creatures.CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub
        Public Overrides Sub OnThink()
            NextInfectedWound -= AI_UPDATE

            If NextInfectedWound <= 0 Then 'Not really Blizzlike, but it'll make the zombies more blizzlike.
                NextInfectedWound = Infected_Wound_CD
                aiCreature.CastSpell(Spell_Infected_Wound, aiTarget)
            End If
        End Sub

        Public Sub CastInfectedWound()
            For i As Integer = 0 To 0
                Dim target As BaseUnit = aiCreature
                If target Is Nothing Then Exit Sub
                aiCreature.CastSpell(Spell_Infected_Wound, aiTarget)
            Next
        End Sub

        Public Sub HealGluth(ByRef NPC_Gluth As CreatureObject, ByRef Zombie_Chow As CreatureObject)
            Dim Waypoint1 As New coords
            With Waypoint1
                .X = 3304.919922
                .Y = 3139.149902
                .Z = 296.890015
                .Orientation = 1.33
            End With

            aiCreature.MoveTo(Waypoint1.X, Waypoint1.Y, Waypoint1.Z, Waypoint1.Orientation)
            If aiCreature.MoveTo(Waypoint1.X, Waypoint1.Y, Waypoint1.Z, Waypoint1.Orientation, True) Then
                aiCreature.Heal(50000)
            End If
        End Sub

        Private Structure coords
            Dim X As Double
            Dim Y As Double
            Dim Z As Double
            Dim Orientation As Double
        End Structure
    End Class
End Namespace