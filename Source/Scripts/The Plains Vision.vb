Imports System.Collections.Generic
Imports mangosVB.WorldServer

Namespace Scripts
    Public Class CreatureAI_The_Plains_Vision
        Inherits TBaseAI
        Protected aiCreature As CreatureObject = Nothing

        Private CurrentWaypoint As Integer = 0
        Private NextWaypoint As Integer = 0

        Private Waypoints As New List(Of CreatureMovePoint)

        Public Sub New(ByRef Creature As CreatureObject)
            aiCreature = Creature
            InitWaypoints()
        End Sub

        Private Sub InitWaypoints()
            Waypoints.Add(New CreatureMovePoint(-2239.839, -404.8294, -9.424251, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2224.553, -419.0978, -9.319928, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2201.178F, -440.8505F, -5.636199F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2182.829F, -453.5462F, -5.741747F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2163.393F, -461.4627F, -7.541375F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2130.101F, -453.9658F, -9.343233F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2104.559F, -426.2767F, -6.42904F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2098.924F, -418.6772F, -6.739819F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2081.393F, -394.2358F, -10.18329F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2053.604F, -356.7091F, -6.137362F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2036.3F, -325.1877F, -8.734427F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-2004.85F, -252.2856F, -10.78323F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1967.54F, -186.5961F, -10.83361F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1923.677F, -122.1353F, -11.85162F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1839.147F, -37.15145F, -12.28224F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1799.021F, -15.25407F, -10.3242F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1781.322F, 17.70257F, -4.69648F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1754.998F, 70.02794F, 0.8294563F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1726.214F, 108.2988F, -6.750209F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1695.726F, 137.4528F, 0.02649199F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1672.852F, 159.3268F, -2.089812F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1643.49F, 187.4037F, 2.815289F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1609.052F, 220.3357F, 0.2568858F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1571.285F, 254.8306F, 0.743489F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1547.945F, 281.0505F, 22.61983F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1535.038F, 325.911F, 57.57213F, 0, 0, 0, 100))
            Waypoints.Add(New CreatureMovePoint(-1526.743F, 332.8374F, 63.22335F, 0, 0, 0, 100))
        End Sub

        Public Overrides Sub DoThink()
            NextWaypoint -= 1000
            If NextWaypoint > 0 Then Exit Sub
            ' The guide has finished
            If aiCreature.Life.Current OrElse CurrentWaypoint >= Waypoints.Count Then
                aiCreature.Destroy()
                Exit Sub
            End If

            NextWaypoint = aiCreature.MoveTo(Waypoints(CurrentWaypoint).x, Waypoints(CurrentWaypoint).y, Waypoints(CurrentWaypoint).z) + Waypoints(CurrentWaypoint).waittime
            CurrentWaypoint += 1
        End Sub
    End Class
End Namespace