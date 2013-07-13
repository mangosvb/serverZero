' 
' Copyright (C) 2011 SpuriousZero <http://www.spuriousemu.com/>
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.Threading
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Collections.Generic
Imports SpuriousZero.Common.BaseWriter
Imports SpuriousZero.Common


Public Module WS_Maps
#Region "Zones"
    Public AreaTable As New Dictionary(Of Integer, TArea)
    Public Class TArea
        Public ID As Integer
        Public Level As Byte
        Public Zone As Integer
        Public ZoneType As Integer
        Public Team As AreaTeam
        Public Name As String
        Public Enum AreaTeam As Integer
            AREATEAM_NONE = 0
            AREATEAM_ALLY = 2
            AREATEAM_HORDE = 4
        End Enum
        Public Enum AreaFlag As Integer
            AREA_FLAG_SNOW = &H1                ' snow (only Dun Morogh, Naxxramas, Razorfen Downs and Winterspring)
            AREA_FLAG_UNK1 = &H2                ' unknown, (only Naxxramas and Razorfen Downs)
            AREA_FLAG_UNK2 = &H4                ' Only used on development map
            AREA_FLAG_SLAVE_CAPITAL = &H8       ' slave capital city flag?
            AREA_FLAG_UNK3 = &H10               ' unknown
            AREA_FLAG_SLAVE_CAPITAL2 = &H20     ' slave capital city flag?
            AREA_FLAG_UNK4 = &H40               ' many zones have this flag
            AREA_FLAG_ARENA = &H80              ' arena, both instanced and world arenas
            AREA_FLAG_CAPITAL = &H100           ' main capital city flag
            AREA_FLAG_CITY = &H200              ' only for one zone named "City" (where it located?)
            AREA_FLAG_OUTLAND = &H400           ' outland zones? (only Eye of the Storm not have this flag, but have 0x00004000 flag)
            AREA_FLAG_SANCTUARY = &H800         ' sanctuary area (PvP disabled)
            AREA_FLAG_NEED_FLY = &H1000         ' only Netherwing Ledge, Socrethar's Seat, Tempest Keep, The Arcatraz, The Botanica, The Mechanar, Sorrow Wing Point, Dragonspine Ridge, Netherwing Mines, Dragonmaw Base Camp, Dragonmaw Skyway
            AREA_FLAG_UNUSED1 = &H2000          ' not used now (no area/zones with this flag set in 2.4.2)
            AREA_FLAG_OUTLAND2 = &H4000         ' outland zones? (only Circle of Blood Arena not have this flag, but have 0x00000400 flag)
            AREA_FLAG_PVP = &H8000              ' pvp objective area? (Death's Door also has this flag although it's no pvp object area)
            AREA_FLAG_ARENA_INSTANCE = &H10000  ' used by instanced arenas only
            AREA_FLAG_UNUSED2 = &H20000         ' not used now (no area/zones with this flag set in 2.4.2)
            AREA_FLAG_UNK5 = &H40000            ' just used for Amani Pass, Hatchet Hills
            AREA_FLAG_LOWLEVEL = &H100000       ' used for some starting areas with area_level <=15
        End Enum
        Public Function IsMyLand(ByRef c As CharacterObject) As Boolean
            If Team = AreaTeam.AREATEAM_NONE Then Return False
            If c.Side = False Then Return Team = AreaTeam.AREATEAM_ALLY
            If c.Side = True Then Return Team = AreaTeam.AREATEAM_HORDE
        End Function
        Public Function IsCity() As Boolean
            Return ZoneType = 312
        End Function
        Public Function NeedFlyingMount() As Boolean
            Return (ZoneType And AreaFlag.AREA_FLAG_NEED_FLY)
        End Function
        Public Function IsSanctuary() As Boolean
            Return (ZoneType And AreaFlag.AREA_FLAG_SANCTUARY)
        End Function
        Public Function IsArena() As Boolean
            Return (ZoneType And AreaFlag.AREA_FLAG_ARENA)
        End Function
    End Class
#End Region

    Public RESOLUTION_ZMAP As Integer = 0

    Public Const VMAP_MAGIC As String = "VMAP_2.0"
    Public Const VMAP_MAX_CAN_FALL_DISTANCE As Single = 10.0F
    Public Const VMAP_INVALID_HEIGHT As Single = -100000.0F 'for check
    Public Const VMAP_INVALID_HEIGHT_VALUE As Single = -200000.0F 'real assigned value in unknown height case

#Region "Continents"

    'NOTE: Map resolution. The resolution of your map files in your maps folder.
    'Public Const RESOLUTION_ZMAP As Integer = 256 - 1

    Public Class TMapTile
        Implements IDisposable

        Public Const SIZE As Single = 533.3333F
        Public Const RESOLUTION_WATER As Integer = 128 - 1
        Public Const RESOLUTION_FLAGS As Integer = 16 - 1
        Public Const RESOLUTION_TERRAIN As Integer = 16 - 1

        'TMap contains 64x64 TMapTile(s)
        Public AreaFlag(RESOLUTION_FLAGS, RESOLUTION_FLAGS) As UShort
        Public AreaTerrain(RESOLUTION_TERRAIN, RESOLUTION_TERRAIN) As Byte
        Public WaterLevel(RESOLUTION_WATER, RESOLUTION_WATER) As Single
        'Public ZCoord(RESOLUTION_ZMAP, RESOLUTION_ZMAP) As Single
        Public ZCoord(,) As Single

#If ENABLE_PPOINTS Then
        'Public ZCoord_PP(RESOLUTION_ZMAP, RESOLUTION_ZMAP) As Single
        Public ZCoord_PP(,) As Single
        Public ZCoord_PP_ModTimes As Integer = 0

        Public Sub ZCoord_PP_Save()
            ZCoord_PP_ModTimes = 0

            Dim fileName As String = String.Format("maps\{0}{1}{2}.pp", Format(CellMap, "000"), Format(CellX, "00"), Format(CellY, "00"))

            Log.WriteLine(LogType.INFORMATION, "Saving PP file [{0}] version [{1}]", fileName, PPOINT_VERSION)

            Dim f As New IO.FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 16392, FileOptions.WriteThrough)
            Dim w As New BinaryWriter(f)
            w.Write(System.Text.Encoding.ASCII.GetBytes(PPOINT_VERSION))
            For x As Integer = 0 To RESOLUTION_ZMAP
                For y As Integer = 0 To RESOLUTION_ZMAP
                    w.Write(ZCoord_PP(x, y))
                Next y
            Next x
            w.Close()
            f.Close()
        End Sub
#End If

        Public PlayersHere As New List(Of ULong)
        Public CreaturesHere As New List(Of ULong)
        Public GameObjectsHere As New List(Of ULong)
        Public CorpseObjectsHere As New List(Of ULong)
        Public DynamicObjectsHere As New List(Of ULong)

        Private CellX As Byte
        Private CellY As Byte
        Private CellMap As UInteger

        Public Sub New(ByVal tileX As Byte, ByVal tileY As Byte, ByVal tileMap As UInteger)
            'DONE: Don't load maptiles we don't handle
            If Not Maps.ContainsKey(tileMap) Then Exit Sub

            ReDim ZCoord(RESOLUTION_ZMAP, RESOLUTION_ZMAP)
