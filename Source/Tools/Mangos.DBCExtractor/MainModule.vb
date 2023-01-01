'
' Copyright (C) 2013-2023 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.IO
Imports Foole.Mpq
Imports Mangos.Common.DataStores
Imports Mangos.Common.Enums.Global

Module MainModule

    Public MPQArchives As New List(Of MpqArchive)
    Public MapIDs As New List(Of Integer)
    Public MapNames As New List(Of String)
    Public MapAreas As New Dictionary(Of Integer, Integer)
    Public MaxAreaID As Integer = -1
    Public MapLiqTypes As New Dictionary(Of Integer, Integer)

    Sub Main()
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine("DBC extractor by UniX")
        Console.WriteLine("-----------------------------")
        Console.WriteLine()

        Console.ForegroundColor = ConsoleColor.Red
        If Directory.Exists("Data") = False Then
            Console.WriteLine("No data folder is found. Make sure this extractor is put in your World of Warcraft directory.")
            GoTo ExitNow
        End If

        Dim MPQFilesToOpen As New List(Of String) From {
            "terrain.MPQ",
            "dbc.MPQ",
            "misc.MPQ",
            "patch.MPQ",
            "patch-2.MPQ"
        }

        For Each mpq As String In MPQFilesToOpen
            If File.Exists("Data\" & mpq) = False Then
                Console.WriteLine("Missing [{0}]. Make sure this extractor is put in your World of Warcraft directory.", mpq)
                GoTo ExitNow
            End If
        Next

        Console.ForegroundColor = ConsoleColor.Yellow
        For Each mpq As String In MPQFilesToOpen
            Dim stream = File.Open(Path.GetFullPath("Data\" & mpq), FileMode.Open)
            Dim newArchive = New MpqArchive(stream, True)
            MPQArchives.Add(newArchive)
            Console.WriteLine("Loaded archive [{0}].", mpq)
        Next

        Try
            Directory.CreateDirectory("dbc")
            Directory.CreateDirectory("maps")
            Console.WriteLine("Created extract folders.")
        Catch ex As UnauthorizedAccessException
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Unable to create extract folders, you don't seem to have admin rights.")
            GoTo ExitNow
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Unable to create extract folders. Error: " & ex.Message)
            GoTo ExitNow
        End Try

        Try
            ExtractDBCs()
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Unable to extract DBC Files. Error: " & ex.Message)
            GoTo ExitNow
        End Try

        'Try
        '    ExtractMaps()
        'Catch ex As Exception
        '    Console.ForegroundColor = ConsoleColor.Red
        '    Console.WriteLine("Unable to extract Maps. Error: " & ex.Message)
        '    GoTo ExitNow
        'End Try

ExitNow:
        Console.ForegroundColor = ConsoleColor.DarkMagenta
        Console.WriteLine()
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey()
    End Sub

    Public Sub ExtractDBCs()
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("Extracting DBC Files")
        Console.ForegroundColor = ConsoleColor.Gray

        Dim dbcFolder As String = Path.GetFullPath("dbc")

        Dim numDBCs As Integer = 0
        For Each mpqArchive In MPQArchives
            numDBCs += mpqArchive.Where(Function(x) x.Filename IsNot Nothing).Where(Function(x) x.Filename.EndsWith(".dbc")).Count()
        Next

        Dim i As Integer = 0
        Dim numDiv30 As Integer = numDBCs \ 30
        For Each mpqArchive In MPQArchives
            For Each mpqFile In mpqArchive.Where(Function(x) x.Filename IsNot Nothing).Where(Function(x) x.Filename.EndsWith(".dbc"))
                Using mpqStream = mpqArchive.OpenFile(mpqFile)
                    Using fileStream = File.Create(Path.Combine(dbcFolder, Path.GetFileName(mpqFile.Filename)))
                        mpqStream.CopyTo(fileStream)
                    End Using
                End Using
                i += 1

                If (i Mod numDiv30) = 0 Then
                    Console.Write(".")
                End If
            Next
        Next

        Console.ForegroundColor = ConsoleColor.Green
        Console.Write(" Done.")
    End Sub

    'Public Sub ExtractMaps()
    '    Dim mapCount As Integer = ReadMapDBC()
    '    ReadAreaTableDBC()
    '    ReadLiquidTypeTableDBC()

    '    Console.WriteLine()
    '    Console.ForegroundColor = ConsoleColor.White
    '    Console.Write("Extracting Maps")
    '    Console.ForegroundColor = ConsoleColor.Gray

    '    Dim mapFolder As String = Path.GetFullPath("maps")

    '    Dim total As Integer = mapCount * 64 * 64
    '    Dim done As Integer = 0
    '    Dim totalDiv30 As Integer = total \ 30
    '    For i As Integer = 0 To mapCount - 1
    '        'Dim wdtName As String = String.Format("World\Maps\{0}\{0}.wdt", MapNames(i))
    '        'Dim wdt As New WDT_File()
    '        'If Not wdt.LoadFile(wdtName) Then
    '        '    Console.ForegroundColor = ConsoleColor.Red
    '        '    Console.WriteLine()
    '        '    Console.WriteLine("WDT File [{0}] could not be read.", wdtName)
    '        '    Console.ForegroundColor = ConsoleColor.Gray
    '        'End If

    '        For x As Integer = 0 To 63
    '            For y As Integer = 0 To 63
    '                Dim mpqFileName As String = String.Format("World\Maps\{0}\{0}_{1}_{2}.adt", MapNames(i), x, y)
    '                Dim outputName As String = String.Format("{0}{1}{2}.map", Format(MapIDs(i), "000"), Format(x, "00"), Format(y, "00"))
    '                ConvertADT(mpqFileName, Path.Combine(mapFolder, outputName))
    '                done += 1
    '                If (done Mod totalDiv30) = 0 Then Console.Write(".")
    '            Next
    '        Next
    '    Next

    '    Console.Write(" Done!")
    'End Sub

    Public Function ReadMapDBC() As Integer
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("Reading Map.dbc... ")

        Dim mapDBC As New BufferedDbc("dbc\Map.dbc")

        For i As Integer = 0 To mapDBC.Rows - 1
            MapIDs.Add(CType(mapDBC.Item(i, 0), Integer))
            MapNames.Add(CType(mapDBC.Item(i, 1, DBCValueType.DBC_STRING), String))
        Next

        Console.WriteLine("Done! ({0} maps loaded)", mapDBC.Rows)

        Return mapDBC.Rows
    End Function

    Public Sub ReadAreaTableDBC()
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("Reading AreaTable.dbc... ")

        Dim areaDBC As New BufferedDbc("dbc\AreaTable.dbc")

        Dim maxID As Integer = -1
        For i As Integer = 0 To areaDBC.Rows - 1
            Dim areaID As Integer = areaDBC.Item(i, 0)
            Dim areaFlag As Integer = areaDBC.Item(i, 3)
            MapAreas.Add(areaID, areaFlag)
            If areaID > maxID Then maxID = areaID
        Next
        MaxAreaID = maxID

        Console.WriteLine("Done! ({0} areas loaded)", areaDBC.Rows)
    End Sub

    Public Sub ReadLiquidTypeTableDBC()
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("Reading LiquidType.dbc... ")

        Dim liquidDBC As New BufferedDbc("dbc\LiquidType.dbc")

        For i As Integer = 0 To liquidDBC.Rows - 1
            MapLiqTypes.Add(liquidDBC.Item(i, 0), liquidDBC.Item(i, 3))
        Next

        Console.WriteLine("Done! ({0} LiqTypes loaded)", liquidDBC.Rows)
    End Sub

End Module
