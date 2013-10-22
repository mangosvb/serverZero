Imports System
Imports System.Threading
Imports mangosVB.WorldServer

Namespace Scripts
    Public Class CreatureAI
        Inherits BossAI

        Private Const AI_UPDATE As Integer = 1000

        Private Const NPC_Gluth As Integer = 15932


        Public NextWaypoint As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub
        Public Overrides Sub OnThink()


        End Sub
        Public Sub HealGluth(ByRef NPC_Gluth As CreatureObject, ByRef Zombie_Chow As CreatureObject)
            Dim Waypoint1 As New coOrds
            With Waypoint1
                .X = 3304.919922
                .Y = 3139.149902
                .Z = 296.890015
                .Orientation = 1.33
            End With

            aiCreature.MoveTo(Waypoint1.X, Waypoint1.Y, Waypoint1.Z, Waypoint1.Orientation)
            If aiCreature.MoveTo(Waypoint1.X, Waypoint1.Y, Waypoint1.Z, Waypoint1.Orientation, True) Then
                aiCreature.Heal(50000, NPC_Gluth)
            End If
        End Sub

        Private Structure coOrds
            Dim X As Double
            Dim Y As Double
            Dim Z As Double
            Dim Orientation As Double
        End Structure
    End Class
End Namespace