#If ENABLE_PPOINTS Then
            ReDim ZCoord_PP(RESOLUTION_ZMAP, RESOLUTION_ZMAP)
#End If

            CellX = tileX
            CellY = tileY
            CellMap = tileMap

            Dim fileName As String
            Dim fileVersion As String
            Dim f As IO.FileStream
            Dim b As BinaryReader
            Dim x, y As Integer

            'DONE: Loading MAP file
            fileName = String.Format("{0}{1}{2}.map", Format(tileMap, "000"), Format(tileX, "00"), Format(tileY, "00"))
            If Not File.Exists("maps\" & fileName) Then
                Log.WriteLine(LogType.WARNING, "Map file [{0}] not found", fileName)
            Else
                f = New IO.FileStream("maps\" & fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 82704, FileOptions.SequentialScan)
                b = New BinaryReader(f)

                fileVersion = System.Text.Encoding.ASCII.GetString(b.ReadBytes(8), 0, 8)
                Log.WriteLine(LogType.INFORMATION, "Loading map file [{0}] version [{1}]", fileName, fileVersion)

                For x = 0 To RESOLUTION_FLAGS
                    For y = 0 To RESOLUTION_FLAGS
                        AreaFlag(x, y) = b.ReadUInt16()
                    Next y
                Next x
                For x = 0 To RESOLUTION_TERRAIN
                    For y = 0 To RESOLUTION_TERRAIN
                        AreaTerrain(x, y) = b.ReadByte
                    Next y
                Next x
                For x = 0 To RESOLUTION_WATER
                    For y = 0 To RESOLUTION_WATER
                        WaterLevel(x, y) = b.ReadSingle
                    Next y
                Next x
                For x = 0 To RESOLUTION_ZMAP
                    For y = 0 To RESOLUTION_ZMAP
                        ZCoord(x, y) = b.ReadSingle
                    Next y
                Next x
                b.Close()
                f.Close()
                f.Dispose()
            End If




#If ENABLE_PPOINTS Then
            'DONE: Initializing PPoints to unused values
            For x = 0 To RESOLUTION_ZMAP
                For y = 0 To RESOLUTION_ZMAP
                    ZCoord_PP(x, y) = PPOINT_BAD
                Next y
            Next x

            'DONE: Loading PPoints file
            fileName = String.Format("{0}{1}{2}.pp", Format(tileMap, "000"), Format(tileX, "00"), Format(tileY, "00"))

            If not file.exists("maps\" & fileName) Then
                'DONE: We are loading it only for instance maps
                If Not IsContinentMap(tileMap) Then ZCoord_PP_Save()
            Else
                f = New IO.FileStream("maps\" & fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 16392, FileOptions.SequentialScan)
                b = New BinaryReader(f)

                fileVersion = System.Text.Encoding.ASCII.GetString(b.ReadBytes(8), 0, 8)
                Log.WriteLine(LogType.INFORMATION, "Loading PP file [{0}] version [{1}]", fileName, fileVersion)
                For x = 0 To RESOLUTION_ZMAP
                    For y = 0 To RESOLUTION_ZMAP
                        ZCoord_PP(x, y) = b.ReadSingle
                    Next y
                Next x
                b.Close()
                f.Close()
                f.Dispose()
            End If

#End If

#If VMAPS Then
            LoadVMAP()
#End If
        End Sub

#If VMAPS Then
        Public Sub LoadVMAP()
            'DONE: Loading VMDIR file
            Dim fileName As String = String.Format("{0}_{1}_{2}.vmdir", Format(CellMap, "000"), Format(CellX, "00"), Format(CellY, "00"))
            If Not File.Exists("vmaps\" & fileName) Then
                fileName = String.Format("{0}.vmdir", Format(CellMap, "000"))
            End If

            If Not File.Exists("vmaps\" & fileName) Then
                Log.WriteLine(LogType.WARNING, "VMap file [{0}] not found", fileName)
            Else
                Dim map As TMap = Maps(CellMap)
                fileName = Trim(File.ReadAllText("vmaps\" & fileName))
                Dim fileNames() As String = fileName.Split(New String() {vbLf}, StringSplitOptions.RemoveEmptyEntries)

                fileName = fileNames(0)
                If fileNames.Length > 1 AndAlso fileNames(1).Length > 0 Then
                    fileName = fileNames(1)
                End If

                Dim newModelLoaded As Boolean = False
                If fileName.Length > 0 AndAlso File.Exists("vmaps\" & fileName) Then
                    Dim mc As ModelContainer
                    If Not map.ContainsModelContainer(fileName) Then
                        mc = New ModelContainer()
                        If mc.ReadFile(fileName) Then
                            If Not map.ContainsModelContainer(fileName) Then
                                map.AddModelContainer(fileName, mc)
                                newModelLoaded = True
                            Else
                                'Already loaded? :/ Dispose it
                                mc.Dispose()
                            End If
                        End If
                    Else
                        'Not really needed is it?
                        'mc = map.GetModelContainer(fileName)
                    End If
                Else
                    Log.WriteLine(LogType.WARNING, "VMap file [{0}] not found", fileName)
                End If

                If newModelLoaded Then
                    map.BalanceTree()
                End If
            End If
        End Sub
#End If

        Public Sub Dispose() Implements System.IDisposable.Dispose
            'Done: Remove spawns
            UnloadSpawns(CellX, CellY, CellMap)
        End Sub
    End Class

    Public Class TMap
        Implements IDisposable

        Public ID As Integer
        Public Type As MapTypes = MapTypes.MAP_COMMON
        Public Name As String = ""

        Public TileUsed(63, 63) As Boolean 'The same maptile should no longer be loaded twice
        Public Tiles(63, 63) As TMapTile

#If VMAPS Then
        Private iLoadedModelContainer As New Dictionary(Of String, ModelContainer)
        Private iTree As New AABSPTree(Of ModelContainer)
#End If

        Public ReadOnly Property IsDungeon() As Boolean
            Get
                Return Type = MapTypes.MAP_INSTANCE OrElse Type = MapTypes.MAP_RAID
            End Get
        End Property
        Public ReadOnly Property IsRaid() As Boolean
            Get
                Return Type = MapTypes.MAP_RAID
            End Get
        End Property
        Public ReadOnly Property IsBattleGround() As Boolean
            Get
                Return Type = MapTypes.MAP_BATTLEGROUND
            End Get
        End Property

        Public ReadOnly Property ResetTime() As Integer
            Get
                Select Case Type
                    Case MapTypes.MAP_BATTLEGROUND
                        Return DEFAULT_BATTLEFIELD_EXPIRE_TIME

                    Case MapTypes.MAP_RAID, MapTypes.MAP_INSTANCE
                        '* Molten Core: Every Tuesday at 3:00AM or during weekly maintenance
                        '* Blackwing Lair: Every Tuesday at 3:00AM or during weekly maintenance
                        '* Onyxia's Lair: Every 5 days at 3:00AM
                        '* Zul'Gurub: Every 3 days at 3:00AM
                        '* Ruins of Ahn'Qiraj: Every 3 days at 3:00AM
                        '* Temple of Ahn'Qiraj: Every Tuesday at 3:00AM or during weekly maintenance
                        '* Naxxramas: Every Tuesday at 3:00AM or during weekly maintenance 
                        Select Case ID
                            Case 249 'Onyxia's Lair
                                Return GetNextDate(5, 3).Subtract(Now).TotalSeconds
                            Case 309, 509 'Zul'Gurub and Ruins of Ahn'Qiraj
                                Return GetNextDate(3, 3).Subtract(Now).TotalSeconds
                            Case 409, 469, 531, 533 'Molten Core, Blackwing Lair, Temple of Ahn'Qiraj and Naxxramas
                                Return GetNextDay(DayOfWeek.Tuesday, 3).Subtract(Now).TotalSeconds
                        End Select

                        Return DEFAULT_INSTANCE_EXPIRE_TIME
                End Select
            End Get
        End Property

#If VMAPS Then
        Public Function ContainsModelContainer(ByVal name As String) As Boolean
            Return iLoadedModelContainer.ContainsKey(name)
        End Function

        Public Sub AddModelContainer(ByVal name As String, ByRef mc As ModelContainer)
            iLoadedModelContainer.Add(name, mc)
            iTree.Insert(mc)
        End Sub

        Public Function GetModelContainer(ByVal name As String) As ModelContainer
            Return iLoadedModelContainer(name)
        End Function

        Public Sub BalanceTree()
            iTree.Balance()
        End Sub

        Public Function IsInLineOfSight(ByVal pos1 As Vector3, ByVal pos2 As Vector3) As Boolean
            Dim result As Boolean = True
            Dim maxDist As Single = (pos2 - pos1).Magnitude()

            Dim ray As New Ray(pos1, (pos2 - pos1) / maxDist)
            Dim resultDist As Single = GetIntersectionTime(ray, maxDist, True)
            If resultDist < maxDist Then
                result = False
            End If

            Return result
        End Function

        Public Function GetHeight(ByVal pos As Vector3) As Single
            Dim height As Single = Single.PositiveInfinity
            Dim dir As New Vector3(0.0F, -1.0F, 0.0F)
            Dim ray As New Ray(pos, dir)
            Dim maxDist As Single = VMAP_MAX_CAN_FALL_DISTANCE
            Dim dist As Single = GetIntersectionTime(ray, maxDist, False)
#If VMAPS_DEBUG Then
            Log.WriteLine(LogType.DEBUG, "GetHeight dist: {0}", dist)
#End If
            If dist < Single.PositiveInfinity Then
                height = (pos + dir * dist).y
            End If
            Return height
        End Function

        Public Function GetObjectHitPos(ByVal pos1 As Vector3, ByVal pos2 As Vector3, ByRef resultPos As Vector3, ByVal pModifyDist As Single) As Boolean
            Dim result As Boolean = False
            Dim maxDist As Single = (pos2 - pos1).Magnitude()
            Dim dir As Vector3 = (pos2 - pos1) / maxDist
            Dim ray As New Ray(pos1, dir)
            Dim dist As Single = GetIntersectionTime(ray, maxDist, False)
            If dist < maxDist Then
                resultPos = pos1 + dir * dist
                If pModifyDist < 0.0F Then
                    If (resultPos - pos1).Magnitude() > (-pModifyDist) Then
                        resultPos = resultPos + dir * pModifyDist
                    Else
                        resultPos = pos1
                    End If
                Else
                    resultPos = resultPos + dir * pModifyDist
                End If
                result = True
            Else
                resultPos = pos2
                result = False
            End If

            Return result
        End Function

        Public Function GetIntersectionTime(ByVal pRay As Ray, ByVal pMaxDist As Single, ByVal pStopAtFirstHit As Boolean) As Single
            Dim firstDistance As Single = Single.PositiveInfinity
            Dim t As Single = pMaxDist
            iTree.IntersectRay(pRay, t, pStopAtFirstHit, False)
#If VMAPS_DEBUG Then
            Log.WriteLine(LogType.DEBUG, "GetIntersectionTime: {0}", t)
#End If
            If t > 0 AndAlso t < Single.PositiveInfinity AndAlso pMaxDist > t Then
                firstDistance = t
            End If
            Return firstDistance
        End Function
#End If

        Public Sub New(ByVal Map As UInteger)
            Maps.Add(Map, Me)

            For x As Integer = 0 To 63
                For y As Integer = 0 To 63
                    TileUsed(x, y) = False
                Next
            Next

            Try
                Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\Map.dbc")
                Dim tmpMap As Integer

                Dim i As Integer = 0
                For i = 0 To tmpDBC.Rows - 1
                    tmpMap = tmpDBC.Item(i, 0)

                    If tmpMap = Map Then
                        ID = Map
                        Type = tmpDBC.Item(i, 2, DBC.DBCValueType.DBC_INTEGER)
                        Name = tmpDBC.Item(i, 4, DBC.DBCValueType.DBC_STRING)
                        Exit For
                    End If
                Next i

                tmpDBC.Dispose()
                Log.WriteLine(LogType.INFORMATION, "DBC: 1 Map initialized.", i)
            Catch e As System.IO.DirectoryNotFoundException
                Console.ForegroundColor = System.ConsoleColor.DarkRed
                Console.WriteLine("DBC File : Map missing.")
                Console.ForegroundColor = System.ConsoleColor.Gray
            End Try
        End Sub
        Public Sub Dispose() Implements System.IDisposable.Dispose
            For i As Integer = 0 To 63
                For j As Integer = 0 To 63
                    If Not Tiles(i, j) Is Nothing Then Tiles(i, j).Dispose()
                Next
            Next

            Maps.Remove(ID)
        End Sub
    End Class

    Public Maps As New Collections.Generic.Dictionary(Of UInteger, TMap)
    Public MapList As String

    Public Sub InitializeMaps()
        'DONE: Creating map list for queries
        Dim e As IEnumerator = Config.Maps.GetEnumerator
        e.Reset()
        If e.MoveNext() Then
            MapList = e.Current
            While e.MoveNext
                MapList += ", " & e.Current
            End While
        End If

        'DONE: Loading maps
        For Each ID As UInteger In Config.Maps
            Dim Map As New TMap(ID)
        Next

        Log.WriteLine(LogType.INFORMATION, "Initalizing: {0} Maps initialized.", Maps.Count)
    End Sub

    Public Sub GetMapTile(ByVal x As Single, ByVal y As Single, ByRef MapTileX As Byte, ByRef MapTileY As Byte)
        'How to calculate where is X,Y:
        MapTileX = Fix(32 - (x / TMapTile.SIZE))
        MapTileY = Fix(32 - (y / TMapTile.SIZE))
    End Sub
    Public Function GetMapTileX(ByVal x As Single) As Byte
        Return Fix(32 - (x / TMapTile.SIZE))
    End Function
    Public Function GetMapTileY(ByVal y As Single) As Byte
        Return Fix(32 - (y / TMapTile.SIZE))
    End Function
    Public Function GetSubMapTileX(ByVal x As Single) As Byte
        Return Fix(RESOLUTION_ZMAP * (32 - (x / TMapTile.SIZE) - Fix(32 - (x / TMapTile.SIZE))))
    End Function
    Public Function GetSubMapTileY(ByVal y As Single) As Byte
        Return Fix(RESOLUTION_ZMAP * (32 - (y / TMapTile.SIZE) - Fix(32 - (y / TMapTile.SIZE))))
    End Function
    Public Function GetZCoord(ByVal x As Single, ByVal y As Single, ByVal Map As UInteger) As Single
        Try
            Dim MapTileX As Byte = Fix(32 - (x / TMapTile.SIZE))
            Dim MapTileY As Byte = Fix(32 - (y / TMapTile.SIZE))
            Dim MapTile_LocalX As Byte = CType(RESOLUTION_ZMAP * (32 - (x / TMapTile.SIZE) - MapTileX), Byte)
            Dim MapTile_LocalY As Byte = CType(RESOLUTION_ZMAP * (32 - (y / TMapTile.SIZE) - MapTileY), Byte)
            Dim xNormalized As Single = RESOLUTION_ZMAP * (32 - (x / TMapTile.SIZE) - MapTileX) - MapTile_LocalX
            Dim yNormalized As Single = RESOLUTION_ZMAP * (32 - (y / TMapTile.SIZE) - MapTileY) - MapTile_LocalY

            If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then Return 0.0F

            Try
                Dim topHeight As Single = MathLerp( _
                    GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX, MapTile_LocalY), _
                    GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX + 1, MapTile_LocalY), _
                    xNormalized)

                Dim bottomHeight As Single = MathLerp( _
                    GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX, MapTile_LocalY + 1), _
                    GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX + 1, MapTile_LocalY + 1), _
                    xNormalized)

                Return MathLerp(topHeight, bottomHeight, yNormalized)
            Catch
                Return Maps(Map).Tiles(MapTileX, MapTileY).ZCoord(MapTile_LocalX, MapTile_LocalY)
            End Try
        Catch e As Exception
            Return 0.0F
        End Try
    End Function
    Public Function GetWaterLevel(ByVal x As Single, ByVal y As Single, ByVal Map As Integer) As Single
        Dim MapTileX As Byte = Fix(32 - (x / TMapTile.SIZE))
        Dim MapTileY As Byte = Fix(32 - (y / TMapTile.SIZE))
        Dim MapTile_LocalX As Byte = CType(TMapTile.RESOLUTION_WATER * (32 - (x / TMapTile.SIZE) - MapTileX), Byte)
        Dim MapTile_LocalY As Byte = CType(TMapTile.RESOLUTION_WATER * (32 - (y / TMapTile.SIZE) - MapTileY), Byte)

        If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then Return 0
        Return Maps(Map).Tiles(MapTileX, MapTileY).WaterLevel(MapTile_LocalX, MapTile_LocalY)
    End Function
    Public Function GetTerrainType(ByVal x As Single, ByVal y As Single, ByVal Map As Integer) As Byte
        Dim MapTileX As Byte = Fix(32 - (x / TMapTile.SIZE))
        Dim MapTileY As Byte = Fix(32 - (y / TMapTile.SIZE))
        Dim MapTile_LocalX As Byte = CType(TMapTile.RESOLUTION_TERRAIN * (32 - (x / TMapTile.SIZE) - MapTileX), Byte)
        Dim MapTile_LocalY As Byte = CType(TMapTile.RESOLUTION_TERRAIN * (32 - (y / TMapTile.SIZE) - MapTileY), Byte)

        If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then Return 0
        Return Maps(Map).Tiles(MapTileX, MapTileY).AreaTerrain(MapTile_LocalX, MapTile_LocalY)
    End Function
    Public Function GetAreaFlag(ByVal x As Single, ByVal y As Single, ByVal Map As Integer) As Integer
        Dim MapTileX As Byte = Fix(32 - (x / TMapTile.SIZE))
        Dim MapTileY As Byte = Fix(32 - (y / TMapTile.SIZE))
        Dim MapTile_LocalX As Byte = CType(TMapTile.RESOLUTION_FLAGS * (32 - (x / TMapTile.SIZE) - MapTileX), Byte)
        Dim MapTile_LocalY As Byte = CType(TMapTile.RESOLUTION_FLAGS * (32 - (y / TMapTile.SIZE) - MapTileY), Byte)

        If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then Return 0
        Return Maps(Map).Tiles(MapTileX, MapTileY).AreaFlag(MapTile_LocalX, MapTile_LocalY)
    End Function

    Public Function IsOutsideOfMap(ByRef c As BaseObject) As Boolean
        'NOTE: Disabled these checks because DBC data contains too big X/Y coords to be usefull
        Return False

        'Dim x As Single = c.positionX
        'Dim y As Single = c.positionY
        'Dim m As UInteger = c.MapID

        ''Check transform data
        'For Each i As WorldMapTransformsDimension In WorldMapTransforms
        '    If i.Map = m Then
        '        With i
        '            If x < .X_Maximum And x > .X_Minimum And _
        '               y < .Y_Maximum And y > .Y_Minimum Then

        '                Log.WriteLine(LogType.USER, "Applying map transform {0},{1},{2} -> {3},{4},{5}", x, y, m, .Dest_X, .Dest_Y, .Dest_Map)
        '                'x += .Dest_X
        '                'y += .Dest_Y
        '                'm = .Dest_Map
        '                'Exit For
        '                Return False
        '            End If
        '        End With
        '    End If
        'Next

        ''Check Map data
        'If WorldMapContinent.ContainsKey(m) Then
        '    With WorldMapContinent(m)
        '        If x > .X_Maximum Or x < .X_Minimum Or _
        '           y > .Y_Maximum Or y < .Y_Minimum Then
        '            Log.WriteLine(LogType.USER, "Outside map: {0:X}", c.GUID)
        '            Return True
        '        Else
        '            Return False
        '        End If
        '    End With
        'End If

        'Log.WriteLine(LogType.USER, "WorldMapContinent not found for map {0}.", c.MapID)
        'Return False
    End Function

#If ENABLE_PPOINTS Then
    Public Const PPOINT_BAD As Single = Single.MinValue
    Public Const PPOINT_LIMIT As Single = 5.0F
    Public Const PPOINT_SAVE As Integer = 5
    Public Const PPOINT_VERSION As String = "PP__1.00"

    Public Function GetZCoord_PP(ByVal x As Single, ByVal y As Single, ByVal Map As Integer) As Single
        Try
            Dim MapTileX As Byte = Fix(32 - (x / TMapTile.SIZE))
            Dim MapTileY As Byte = Fix(32 - (y / TMapTile.SIZE))
            Dim MapTile_LocalX As Byte = CType(RESOLUTION_ZMAP * (32 - (x / TMapTile.SIZE) - MapTileX), Byte)
            Dim MapTile_LocalY As Byte = CType(RESOLUTION_ZMAP * (32 - (y / TMapTile.SIZE) - MapTileY), Byte)

            If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then Return 0

            Return Maps(Map).Tiles(MapTileX, MapTileY).ZCoord_PP(MapTile_LocalX, MapTile_LocalY)
        Catch e As Exception
            Return 0
        End Try
    End Function
    Public Sub SetZCoord_PP(ByVal x As Single, ByVal y As Single, ByVal Map As Integer, ByVal z As Single)
        Try
            Dim MapTileX As Byte = Fix(32 - (x / TMapTile.SIZE))
            Dim MapTileY As Byte = Fix(32 - (y / TMapTile.SIZE))
            Dim MapTile_LocalX As Byte = CType(RESOLUTION_ZMAP * (32 - (x / TMapTile.SIZE) - MapTileX), Byte)
            Dim MapTile_LocalY As Byte = CType(RESOLUTION_ZMAP * (32 - (y / TMapTile.SIZE) - MapTileY), Byte)

            If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then Return
            Maps(Map).Tiles(MapTileX, MapTileY).ZCoord_PP(MapTile_LocalX, MapTile_LocalY) = z

            'Notify PPoints changes
            Maps(Map).Tiles(MapTileX, MapTileY).ZCoord_PP_ModTimes += 1

            If Maps(Map).Tiles(MapTileX, MapTileY).ZCoord_PP_ModTimes > PPOINT_SAVE Then
                Maps(Map).Tiles(MapTileX, MapTileY).ZCoord_PP_Save()
            End If

        Catch e As Exception
            Return
        End Try
    End Sub

    Public Function GetZCoord(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal Map As Integer) As Single
        Try
            Dim MapTileX As Byte = Fix(32 - (x / TMapTile.SIZE))
            Dim MapTileY As Byte = Fix(32 - (y / TMapTile.SIZE))
            Dim MapTile_LocalX As Byte = CType(RESOLUTION_ZMAP * (32 - (x / TMapTile.SIZE) - MapTileX), Byte)
            Dim MapTile_LocalY As Byte = CType(RESOLUTION_ZMAP * (32 - (y / TMapTile.SIZE) - MapTileY), Byte)
            Dim xNormalized As Single = RESOLUTION_ZMAP * (32 - (x / TMapTile.SIZE) - MapTileX) - MapTile_LocalX
            Dim yNormalized As Single = RESOLUTION_ZMAP * (32 - (y / TMapTile.SIZE) - MapTileY) - MapTile_LocalY

            If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then
                'Return vmap height if one was found
                Dim VMapHeight As Single = GetVMapHeight(Map, x, y, z + 2.0F)
                If VMapHeight <> VMAP_INVALID_HEIGHT_VALUE Then
                    Return VMapHeight
                End If

                Return 0.0F
            End If

            'Return map info if we are near the ground
            If Math.Abs(Maps(Map).Tiles(MapTileX, MapTileY).ZCoord(MapTile_LocalX, MapTile_LocalY) - z) < PPOINT_LIMIT _
             OrElse Maps(Map).Tiles(MapTileX, MapTileY).ZCoord_PP(MapTile_LocalX, MapTile_LocalY) = PPOINT_BAD Then

                Try
                    Dim topHeight As Single = MathLerp( _
                        GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX, MapTile_LocalY), _
                        GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX + 1, MapTile_LocalY), _
                        xNormalized)

                    Dim bottomHeight As Single = MathLerp( _
                        GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX, MapTile_LocalY + 1), _
                        GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX + 1, MapTile_LocalY + 1), _
                        xNormalized)

                    Return MathLerp(topHeight, bottomHeight, yNormalized)
                Catch
                    Return Maps(Map).Tiles(MapTileX, MapTileY).ZCoord(MapTile_LocalX, MapTile_LocalY)
                End Try
            End If

            'Return vmap height if one was found
            Dim VMapHeight As Single = GetVMapHeight(map, x, y, z + 2.0F)
            If VMapHeight <> VMAP_INVALID_HEIGHT_VALUE Then
                return VMapHeight
            End If

            'Return pp info if we are too far from ground
            Try
                Dim topHeight As Single = MathLerp( _
                    GetHeightPP(Map, MapTileX, MapTileY, MapTile_LocalX, MapTile_LocalY), _
                    GetHeightPP(Map, MapTileX, MapTileY, MapTile_LocalX + 1, MapTile_LocalY), _
                    xNormalized)

                Dim bottomHeight As Single = MathLerp( _
                    GetHeightPP(Map, MapTileX, MapTileY, MapTile_LocalX, MapTile_LocalY + 1), _
                    GetHeightPP(Map, MapTileX, MapTileY, MapTile_LocalX + 1, MapTile_LocalY + 1), _
                    xNormalized)

                Return MathLerp(topHeight, bottomHeight, yNormalized)
            Catch
                Return Maps(Map).Tiles(MapTileX, MapTileY).ZCoord_PP(MapTile_LocalX, MapTile_LocalY)
            End Try
        Catch ex As Exception
            Log.WriteLine(LogType.FAILED, ex.ToString)
            Return z
        End Try
    End Function

    Private Function GetHeightPP(ByVal Map As UInteger, ByVal MapTileX As Byte, ByVal MapTileY As Byte, ByVal MapTileLocalX As Byte, ByVal MapTileLocalY As Byte) As Single
        If MapTileLocalX > RESOLUTION_ZMAP Then
            MapTileX += 1
            MapTileLocalX -= RESOLUTION_ZMAP + 1
        ElseIf MapTileLocalX < 0 Then
            MapTileX -= 1
            MapTileLocalX = (-MapTileLocalX) - 1
        End If
        If MapTileLocalY > RESOLUTION_ZMAP Then
            MapTileY += 1
            MapTileLocalY -= RESOLUTION_ZMAP + 1
        ElseIf MapTileLocalY < 0 Then
            MapTileY -= 1
            MapTileLocalY = (-MapTileLocalY) - 1
        End If

        Dim height As Single = Maps(Map).Tiles(MapTileX, MapTileY).ZCoord_PP(MapTileLocalX, MapTileLocalY)
        If height = PPOINT_BAD Then
            Return Maps(Map).Tiles(MapTileX, MapTileY).ZCoord(MapTileLocalX, MapTileLocalY)
        Else
            Return height
        End If
    End Function
#Else
    Public Function GetZCoord(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal Map As UInteger) As Single
        Try
            Dim MapTileX As Byte = Fix(32 - (x / TMapTile.SIZE))
            Dim MapTileY As Byte = Fix(32 - (y / TMapTile.SIZE))
            Dim MapTile_LocalX As Byte = CType(RESOLUTION_ZMAP * (32 - (x / TMapTile.SIZE) - MapTileX), Byte)
            Dim MapTile_LocalY As Byte = CType(RESOLUTION_ZMAP * (32 - (y / TMapTile.SIZE) - MapTileY), Byte)
            Dim xNormalized As Single = RESOLUTION_ZMAP * (32 - (x / TMapTile.SIZE) - MapTileX) - MapTile_LocalX
            Dim yNormalized As Single = RESOLUTION_ZMAP * (32 - (y / TMapTile.SIZE) - MapTileY) - MapTile_LocalY

            If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then
                'Return vmap height if one was found
                Dim VMapHeight As Single = GetVMapHeight(Map, x, y, z + 5.0F)
                If VMapHeight <> VMAP_INVALID_HEIGHT_VALUE Then
                    Return VMapHeight
                End If

                Return 0.0F
            End If

            If Math.Abs(Maps(Map).Tiles(MapTileX, MapTileY).ZCoord(MapTile_LocalX, MapTile_LocalY) - z) >= 2.0F Then
                'Return vmap height if one was found
                Dim VMapHeight As Single = GetVMapHeight(Map, x, y, z + 5.0F)
                If VMapHeight <> VMAP_INVALID_HEIGHT_VALUE Then
                    Return VMapHeight
                End If
            End If

            Try
                Dim topHeight As Single = MathLerp( _
                    GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX, MapTile_LocalY), _
                    GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX + 1, MapTile_LocalY), _
                    xNormalized)

                Dim bottomHeight As Single = MathLerp( _
                    GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX, MapTile_LocalY + 1), _
                    GetHeight(Map, MapTileX, MapTileY, MapTile_LocalX + 1, MapTile_LocalY + 1), _
                    xNormalized)

                Return MathLerp(topHeight, bottomHeight, yNormalized)
            Catch
                Return Maps(Map).Tiles(MapTileX, MapTileY).ZCoord(MapTile_LocalX, MapTile_LocalY)
            End Try
        Catch ex As Exception
            Log.WriteLine(LogType.FAILED, ex.ToString)
            Return z
        End Try
    End Function
