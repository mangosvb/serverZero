'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
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

Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports mangosVB.Common
Imports mangosVB.Common.BaseWriter

Public Delegate Function HeightFuncDelegate(posX As Single, posY As Single) As Single

Public Module WS_Maps
#Region "Zones"
    Public AreaTable As New Dictionary(Of Integer, TArea)

    Public Function GetAreaIDByMapandParent(ByVal mapId As Integer, ByVal parentID As Integer) As Integer
        For Each thisArea As KeyValuePair(Of Integer, TArea) In AreaTable
            Dim thisMap As Integer = thisArea.Value.mapId
            Dim thisParent As Integer = thisArea.Value.Zone

            If thisMap = mapId And thisParent = parentID Then
                Return thisArea.Key
            End If
        Next

        Return -999
    End Function

    Public Class TArea
        Public ID As Integer
        Public mapId As Integer
        Public Level As Byte
        Public Zone As Integer
        Public ZoneType As Integer
        Public Team As AreaTeam
        Public Name As String

        Public Function IsMyLand(ByRef objCharacter As CharacterObject) As Boolean
            If Team = AreaTeam.AREATEAM_NONE Then Return False
            If objCharacter.IsHorde = False Then Return Team = AreaTeam.AREATEAM_ALLY
            If objCharacter.IsHorde = True Then Return Team = AreaTeam.AREATEAM_HORDE
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

    Public Const RESOLUTION_ZMAP As Integer = 64 - 1

#Region "Continents"

    Public Class TMapTile
        Implements IDisposable

        Public Const RESOLUTION_WATER As Integer = 128 - 1
        Public Const RESOLUTION_TERRAIN As Integer = 16 - 1

        Public Const SIZE As Single = 533.3333F

        Public Const RESOLUTION_FLAGS As Integer = 16 - 1
        Public Const RESOLUTION_HEIGHT As Integer = 128 - 1

        Public Const MAX_HEIGHT As Single = 100000.0F
        Public Const INVALID_HEIGHT As Single = -100000.0F
        Public Const INVALID_HEIGHT_VALUE As Single = -200000.0F
        Public Const MAX_FALL_DISTANCE As Single = -250000.0F
        Public Const DEFAULT_HEIGHT_SEARCH As Single = 10.0F
        Public Const DEFAULT_WATER_SEARCH As Single = 50.0F

        'TMap contains 64x64 TMapTile(s)
        Public ZCoord(,) As Single

#If ENABLE_PPOINTS Then
        'Public ZCoord_PP(RESOLUTION_ZMAP, RESOLUTION_ZMAP) As Single
        Public ZCoord_PP(,) As Single
        Public ZCoord_PP_ModTimes As Integer = 0
        Friend Shared SIZE As Single

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

        Private gridArea As UShort = 0
        Private gridHeight As Single = 0.0F
        Private areaFlags As UShort(,) = Nothing

        Private gridHeightMultiplier As Single = 0.0F
        Private heightsV8 As Single() = Nothing
        Private heightsV9 As Single() = Nothing
        Private heights16V8 As UShort() = Nothing
        Private heights16V9 As UShort() = Nothing
        Private heights8V8 As Byte() = Nothing
        Private heights8V9 As Byte() = Nothing
        Private getHeightFunc As HeightFuncDelegate = Nothing

        Private liquidType As UShort = 0
        Private liquidOffX As Integer = 0
        Private liquidOffY As Integer = 0
        Private liquidWidth As Integer = 0
        Private liquidHeight As Integer = 0
        Private liquidLevel As Single = 0.0F
        Private liquidEntry As UShort(,) = Nothing
        Private liquidFlags As UShort(,) = Nothing
        Private waterLevel As Single(,) = Nothing

        Private cellX As Integer = 0
        Private cellY As Integer = 0
        Private cellMap As UInteger = 0

        Private m_hasVmapData As Boolean = False
        Private m_hasMmapData As Boolean = False

