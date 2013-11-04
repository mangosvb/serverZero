Imports mangosVB.Common.BaseWriter

Public Class WS_GraveYards
    Implements IDisposable
#Region "Graveyards"
    Public Graveyards As New Dictionary(Of Integer, TGraveyard)
    Public Structure TGraveyard
        'Dim x As Single
        'Dim y As Single
        'Dim z As Single
        'Dim MapID As Integer
        Private _locationPosX As Single
        Private _locationPosY As Single
        Private _locationPosZ As Single
        Private _locationMapID As Integer

        Sub New(locationPosX As Single, locationPosY As Single, locationPosZ As Single, locationMapID As Integer)
            ' TODO: Complete member initialization
            _locationPosX = locationPosX
            _locationPosY = locationPosY
            _locationPosZ = locationPosZ
            _locationMapID = locationMapID
        End Sub

        ''' <summary>
        ''' Gets or sets the X Coord.
        ''' </summary>
        ''' <value>The X Coord.</value>
        Property X As Integer

            Get
                Return Me._locationPosX
            End Get
            Set(value As Integer)
                Me._locationPosX = X
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Y Coord.
        ''' </summary>
        ''' <value>The Y Coord.</value>
        Property Y As Integer

            Get
                Return Me._locationPosY
            End Get
            Set(value As Integer)
                Me._locationPosY = Y
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Z Coord.
        ''' </summary>
        ''' <value>The Z Coord.</value>
        Property Z As Integer

            Get
                Return Me._locationPosZ
            End Get
            Set(value As Integer)
                Me._locationPosZ = Z
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Map ID.
        ''' </summary>
        ''' <value>The Map ID.</value>
        Property Map As Integer

            Get
                Return Me._locationMapID
            End Get
            Set(value As Integer)
                Me._locationMapID = Map
            End Set
        End Property
    End Structure

    ''' <summary>
    ''' Adds the coords.
    ''' </summary>
    ''' <param name="ID">The ID.</param>
    ''' <param name="locationPosX">The location pos X.</param>
    ''' <param name="locationPosY">The location pos Y.</param>
    ''' <param name="locationPosZ">The location pos Z.</param>
    ''' <param name="locationMapID">The location map ID.</param>
    Public Sub AddGraveYard(ID As Integer, locationPosX As Single, locationPosY As Single, locationPosZ As Single, locationMapID As Integer)
        ' TODO: Complete member initialization
        Me.Graveyards.Add(ID, New TGraveyard(locationPosX, locationPosY, locationPosZ, locationMapID))
    End Sub

    ''' <summary>
    ''' Gets the coords.
    ''' </summary>
    ''' <param name="ID">The ID.</param>
    ''' <returns>a <c>classCoords</c> structure</returns>
    Public Function GetGraveYard(ID As Integer) As TGraveyard
        Dim ret As New TGraveyard
        ret = Me.Graveyards(ID)
        Return ret
    End Function

    'Public Sub New(ByVal px As Single, ByVal py As Single, ByVal pz As Single, ByVal pMap As Integer)
    '    x = px
    '    y = py
    '    z = pz
    '    Map = pMap
    'End Sub

#Region "GraveYards"
    Public Sub InitializeGraveyards()
        Try
            Graveyards.Clear()
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\WorldSafeLocs.dbc")

            Dim locationPosX As Single
            Dim locationPosY As Single
            Dim locationPosZ As Single
            Dim locationMapID As Integer
            Dim locationIndex As Integer

            Log.WriteLine(LogType.INFORMATION, "Loading.... {0} Graveyard Locations", tmpDBC.Rows - 1)
            For i As Integer = 0 To tmpDBC.Rows - 1
                locationIndex = tmpDBC.Item(i, 0)
                locationMapID = tmpDBC.Item(i, 1)
                locationPosX = tmpDBC.Item(i, 2, DBC.DBCValueType.DBC_FLOAT)
                locationPosY = tmpDBC.Item(i, 3, DBC.DBCValueType.DBC_FLOAT)
                locationPosZ = tmpDBC.Item(i, 4, DBC.DBCValueType.DBC_FLOAT)

                If Config.Maps.Contains(locationMapID.ToString) Then
                    Graveyards.Add(locationIndex, New TGraveyard(locationPosX, locationPosY, locationPosZ, locationMapID))
                    Log.WriteLine(LogType.DEBUG, "         : Map: {0}  X: {1}  Y: {2}  Z: {3}", locationMapID, locationPosX, locationPosY, locationPosZ)
                End If
            Next i
            Log.WriteLine(LogType.INFORMATION, "Finished loading Graveyard Locations", tmpDBC.Rows - 1)

            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Graveyards initialized.", tmpDBC.Rows - 1)
            tmpDBC.Dispose()
        Catch e As IO.DirectoryNotFoundException
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("DBC File : WorldSafeLocs missing.")
            Console.ForegroundColor = ConsoleColor.Gray
        End Try
    End Sub
#End Region

    Public Sub GoToNearestGraveyard(ByRef Character As CharacterObject, ByVal Alive As Boolean, ByVal Teleport As Boolean)
        
        Dim GraveQuery As New DataTable
        Dim Ghostzone As Integer
        Dim foundNear As Boolean = False
        Dim distNear As Single = 0.0F
        Dim entryNear As TGraveyard = Nothing
        Dim entryFar As TGraveyard = Nothing

        'Death in an instance ?
        If Maps(Character.MapID).IsDungeon = True Or Maps(Character.MapID).IsBattleGround = True Or Maps(Character.MapID).IsRaid = True Then   'In an instance
            Character.ZoneCheckInstance()
            Ghostzone = AreaTable(GetAreaIDByMapandParent(Character.MapID, AreaTable(GetAreaFlag(Character.resurrectPositionX, Character.resurrectPositionY, Character.MapID)).Zone)).ID
            WorldDatabase.Query(String.Format("SELECT id, faction FROM game_graveyard_zone WHERE ghost_zone = {0} and (faction = 0 or faction = {1}) ", Ghostzone, Character.Team), GraveQuery)

            'AreaTable(GetAreaFlag(Character.resurrectPositionX, Character.resurrectPositionY, Character.MapID)).Zone()
            If GraveQuery.Rows.Count = 0 Then
                Log.WriteLine(LogType.INFORMATION, "GraveYards: No near graveyards for map [{0}], zone [{1}]", Character.MapID, Character.ZoneID)
                Exit Sub
            End If

            If Maps(Character.MapID).IsDungeon = True Or Maps(Character.MapID).IsBattleGround = True Or Maps(Character.MapID).IsRaid = True Then   'In an instance
                entryFar = Graveyards(GraveQuery.Rows(0).Item("id"))
                entryNear = entryFar
            Else

                For Each GraveLink As DataRow In GraveQuery.Rows
                    Dim GraveyardID As Integer = GraveLink.Item("id")
                    Dim GraveyardFaction As Integer = GraveLink.Item("faction")
                    If Graveyards.ContainsKey(GraveyardID) = False Then
                        Log.WriteLine(LogType.INFORMATION, "GraveYards: Graveyard link invalid [{0}]", GraveyardID)
                        Continue For
                    End If

                    If Character.MapID <> Graveyards(GraveyardID).Map Then
                        If IsNothing(entryFar) Then entryFar = Graveyards(GraveyardID)
                        Continue For
                    End If

                    'Skip graveyards that ain't for your faction
                    If GraveyardFaction <> 0 AndAlso GraveyardFaction <> Character.Team Then Continue For

                    Dim dist2 As Single = GetDistance(Character.positionX, Graveyards(GraveyardID).X, Character.positionY, Graveyards(GraveyardID).Y, Character.positionZ, Graveyards(GraveyardID).Z)
                    If foundNear Then
                        If dist2 < distNear Then
                            distNear = dist2
                            entryNear = Graveyards(GraveyardID)
                        End If
                    Else
                        foundNear = True
                        distNear = dist2
                        entryNear = Graveyards(GraveyardID)
                    End If
                Next
            End If
            Dim selectedGraveyard As TGraveyard = entryNear
            If IsNothing(selectedGraveyard) Then selectedGraveyard = entryFar

            'If Teleport Then
            '                If Alive = False And Character.DEAD = True Then
            '                    CharacterResurrect(Character)
            '                    Character.Life.Current = Character.Life.Maximum
            '                   If Character.ManaType = ManaTypes.TYPE_MANA Then Character.Mana.Current = Character.Mana.Maximum
            'If selectedGraveyard.Map = Character.MapID Then
            '                       Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Character.Life.Current)
            '                       If Character.ManaType = ManaTypes.TYPE_MANA Then Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, Character.Mana.Current)
            'Character.SendCharacterUpdate()
            'End If
            'End If

            Log.WriteLine(LogType.INFORMATION, "GraveYards: GraveYard.Map[{0}], GraveYard.X[{1}], GraveYard.Y[{2}], GraveYard.Z[{3}]", selectedGraveyard.Map, selectedGraveyard.X, selectedGraveyard.Y, selectedGraveyard.Z)
            Character.Teleport(selectedGraveyard.X, selectedGraveyard.Y, selectedGraveyard.Z, 0, selectedGraveyard.Map)
            Character.SendDeathReleaseLoc(selectedGraveyard.X, selectedGraveyard.Y, selectedGraveyard.Z, selectedGraveyard.Map)
            'Else
            '    Character.positionX = selectedGraveyard.X
            '    Character.positionY = selectedGraveyard.Y
            '    Character.positionZ = selectedGraveyard.Z
            '    Character.MapID = selectedGraveyard.Map
            'End If
        Else            'Non instanced Death
            Character.ZoneCheck()

            '            WorldDatabase.Query(String.Format("SELECT id, faction FROM world_graveyard_zone WHERE ghost_map = {0} AND ghost_zone = {1}", Character.MapID, Character.ZoneID), GraveQuery)
            WorldDatabase.Query(String.Format("SELECT id, faction FROM game_graveyard_zone WHERE ghost_zone = {0}", Character.ZoneID), GraveQuery)

            If GraveQuery.Rows.Count = 0 Then
                Log.WriteLine(LogType.INFORMATION, "GraveYards: No near graveyards for map [{0}], zone [{1}]", Character.MapID, Character.ZoneID)
                Exit Sub
            End If

            For Each GraveLink As DataRow In GraveQuery.Rows
                Dim GraveyardID As Integer = GraveLink.Item("id")
                Dim GraveyardFaction As Integer = GraveLink.Item("faction")
                If Graveyards.ContainsKey(GraveyardID) = False Then
                    Log.WriteLine(LogType.INFORMATION, "GraveYards: Graveyard link invalid [{0}]", GraveyardID)
                    Continue For
                End If

                If Character.MapID <> Graveyards(GraveyardID).Map Then
                    If IsNothing(entryFar) Then entryFar = Graveyards(GraveyardID)
                    Continue For
                End If

                'Skip graveyards that ain't for your faction
                If GraveyardFaction <> 0 AndAlso GraveyardFaction <> Character.Team Then Continue For

                Dim dist2 As Single = GetDistance(Character.positionX, Graveyards(GraveyardID).X, Character.positionY, Graveyards(GraveyardID).Y, Character.positionZ, Graveyards(GraveyardID).Z)
                If foundNear Then
                    If dist2 < distNear Then
                        distNear = dist2
                        entryNear = Graveyards(GraveyardID)
                    End If
                Else
                    foundNear = True
                    distNear = dist2
                    entryNear = Graveyards(GraveyardID)
                End If
            Next

            Dim selectedGraveyard As TGraveyard = entryNear
            If IsNothing(selectedGraveyard) Then selectedGraveyard = entryFar

            If Teleport Then
                If Alive And Character.DEAD Then
                    CharacterResurrect(Character)
                    Character.Life.Current = Character.Life.Maximum
                    If Character.ManaType = ManaTypes.TYPE_MANA Then Character.Mana.Current = Character.Mana.Maximum
                    If selectedGraveyard.Map = Character.MapID Then
                        Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Character.Life.Current)
                        If Character.ManaType = ManaTypes.TYPE_MANA Then Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1, Character.Mana.Current)
                        Character.SendCharacterUpdate()
                    End If
                End If

                Log.WriteLine(LogType.INFORMATION, "GraveYards: GraveYard.Map[{0}], GraveYard.X[{1}], GraveYard.Y[{2}], GraveYard.Z[{3}]", selectedGraveyard.Map, selectedGraveyard.X, selectedGraveyard.Y, selectedGraveyard.Z)
                Character.Teleport(selectedGraveyard.X, selectedGraveyard.Y, selectedGraveyard.Z, 0, selectedGraveyard.Map)
                Character.SendDeathReleaseLoc(selectedGraveyard.X, selectedGraveyard.Y, selectedGraveyard.Z, selectedGraveyard.Map)
            Else
                Character.positionX = selectedGraveyard.X
                Character.positionY = selectedGraveyard.Y
                Character.positionZ = selectedGraveyard.Z
                Character.MapID = selectedGraveyard.Map
            End If

        End If
    End Sub

#End Region

#Region "IDisposable Support"
    Private _disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not _disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        _disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class