#End If

    Private Function GetHeight(ByVal Map As UInteger, ByVal MapTileX As Byte, ByVal MapTileY As Byte, ByVal MapTileLocalX As Byte, ByVal MapTileLocalY As Byte) As Single
        If MapTileLocalX > RESOLUTION_ZMAP Then
            MapTileX += 1
            MapTileLocalX -= RESOLUTION_ZMAP + 1
        ElseIf MapTileLocalX < 0 Then
            MapTileX -= 1
            MapTileLocalX = (-MapTileLocalX) - 1
        End If
        If MapTileLocalY > RESOLUTION_ZMAP Then
            MapTileY += 1
            MapTileLocalY -= RESOLUTION_ZMAP + 1
        ElseIf MapTileLocalY < 0 Then
            MapTileY -= 1
            MapTileLocalY = (-MapTileLocalY) - 1
        End If

        Return Maps(Map).Tiles(MapTileX, MapTileY).ZCoord(MapTileLocalX, MapTileLocalY)
    End Function

    Public Function IsInLineOfSight(ByRef obj As BaseObject, ByRef obj2 As BaseObject) As Boolean
        Return IsInLineOfSight(obj.MapID, obj.positionX, obj.positionY, obj.positionZ + 2.0F, obj2.positionX, obj2.positionY, obj2.positionZ + 2.0F)
    End Function

    Public Function IsInLineOfSight(ByRef obj As BaseObject, ByVal x2 As Single, ByVal y2 As Single, ByVal z2 As Single) As Boolean
        Return IsInLineOfSight(obj.MapID, obj.positionX, obj.positionY, obj.positionZ + 2.0F, x2, y2, z2)
    End Function

    Public Function IsInLineOfSight(ByVal MapID As UInteger, ByVal x1 As Single, ByVal y1 As Single, ByVal z1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal z2 As Single) As Boolean
        Dim result As Boolean = True