#Region "Properties"
        ''' <summary>
        ''' Gets if this map tile has Vmap data.
        ''' </summary>
        Public ReadOnly Property HasVmapData() As Boolean
            Get
                Return m_hasVmapData
            End Get
        End Property

        ''' <summary>
        ''' Gets if this map tile has Mmap data.
        ''' </summary>
        Public ReadOnly Property HasMmapData() As Boolean
            Get
                Return m_hasMmapData
            End Get
        End Property

        ''' <summary>
        ''' Gets the bit count for the height data.
        ''' </summary>
        Public ReadOnly Property HeightDataBits() As Integer
            Get
                Return (If(heightsV8 IsNot Nothing, 32, (If(heights16V8 IsNot Nothing, 16, (If(heights8V8 IsNot Nothing, 8, 0))))))
            End Get
        End Property
#End Region

        ''' <summary>
        ''' Creates a new map tile.
        ''' </summary>
        ''' <param name="tileX">The X-tile.</param>
        ''' <param name="tileY">the Y-tile.</param>
        ''' <param name="tileMap">The tile map ID.</param>
        Public Sub New(ByVal tileX As Byte, ByVal tileY As Byte, ByVal tileMap As UInteger)
            'DONE: Don't load maptiles we don't handle
            If Not Maps.ContainsKey(tileMap) Then Exit Sub

            ReDim ZCoord(RESOLUTION_ZMAP, RESOLUTION_ZMAP)
#If ENABLE_PPOINTS Then
            ReDim ZCoord_PP(RESOLUTION_ZMAP, RESOLUTION_ZMAP)
#End If

            cellX = tileX
            cellY = tileY
            cellMap = tileMap

            ' Load the map file
            Dim filePath As String = String.Format("{0}{1}{2}.map", Format(tileMap, "000"), Format(tileX, "00"), Format(tileY, "00"))
            If Not File.Exists("maps\" & filePath) Then
                Log.WriteLine(LogType.WARNING, "Map file [{0}] not found", filePath)
            Else
                Dim stream As New FileStream("maps\" & filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 82704, FileOptions.SequentialScan)
                Dim reader As New BinaryReader(stream)

                Dim mapMagic As New String(reader.ReadChars(4))
                If mapMagic <> "MAPS" Then
                    Log.WriteLine(LogType.WARNING, "Invalid map magic in file [{0}].", filePath)
                    Return
                End If
                Dim versionMagic As New String(reader.ReadChars(4))
                If versionMagic <> "z1.3" Then
                    Log.WriteLine(LogType.WARNING, "Invalid map magic in file [{0}].", filePath)
                    Return
                End If

                Dim areaMapOffset As Integer = reader.ReadInt32()
                Dim areaMapSize As Integer = reader.ReadInt32()
                Dim heightMapOffset As Integer = reader.ReadInt32()
                Dim heightMapSize As Integer = reader.ReadInt32()
                Dim liquidMapOffset As Integer = reader.ReadInt32()
                Dim liquidMapSize As Integer = reader.ReadInt32()
                Dim holesOffset As Integer = reader.ReadInt32()
                Dim holesSize As Integer = reader.ReadInt32()

                If areaMapOffset > 0 AndAlso Not LoadAreaData(reader, areaMapOffset, areaMapSize) Then
                    Log.WriteLine(LogType.WARNING, "Failed to load area data from map file [{0}].", filePath)
                    Return
                End If

                If heightMapOffset > 0 AndAlso Not LoadHeightData(reader, heightMapOffset, heightMapSize) Then
                    Log.WriteLine(LogType.WARNING, "Failed to load height data from map file [{0}].", filePath)
                    Return
                End If

                If liquidMapOffset > 0 AndAlso Not LoadLiquidData(reader, liquidMapOffset, liquidMapSize) Then
                    Log.WriteLine(LogType.WARNING, "Failed to load liquid data from map file [{0}].", filePath)
                    Return
                End If

                reader.Close()
                reader.Dispose()
                stream.Close()
                stream.Dispose()
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
#Region "Loading"
#Region "Area Data"
        ''' <summary>
        ''' Reads the area data from a map file.
        ''' </summary>
        ''' <param name="reader">The binary reader.</param>
        ''' <param name="offset">The offset for the area data.</param>
        ''' <param name="size">The size of the area data.</param>
        Private Function LoadAreaData(ByVal reader As BinaryReader, ByVal offset As Integer, ByVal size As Integer) As Boolean
            reader.BaseStream.Seek(offset, SeekOrigin.Begin)
            Dim magic As New String(reader.ReadChars(4))
            If magic <> "AREA" Then
                Return False
            End If

            Dim flags As UShort = reader.ReadUInt16()
            gridArea = reader.ReadUInt16()
            If (flags And 1) = 0 Then
                areaFlags = New UShort(RESOLUTION_FLAGS - 1, RESOLUTION_FLAGS - 1) {}
                For x As Integer = 0 To RESOLUTION_FLAGS - 1
                    For y As Integer = 0 To RESOLUTION_FLAGS - 1
                        areaFlags(x, y) = reader.ReadUInt16()
                    Next
                Next
            End If
            Return True
        End Function
#End Region

#Region "Height Data"
        ''' <summary>
        ''' Reads the height data from a map file.
        ''' </summary>
        ''' <param name="reader">The binary reader.</param>
        ''' <param name="offset">The offset for the height data.</param>
        ''' <param name="size">The size of the height data.</param>
        Private Function LoadHeightData(ByVal reader As BinaryReader, ByVal offset As Integer, ByVal size As Integer) As Boolean
            reader.BaseStream.Seek(offset, SeekOrigin.Begin)
            Dim magic As New String(reader.ReadChars(4))
            If magic <> "MHGT" Then
                Return False
            End If

            Dim flags As UInteger = reader.ReadUInt32()
            gridHeight = reader.ReadSingle()
            Dim gridMaxHeight As Single = reader.ReadSingle()
            If (flags And 1) = 0 Then
                If (flags And 2) <> 0 Then
                    ' uint16 height
                    heights16V9 = New UShort((RESOLUTION_HEIGHT + 1) * (RESOLUTION_HEIGHT + 1) - 1) {}
                    heights16V8 = New UShort(RESOLUTION_HEIGHT * RESOLUTION_HEIGHT - 1) {}
                    For i As Integer = 0 To heights16V9.Length - 1
                        heights16V9(i) = reader.ReadUInt16()
                    Next
                    For i As Integer = 0 To heights16V8.Length - 1
                        heights16V8(i) = reader.ReadUInt16()
                    Next
                    gridHeightMultiplier = (gridMaxHeight - gridHeight) / 65535.0F
                    getHeightFunc = AddressOf GetHeightUInt16
                ElseIf (flags And 4) <> 0 Then
                    ' uint8 height
                    heights8V9 = New Byte((RESOLUTION_HEIGHT + 1) * (RESOLUTION_HEIGHT + 1) - 1) {}
                    heights8V8 = New Byte(RESOLUTION_HEIGHT * RESOLUTION_HEIGHT - 1) {}
                    For i As Integer = 0 To heights8V9.Length - 1
                        heights8V9(i) = reader.ReadByte()
                    Next
                    For i As Integer = 0 To heights8V8.Length - 1
                        heights8V8(i) = reader.ReadByte()
                    Next
                    gridHeightMultiplier = (gridMaxHeight - gridHeight) / 255.0F
                    getHeightFunc = AddressOf GetHeightUInt8
                Else
                    ' float height
                    heightsV9 = New Single((RESOLUTION_HEIGHT + 1) * (RESOLUTION_HEIGHT + 1) - 1) {}
                    heightsV8 = New Single(RESOLUTION_HEIGHT * RESOLUTION_HEIGHT - 1) {}
                    For i As Integer = 0 To heightsV9.Length - 1
                        heightsV9(i) = reader.ReadSingle()
                    Next
                    For i As Integer = 0 To heightsV8.Length - 1
                        heightsV8(i) = reader.ReadSingle()
                    Next
                    getHeightFunc = AddressOf GetHeightFloat
                End If
            Else
                getHeightFunc = AddressOf GetHeightFlat
            End If
            Return True
        End Function
#End Region

#Region "Liquid Data"
        ''' <summary>
        ''' Reads the liquid data from a map file.
        ''' </summary>
        ''' <param name="reader">The binary reader.</param>
        ''' <param name="offset">The offset for the liquid data.</param>
        ''' <param name="size">The size of the liquid data.</param>
        Private Function LoadLiquidData(ByVal reader As BinaryReader, ByVal offset As Integer, ByVal size As Integer) As Boolean
            reader.BaseStream.Seek(offset, SeekOrigin.Begin)
            Dim magic As New String(reader.ReadChars(4))
            If magic <> "MLIQ" Then
                Return False
            End If

            Dim flags As UShort = reader.ReadUInt16()
            liquidType = reader.ReadUInt16()
            liquidOffX = Fix(reader.ReadByte())
            liquidOffY = Fix(reader.ReadByte())
            liquidWidth = Fix(reader.ReadByte())
            liquidHeight = Fix(reader.ReadByte())
            liquidLevel = reader.ReadSingle()

            If (flags And 1) = 0 Then
                liquidEntry = New UShort(RESOLUTION_FLAGS - 1, RESOLUTION_FLAGS - 1) {}
                For x As Integer = 0 To RESOLUTION_FLAGS - 1
                    For y As Integer = 0 To RESOLUTION_FLAGS - 1
                        liquidEntry(x, y) = reader.ReadUInt16()
                    Next
                Next

                liquidFlags = New UShort(RESOLUTION_FLAGS - 1, RESOLUTION_FLAGS - 1) {}
                For x As Integer = 0 To RESOLUTION_FLAGS - 1
                    For y As Integer = 0 To RESOLUTION_FLAGS - 1
                        liquidFlags(x, y) = reader.ReadUInt16()
                    Next
                Next
            End If

            If (flags And 2) = 0 Then
                waterLevel = New Single(liquidWidth - 1, liquidHeight - 1) {}
                For x As Integer = 0 To liquidWidth - 1
                    For y As Integer = 0 To liquidHeight - 1
                        waterLevel(x, y) = reader.ReadSingle()
                    Next
                Next
            End If
            Return True
        End Function
#End Region
#End Region

#Region "Get Height"
#Region "Get Height"
        ''' <summary>
        ''' Gets the height for a location on this tile.
        ''' </summary>
        ''' <param name="posX">The position X-coordinate.</param>
        ''' <param name="posY">The position X-coordinate.</param>
        ''' <returns></returns>
        Public Function GetHeight(ByVal posX As Single, ByVal posY As Single) As Single
            Return getHeightFunc(posX, posY)
        End Function
#End Region

#Region "Flat"
        ''' <summary>
        ''' Gets the height for a location on this tile.
        ''' </summary>
        ''' <param name="posX">The position X-coordinate.</param>
        ''' <param name="posY">The position X-coordinate.</param>
        ''' <returns></returns>
        Private Function GetHeightFlat(ByVal posX As Single, ByVal posY As Single) As Single
            Return gridHeight
        End Function
#End Region

#Region "Float"
        ''' <summary>
        ''' Gets the height for a location on this tile.
        ''' </summary>
        ''' <param name="x">The position X-coordinate.</param>
        ''' <param name="y">The position X-coordinate.</param>
        ''' <returns></returns>
        Private Function GetHeightFloat(ByVal x As Single, ByVal y As Single) As Single
            If heightsV8 Is Nothing OrElse heightsV9 Is Nothing Then
                Return gridHeight
            End If

            x = RESOLUTION_HEIGHT * (32 - x / SIZE)
            y = RESOLUTION_HEIGHT * (32 - y / SIZE)
            Dim x_int As Integer = Fix(x)
            Dim y_int As Integer = Fix(y)
            x -= x_int
            y -= y_int
            x_int = x_int And (RESOLUTION_HEIGHT - 1)
            y_int = y_int And (RESOLUTION_HEIGHT - 1)

            ' Height stored as: h5 - its v8 grid, h1-h4 - its v9 grid
            ' +--------------> X
            ' | h1-------h2     Coordinates is:
            ' | | \  1  / |     h1 0,0
            ' | |  \   /  |     h2 0,1
            ' | | 2  h5 3 |     h3 1,0
            ' | |  /   \  |     h4 1,1
            ' | | /  4  \ |     h5 1/2,1/2
            ' | h3-------h4
            ' V Y
            ' For find height need
            ' 1 - detect triangle
            ' 2 - solve linear equation from triangle points
            ' Calculate coefficients for solve h = a*x + b*y + c

            Dim a As Single, b As Single, c As Single
            ' Select triangle:
            If x + y < 1 Then
                If x > y Then
                    ' 1 triangle (h1, h2, h5 points)
                    Dim h1 As Single = heightsV9((x_int) * 129 + y_int)
                    Dim h2 As Single = heightsV9((x_int + 1) * 129 + y_int)
                    Dim h5 As Single = 2 * heightsV8(x_int * 128 + y_int)
                    a = h2 - h1
                    b = h5 - h1 - h2
                    c = h1
                Else
                    ' 2 triangle (h1, h3, h5 points)
                    Dim h1 As Single = heightsV9(x_int * 129 + y_int)
                    Dim h3 As Single = heightsV9(x_int * 129 + y_int + 1)
                    Dim h5 As Single = 2 * heightsV8(x_int * 128 + y_int)
                    a = h5 - h1 - h3
                    b = h3 - h1
                    c = h1
                End If
            Else
                If x > y Then
                    ' 3 triangle (h2, h4, h5 points)
                    Dim h2 As Single = heightsV9((x_int + 1) * 129 + y_int)
                    Dim h4 As Single = heightsV9((x_int + 1) * 129 + y_int + 1)
                    Dim h5 As Single = 2 * heightsV8(x_int * 128 + y_int)
                    a = h2 + h4 - h5
                    b = h4 - h2
                    c = h5 - h4
                Else
                    ' 4 triangle (h3, h4, h5 points)
                    Dim h3 As Single = heightsV9((x_int) * 129 + y_int + 1)
                    Dim h4 As Single = heightsV9((x_int + 1) * 129 + y_int + 1)
                    Dim h5 As Single = 2 * heightsV8(x_int * 128 + y_int)
                    a = h4 - h3
                    b = h3 + h4 - h5
                    c = h5 - h4
                End If
            End If
            ' Calculate height
            Return a * x + b * y + c
        End Function
#End Region

#Region "UInt16"
        ''' <summary>
        ''' Gets the height for a location on this tile.
        ''' </summary>
        ''' <param name="x">The position X-coordinate.</param>
        ''' <param name="y">The position X-coordinate.</param>
        ''' <returns></returns>
        Private Function GetHeightUInt16(ByVal x As Single, ByVal y As Single) As Single
            If heights16V8 Is Nothing OrElse heights16V9 Is Nothing Then
                Return gridHeight
            End If

            x = RESOLUTION_HEIGHT * (32 - x / SIZE)
            y = RESOLUTION_HEIGHT * (32 - y / SIZE)
            Dim x_int As Integer = Fix(x)
            Dim y_int As Integer = Fix(y)
            x -= x_int
            y -= y_int
            x_int = x_int And (RESOLUTION_HEIGHT - 1)
            y_int = y_int And (RESOLUTION_HEIGHT - 1)

            Dim a As Integer, b As Integer, c As Integer
            Dim v9index As Integer = x_int * 128 + x_int + y_int
            If x + y < 1 Then
                If x > y Then
                    ' 1 triangle (h1, h2, h5 points)
                    Dim h1 As Integer = heights16V9(v9index + 0)
                    Dim h2 As Integer = heights16V9(v9index + 129)
                    Dim h5 As Integer = 2 * heights16V8(x_int * 128 + y_int)
                    a = h2 - h1
                    b = h5 - h1 - h2
                    c = h1
                Else
                    ' 2 triangle (h1, h3, h5 points)
                    Dim h1 As Integer = heights16V9(v9index + 0)
                    Dim h3 As Integer = heights16V9(v9index + 1)
                    Dim h5 As Integer = 2 * heights16V8(x_int * 128 + y_int)
                    a = h5 - h1 - h3
                    b = h3 - h1
                    c = h1
                End If
            Else
                If x > y Then
                    ' 3 triangle (h2, h4, h5 points)
                    Dim h2 As Integer = heights16V9(v9index + 129)
                    Dim h4 As Integer = heights16V9(v9index + 130)
                    Dim h5 As Integer = 2 * heights16V8(x_int * 128 + y_int)
                    a = h2 + h4 - h5
                    b = h4 - h2
                    c = h5 - h4
                Else
                    ' 4 triangle (h3, h4, h5 points)
                    Dim h3 As Integer = heights16V9(v9index + 1)
                    Dim h4 As Integer = heights16V9(v9index + 130)
                    Dim h5 As Integer = 2 * heights16V8(x_int * 128 + y_int)
                    a = h4 - h3
                    b = h3 + h4 - h5
                    c = h5 - h4
                End If
            End If

            ' Calculate height
            Return CSng((a * x) + (b * y) + c) * gridHeightMultiplier + gridHeight
        End Function
#End Region

#Region "UInt8"
        ''' <summary>
        ''' Gets the height for a location on this tile.
        ''' </summary>
        ''' <param name="x">The position X-coordinate.</param>
        ''' <param name="y">The position X-coordinate.</param>
        ''' <returns></returns>
        Private Function GetHeightUInt8(ByVal x As Single, ByVal y As Single) As Single
            If heights8V8 Is Nothing OrElse heights8V9 Is Nothing Then
                Return gridHeight
            End If

            x = RESOLUTION_HEIGHT * (32 - x / SIZE)
            y = RESOLUTION_HEIGHT * (32 - y / SIZE)
            Dim x_int As Integer = Fix(x)
            Dim y_int As Integer = Fix(y)
            x -= x_int
            y -= y_int
            x_int = x_int And (RESOLUTION_HEIGHT - 1)
            y_int = y_int And (RESOLUTION_HEIGHT - 1)

            Dim a As Integer, b As Integer, c As Integer
            Dim v9index As Integer = x_int * 128 + x_int + y_int
            If x + y < 1 Then
                If x > y Then
                    ' 1 triangle (h1, h2, h5 points)
                    Dim h1 As Integer = heights8V9(v9index + 0)
                    Dim h2 As Integer = heights8V9(v9index + 129)
                    Dim h5 As Integer = 2 * heights8V8(x_int * 128 + y_int)
                    a = h2 - h1
                    b = h5 - h1 - h2
                    c = h1
                Else
                    ' 2 triangle (h1, h3, h5 points)
                    Dim h1 As Integer = heights8V9(v9index + 0)
                    Dim h3 As Integer = heights8V9(v9index + 1)
                    Dim h5 As Integer = 2 * heights8V8(x_int * 128 + y_int)
                    a = h5 - h1 - h3
                    b = h3 - h1
                    c = h1
                End If
            Else
                If x > y Then
                    ' 3 triangle (h2, h4, h5 points)
                    Dim h2 As Integer = heights8V9(v9index + 129)
                    Dim h4 As Integer = heights8V9(v9index + 130)
                    Dim h5 As Integer = 2 * heights8V8(x_int * 128 + y_int)
                    a = h2 + h4 - h5
                    b = h4 - h2
                    c = h5 - h4
                Else
                    ' 4 triangle (h3, h4, h5 points)
                    Dim h3 As Integer = heights8V9(v9index + 1)
                    Dim h4 As Integer = heights8V9(v9index + 130)
                    Dim h5 As Integer = 2 * heights8V8(x_int * 128 + y_int)
                    a = h4 - h3
                    b = h3 + h4 - h5
                    c = h5 - h4
                End If
            End If

            ' Calculate height
            Return CSng((a * x) + (b * y) + c) * gridHeightMultiplier + gridHeight
        End Function
#End Region
#End Region

#Region "Get Area Flag"
        ''' <summary>
        ''' Gets the area flag for a subtile on this tile.
        ''' </summary>
        ''' <param name="subTileX">The subtile X.</param>
        ''' <param name="subTileY">The subtile Y.</param>
        ''' <returns></returns>
        Public Function GetAreaFlag(ByVal subTileX As Integer, ByVal subTileY As Integer) As UShort
            If areaFlags Is Nothing Then
                Return gridArea
            End If
            If subTileX >= 0 AndAlso subTileY >= 0 AndAlso subTileX < RESOLUTION_FLAGS AndAlso subTileY < RESOLUTION_FLAGS Then
                Return areaFlags(subTileX, subTileY)
            Else
                Return 0
            End If
        End Function
#End Region

#Region "Get Terrain Type"
        ''' <summary>
        ''' Gets the terrain type for a subtile on this tile.
        ''' </summary>
        ''' <param name="subTileX">The subtile X.</param>
        ''' <param name="subTileY">The subtile Y.</param>
        ''' <returns></returns>
        Public Function GetTerrainType(ByVal subTileX As Integer, ByVal subTileY As Integer) As Byte
            If liquidFlags Is Nothing Then
                Return CByte(liquidType)
            End If
            If subTileX >= 0 AndAlso subTileY >= 0 AndAlso subTileX < RESOLUTION_FLAGS AndAlso subTileY < RESOLUTION_FLAGS Then
                Return CByte(liquidFlags(subTileX, subTileY))
            Else
                Return 0
            End If
        End Function
#End Region

#Region "Get Water Level"
        ''' <summary>
        ''' Gets the water level for a subtile on this tile.
        ''' </summary>
        ''' <param name="subTileX">The subtile X.</param>
        ''' <param name="subTileY">The subtile Y.</param>
        ''' <returns></returns>
        Public Function GetWaterLevel(ByVal subTileX As Integer, ByVal subTileY As Integer) As Single
            If waterLevel Is Nothing Then
                Return liquidLevel
            End If
            subTileX -= liquidOffX
            subTileY -= liquidOffY
            If subTileX >= 0 AndAlso subTileY >= 0 AndAlso subTileX < liquidWidth AndAlso subTileY < liquidHeight Then
                Return waterLevel(subTileX, subTileY)
            Else
                Return INVALID_HEIGHT_VALUE
            End If
        End Function
#End Region

#If VMAPS Then
        Public Sub LoadVMAP()
            'DONE: Loading VMDIR file
            Dim fileName As String = String.Format("{0}_{1}_{2}.vmdir", Format(cellMap, "000"), Format(cellX, "00"), Format(cellY, "00"))
            If Not File.Exists("vmaps\" & fileName) Then
                fileName = String.Format("{0}.vmdir", Format(cellMap, "000"))
            End If

            If Not File.Exists("vmaps\" & fileName) Then
                Log.WriteLine(LogType.WARNING, "VMap file [{0}] not found", fileName)
            Else
                Dim thisTmap As TMap = Maps(cellMap)
                fileName = Trim(File.ReadAllText("vmaps\" & fileName))
                Dim fileNames() As String = fileName.Split(New String() {vbLf}, StringSplitOptions.RemoveEmptyEntries)

                fileName = fileNames(0)
                If fileNames.Length > 1 AndAlso fileNames(1).Length > 0 Then
                    fileName = fileNames(1)
                End If

                Dim newModelLoaded As Boolean = False
                If fileName.Length > 0 AndAlso File.Exists("vmaps\" & fileName) Then
                    Dim mc As ModelContainer
                    If Not thisTmap.ContainsModelContainer(fileName) Then
                        mc = New ModelContainer()
                        If mc.ReadFile(fileName) Then
                            If Not thisTmap.ContainsModelContainer(fileName) Then
                                thisTmap.AddModelContainer(fileName, mc)
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
                    thisTmap.BalanceTree()
                End If
            End If
        End Sub
#End If

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                UnloadSpawns(cellX, cellY, cellMap)
            End If
            _disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

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
            Try
                Dim maxDist As Single = (pos2 - pos1).Magnitude()

                Dim ray As New Ray(pos1, (pos2 - pos1) / maxDist)
                Dim resultDist As Single = GetIntersectionTime(ray, maxDist, True)
                If resultDist < maxDist Then
                    result = False
                End If
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "Trapped Exception in WS.MAPS.IsInLineOfSight")
            End Try

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
            Try
                iTree.IntersectRay(pRay, t, pStopAtFirstHit, False)
#If VMAPS_DEBUG Then
            Log.WriteLine(LogType.DEBUG, "GetIntersectionTime: {0}", t)
#End If
                If t > 0 AndAlso t < Single.PositiveInfinity AndAlso pMaxDist > t Then
                    firstDistance = t
                End If
            Catch ex As Exception
                Log.WriteLine(LogType.DEBUG, "Trapped Exception in WS.MAPS.GetIntersectionTime")
            End Try

            Return firstDistance
        End Function
#End If

        Public Sub New(ByVal Map As Integer)
            Maps.Add(Map, Me)

            For x As Integer = 0 To 63
                For y As Integer = 0 To 63
                    TileUsed(x, y) = False
                Next
            Next

            Try
                Dim tmpDBC As DBC.BufferedDbc = New DBC.BufferedDbc("dbc\Map.dbc")
                Dim tmpMap As Integer

                For i As Integer = 0 To tmpDBC.Rows - 1
                    tmpMap = tmpDBC.Item(i, 0)

                    If tmpMap = Map Then
                        ID = Map
                        Type = tmpDBC.Item(i, 2, DBC.DBCValueType.DBC_INTEGER)
                        Name = tmpDBC.Item(i, 4, DBC.DBCValueType.DBC_STRING)
                        Exit For
                    End If
                Next i

                Log.WriteLine(LogType.INFORMATION, "DBC: 1 Map initialized.", tmpDBC.Rows - 1)
                tmpDBC.Dispose()
            Catch e As DirectoryNotFoundException
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("DBC File : Map missing.")
                Console.ForegroundColor = ConsoleColor.Gray
            End Try
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                For i As Integer = 0 To 63
                    For j As Integer = 0 To 63
                        If Not Tiles(i, j) Is Nothing Then Tiles(i, j).Dispose()
                    Next
                Next

                Maps.Remove(ID)
                'iTree.Dispose()
            End If
            _disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class

    Public Maps As New Dictionary(Of UInteger, TMap)
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

        ''DONE: Loading maps
        For Each id As UInteger In Config.Maps
            Dim map As New TMap(id)
        Next

        Log.WriteLine(LogType.INFORMATION, "Initalizing: {0} Maps initialized.", Maps.Count)
    End Sub

    Public Sub GetMapTile(ByVal x As Single, ByVal y As Single, ByRef MapTileX As Byte, ByRef MapTileY As Byte)
        'How to calculate where is X,Y:
        'MapTileX = Fix(32 - (x / SIZE))
        'MapTileY = Fix(32 - (y / SIZE))
        MapTileX = Fix(32 - (x / TMapTile.SIZE))
        MapTileY = Fix(32 - (y / TMapTile.SIZE))
    End Sub

    Public Function GetMapTileX(ByVal x As Single) As Integer
        'Return Fix(32 - (x / SIZE))
        'Dim cX As Single = x / TMapTile.SIZE
        'Return CByte(32 - cX)
        Return Fix(32 - (x / TMapTile.SIZE))
    End Function

    Public Function GetMapTileY(ByVal y As Single) As Integer
        'Return Fix(32 - (y / SIZE))
        'Dim cY As Single = y / TMapTile.SIZE
        'Return CByte(32 - cY)
        Return Fix(32 - (y / TMapTile.SIZE))
    End Function

    Public Function GetSubMapTileX(ByVal x As Single) As Integer
        'Return Fix(RESOLUTION_ZMAP * (32 - (x / SIZE) - Fix(32 - (x / SIZE))))
        'Dim cX As Single = x / TMapTile.SIZE
        'Dim tileX As Integer = CInt(32 - cX)
        'Return CByte(TMapTile.RESOLUTION_FLAGS * (32.0F - cX - tileX))
        Dim cX As Single = x / TMapTile.SIZE
        Return Fix(TMapTile.RESOLUTION_HEIGHT * (32.0F - cX - CSng(32 - Fix(cX))))
    End Function

    Public Function GetSubMapTileY(ByVal y As Single) As Integer
        'Return Fix(RESOLUTION_ZMAP * (32 - (y / SIZE) - Fix(32 - (y / SIZE))))
        'Dim cY As Single = y / TMapTile.SIZE
        'Dim tileY As Integer = CInt(Math.Truncate(32 - cY))
        'Return CByte(TMapTile.RESOLUTION_FLAGS * (32.0F - cY - tileY))
        Dim cY As Single = y / TMapTile.SIZE
        Return Fix(TMapTile.RESOLUTION_HEIGHT * (32.0F - cY - CSng(32 - Fix(cY))))
    End Function

    ''' <summary>
    ''' Attempts to get the Z-coord of a specific location in the world.
    ''' </summary>
    ''' <param name="mapID">The map ID.</param>
    ''' <param name="posX">The X-position coordinate.</param>
    ''' <param name="posY">The Y-position coordinate.</param>
    ''' <returns>The Z-coord.</returns>
    Public Function GetZCoord(ByVal posX As Single, ByVal posY As Single, ByVal mapID As UInteger) As Single
        'Dim MapTileX As Byte = Fix(32 - (posX / TMapTile.SIZE))
        'Dim MapTileY As Byte = Fix(32 - (posY / TMapTile.SIZE))

        Dim cX As Single = posX / TMapTile.SIZE
        Dim cY As Single = posY / TMapTile.SIZE
        Dim tileX As Integer = CInt(32 - cX)
        Dim tileY As Integer = CInt(32 - cY)

        Dim map As TMap = Maps(mapID)
        If map IsNot Nothing Then
            Dim tile As TMapTile = Maps(mapID).Tiles(tileX, tileY)
            If tile IsNot Nothing Then
                Return tile.GetHeight(posX, posY)
            End If
        End If
        Return TMapTile.INVALID_HEIGHT_VALUE
    End Function

    Public Function GetWaterLevel(ByVal x As Single, ByVal y As Single, ByVal Map As Integer) As Single
        'Dim MapTileX As Byte = Fix(32 - (x / SIZE))
        'Dim MapTileY As Byte = Fix(32 - (y / SIZE))
        'Dim MapTile_LocalX As Byte = CType(RESOLUTION_WATER * (32 - (x / SIZE) - MapTileX), Byte)
        'Dim MapTile_LocalY As Byte = CType(RESOLUTION_WATER * (32 - (y / SIZE) - MapTileY), Byte)

        'If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then Return 0
        'Return Maps(Map).Tiles(MapTileX, MapTileY).WaterLevel(MapTile_LocalX, MapTile_LocalY)
        Dim cX As Single = x / TMapTile.SIZE
        Dim cY As Single = y / TMapTile.SIZE
        Dim tileX As Integer = CInt(32 - cX)
        Dim tileY As Integer = CInt(32 - cY)
        Dim subTileX As Integer = CInt(TMapTile.RESOLUTION_WATER * (32.0F - cX - tileX))
        Dim subTileY As Integer = CInt(TMapTile.RESOLUTION_WATER * (32.0F - cY - tileY))

        'Dim map As Map = MovementHandlers.GetMap(mapID)
        Dim tmpMap As TMap = Maps(Map)
        If tmpMap IsNot Nothing Then
            Dim tile As TMapTile = tmpMap.Tiles(tileX, tileY)
            If tile IsNot Nothing Then
                Return CInt(tile.GetWaterLevel(subTileX, subTileY))
            End If
        End If
        Return 0
    End Function

    Public Function GetTerrainType(ByVal x As Single, ByVal y As Single, ByVal Map As Integer) As Byte
        'Dim MapTileX As Byte = Fix(32 - (x / SIZE))
        'Dim MapTileY As Byte = Fix(32 - (y / SIZE))
        'Dim MapTile_LocalX As Byte = CType(RESOLUTION_TERRAIN * (32 - (x / SIZE) - MapTileX), Byte)
        'Dim MapTile_LocalY As Byte = CType(RESOLUTION_TERRAIN * (32 - (y / SIZE) - MapTileY), Byte)

        'If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then Return 0
        'Return Maps(Map).Tiles(MapTileX, MapTileY).AreaTerrain(MapTile_LocalX, MapTile_LocalY)
        Dim cX As Single = x / TMapTile.SIZE
        Dim cY As Single = y / TMapTile.SIZE
        Dim tileX As Integer = CInt(32 - cX)
        Dim tileY As Integer = CInt(32 - cY)
        Dim subTileX As Integer = CInt(TMapTile.RESOLUTION_TERRAIN * (32.0F - cX - tileX))
        Dim subTileY As Integer = CInt(TMapTile.RESOLUTION_TERRAIN * (32.0F - cY - tileY))

        'Dim map As Map = MovementHandlers.GetMap(mapID)
        Dim tmpMap As TMap = Maps(Map)
        If tmpMap IsNot Nothing Then
            Dim tile As TMapTile = tmpMap.Tiles(tileX, tileY)
            If tile IsNot Nothing Then
                Return CInt(tile.GetTerrainType(subTileX, subTileY))
            End If
        End If
        Return 0
    End Function

    Public Function GetAreaFlag(ByVal x As Single, ByVal y As Single, ByVal Map As Integer) As Integer
        'Dim MapTileX As Byte = Fix(32 - (x / SIZE))
        'Dim MapTileY As Byte = Fix(32 - (y / SIZE))
        'Dim MapTile_LocalX As Byte = CType(RESOLUTION_FLAGS * (32 - (x / SIZE) - MapTileX), Byte)
        'Dim MapTile_LocalY As Byte = CType(RESOLUTION_FLAGS * (32 - (y / SIZE) - MapTileY), Byte)

        'If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then Return 0
        'Return Maps(Map).Tiles(MapTileX, MapTileY).AreaFlag(MapTile_LocalX, MapTile_LocalY)
        Dim cX As Single = x / TMapTile.SIZE
        Dim cY As Single = y / TMapTile.SIZE
        Dim tileX As Integer = CInt(32 - cX)
        Dim tileY As Integer = CInt(32 - cY)
        Dim subTileX As Integer = CInt(TMapTile.RESOLUTION_FLAGS * (32.0F - cX - tileX))
        Dim subTileY As Integer = CInt(TMapTile.RESOLUTION_FLAGS * (32.0F - cY - tileY))

        'Dim map As Map = MovementHandlers.GetMap(mapID)
        Dim tmpMap As TMap = Maps(Map)
        If tmpMap IsNot Nothing Then
            Dim tile As TMapTile = tmpMap.Tiles(tileX, tileY)
            If tile IsNot Nothing Then
                Return CInt(tile.GetAreaFlag(subTileX, subTileY))
            End If
        End If
        Return 0
    End Function

    Public Function IsOutsideOfMap(ByRef objCharacter As BaseObject) As Boolean
        'NOTE: Disabled these checks because DBC data contains too big X/Y coords to be usefull
        Return False

        'Dim x As Single = objCharacter.positionX
        'Dim y As Single = objCharacter.positionY
        'Dim m As UInteger = objCharacter.MapID

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
        '            Log.WriteLine(LogType.USER, "Outside map: {0:X}", objCharacter.GUID)
        '            Return True
        '        Else
        '            Return False
        '        End If
        '    End With
        'End If

        'Log.WriteLine(LogType.USER, "WorldMapContinent not found for map {0}.", objCharacter.MapID)
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

            If Maps(Map).Tiles(MapTileX, MapTileY) Is Nothing Then
                'Return vmap height if one was found
                Dim VMapHeight As Single = GetVMapHeight(Map, x, y, z + 2.0F)
                If VMapHeight <> VMAP_INVALID_HEIGHT_VALUE Then
                    Return VMapHeight
                End If
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
            Dim MapTileX As Byte = Fix(32 - (x / SIZE))
            Dim MapTileY As Byte = Fix(32 - (y / SIZE))
            Dim MapTile_LocalX As Byte = CType(RESOLUTION_ZMAP * (32 - (x / SIZE) - MapTileX), Byte)
            Dim MapTile_LocalY As Byte = CType(RESOLUTION_ZMAP * (32 - (y / SIZE) - MapTileY), Byte)
            Dim xNormalized As Single = RESOLUTION_ZMAP * (32 - (x / SIZE) - MapTileX) - MapTile_LocalX
            Dim yNormalized As Single = RESOLUTION_ZMAP * (32 - (y / SIZE) - MapTileY) - MapTile_LocalY

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
        Dim MinX As Single = ((32 - TileX) * SIZE)
        Dim MaxX As Single = ((32 - (TileX + 1)) * SIZE)
        Dim MinY As Single = ((32 - TileY) * SIZE)
        Dim MaxY As Single = ((32 - (TileY + 1)) * SIZE)
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
        CharacterDatabase.Query(String.Format("SELECT * FROM corpse WHERE map={0} AND instance={5} AND position_x BETWEEN '{1}' AND '{2}' AND position_y BETWEEN '{3}' AND '{4}';", TileMap, MinX, MaxX, MinY, MaxY, TileInstance), MysqlQuery)
        For Each InfoRow As DataRow In MysqlQuery.Rows
            If Not WORLD_CORPSEOBJECTs.ContainsKey(CType(InfoRow.Item("guid"), ULong) + GUID_CORPSE) Then
                Try
                    Dim tmpCorpse As CorpseObject = New CorpseObject(CType(InfoRow.Item("guid"), ULong), InfoRow)
                    tmpCorpse.instance = TileInstance
                    tmpCorpse.AddToWorld()
                Catch ex As Exception
                    Log.WriteLine(LogType.CRITICAL, "Error when creating corpse [{0}].{1}{2}", InfoRow.Item("guid"), vbNewLine, ex.ToString)
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
        Dim MinX As Single = ((32 - TileX) * SIZE)
        Dim MaxX As Single = ((32 - (TileX + 1)) * SIZE)
        Dim MinY As Single = ((32 - TileY) * SIZE)
        Dim MaxY As Single = ((32 - (TileY + 1)) * SIZE)
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
                If Creature.Value.MapID = TileMap AndAlso Creature.Value.SpawnX >= MinX AndAlso Creature.Value.SpawnX <= MaxX AndAlso Creature.Value.SpawnY >= MinY AndAlso CType(Creature.Value, CreatureObject).SpawnY <= MaxY Then
                    Creature.Value.Destroy()
                End If
            Next
        Catch ex As Exception
            Log.WriteLine(LogType.CRITICAL, ex.ToString, Nothing)
        Finally
            WORLD_CREATUREs_Lock.ReleaseReaderLock()
        End Try

        For Each Gameobject As KeyValuePair(Of ULong, GameObjectObject) In WORLD_GAMEOBJECTs
            If CType(Gameobject.Value, GameObjectObject).MapID = TileMap AndAlso CType(Gameobject.Value, GameObjectObject).positionX >= MinX AndAlso CType(Gameobject.Value, GameObjectObject).positionX <= MaxX AndAlso CType(Gameobject.Value, GameObjectObject).positionY >= MinY AndAlso CType(Gameobject.Value, GameObjectObject).positionY <= MaxY Then
                CType(Gameobject.Value, GameObjectObject).Destroy(Gameobject)
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
    Public Sub SendTransferAborted(ByRef client As ClientClass, ByVal Map As Integer, ByVal Reason As TransferAbortReason)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_TRANSFER_ABORTED [{2}:{3}]", client.IP, client.Port, Map, Reason)

        Dim p As New PacketClass(OPCODES.SMSG_TRANSFER_ABORTED)
        Try
            p.AddInt32(Map)
            p.AddInt16(Reason)
            client.Send(p)
        Finally
            p.Dispose()
        End Try
    End Sub

#End Region

End Module