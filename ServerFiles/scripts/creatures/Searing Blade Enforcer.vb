Imports System
Imports System.Threading
Imports MangosVB.WorldServer
Imports mangosVB.Common

Namespace Scripts
    Public Class CreatureAI_Searing_Blade_Enforcer
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const SLAM_COOLDOWN As Integer = 8000

        Private Const SLAM_SPELL As Integer = 8242

        Public NextWaypoint As Integer = 0
        Public NextSLAM As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnThink()

            NextSLAM -= AI_UPDATE

            If NextSLAM <= 0 Then
                NextSLAM = SLAM_COOLDOWN
                aiCreature.CastSpell(SLAM_SPELL, aiTarget) 'Curse of Agony
            End If
        End Sub

        Public Sub CastSLAM()
            For i As Integer = 0 To 3
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(SLAM_SPELL, aiTarget)
            Next
        End Sub
    End Class
End Namespace