#If VMAPS Then
        If Config.LineOfSightEnabled AndAlso Maps.ContainsKey(MapID) Then
            Dim pos1 As Vector3 = convertPositionToInternalRep(x1, y1, z1)
            Dim pos2 As Vector3 = convertPositionToInternalRep(x2, y2, z2)
            If pos1 <> pos2 Then
                Try
                    result = Maps(MapID).IsInLineOfSight(pos1, pos2)
                Catch ex As Exception
                    result = True
                    Log.WriteLine(LogType.CRITICAL, "Error checking line of sight.{0}{1}", vbNewLine, ex.ToString)
                End Try
            End If
        End If
#End If
        Return result
    End Function

    Public Function GetVMapHeight(ByVal MapID As UInteger, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Single
        Dim height As Single = VMAP_INVALID_HEIGHT_VALUE
#If VMAPS Then
        If Config.HeightCalcEnabled AndAlso Maps.ContainsKey(MapID) Then
            Dim pos As Vector3 = convertPositionToInternalRep(x, y, z)
#If VMAPS_DEBUG Then
            Log.WriteLine(LogType.DEBUG, "Location transformed: {0} {1} {2}", pos.x, pos.y, pos.z)
#End If
            Try
                height = Maps(MapID).GetHeight(pos)
            Catch ex As Exception
                height = Single.PositiveInfinity
                Log.WriteLine(LogType.CRITICAL, "Error checking height of map.{0}{1}", vbNewLine, ex.ToString)
            End Try
#If VMAPS_DEBUG Then
            Log.WriteLine(LogType.DEBUG, "GetVMapHeight: {0}", height)
#End If
            If Not (height < Single.PositiveInfinity) Then
                height = VMAP_INVALID_HEIGHT_VALUE
            End If
        End If
#End If
        Return height
    End Function

    Public Function GetObjectHitPos(ByRef obj As BaseObject, ByRef obj2 As BaseObject, ByRef rx As Single, ByRef ry As Single, ByRef rz As Single, ByVal pModifyDist As Single) As Boolean
        Return GetObjectHitPos(obj.MapID, obj.positionX, obj.positionY, obj.positionZ + 2.0F, obj2.positionX, obj2.positionY, obj2.positionZ + 2.0F, rx, ry, rz, pModifyDist)
    End Function

    Public Function GetObjectHitPos(ByRef obj As BaseObject, ByVal x2 As Single, ByVal y2 As Single, ByVal z2 As Single, ByRef rx As Single, ByRef ry As Single, ByRef rz As Single, ByVal pModifyDist As Single) As Boolean
        Return GetObjectHitPos(obj.MapID, obj.positionX, obj.positionY, obj.positionZ + 2.0F, x2, y2, z2, rx, ry, rz, pModifyDist)
    End Function

    Public Function GetObjectHitPos(ByVal MapID As UInteger, ByVal x1 As Single, ByVal y1 As Single, ByVal z1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal z2 As Single, ByRef rx As Single, ByRef ry As Single, ByRef rz As Single, ByVal pModifyDist As Single) As Boolean
        Dim result As Boolean = False
#If VMAPS Then
        If Config.LineOfSightEnabled AndAlso Maps.ContainsKey(MapID) Then
            Dim pos1 As Vector3 = convertPositionToInternalRep(x1, y1, z1)
            Dim pos2 As Vector3 = convertPositionToInternalRep(x2, y2, z2)
            Dim resultPos As Vector3
            If pos1 <> pos2 Then
                Try
                    result = Maps(MapID).GetObjectHitPos(pos1, pos2, resultPos, pModifyDist)
                    resultPos = convertPositionToNormalRep(resultPos.x, resultPos.y, resultPos.z)
                Catch ex As Exception
                    result = False
                    resultPos = pos2
                    Log.WriteLine(LogType.CRITICAL, "Error checking object hit position.{0}{1}", vbNewLine, ex.ToString)
                End Try
                rx = resultPos.x
                ry = resultPos.y
                rz = resultPos.z
            End If
        End If
#End If
        Return result
    End Function

    Public Sub LoadSpawns(ByVal TileX As Byte, ByVal TileY As Byte, ByVal TileMap As UInteger, ByVal TileInstance As UInteger)
        'Caluclate (x1, y1) and (x2, y2)
        Dim MinX As Single = ((32 - TileX) * TMapTile.SIZE)
        Dim MaxX As Single = ((32 - (TileX + 1)) * TMapTile.SIZE)
        Dim MinY As Single = ((32 - TileY) * TMapTile.SIZE)
        Dim MaxY As Single = ((32 - (TileY + 1)) * TMapTile.SIZE)
        'We need the maximum value to be the largest value
        If MinX > MaxX Then
            Dim tmpSng As Single = MinX
            MinX = MaxX
            MaxX = tmpSng
        End If
        If MinY > MaxY Then
            Dim tmpSng As Single = MinY
            MinY = MaxY
            MaxY = tmpSng
        End If

        'DONE: Instance units get a new specific GUID
        Dim InstanceGuidAdd As ULong = 0UL
        If TileInstance > 0 Then
            InstanceGuidAdd = 1000000UL + ((TileInstance - 1) * 100000UL)
        End If

        'DONE: Creatures
        Dim MysqlQuery As New DataTable
        WorldDatabase.Query(String.Format("SELECT * FROM spawns_creatures LEFT OUTER JOIN game_event_creature ON spawns_creatures.spawn_id = game_event_creature.guid WHERE spawn_map={0} AND spawn_positionX BETWEEN '{1}' AND '{2}' AND spawn_positionY BETWEEN '{3}' AND '{4}';", TileMap, MinX, MaxX, MinY, MaxY), MysqlQuery)
        For Each InfoRow As DataRow In MysqlQuery.Rows
            If Not WORLD_CREATUREs.ContainsKey(CType(InfoRow.Item("spawn_id"), Long) + InstanceGuidAdd + GUID_UNIT) Then
                Try
                    Dim tmpCr As CreatureObject = New CreatureObject(CType(InfoRow.Item("spawn_id"), Long) + InstanceGuidAdd, InfoRow)
                    If tmpCr.GameEvent = 0 Then
                        tmpCr.instance = TileInstance
                        tmpCr.AddToWorld()
                    End If
                Catch ex As Exception
                    Log.WriteLine(LogType.CRITICAL, "Error when creating creature [{0}].{1}{2}", InfoRow.Item("spawn_entry"), vbNewLine, ex.ToString)
                End Try
            End If
        Next

        'DONE: Gameobjects
        MysqlQuery.Clear()
        WorldDatabase.Query(String.Format("SELECT * FROM spawns_gameobjects LEFT OUTER JOIN game_event_gameobject ON spawns_gameobjects.spawn_id = game_event_gameobject.guid WHERE spawn_map={0} AND spawn_spawntime>=0 AND spawn_positionX BETWEEN '{1}' AND '{2}' AND spawn_positionY BETWEEN '{3}' AND '{4}';", TileMap, MinX, MaxX, MinY, MaxY), MysqlQuery)
        For Each InfoRow As DataRow In MysqlQuery.Rows
            If Not WORLD_GAMEOBJECTs.ContainsKey(CType(InfoRow.Item("spawn_id"), ULong) + InstanceGuidAdd + GUID_GAMEOBJECT) AndAlso _
              Not WORLD_GAMEOBJECTs.ContainsKey(CType(InfoRow.Item("spawn_id"), ULong) + InstanceGuidAdd + GUID_TRANSPORT) Then
                Try
                    Dim tmpGo As GameObjectObject = New GameObjectObject(CType(InfoRow.Item("spawn_id"), ULong) + InstanceGuidAdd, InfoRow)
                    If tmpGo.GameEvent = 0 Then
                        tmpGo.instance = TileInstance
                        tmpGo.AddToWorld()
                    End If
                Catch ex As Exception
                    Log.WriteLine(LogType.CRITICAL, "Error when creating gameobject [{0}].{1}{2}", InfoRow.Item("spawn_entry"), vbNewLine, ex.ToString)
                End Try
            End If
        Next

        'DONE: Corpses
        MysqlQuery.Clear()
        CharacterDatabase.Query(String.Format("SELECT * FROM tmpspawnedcorpses WHERE corpse_mapId={0} AND corpse_instance={5} AND corpse_positionX BETWEEN '{1}' AND '{2}' AND corpse_positionY BETWEEN '{3}' AND '{4}';", TileMap, MinX, MaxX, MinY, MaxY, TileInstance), MysqlQuery)
        For Each InfoRow As DataRow In MysqlQuery.Rows
            If Not WORLD_CORPSEOBJECTs.ContainsKey(CType(InfoRow.Item("corpse_guid"), ULong) + GUID_CORPSE) Then
                Try
                    Dim tmpCorpse As CorpseObject = New CorpseObject(CType(InfoRow.Item("corpse_guid"), ULong), InfoRow)
                    tmpCorpse.instance = TileInstance
                    tmpCorpse.AddToWorld()
                Catch ex As Exception
                    Log.WriteLine(LogType.CRITICAL, "Error when creating corpse [{0}].{1}{2}", InfoRow.Item("corpse_guid"), vbNewLine, ex.ToString)
                End Try
            End If
        Next

        'DONE: Transports
        Try
            WORLD_TRANSPORTs_Lock.AcquireReaderLock(1000)
            For Each Transport As KeyValuePair(Of ULong, TransportObject) In WORLD_TRANSPORTs
                Try
                    If Transport.Value.MapID = TileMap AndAlso Transport.Value.positionX >= MinX AndAlso Transport.Value.positionX <= MaxX AndAlso Transport.Value.positionY >= MinY AndAlso Transport.Value.positionY <= MaxY Then
                        If Maps(TileMap).Tiles(TileX, TileY).GameObjectsHere.Contains(Transport.Value.GUID) = False Then
                            Maps(TileMap).Tiles(TileX, TileY).GameObjectsHere.Add(Transport.Value.GUID)
                        End If
                        Transport.Value.NotifyEnter()
                    End If
                Catch ex As Exception
                    Log.WriteLine(LogType.CRITICAL, "Error when creating transport [{0}].{1}{2}", Transport.Key - GUID_MO_TRANSPORT, vbNewLine, ex.ToString)
                End Try
            Next
        Catch
        Finally
            WORLD_TRANSPORTs_Lock.ReleaseReaderLock()
        End Try
    End Sub
    Public Sub UnloadSpawns(ByVal TileX As Byte, ByVal TileY As Byte, ByVal TileMap As UInteger)
        'Caluclate (x1, y1) and (x2, y2)
        Dim MinX As Single = ((32 - TileX) * TMapTile.SIZE)
        Dim MaxX As Single = ((32 - (TileX + 1)) * TMapTile.SIZE)
        Dim MinY As Single = ((32 - TileY) * TMapTile.SIZE)
        Dim MaxY As Single = ((32 - (TileY + 1)) * TMapTile.SIZE)
        'We need the maximum value to be the largest value
        If MinX > MaxX Then
            Dim tmpSng As Single = MinX
            MinX = MaxX
            MaxX = tmpSng
        End If
        If MinY > MaxY Then
            Dim tmpSng As Single = MinY
            MinY = MaxY
            MaxY = tmpSng
        End If


        Try
            WORLD_CREATUREs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each Creature As KeyValuePair(Of ULong, CreatureObject) In WORLD_CREATUREs
                If CType(Creature.Value, CreatureObject).MapID = TileMap AndAlso CType(Creature.Value, CreatureObject).SpawnX >= MinX AndAlso CType(Creature.Value, CreatureObject).SpawnX <= MaxX AndAlso CType(Creature.Value, CreatureObject).SpawnY >= MinY AndAlso CType(Creature.Value, CreatureObject).SpawnY <= MaxY Then
                    CType(Creature.Value, CreatureObject).Destroy()
                End If
            Next
        Catch ex As Exception
            Log.WriteLine(LogType.CRITICAL, ex.ToString, Nothing)
        Finally
            WORLD_CREATUREs_Lock.ReleaseReaderLock()
        End Try


        For Each Gameobject As KeyValuePair(Of ULong, GameObjectObject) In WORLD_GAMEOBJECTs
            If CType(Gameobject.Value, GameObjectObject).MapID = TileMap AndAlso CType(Gameobject.Value, GameObjectObject).positionX >= MinX AndAlso CType(Gameobject.Value, GameObjectObject).positionX <= MaxX AndAlso CType(Gameobject.Value, GameObjectObject).positionY >= MinY AndAlso CType(Gameobject.Value, GameObjectObject).positionY <= MaxY Then
                CType(Gameobject.Value, GameObjectObject).Destroy()
            End If
        Next

        For Each Corpseobject As KeyValuePair(Of ULong, CorpseObject) In WORLD_CORPSEOBJECTs
            If CType(Corpseobject.Value, CorpseObject).MapID = TileMap AndAlso CType(Corpseobject.Value, CorpseObject).positionX >= MinX AndAlso CType(Corpseobject.Value, CorpseObject).positionX <= MaxX AndAlso CType(Corpseobject.Value, CorpseObject).positionY >= MinY AndAlso CType(Corpseobject.Value, CorpseObject).positionY <= MaxY Then
                CType(Corpseobject.Value, CorpseObject).Destroy()
            End If
        Next


    End Sub


#End Region
#Region "Instances"


    Public Enum TransferAbortReason As Short
        TRANSFER_ABORT_MAX_PLAYERS = &H1                ' Transfer Aborted: instance is full
        TRANSFER_ABORT_NOT_FOUND = &H2                  ' Transfer Aborted: instance not found
        TRANSFER_ABORT_TOO_MANY_INSTANCES = &H3         ' You have entered too many instances recently.
        TRANSFER_ABORT_ZONE_IN_COMBAT = &H5             ' Unable to zone in while an encounter is in progress.
        TRANSFER_ABORT_INSUF_EXPAN_LVL1 = &H106         ' You must have TBC expansion installed to access this area.
        TRANSFER_ABORT_DIFFICULTY1 = &H7                ' Normal difficulty mode is not available for %s.
        TRANSFER_ABORT_DIFFICULTY2 = &H107              ' Heroic difficulty mode is not available for %s.
        TRANSFER_ABORT_DIFFICULTY3 = &H207              ' Epic difficulty mode is not available for %s.
    End Enum
    Public Sub SendTransferAborted(ByRef Client As ClientClass, ByVal Map As Integer, ByVal Reason As TransferAbortReason)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_TRANSFER_ABORTED [{2}:{3}]", Client.IP, Client.Port, Map, Reason)

        Dim p As New PacketClass(OPCODES.SMSG_TRANSFER_ABORTED)
        p.AddInt32(Map)
        p.AddInt16(Reason)
        Client.Send(p)
        p.Dispose()
    End Sub


#End Region

#Region "Graveyards"
    Public Graveyards As New Dictionary(Of Integer, TGraveyard)
    Public Class TGraveyard
        Public x As Single
        Public y As Single
        Public z As Single
        Public Map As Integer

        Public Sub New(ByVal px As Single, ByVal py As Single, ByVal pz As Single, ByVal pMap As Integer)
            x = px
            y = py
            z = pz
            Map = pMap
        End Sub
    End Class
    Public Sub GoToNearestGraveyard(ByRef Character As CharacterObject, Optional ByVal Alive As Boolean = False, Optional ByVal Teleport As Boolean = True)
        Character.ZoneCheck()

        Dim GraveQuery As New DataTable
        WorldDatabase.Query(String.Format("SELECT id, faction FROM world_graveyard_zone WHERE ghost_map = {0} AND ghost_zone = {1}", Character.MapID, Character.ZoneID), GraveQuery)

        If GraveQuery.Rows.Count = 0 Then
            Log.WriteLine(LogType.INFORMATION, "GraveYards: No near graveyards for map [{0}], zone [{1}]", Character.MapID, Character.ZoneID)
            Exit Sub
        End If

        Dim foundNear As Boolean = False
        Dim distNear As Single = 0.0F
        Dim entryNear As TGraveyard = Nothing
        Dim entryFar As TGraveyard = Nothing

        For Each GraveLink As DataRow In GraveQuery.Rows
            Dim GraveyardID As Integer = GraveLink.Item("id")
            Dim GraveyardFaction As Integer = GraveLink.Item("faction")
            If Graveyards.ContainsKey(GraveyardID) = False Then
                Log.WriteLine(LogType.INFORMATION, "GraveYards: Graveyard link invalid [{0}]", GraveyardID)
                Continue For
            End If

            If Character.MapID <> Graveyards(GraveyardID).Map Then
                If entryFar Is Nothing Then entryFar = Graveyards(GraveyardID)
                Continue For
            End If

            'Skip graveyards that ain't for your faction
            If GraveyardFaction <> 0 AndAlso GraveyardFaction <> Character.Team Then Continue For

            Dim dist2 As Single = GetDistance(Character.positionX, Graveyards(GraveyardID).x, Character.positionY, Graveyards(GraveyardID).y, Character.positionZ, Graveyards(GraveyardID).z)
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
        If selectedGraveyard Is Nothing Then selectedGraveyard = entryFar

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

            Log.WriteLine(LogType.INFORMATION, "GraveYards: GraveYard.Map[{0}], GraveYard.X[{1}], GraveYard.Y[{2}], GraveYard.Z[{3}]", selectedGraveyard.Map, selectedGraveyard.x, selectedGraveyard.y, selectedGraveyard.z)
            Character.Teleport(selectedGraveyard.x, selectedGraveyard.y, selectedGraveyard.z, 0, selectedGraveyard.Map)
            Character.SendDeathReleaseLoc(selectedGraveyard.x, selectedGraveyard.y, selectedGraveyard.z, selectedGraveyard.Map)
        Else
            Character.positionX = selectedGraveyard.x
            Character.positionY = selectedGraveyard.y
            Character.positionZ = selectedGraveyard.z
            Character.MapID = selectedGraveyard.Map
        End If
    End Sub

#End Region


End Module
